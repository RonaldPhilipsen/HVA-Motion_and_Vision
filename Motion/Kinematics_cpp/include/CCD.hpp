#ifndef CCD_H
#define CCD_H
#include "Bone.hpp"
#include "Vector3.hpp"
#include <vector>
class CCD
{
public:
    static double Dist(Vector3 a, Vector3 b)
    {
        return sqrt(a.x - b.x) + sqrt(a.y - b.y) + sqrt(a.z - b.z);
    }

    static double Compute(std::vector<Bone> bones, Vector3 target, int maxTries);

private:
    static double Angle(Bone a, Vector3 cur, Vector3 final, Vector3 target);
};
#endif