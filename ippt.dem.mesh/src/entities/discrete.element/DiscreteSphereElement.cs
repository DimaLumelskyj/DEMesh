using System;
using ippt.dem.mesh.entities.core;
using ippt.dem.mesh.repository;

namespace ippt.dem.mesh.entities.discrete.element
{
    public class DiscreteSphereElement : IDiscreteElement
    {
        private readonly long _id;
        private readonly double _radius;
        private readonly long _nodeId;
        private readonly int _groupId;
        private readonly long _finiteElementId;

        public DiscreteSphereElement(DiscreteElementDto elementDto)
        {
            _finiteElementId = elementDto.FiniteElementId;
            _groupId = elementDto.GroupId;
            _nodeId = elementDto.NodeId;
            _radius = elementDto.Radius;
            _id = elementDto.Id;
        }
        
        public DiscreteSphereElement(IDiscreteElement element)
        {
            _finiteElementId = element.GetFiniteElementId();
            _groupId = element.GetGroupId();
            _nodeId = element.GetId();
            _radius = element.GetRadius();
            _id = element.GetId();
        }

        public static DiscreteSphereElement Get(IDiscreteElement element)
        {
            return new DiscreteSphereElement(element);
        }

        public long GetId()
        {
            return _id;
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

        public string ToString(FileFormat format)
        {
            switch (format)
            {
                case FileFormat.Dat:
                    return $"            {_nodeId.ToString()} 1 {_radius.ToString()}";
                case FileFormat.Msh:
                    return $"            {_id.ToString()} {_nodeId.ToString()} {_radius.ToString()} 1";;
                default:
                    throw new Exception($"unknown file format: {format}");
            }
        }

        public long GetFiniteElementId()
        {
            return _finiteElementId;
        }
    }
}