#include "CheckDice.hpp"

int main(int argc, char **argv)
{
    if (argc < 2)
    {
        std::cout << "usage:"
                  << "CheckDice <Path/To/Images>"
                  << std::endl;

        return -1;
    }

    auto nFiles = nFilesInDir(argv[1]);

    for (int i = 1; i <= nFiles; i++)
    {
        std::stringstream imageName;
        imageName << "dice-" << i << "-md.png";

        std::stringstream ss;
        ss << argv[1] << imageName.str();

        auto img = cv::imread(ss.str());
        if (!img.data)
        {
            std::cout << "Could not open or find: "
                      << ss.str()
                      << std::endl;
            exit(-1);
        }

        cv::Mat gray;
        cv::cvtColor(img, gray, cv::COLOR_BGR2GRAY);
        medianBlur(gray, gray, 5);

        std::vector<cv::Vec3f> circles;
        cv::HoughCircles(gray, circles, cv::HOUGH_GRADIENT, 1,
                         gray.rows / 16,
                         100, 30, 1, 30);

        std::vector<cv::Point> centers;

        for (size_t i = 0; i < circles.size(); i++)
        {
            cv::Vec3i c = circles[i];
            centers.push_back(cv::Point(c[0], c[1]));
            // circle center
            circle(img, centers[i], 1, cv::Scalar(0, 100, 100), 3, cv::LINE_AA);
            // circle outline
            int radius = c[2];
            circle(img, centers[i], radius, cv::Scalar(255, 0, 255), 3, cv::LINE_AA);
        }
        std::stringstream output;
        std::cout << imageName.str() << ": Number: " << centers.size() << std::endl;
    }
}