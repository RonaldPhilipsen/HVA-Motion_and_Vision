cmake_minimum_required(VERSION 2.8.9)
project(vision)

if(NOT CMAKE_BUILD_TYPE) 
    set(CMAKE_BUILD_TYPE Debug)
endif(NOT CMAKE_BUILD_TYPE)

find_package( OpenCV REQUIRED )

add_executable(CheckConnector ./Assignment1/CheckConnector.cpp)
target_link_libraries( CheckConnector ${OpenCV_LIBS} )