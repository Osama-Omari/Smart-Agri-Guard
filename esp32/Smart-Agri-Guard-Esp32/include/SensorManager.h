#ifndef SENSOR_MANAGER_H
#define SENSOR_MANAGER_H

#include <Arduino.h>
#include <DHT.h>
#include <ModbusMaster.h>

struct AirData {
    float temperature;
    float humidity;
    bool isValid;
};

struct SoilData {
    float moisture;
    float ph;
    float nitrogen;
    float phosphorus;
    float potassium;
    bool isValid;
};

class SensorManager {
private:
    DHT dht;
    ModbusMaster node;
    const uint8_t DHT_PIN = 4;
    const uint8_t DHT_TYPE = DHT22;
    const uint8_t RX2_PIN = 16;
    const uint8_t TX2_PIN = 17;

public:
    SensorManager();
    void begin();
    AirData readAirSensors();
    SoilData readSoilSensors();
    String formatJSON(const AirData& air, const SoilData& soil, String timestamp);
};

#endif
