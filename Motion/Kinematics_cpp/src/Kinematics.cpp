#include "Kinematics.hpp"

Vector3 Kinematics::Forward(std::vector<Bone> bones)
{
    Vector3 res;
    double cosTheta, sinTheta = 0;
    for (int i = 0; i < bones.size(); i++)
    {
        cosTheta += bones[i].rads;
        sinTheta += bones[i].rads;
        res.x += bones[i].length * cos(cosTheta);
        res.y += bones[i].length * sin(sinTheta);
    }
    return res;
}

Vector3 Kinematics::Forward(std::vector<Bone> bones, int link)
{
    Vector3 res;
    double cosTheta, sinTheta = 0;
    for (int i = 0; i < link; i++)
    {
        cosTheta += bones[i].rads;
        sinTheta += bones[i].rads;
        res.x += bones[i].length * cos(cosTheta);
        res.y += bones[i].length * sin(sinTheta);
    }
    return res;
}
void Kinematics::Inverse(std::vector<Bone> bones, Vector3 targetPosition)
{
    CCD::Compute(bones, targetPosition, 20);
}
