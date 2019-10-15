#include "CCD.hpp"
#include "Kinematics.hpp"

double CCD::Compute(std::vector<Bone> bones, Vector3 target, int maxTries)
{
    double epsilon = .0005;
    int nSegments = bones.size() - 1;
    int tries = 0;

    Bone b = bones[nSegments];
    Vector3 endPos = Kinematics::Forward(bones, nSegments);
    Vector3 curPos;

    while (tries++ < maxTries)
    {
        if (Dist(target, endPos) < epsilon)
            return true;
        double rads = CCD::Angle(b, curPos, endPos, target);
        b.rads += rads;

        if (--nSegments < 0)
        {
            nSegments = bones.size() - 1;
        }

        b = bones[nSegments];
    }
    return false;
}

double CCD::Angle(Bone a, Vector3 cur, Vector3 final, Vector3 target)
{
    auto cv = cur - final;
    auto tv = cur - target;
    cv.normalize();
    tv.normalize();

    double rads = cv.dot(tv);
    Vector3 dir = cv.cross(tv);
    a.rads = (rads > a.damping) ? a.damping : rads;
    return (dir.y > 0) ? rads : !rads;
}
