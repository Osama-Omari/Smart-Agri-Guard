#ifndef STORAGE_MANAGER_H
#define STORAGE_MANAGER_H

#include <Arduino.h>
#include <LittleFS.h>
#include "NetworkManager.h"

class StorageManager {
private:
    const char* CACHE_FILE = "/sensor_cache.txt";
    NetworkManager* networkManager;

public:
    StorageManager(NetworkManager* networkManager);
    void begin();
    void saveToCache(String payload);
    void syncCache();
};

#endif
