using ippt.dem.mesh.entities.core;

namespace ippt.dem.mesh.entities.discrete.element
{
    public class DiscreteSphereElement : IDiscreteElement
    {
        private readonly long _id;
        private readonly double _radius;
        private readonly long _nodeId;
        private readonly int _groupId;
        private const ElementType Type = ElementType.Sphere;

        public DiscreteSphereElement(DiscreteElementDto elementDto)
        {
            _groupId = elementDto.GroupId;
            _nodeId = elementDto.NodeId;
            _radius = elementDto.Radius;
            _id = elementDto.Id;
        }

        public long GetId()
        {
            return _id;
        }

        public ElementType GetType()
        {
            return Type;
        }

        public long GetCenterNodeId()
        {
            return _nodeId;
        }

        public double GetRadius()
        {
            return _radius;
        }

        public int GetGroupId()
        {
            return _groupId;
        }
    }
}