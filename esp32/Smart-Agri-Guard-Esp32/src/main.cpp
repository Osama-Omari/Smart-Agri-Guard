#include <Arduino.h>
#include "credentials.h"
#include "SensorManager.h"
#include "NetworkManager.h"
#include "StorageManager.h"

// Instantiate Managers
SensorManager sensorManager;
NetworkManager networkManager(ssid, password, serverUrl, apiKey);
StorageManager storageManager(&networkManager);

// Timing variables
long lastMsg = 0;
const long interval = 300000; // 5 minutes

void setup() {
  Serial.begin(115200);

  // Initialize subsystems
  storageManager.begin();
  sensorManager.begin();
  networkManager.setup_wifi();
}

void loop() { 
  long now = millis();
  
  if (now - lastMsg > interval || lastMsg == 0) {
    lastMsg = now;

    // 1. Read Sensors
    AirData air = sensorManager.readAirSensors();
    SoilData soil = sensorManager.readSoilSensors();

    // 2. Prepare Payload
    String currentTime = networkManager.getTimestamp();
    String jsonPayload = sensorManager.formatJSON(air, soil, currentTime);

    // 3. Sync & Send Logic
    if (networkManager.isConnected()) {
      // Try to push old data first
      storageManager.syncCache();
      
      // Try to push current data
      if (!networkManager.sendToServer(jsonPayload)) {
        storageManager.saveToCache(jsonPayload);
      }
    } else {
      Serial.println("Offline. Caching data...");
      storageManager.saveToCache(jsonPayload);
      networkManager.setup_wifi(); // Attempt reconnect for next time
    }
  }
}