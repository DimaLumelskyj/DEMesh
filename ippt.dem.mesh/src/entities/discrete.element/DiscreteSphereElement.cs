using System;
using System.Collections.Generic;
using ippt.dem.mesh.entities.core;
using ippt.dem.mesh.repository;

namespace ippt.dem.mesh.entities.discrete.element
{
    public class DiscreteSphereElement : IDiscreteElement
    {
        private readonly long _id;
        private readonly double _radius;
        private double _maxRemeshRadius;
        private readonly long _nodeId;
        private readonly int _groupId;
        private readonly long _finiteElementId;
        private HashSet<long> _neighboursElementsId;
        private bool _isOnInterface = false;
        private int _nOfLayersAroundTheElement = 0;
        
        public DiscreteSphereElement(DiscreteElementDto elementDto)
        {
            _finiteElementId = elementDto.FiniteElementId;
            _groupId = elementDto.GroupId;
            _nodeId = elementDto.NodeId;
            _radius = elementDto.Radius;
            _maxRemeshRadius = _radius;
            _id = elementDto.Id;
        }
        
        public DiscreteSphereElement(IDiscreteElement element)
        {
            _finiteElementId = element.GetFiniteElementId();
            _groupId = element.GetGroupId();
            _nodeId = element.GetId();
            _radius = element.GetRadius();
            _id = element.GetId();
            _isOnInterface = element.IsOnInterface();
            _neighboursElementsId = element.GetNeighbours();
            _nOfLayersAroundTheElement = element.GetNOfLayersAroundTheElement();
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
                    return $"            {_nodeId.ToString()} {_groupId.ToString()} {_radius.ToString()}";
                case FileFormat.Msh:
                    return $"            {_id.ToString()} {_nodeId.ToString()} {_radius.ToString()} {_groupId.ToString()}";;
                default:
                    throw new Exception($"unknown file format: {format}");
            }
        }

        public long GetFiniteElementId()
        {
            return _finiteElementId;
        }

        public void UpdateBoundaryData(HashSet<long> neighboursElementsId, bool isOnInterface)
        {
            _isOnInterface = isOnInterface;
            _neighboursElementsId = new HashSet<long>(neighboursElementsId);
        }

        public bool IsOnInterface()
        {
            return _isOnInterface;
        }

        public HashSet<long> GetNeighbours()
        {
            return _neighboursElementsId;
        }

        public void SetMaxRadius(in int iteration)
        {
            _maxRemeshRadius = _radius + _radius * iteration;
        }

        public double GetMaxRadius()
        {
            return _maxRemeshRadius;
        }

        public int GetNOfLayersAroundTheElement()
        {
            return _nOfLayersAroundTheElement;
        }

        public void SetNOfLayersAroundTheElement(int nOfLayersAroundTheElement)
        {
            _nOfLayersAroundTheElement = nOfLayersAroundTheElement;
        }
    }
}