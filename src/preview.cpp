#include <opencv2/opencv.hpp>
#include <string>

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

    cv::VideoWriter get_video_writer(cv::VideoCapture cap) {
        const int width = cap.get(cv::CAP_PROP_FRAME_WIDTH);
        const int height = cap.get(cv::CAP_PROP_FRAME_HEIGHT);
        const cv::Size size = cv::Size(width, height);
        const double fps = cap.get(cv::CAP_PROP_FPS);
        const int extension = cv::VideoWriter::fourcc('m','p','4','v');
        cv::VideoWriter vw;
        vw.open("output.mp4", extension, fps, size);
        return vw;
    }

    void output_progress(cv::VideoCapture cap) {
        const double frame_position = cap.get(cv::CAP_PROP_POS_FRAMES);
        const double all_frames = cap.get(cv::CAP_PROP_FRAME_COUNT);
        const double frame_percent = frame_position / all_frames * 100;
        std::cout << "Progress: " << frame_percent << "%" << std::endl;
    }

    void output_movie(cv::VideoCapture cap) {
        cv::VideoWriter vw = get_video_writer(cap);
        cv::Mat frame, dst;
        while(true) {
            cap >> frame;
            if(frame.empty() == true) {
                break;
            }
            output_progress(cap);
            cv::flip(frame, dst, 1);
            vw << dst;
        }
    }

    void on_text_output(cv::VideoCapture cap, std::string text) {
        const int width = cap.get(cv::CAP_PROP_FRAME_WIDTH);
        const int height = cap.get(cv::CAP_PROP_FRAME_HEIGHT);
        cv::VideoWriter vw = get_video_writer(cap);
        cv::Mat frame;
        while(true) {
            cap >> frame;
            if(frame.empty() == true) {
                break;
            }
            output_progress(cap);
            cv::Point pnt = cv::Point(width / 2, height / 2);
            cv::putText(frame, text, pnt, cv::FONT_HERSHEY_PLAIN,
                    5.0, cv::Scalar(255,0,0), 2, cv::LINE_AA);
            vw << frame;
        }
    }
}
