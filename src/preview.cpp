#include <opencv2/opencv.hpp>

namespace preview {
    void regeneration(cv::VideoCapture cap) {
        cv::Mat frame;

        while(true) {
            cap >> frame;
            if(frame.empty() == true) {
                break;
            }
            cv::imshow("preview", frame);
            cv::waitKey(33);
        }
    }
}
