#include "StorageManager.h"

StorageManager::StorageManager(NetworkManager* nm) {
    this->networkManager = nm;
}

void StorageManager::begin() {
    if (!LittleFS.begin(true)) {
        Serial.println("LittleFS Mount Failed");
    }
}

void StorageManager::saveToCache(String payload) {
    File file = LittleFS.open(CACHE_FILE, FILE_APPEND);
    if (!file) {
        Serial.println("Failed to open cache file");
        return;
    }
    file.println(payload); // Store as a new line
    file.close();
    Serial.println("Data saved to local cache.");
}

void StorageManager::syncCache() {
    if (!LittleFS.exists(CACHE_FILE)) return;
    if (!networkManager->isConnected()) return;

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
            if (networkManager->sendToServer(cachedPayload)) {
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
