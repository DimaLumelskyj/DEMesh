using ippt.dem.mesh.entities.core;
using ippt.dem.mesh.repository;

namespace ippt.dem.mesh.entities.discrete.element
{
    public interface IDiscreteElement
    {
        public long GetId();

        public ElementType GetType();
        
        public long GetCenterNodeId();

        public double GetRadius();

        public int GetGroupId();

        public string ToString(FileFormat format);
    }
}