#include <iostream>
#include <vector>
#include "Kinematics.hpp"

void printAngles(std::vector<Bone> bones)
{
    std::cout << "Printing current link lenghts and current joint angles" << std::endl;
    for (int i = 0; i < bones.size(); i++)
    {
        std::cout << "Link["
                  << i
                  << "] length: "
                  << bones[i].length
                  << " , angle: "
                  << bones[i].angle
                  << std::endl;
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
    std::cout << "End effector pos: " << endpos << std::endl;
    printAngles(bones);

    Kinematics::Inverse(bones, targetPos);
    endpos = Kinematics::Forward(bones);
    std::cout << "New end effector pos: " << endpos << std::endl;
    printAngles(bones);
}