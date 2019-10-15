cmake_minimum_required(VERSION 3.8)
project(motion)

set(CMAKE_CXX_STANDARD 17)

if(NOT CMAKE_BUILD_TYPE) 
    set(CMAKE_BUILD_TYPE Debug)
endif(NOT CMAKE_BUILD_TYPE)

set(Boost_USE_STATIC_LIBS OFF) 
set(Boost_USE_MULTITHREADED ON)  
set(Boost_USE_STATIC_RUNTIME OFF) 

find_package(Boost 1.67.0 REQUIRED ) 

set(HEADER_FILES include/Bone.hpp include/CCD.hpp include/Kinematics.hpp include/Vector3.hpp)

set(CMAKE_C_FLAGS ${CMAKE_C_FLAGS} "-msse4")
set(CMAKE_CXX_FLAGS ${CMAKE_CXX_FLAGS} "-msse4")

add_executable(Kinematics ./src/main.cpp ./src/CCD.cpp ./src/Kinematics.cpp ./src/Vector3.cpp ./src/Bone.cpp ${HEADER_FILES} )
target_include_directories(Kinematics PRIVATE include)
set_target_properties(Kinematics PROPERTIES COMPILE_OPTIONS "-m32" LINK_FLAGS "-m32")
