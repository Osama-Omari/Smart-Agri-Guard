#include "NetworkManager.h"

NetworkManager::NetworkManager(String ssid, String password, String serverUrl, String apiKey) {
    this->ssid = ssid;
    this->password = password;
    this->serverUrl = serverUrl;
    this->apiKey = apiKey;
}

void NetworkManager::setup_wifi() {
    Serial.print("Connecting to WiFi...");
    WiFi.begin(ssid.c_str(), password.c_str());
    int retry = 0;
    while (WiFi.status() != WL_CONNECTED && retry < 20) {
        delay(500);
        Serial.print(".");
        retry++;
    }
    if(WiFi.status() == WL_CONNECTED) {
        Serial.println("\nWiFi Connected!");
        configTime(gmtOffset_sec, daylightOffset_sec, ntpServer);
        Serial.println("Synchronizing Time...");
    }
    else Serial.println("\nWiFi Failed");
}

bool NetworkManager::sendToServer(String payload) {
    if (WiFi.status() != WL_CONNECTED) return false;

    WiFiClient client;
    HTTPClient http;
    http.setTimeout(10000); // 10s timeout

    if (http.begin(client, serverUrl)) {
        http.addHeader("Content-Type", "application/json");
        http.addHeader("X-API-KEY", apiKey);
        http.addHeader("ngrok-skip-browser-warning", "true");
        Serial.println("Sending data to server: " + payload);

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

String NetworkManager::getTimestamp() {
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

bool NetworkManager::isConnected() {
    return WiFi.status() == WL_CONNECTED;
}
