/*
/////////////////실행 방법/////////////////
////터미널 창에 아래와 같이 순서대로 입력////
g++ -o GAME GAME.cpp -lportaudio -lsndfile -lcurl -lespeak -ljsoncpp -I/usr/include/jsoncpp -lwiringPi -lpthread `pkg-config --cflags --libs opencv4`
./GAME
//////////////////////////////////////////
*/

#include <iostream>
#include <vector>
#include <thread>
#include <opencv2/opencv.hpp>

#include "api.h"
#include "TTS.h"
#include "STT.h"

using namespace std;

void displayCameraFeed() {
    // 카메라 장치를 열기
    cv::VideoCapture cap(0); // 0번 카메라를 열거나, 웹캠을 사용하려면 0을 사용합니다. 다른 카메라를 사용할 경우 해당 장치 번호를 사용하세요.

    // 카메라가 올바르게 열렸는지 확인
    if (!cap.isOpened()) {
        std::cerr << "Error: Failed to open camera" << std::endl;
        return;
    }

    // 윈도우 생성
    cv::namedWindow("Camera Feed", cv::WINDOW_NORMAL);

    // 영상을 읽어와서 화면에 표시
    while (true) {
        cv::Mat frame;
        cap >> frame; // 카메라로부터 한 프레임을 읽어옴

        // 영상이 없으면 종료
        if (frame.empty()) {
            std::cerr << "Error: Failed to capture frame" << std::endl;
            break;
        }

        // 화면에 영상 표시
        cv::imshow("Camera Feed", frame);

        // 'q' 키를 누르면 종료
        if (cv::waitKey(1) == 'q') {
            break;
        }
    }

    // 리소스 해제 및 윈도우 닫기
    cap.release();
    cv::destroyAllWindows();
}

void Game_start()
{
    locale::global(locale(""));
    
    // 이미 있는 단어 알기위해 단어목록 저장
    vector<wstring> history;
    bool playing = true;

    locale originalLocale = locale::global(locale(""));

    wstring answord = L"";
    wstring sword = L"";
    wstring query = L"";
    wstring squery = L"";

    cout << "졸음 방지를 위해 끝말잇기를 시작합니다.\n"
        "'그만'을 입력하면 게임이 종료되며, '다시'를 입력하여 게임을 다시 시작할 수 있습니다.\n"
        "가장 처음 단어를 제시하면 끝말잇기가 시작됩니다\n";
    TTS(L"졸음 방지를 위해 끝말잇기를 시작합니다.\n"
        "그만을 입력하면 게임이 종료되며, 다시를 입력하여 게임을 다시 시작할 수 있습니다.\n"
        "가장 처음 단어를 제시하면 끝말잇기가 시작됩니다\n");

    while (playing) 
    {
        bool wordOK = false;    //단어 입력 완료 여부

        while (!wordOK) 
        {
            wordOK = true;
            wcin >> query;
            // query = STT();
            wcout << query << endl;

            if (query == L"그만") 
            {
                playing = false;
                cout << "컴퓨터의 승리!\n";
                TTS(L"컴퓨터의 승리");
                break;
            }
            else if (query == L"다시") 
            {
                history.clear();
                answord = L"";
                cout << "게임을 다시 시작합니다.\n";
                TTS(L"게임을 다시 시작합니다");
                wordOK = false;
            }
            else 
            {
                if (query.empty()) {
                    wordOK = false;
                    if (history.empty()) {
                        cout << "단어를 말하십시오.\n";
                        TTS(L"단어를 말하십시오");
                    }
                }
                else
                {
                    squery = query[0];
                    if (query.size() == 1)
                    {
                        wordOK = false;
                        cout << "적어도 두 글자가 되어야 합니다.\n";
                        TTS(L"적어도 두 글자가 되어야 합니다");
                    }
                    if (find(history.begin(), history.end(), query) != history.end()) 
                    {
                        wordOK = false;
                        cout << "이미 입력한 단어입니다\n";
                        TTS(L"이미 입력한 단어입니다");
                    }
                    if (sword.compare(squery) && sword != L"")
                    {
                        wordOK = false;
                        cout << utf8_encode(sword) << "으로 시작하는 단어를 입력해 주십시오.\n";
                        TTS(sword + L"으로 시작하는 단어를 입력해 주십시오");
                    }
                    if (wordOK) //단어 유효성 여부 확인
                    {
                        wstring ans = checkexists(query);
                        if (ans == L"")
                        {
                            wordOK = false;
                            cout << "유효한 단어를 입력해 주십시오." << endl;
                            TTS(L"유효한 단어를 입력해 주십시오");
                        }
                    }
                }
            }
        }
        history.push_back(query);

        if (playing)
        {
            wchar_t start_c = query[query.length() - 1]; 
            wstring start(1, start_c);

            answord = findword(start + L"*", history);
            if (answord == L"")
            {
                cout << "당신의 승리" << endl;
                TTS(L"당신의 승리");
                break;
            }
            else
            {
                history.push_back(answord);
                sword = answord[answord.length() - 1];
                cout << utf8_encode(answord) << endl;
                TTS(answord);
            }
        }
    }
}

int main()
{
    thread GAME_thread(GAME);
    thread displayCameraFeed_thread(displayCameraFeed);
    GAME_thread.join();
    displayCameraFeed_thread.join();
    // GAME();

    return 0;
}