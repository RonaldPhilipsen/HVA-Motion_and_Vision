#ifndef KINEMATICS_H
#define KINEMATICS_H

#include <vector>
#include "Vector3.hpp"
#include "Bone.hpp"
#include "CCD.hpp"

class Kinematics
{
public:
    static Vector3 Forward(std::vector<Bone> bones);
    static Vector3 Forward(std::vector<Bone> bones, int link);
    static void Inverse(std::vector<Bone> bones, Vector3 targetPosition);
};
#endif //KINEMATICS_KINEMATICS_H