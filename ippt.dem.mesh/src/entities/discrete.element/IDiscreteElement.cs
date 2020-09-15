using ippt.dem.mesh.entities.core;

namespace ippt.dem.mesh.entities.discrete.element
{
    public interface IDiscreteElement
    {
        public long GetId();

        public ElementType GetType();
        
        public long GetCenterNodeId();

        public double GetRadius();
    }
}