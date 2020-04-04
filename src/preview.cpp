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

    void output_movie(cv::VideoCapture cap) {
        int width = cap.get(cv::CAP_PROP_FRAME_WIDTH);
        int height = cap.get(cv::CAP_PROP_FRAME_HEIGHT);
        auto size = cv::Size(width, height);
        double fps = cap.get(cv::CAP_PROP_FPS);
        int fourcc = cv::VideoWriter::fourcc('m','p','4','v');
        cv::VideoWriter vw;
        vw.open("output.mp4", fourcc, fps, size);
        cv::Mat frame, dst;
        while(true) {
            cap >> frame;
            if(frame.empty() == true) {
                break;
            }
            cv::imshow("outputting...", frame);
            cv::flip(frame, dst, 1);
            vw << dst;
            cv::waitKey(1);
        }
    }
}
