#include "Database.h"       // Database 클래스
#include <ctime>

DataBase::DataBase(const std::string& dbName) {
    int rc = sqlite3_open(dbName.c_str(), &db);
    if (rc) {
        std::cerr << "Can't open database: " << sqlite3_errmsg(db) << std::endl;
        db = nullptr;
    }
}

DataBase::~DataBase() {
    if (db) {
        sqlite3_close(db);
    }
}

std::string DataBase::getCurrentTimestamp() {
    time_t now = time(nullptr);
    char timestamp[20];
    strftime(timestamp, sizeof(timestamp), "%Y-%m-%d %H:%M:%S", localtime(&now));
    return std::string(timestamp);
}

int DataBase::getLastId() {
    sqlite3_stmt* stmt;
    const char* sql = "SELECT MAX(ID) FROM CAR;";
    int lastId = 0;
    int rc = sqlite3_prepare_v2(db, sql, -1, &stmt, 0);
    if (rc == SQLITE_OK) {
        if (sqlite3_step(stmt) == SQLITE_ROW) {
            if (sqlite3_column_type(stmt, 0) != SQLITE_NULL) {
                lastId = sqlite3_column_int(stmt, 0);
            }
        }
        sqlite3_finalize(stmt);
    }
    else {
        std::cout << "SQL error: " << sqlite3_errmsg(db) << std::endl;
    }
    return lastId;
}

void DataBase::insertData(int id, const std::string& plateNumber) {
    char* errMsg = 0;
    std::string timestamp = getCurrentTimestamp();
    std::string sql = "BEGIN TRANSACTION;"
        "INSERT INTO CAR (ID, PLATE_NUMBER, CREATED_AT) VALUES "
        "(" + std::to_string(id) + ", '" + plateNumber + "', '" + timestamp + "');"
        "COMMIT;";
    int rc = sqlite3_exec(db, sql.c_str(), NULL, NULL, &errMsg);
    if (rc != SQLITE_OK) {
        std::cout << "SQL error: " << errMsg << std::endl;
        sqlite3_free(errMsg);
    }
    else {
        std::cout << "기록을 성공적으로 남겼습니다." << std::endl;
    }
}

void DataBase::printAllData() {
    sqlite3_stmt* stmt;
    int rc;

    const char* sql = "SELECT * FROM CAR;";

    rc = sqlite3_prepare_v2(db, sql, -1, &stmt, nullptr);
    if (rc != SQLITE_OK) {
        std::cerr << "Failed to prepare statement: " << sqlite3_errmsg(db) << std::endl;
        return;
    }

    while ((rc = sqlite3_step(stmt)) == SQLITE_ROW) {
        int cols = sqlite3_column_count(stmt);
        for (int col = 0; col < cols; col++) {
            const char* colName = sqlite3_column_name(stmt, col);
            const char* colText = (const char*)sqlite3_column_text(stmt, col);
            std::cout << colName << ": " << (colText ? colText : "NULL") << "\t";
        }
        std::cout << std::endl;
    }

    if (rc != SQLITE_DONE) {
        std::cerr << "Failed to execute statement: " << sqlite3_errmsg(db) << std::endl;
    }

    sqlite3_finalize(stmt);
}
