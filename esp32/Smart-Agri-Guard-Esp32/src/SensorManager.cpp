#include "SensorManager.h"

SensorManager::SensorManager() : dht(DHT_PIN, DHT_TYPE) {
}

void SensorManager::begin() {
    dht.begin();
    Serial2.begin(9600, SERIAL_8N1, RX2_PIN, TX2_PIN);
    node.begin(0x01, Serial2);
}

AirData SensorManager::readAirSensors() {
    AirData data;
    data.humidity = dht.readHumidity();
    data.temperature = dht.readTemperature();
    data.isValid = !(isnan(data.humidity) || isnan(data.temperature));
    return data;
}

SoilData SensorManager::readSoilSensors() {
    SoilData data = {0};
    uint8_t result = node.readHoldingRegisters(0x00, 8);
    
    if (result == node.ku8MBSuccess) {
        data.moisture = node.getResponseBuffer(1);
        data.ph = node.getResponseBuffer(3) / 100.0;
        data.nitrogen = (float)node.getResponseBuffer(4);
        data.phosphorus = (float)node.getResponseBuffer(5);
        data.potassium = (float)node.getResponseBuffer(6);
        data.isValid = true;
    } else {
        data.isValid = false;
    }
    return data;
}

String SensorManager::formatJSON(const AirData& air, const SoilData& soil, String timestamp) {
    String jsonPayload = "{";
    jsonPayload += "\"Timestamp\": \"" + timestamp + "\",";
    
    // Air Data & Status
    jsonPayload += "\"Temperature\": " + String(air.isValid ? String(air.temperature) : "null") + ","; 
    jsonPayload += "\"Humidity\": " + String(air.isValid ? String(air.humidity) : "null") + ","; 
    jsonPayload += "\"AirSensorStatus\": \"" + String(air.isValid ? "OK" : "Faulty") + "\",";

    // Soil Data & Status
    jsonPayload += "\"SoilMoisture\": " + String(soil.isValid ? String(soil.moisture) : "null") + ",";
    jsonPayload += "\"PH\": " + String(soil.isValid ? String(soil.ph) : "null") + ",";
    jsonPayload += "\"Potassium\": " + String(soil.isValid ? String(soil.potassium) : "null") + ",";
    jsonPayload += "\"Phosphorus\": " + String(soil.isValid ? String(soil.phosphorus) : "null") + ",";
    jsonPayload += "\"Nitrogen\": " + String(soil.isValid ? String(soil.nitrogen) : "null") + ",";
    jsonPayload += "\"SoilSensorStatus\": \"" + String(soil.isValid ? "OK" : "Faulty") + "\"";
    
    jsonPayload += "}";
    return jsonPayload;
}
