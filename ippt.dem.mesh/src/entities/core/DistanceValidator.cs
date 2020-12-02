﻿using ippt.dem.mesh.entities.nodes;

namespace ippt.dem.mesh.entities.core
{
    public static class DistanceValidator
    {
        public static bool IsNodeInSphereRange(INode center, INode node, double radius)
        {
            return (Vector.Length(center, node) <= radius);
        }
    }
}