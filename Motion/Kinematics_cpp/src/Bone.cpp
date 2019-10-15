#include "Bone.hpp"

Bone::Bone(double length, double angle)
{
    this->length = length;
    this->angle = angle;
    this->rads = this->toRads(angle);
}