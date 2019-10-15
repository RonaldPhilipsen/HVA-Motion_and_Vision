#include "CheckConnector.hpp"

int main(int argc, char **argv)
{
    cv::Mat img;

    if (argc < 2)
    {
        std::cout << "usage:"
                  << "CheckConnector <Path/To/Images>"
                  << std::endl;

        return -1;
    }

    auto nFiles = nFilesInDir(argv[1]);

    for (int i = 0; i < nFiles; i++)
    {
        char s[12];
        snprintf(s, sizeof(s), "Image%02d.jpg", i);
        std::string sAsString = s;
        auto imagePath = argv[1] + sAsString;
        img = cv::imread(imagePath, cv::IMREAD_GRAYSCALE);
        if (!img.data)
        {
            std::cout << "Could not open or find: "
                      << imagePath
                      << std::endl;
            exit(-1);
        }

        auto nIntersections = GetNumWires(img);

        std::cout << s << " :";
        if (nIntersections >= 8)
            std::cout << "the 4 wires are connected" << std::endl;
        else
            std::cout << "the 4 wires aren't connected" << std::endl;
    }
}

int GetNumWires(cv::Mat img)
{
    cv::Mat dst;
    cv::Canny(img, dst, 50, 200, 3);
    int x, y, nIntersections = 0;

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

    return nIntersections;
}