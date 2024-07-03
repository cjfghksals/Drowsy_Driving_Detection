#define _CRT_SECURE_NO_WARNINGS

#include "sqlite3.h"
#include <ctime>
#include <string>
#include <iostream>
#include <boost/asio.hpp>
#include <thread>
#include <mutex>

using boost::asio::ip::tcp;
using namespace std;

const string DB_NAME = "data.db";
const int BUFFER_SIZE = 1024;

std::mutex db_mutex;

void clearConsole() {
    cout << "\033[2J\033[1;1H"; // ANSI 이스케이프 시퀀스를 사용하여 콘솔 지우기
}

string getCurrentTimestamp() {
    time_t now = time(nullptr);
    char timestamp[20];
    strftime(timestamp, sizeof(timestamp), "%Y-%m-%d %H:%M:%S", localtime(&now));
    return std::string(timestamp);
}

int getLastId(sqlite3* db) {
    sqlite3_stmt* stmt;
    const char* sql = "SELECT MAX(ID) FROM CAR;";
    int lastId = 0;
    int rc = sqlite3_prepare_v2(db, sql, -1, &stmt, nullptr);
    if (rc == SQLITE_OK) {
        if (sqlite3_step(stmt) == SQLITE_ROW) {
            if (sqlite3_column_type(stmt, 0) != SQLITE_NULL) {
                lastId = sqlite3_column_int(stmt, 0);
            }
        }
        sqlite3_finalize(stmt);
    }
    else {
        cout << "SQL error: " << sqlite3_errmsg(db) << endl;
    }
    return lastId;
}

void insertData(sqlite3* db, int id, const std::string& plateNumber) {
    char* errMsg = nullptr;
    std::string timestamp = getCurrentTimestamp();
    std::string sql = "BEGIN TRANSACTION;"
        "INSERT INTO CAR (ID, PLATE_NUMBER, CREATED_AT) VALUES "
        "(" + to_string(id) + ", '" + plateNumber + "', '" + timestamp + "');"
        "COMMIT;";
    int rc = sqlite3_exec(db, sql.c_str(), nullptr, nullptr, &errMsg);
    if (rc != SQLITE_OK) {
        cout << "SQL error: " << errMsg << endl;
        sqlite3_free(errMsg);
    }
    else {
        cout << "기록을 성공적으로 남겼습니다." << endl;
    }
}

void printAllData() {
    sqlite3* db;
    sqlite3_stmt* stmt;
    int rc;

    std::lock_guard<std::mutex> guard(db_mutex); // 동기화

    rc = sqlite3_open(DB_NAME.c_str(), &db);
    if (rc) {
        cerr << "Can't open database: " << sqlite3_errmsg(db) << endl;
        return;
    }

    const char* sql = "SELECT * FROM CAR;";
    rc = sqlite3_prepare_v2(db, sql, -1, &stmt, nullptr);
    if (rc != SQLITE_OK) {
        cerr << "Failed to prepare statement: " << sqlite3_errmsg(db) << endl;
        sqlite3_close(db);
        return;
    }

    while ((rc = sqlite3_step(stmt)) == SQLITE_ROW) {
        int cols = sqlite3_column_count(stmt);
        for (int col = 0; col < cols; col++) {
            const char* colName = sqlite3_column_name(stmt, col);
            const char* colText = (const char*)sqlite3_column_text(stmt, col);
            cout << colName << ": " << (colText ? colText : "NULL") << "\t";
        }
        cout << endl;
    }

    if (rc != SQLITE_DONE) {
        cerr << "Failed to execute statement: " << sqlite3_errmsg(db) << endl;
    }

    sqlite3_finalize(stmt);
    sqlite3_close(db);
}

void session(tcp::socket socket) {
    sqlite3* db = nullptr;
    int rc = sqlite3_open(DB_NAME.c_str(), &db);
    if (rc != SQLITE_OK) {
        cout << "데이터베이스를 열지 못했습니다: " << sqlite3_errmsg(db) << endl;
        return;
    }

    try {
        for (;;) {
            char buffer[BUFFER_SIZE];
            boost::system::error_code error;

            size_t len = socket.read_some(boost::asio::buffer(buffer), error);

            if (error == boost::asio::error::eof) {
                break;
            }
            else if (error) {
                throw boost::system::system_error(error);
            }

            clearConsole();
            cout << "신호위반이 감지되었습니다." << endl;

            if (string(buffer, len) == "p") {
                lock_guard<mutex> guard(db_mutex);
                int lastId = getLastId(db);
                int newId = lastId + 1;
                insertData(db, newId, "78가1234");
            }
        }
        sqlite3_close(db);
        printAllData();
    }
    catch (exception& e) {
        cerr << "세션에서 예외 발생: " << e.what() << endl;
    }
}

void server(boost::asio::io_service& io_service, short port) {
    tcp::acceptor acceptor(io_service, tcp::endpoint(tcp::v4(), port));
    for (;;) {
        tcp::socket socket(io_service);
        acceptor.accept(socket);
        thread(session, move(socket)).detach();
    }
}

int main() {
    try {
        boost::asio::io_service io_service;
        server(io_service, 12345);
    }
    catch (exception& e) {
        cerr << "예외 발생: " << e.what() << endl;
    }
    return 0;
}
