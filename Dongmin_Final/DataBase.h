#ifndef DATABASE_H
#define DATABASE_H

#include <sqlite3.h>
#include <string>
#include <iostream>

class DataBase {
public:
    DataBase(const std::string& dbName);
    ~DataBase();
    int getLastId();
    void insertData(int id, const std::string& plateNumber);
    void printAllData();
private:
    sqlite3* db;
    std::string getCurrentTimestamp();
};

#endif // DATABASE_H
