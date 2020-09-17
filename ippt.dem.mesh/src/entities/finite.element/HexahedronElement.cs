using System.Collections.Generic;
using ippt.dem.mesh.entities.core;
using ippt.dem.mesh.entities.discrete.element;

namespace ippt.dem.mesh.entities.finite.element
{
    public class HexahedronElement : IElement
    {
        private readonly long _id;
        
        private readonly List<long> _verticesId;

        private const ElementType Type = ElementType.Hexahedron;
        
        private readonly int _groupId;

        public HexahedronElement(ElementDto elementDto)
        {
            _verticesId = elementDto.GetNodesId();
            _id = elementDto.GetId();
            _groupId = elementDto.GetGroupId();
        }
        
        public long GetId()
        {
            return _id;
        }

        public new ElementType GetType()
        {
            return Type;
        }

        public List<long> GetVerticesId()
        {
            return _verticesId;
        }

        public IDiscreteElement GetSimpleFilledSphereDiscreteElement()
        {
            throw new System.NotImplementedException();
        }

        public int GetGroup()
        {
            return _groupId;
        }
    }
}