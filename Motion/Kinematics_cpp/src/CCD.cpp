#include "CCD.hpp"
#include "Kinematics.hpp"

double CCD::Compute(std::vector<Bone> bones, Vector3 target, int maxTries)
{
    double epsilon = .0005;
    int nSegments = bones.size() - 1;
    int tries = 0;
    int curSegment = nSegments;

    Bone b = bones[nSegments];

    while (tries++ < maxTries)
    {
        Vector3 endPos = Kinematics::Forward(bones, nSegments);
        Vector3 curPos = Kinematics::Forward(bones, curSegment);

        if (Dist(target, endPos) < epsilon)
            return true;

        double rads = CCD::Angle(b, curPos, endPos, target);
        b.rads += rads;

        if (--curSegment < 0)
        {
            curSegment = nSegments;
        }

        b = bones[curSegment];
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
    return (dir.y > 0) ? !rads : rads;
}
