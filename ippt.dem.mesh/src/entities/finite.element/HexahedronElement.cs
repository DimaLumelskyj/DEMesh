using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ippt.dem.mesh.entities.core;
using ippt.dem.mesh.entities.discrete.element;
using ippt.dem.mesh.entities.nodes;
using ippt.dem.mesh.repository;
using Microsoft.Extensions.Logging;
using Vector = ippt.dem.mesh.entities.core.Vector;

namespace ippt.dem.mesh.entities.finite.element
{
    public class HexahedronElement : IElement
    {
        private readonly long _id;
        
        private readonly List<long> _verticesId;

        private const ElementType Type = ElementType.Hexahedron;
        
        private readonly int _groupId;

        private readonly DiscreteElementCreator _discreteElementCreator;
        
        private readonly IDataRepository _dataRepository;

        private bool _isInterfaceBoundary = false;
        
        private bool _isExternalBoundary = false;

        public HexahedronElement(ElementDto elementDto,
            DiscreteElementCreator discreteElementCreator,
            IDataRepository dataRepository)
        {
            _discreteElementCreator = discreteElementCreator;
            _dataRepository = dataRepository;
            _verticesId = elementDto.GetNodesId();
            _id = elementDto.GetId();
            _groupId = elementDto.GetGroupId();
        }

        public bool IsInterfaceBoundary()
        {
            return _isInterfaceBoundary;
        }

        public void SetInterfaceBoundary(bool isInterfaceBoundary)
        {
            _isInterfaceBoundary = isInterfaceBoundary;
        }

        public bool IsExternalBoundary()
        {
            return _isExternalBoundary;
        }

        public void SetExternalBoundary(bool isExternalBoundary)
        {
            _isExternalBoundary = isExternalBoundary; 
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

        public IDiscreteElement GetSimpleFilledSphereDiscreteElement(Dictionary<long, INode> elementNodes, long centerNodeId, int groupId,  long elementId)
        {
            return _discreteElementCreator
                .FactoryMethod(
                    DiscreteElementDto
                        .Get(centerNodeId,
                            getRadius(GetCenterNodeInElement(elementNodes,centerNodeId),elementNodes[0]),
                            centerNodeId,
                            groupId,
                            elementId)
                    );
        }

        private double getRadius(NodeDto nodeA, INode nodeB)
        {
            double[] vector = {0,0,0};
            for (var i = 0; i < vector.Length; i++)
            {
                vector[i] = nodeB.GetCoordinates()[i] - nodeA.GetCoordinates()[i];
            }

            return Math.Sqrt(vector[0]*vector[0]+
                             vector[1]*vector[1]+
                             vector[2]*vector[2]);
        }

        public int GetGroup()
        {
            return _groupId;
        }

        public NodeDto GetCenterNodeInElement(Dictionary<long, INode> elementNodes, long id)
        {
            List<double> coordinates = new List<double>();
            
            coordinates.Add(0);
            coordinates.Add(0);
            coordinates.Add(0);
            
            foreach (var vertex in elementNodes)
            {
                coordinates[0] += vertex.Value.GetCoordinates()[0];
                coordinates[1] += vertex.Value.GetCoordinates()[1];
                coordinates[2] += vertex.Value.GetCoordinates()[2];
            }

            for (int i = 0; i < coordinates.Count; i++)
            {
                coordinates[i] = coordinates[i] / elementNodes.Count;
            }
            
            return NodeDto.Get(id, coordinates);
        }

        public double  GetVolume()
        {
            var nodes = _dataRepository.GetElementNodes(_id);
            return Math.Pow(Vector.Length(nodes[0], nodes[1]), 3);
        }




    }
}