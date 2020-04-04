#include <stdio.h>
#include <string>
#include <opencv2/opencv.hpp>

using namespace cv;

int main(int argc, char** argv) {
    Mat image;
    std::string s;
    std::cin >> s;
    image = imread(s, 1);
    if ( !image.data ) {
        printf("No image data \n");
        return -1;
    }
    namedWindow("Display Image", WINDOW_AUTOSIZE );
    imshow("Display Image", image);
    waitKey(0);
    return 0;
}
