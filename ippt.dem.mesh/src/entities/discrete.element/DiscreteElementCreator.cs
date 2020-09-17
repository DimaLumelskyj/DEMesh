using ippt.dem.mesh.entities.core;

namespace ippt.dem.mesh.entities.discrete.element
{
    public abstract class DiscreteElementCreator
    {
        public abstract IDiscreteElement FactoryMethod(DiscreteElementDto elementDto);
    }
}