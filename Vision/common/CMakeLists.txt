cmake_minimum_required(VERSION 3.8)
project(libFileUtils)

set(CMAKE_CXX_STANDARD 17)

if(NOT CMAKE_BUILD_TYPE) 
    set(CMAKE_BUILD_TYPE Debug)
endif(NOT CMAKE_BUILD_TYPE)

find_package( OpenCV REQUIRED )

add_library(FileUtils FileUtils.cpp FileUtils.hpp)
target_link_libraries( FileUtils -lstdc++fs)
target_link_libraries( FileUtils ${OpenCV_LIBS} )
