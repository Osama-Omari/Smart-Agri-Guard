#ifndef NETWORK_MANAGER_H
#define NETWORK_MANAGER_H

#include <Arduino.h>
#include <WiFi.h>
#include <HTTPClient.h>
#include <WiFiClient.h>
#include <time.h>
#include <sys/time.h>

class NetworkManager {
private:
    String ssid;
    String password;
    String serverUrl;
    String apiKey;
    const char* ntpServer = "pool.ntp.org";
    const long gmtOffset_sec = 10800; // UTC +3
    const int daylightOffset_sec = 0;

public:
    NetworkManager(String ssid, String password, String serverUrl, String apiKey);
    void setup_wifi();
    bool sendToServer(String payload);
    String getTimestamp();
    bool isConnected();
};

#endif
