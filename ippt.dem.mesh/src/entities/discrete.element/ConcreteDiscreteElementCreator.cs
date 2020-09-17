using ippt.dem.mesh.entities.core;

namespace ippt.dem.mesh.entities.discrete.element
{
    public class ConcreteDiscreteElementCreator : DiscreteElementCreator
    {
        public override IDiscreteElement FactoryMethod(DiscreteElementDto elementDto)
        {
            return new DiscreteSphereElement(elementDto);
        }
    }
}