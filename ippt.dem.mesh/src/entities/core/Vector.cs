using System;
using ippt.dem.mesh.entities.nodes;

namespace ippt.dem.mesh.entities.core
{
    public static class Vector
    {
        public static double Length(INode a, INode b)
        {
            double x1 = a.GetCoordinates()[0];
            double y1 = a.GetCoordinates()[1];
            double z1 = a.GetCoordinates()[2];
            double x2 = b.GetCoordinates()[0];
            double y2 = b.GetCoordinates()[1];
            double z2 = b.GetCoordinates()[2];
            return Math.Sqrt((x2-x1)*(x2-x1) + (y2-y1)*(y2-y1)) + (z2-z1)*(z2-z1);
        }
    } }