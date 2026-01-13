#include <Arduino.h>
#include <WiFi.h>
#include <HTTPClient.h>
#include <WiFiClient.h> 
#include <ModbusMaster.h>
#include <DHT.h>
#include <time.h>             
#include <sys/time.h>         
#include <LittleFS.h>         // 🟢 Required for Caching
#include "credentials.h"

// ------------------- CONFIGURATION -------------------
const char* ntpServer = "pool.ntp.org";
const long  gmtOffset_sec = 10800;  // UTC +3
const int   daylightOffset_sec = 0; 
const char* CACHE_FILE = "/sensor_cache.txt";

#define DHTPIN 4
#define DHTTYPE DHT22

DHT dht(DHTPIN, DHTTYPE);
HardwareSerial& modbusSerial = Serial2;
ModbusMaster node;

long lastMsg = 0;
const long interval = 30000; // 🟢 5 Minutes (5 * 60 * 1000)

// ------------------- FILE SYSTEM HELPERS -------------------

void saveToCache(String payload) {
  File file = LittleFS.open(CACHE_FILE, FILE_APPEND);
  if (!file) {
    Serial.println("Failed to open cache file");
    return;
  }
  file.println(payload); // Store as a new line
  file.close();
  Serial.println("Data saved to local cache.");
}

// ------------------- HTTP SENDING LOGIC -------------------

bool sendToServer(String payload) {
  if (WiFi.status() != WL_CONNECTED) return false;

  WiFiClient client;
  HTTPClient http;
  http.setTimeout(10000); // 10s timeout

  if (http.begin(client, serverUrl)) {
    http.addHeader("Content-Type", "application/json");
    http.addHeader("X-API-KEY", apiKey);
    http.addHeader("ngrok-skip-browser-warning", "true");

    int httpResponseCode = http.POST(payload);
    bool success = (httpResponseCode >= 200 && httpResponseCode < 300);
    
    if (success) {
      Serial.println("Successfully sent to server.");
    } else {
      Serial.print("Failed to send. Error: ");
      Serial.println(httpResponseCode);

      String response = http.getString(); 
      Serial.print("Server Response: ");
      Serial.println(response);
    }
    
    http.end();
    return success;
  }
  return false;
}

void syncCache() {
  if (!LittleFS.exists(CACHE_FILE)) return;

  File file = LittleFS.open(CACHE_FILE, FILE_READ);
  if (!file) return;

  Serial.println("Found cached data. Attempting to sync...");
  
  // We read the file and try to send line by line
  bool allSent = true;
  String remainingCache = "";

  while (file.available()) {
    String cachedPayload = file.readStringUntil('\n');
    cachedPayload.trim();
    
    if (cachedPayload.length() > 10) {
      if (sendToServer(cachedPayload)) {
        delay(500); // Short delay between bursts
      } else {
        // If one fails, stop and keep the rest for later
        allSent = false;
        remainingCache += cachedPayload + "\n";
      }
    }
  }
  file.close();

  if (allSent) {
    LittleFS.remove(CACHE_FILE);
    Serial.println("Cache fully cleared.");
  } else {
    // Overwrite file with only what wasn't sent
    File rewriteFile = LittleFS.open(CACHE_FILE, FILE_WRITE);
    rewriteFile.print(remainingCache);
    rewriteFile.close();
    Serial.println("Partial sync complete. Remaining data kept in cache.");
  }
}

// ------------------- TIME & WIFI -------------------

String getTimestamp() {
  struct timeval tv;
  if (gettimeofday(&tv, NULL) != 0) return "";
  
  struct tm timeinfo;
  localtime_r(&tv.tv_sec, &timeinfo); 

  char baseBuffer[30];
  strftime(baseBuffer, sizeof(baseBuffer), "%Y-%m-%dT%H:%M:%S", &timeinfo);
  
  int millisec = tv.tv_usec / 1000;
  char finalBuffer[60];
  sprintf(finalBuffer, "%s.%03d+03:00", baseBuffer, millisec);
  
  return String(finalBuffer);
}

