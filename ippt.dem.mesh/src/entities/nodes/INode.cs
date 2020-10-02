using System.Collections.Generic;
using ippt.dem.mesh.repository;

namespace ippt.dem.mesh.entities.nodes
{
    public interface INode
    {
        public long GetId();
        public List<double> GetCoordinates();
        public string ToString(FileFormat format);
    }
}