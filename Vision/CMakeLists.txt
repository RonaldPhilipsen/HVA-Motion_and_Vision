cmake_minimum_required(VERSION 3.8)
project(vision)

set(CMAKE_CXX_STANDARD 17)

if(NOT CMAKE_BUILD_TYPE) 
    set(CMAKE_BUILD_TYPE Debug)
endif(NOT CMAKE_BUILD_TYPE)

find_package( OpenCV REQUIRED )

add_subdirectory(common)
include_directories(common)

add_executable(CheckConnector ./Assignment1/CheckConnector.cpp ./Assignment1/CheckConnector.hpp)
add_executable(CheckBracket ./Assignment2/CheckBracket.cpp ./Assignment2/CheckBracket.hpp)
add_executable(CheckDice ./Assignment9/CheckDice.cpp ./Assignment9/CheckDice.hpp)
add_executable(CheckDiceBlobs ./Assignment10/CheckDice.cpp ./Assignment10/CheckDice.hpp)


target_link_libraries( CheckConnector ${OpenCV_LIBS} FileUtils)
target_link_libraries( CheckBracket ${OpenCV_LIBS} FileUtils)
target_link_libraries( CheckDice ${OpenCV_LIBS} FileUtils)
target_link_libraries( CheckDiceBlobs ${OpenCV_LIBS} FileUtils)