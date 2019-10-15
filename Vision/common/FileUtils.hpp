#ifndef FileUtils_h
#define FileUtils_h

#include <opencv4/opencv2/opencv.hpp>
#include <filesystem>

std::size_t nFilesInDir(std::filesystem::path path);

#endif