#ifndef BONE_H
#define BONE_H

#include <boost/operators.hpp>
#include <boost/math/constants/constants.hpp>

class Bone
{
public:
    double length;
    double angle;
    double rads;
    double rot;
    double damping;
    double maxAngle;
    double MinAngle;

    Bone(double length, double angle);

    static double toRads(double degrees)
    {
        return ((degrees / 180) * boost::math::constants::pi<double>());
    };

    static double toDegrees(double rads)
    {
        return ((rads / boost::math::constants::pi<double>()) * 180);
    };
};

#endif