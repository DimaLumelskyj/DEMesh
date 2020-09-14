using ippt.dem.mesh.entities.core;

namespace ippt.dem.mesh.entities.finite.element
{
    public abstract class ElementCreator
    {
        public abstract IElement FactoryMethod(ElementDto elementDto);
    }
}