void setup_wifi() {
  Serial.print("Connecting to WiFi...");
  WiFi.begin(ssid, password);
  int retry = 0;
  while (WiFi.status() != WL_CONNECTED && retry < 20) {
    delay(500);
    Serial.print(".");
    retry++;
  }
  if(WiFi.status() == WL_CONNECTED) Serial.println("\nWiFi Connected!");
  else Serial.println("\nWiFi Failed (Will cache data)");
}

// ------------------- SETUP -------------------

void setup() {
  Serial.begin(115200);

  // Initialize LittleFS
  if (!LittleFS.begin(true)) {
    Serial.println("LittleFS Mount Failed");
  }

  dht.begin();
  modbusSerial.begin(9600, SERIAL_8N1, 16, 17); 
  node.begin(0x01, modbusSerial);

  setup_wifi();

  configTime(gmtOffset_sec, daylightOffset_sec, ntpServer);
  Serial.println("Synchronizing Time...");
}

// ------------------- LOOP -------------------

void loop() { 
  long now = millis();
  
  if (now - lastMsg > interval || lastMsg == 0) {
    lastMsg = now;

    // 1. Read Air Sensors (DHT22)
    float airHum = dht.readHumidity();
    float airTemp = dht.readTemperature();
    
    // Check if DHT22 specifically is failing
    bool dhtError = isnan(airHum) || isnan(airTemp);

    // 2. Read Soil Sensors (Modbus)
    float soilMoist = 0, soilPH = 0, soilN = 0, soilP = 0, soilK = 0;
    uint8_t result = node.readHoldingRegisters(0x00, 8);
    
    // Check if Modbus specifically is failing
    bool modbusError = (result != node.ku8MBSuccess);

    if (!modbusError) {
      soilMoist = node.getResponseBuffer(1) / 10.0;
      soilPH = node.getResponseBuffer(3) / 100.0;
      soilN = (float)node.getResponseBuffer(4);
      soilP = (float)node.getResponseBuffer(5);
      soilK = (float)node.getResponseBuffer(6);
    }

    // 3. Prepare Detailed JSON Payload
    String currentTime = getTimestamp();
    String jsonPayload = "{";
    jsonPayload += "\"Timestamp\": \"" + currentTime + "\",";
    
    // Air Data & Status
    jsonPayload += "\"Temperature\": " + String(isnan(airTemp) ? "null" : String(airTemp)) + ","; 
    jsonPayload += "\"Humidity\": " + String(isnan(airHum) ? "null" : String(airHum)) + ","; 
    jsonPayload += "\"AirSensorStatus\": \"" + String(dhtError ? "Faulty" : "OK") + "\",";

    // Soil Data & Status
    jsonPayload += "\"SoilMoisture\": " + String(modbusError ? "null" : String(soilMoist)) + ",";
    jsonPayload += "\"PH\": " + String(modbusError ? "null" : String(soilPH)) + ",";
    jsonPayload += "\"Potassium\": " + String(modbusError ? "null" : String(soilK)) + ",";
    jsonPayload += "\"Phosphorus\": " + String(modbusError ? "null" : String(soilP)) + ",";
    jsonPayload += "\"Nitrogen\": " + String(modbusError ? "null" : String(soilN)) + ",";
    jsonPayload += "\"SoilSensorStatus\": \"" + String(modbusError ? "Faulty" : "OK") + "\"";
    
    jsonPayload += "}";

    // 3. Sync & Send Logic
    if (WiFi.status() == WL_CONNECTED) {
      // Try to push old data first
      syncCache();
      
      // Try to push current data
      if (!sendToServer(jsonPayload)) {
        saveToCache(jsonPayload);
      }
    } else {
      Serial.println("Offline. Caching data...");
      saveToCache(jsonPayload);
      setup_wifi(); // Attempt reconnect for next time
    }
  }
}