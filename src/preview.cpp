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
            int key = cv::waitKey(33);
            int frame_position = cap.get(cv::CAP_PROP_POS_FRAMES);

            if(key == ' ') {
                cv::waitKey();
            } else if(key == 'j') {
                cap.set(cv::CAP_PROP_POS_FRAMES, frame_position - 20);
            } else if(key == 'k') {
                cap.set(cv::CAP_PROP_POS_FRAMES, frame_position + 20);
            } else if(key == 0x1b) {
                break;
            }
        }
    }

    void output_movie() {

    }
}
