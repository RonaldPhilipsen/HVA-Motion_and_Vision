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

        cv::Mat r;
        // Grab the red channel only
        cv::extractChannel(img, r, 0);
        //cv::imshow("Red channel", r);
        cv::Ptr<cv::SimpleBlobDetector> d = cv::SimpleBlobDetector::create();
        //Detect blobs
        std::vector<cv::KeyPoint> keypoints;
        d->detect(r, keypoints);

        cv::Mat im_with_keypoints;
        //drawKeypoints(img, keypoints, im_with_keypoints, cv::Scalar(0, 0, 255), cv::DrawMatchesFlags::DRAW_RICH_KEYPOINTS);

        // Show blobs
        //imshow("keypoints", im_with_keypoints);

        //std::stringstream output;
        std::cout << imageName.str() << ": Number: " << keypoints.size() << std::endl;
    }

    cv::waitKey(0);
}