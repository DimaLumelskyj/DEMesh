using System.Collections.Generic;
using ippt.dem.mesh.entities.core;
using ippt.dem.mesh.entities.discrete.element;

namespace ippt.dem.mesh.entities.finite.element
{
    public interface IElement
    {
        public long GetId();

        public ElementType GetType();
        
        public List<long> GetVerticesId();
        
        public IDiscreteElement GetSimpleFilledSphereDiscreteElement();

        public int GetGroup();
    }
}