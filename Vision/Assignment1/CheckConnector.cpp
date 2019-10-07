#include "iostream"
#include <opencv4/opencv2/opencv.hpp>

int main(int argc, char **argv)
{
    cv::Mat img, dst;
    int x, y, nIntersections = 0;

    if (argc < 2)
    {
        std::cout << "usage:"
                  << "CheckConnector <Path/To/Image>"
                  << std::endl;

        return -1;
    }

    // Read in the image
    img = cv::imread(argv[1]);
    if (!img.data)
    {
        std::cout << "Could not open or find: "
                  << argv[1]
                  << std::endl;
        return -1;
    }

    cv::Canny(img, dst, 50, 200, 3);

    y = dst.cols - 20;
    for (x = 0; x < dst.rows; x++)
    {
        // If line found
        if ((int)dst.at<uchar>(x, y) == 255)
        {
            nIntersections++;
        }

        // Set pixel to 255 (white) to draw a line
        dst.at<uchar>(x, y) = 255;
    }

    std::cout << argv[1]
              << ", N intersections: "
              << nIntersections
              << std::endl;

    if (nIntersections >= 8)
    {
        // 4 x 2 = 8 lines
        std::cout << ", the 4 wires are connected" << std::endl;
    }
    else
    {
        std::cout << ", the 4 wires aren't connected" << std::endl;
    }

    cv::imshow("Original image:", img);
    cv::imshow("Sobel filtered", img);

    std::cout << "Press any key to exit";
    cv::waitKey(0);
}