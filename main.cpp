#include <stdio.h>
#include <string>
#include <opencv2/opencv.hpp>
#include "src/preview.cpp"

int main(int argc, char** argv) {
    cv::VideoCapture cap;
    std::string video_path;
    std::cout << "動画ファイルのパスを入力してください: ";
    std::cin >> video_path;

    cap.open(video_path);
    if(cap.isOpened() == false) {
        std::cout << "ファイルが破損している可能性があります" << std::endl;
        return -1;
    }

    int command = -1;
    std::cout << "動画に対する操作を選んでください: ";
    std::cin >> command;

    if(command == 0) {
        preview::regeneration(cap);
    } else if(command == 1) {
        preview::output_movie(cap);
    } else if(command == 2){
        std::string text;
        std::cout << "表示したいテキストを入力してください: ";
        std::cin >> text;
        preview::on_text_output(cap, text);
    } else {
        std::cout << "そのコマンドは実装されていません！" << std::endl;
    }
    return 0;
}
