#include <string>
#include <opencv2/opencv.hpp>
#include "src/preview.cpp"

/*
 * S >> 映像リンク
 * N >> N個の編集タスクを受け取る
 * T1,T2, ... , TN >> タスク
 *
 * text >> テキスト表示
 * shape >> 図形表示
 * image >> 画像表示
 */

int main(int argc, char* argv[]) {
    cv::VideoCapture cap;

    std::string video_path;
    std::cin >> video_path;
    cap.open(video_path);

    if(cap.isOpened() == false) {
        std::cout << "ファイルが破損している可能性があります" << std::endl;
        return -1;
    }

    int tasks_count;
    std::cin >> tasks_count;

    for(int i = 0; i < tasks_count; ++i) {
        std::string task_type;
        std::cin >> task_type;

        if(task_type == "text") {

        }
    }

    return 0;
}
