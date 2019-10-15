#include "FileUtils.hpp"

std::size_t nFilesInDir(std::filesystem::path path)
{
    using std::filesystem::directory_iterator;
    return std::distance(directory_iterator(path), directory_iterator{});
}