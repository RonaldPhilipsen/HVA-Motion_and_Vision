#include <iostream>
#include <vector>
#include "Kinematics.hpp"

void printStatus(std::vector<Bone> bones)
{
    std::cout << "\n\nPrinting current link lenghts and current joint angles" << std::endl;
    for (int i = 0; i < bones.size(); i++)
    {
        auto pos = Kinematics::Forward(bones, i);
        std::cout
            << "Link[" << i << "] length:\t" << bones[i].length << "\n"
            << "\tangle: " << bones[i].angle << "\n"
            << "\tPos {X: " << pos.x << ", Y: " << pos.y << ", Z: " << pos.z << " }\n\n";
    }
}

int main()
{
    std::vector<Bone> bones;
    Vector3 targetPos(18, 12, 0);

    bones.push_back(Bone(0, 0));
    bones.push_back(Bone(10, 45));
    bones.push_back(Bone(10, 15));

    Vector3 endpos = Kinematics::Forward(bones);
    //std::cout << "End effector pos: " << endpos << std::endl;
    //printStatus(bones);

    Kinematics::Inverse(bones, targetPos);
    endpos = Kinematics::Forward(bones);
    //printStatus(bones);
    std::cout << "End effector pos:\t" << endpos << std::endl;
    std::cout << "Target position:\t" << targetPos << std::endl;
}