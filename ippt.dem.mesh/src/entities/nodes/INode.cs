using System.Collections.Generic;

namespace ippt.dem.mesh.entities.nodes
{
    public interface INode
    {
        public long GetId();
        public List<double> GetCoordinates();
    }
}