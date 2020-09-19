using System;
using System.Collections.Generic;
using System.Numerics;
using ippt.dem.mesh.entities.core;
using ippt.dem.mesh.entities.discrete.element;
using ippt.dem.mesh.entities.nodes;

namespace ippt.dem.mesh.entities.finite.element
{
    public class HexahedronElement : IElement
    {
        private readonly long _id;
        
        private readonly List<long> _verticesId;

        private const ElementType Type = ElementType.Hexahedron;
        
        private readonly int _groupId;

        private readonly DiscreteElementCreator _discreteElementCreator;

        public HexahedronElement(ElementDto elementDto, DiscreteElementCreator discreteElementCreator)
        {
            _discreteElementCreator = discreteElementCreator;
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

        public IDiscreteElement GetSimpleFilledSphereDiscreteElement(Dictionary<long, INode> elementNodes, long centerNodeId, int groupId)
        {
            return _discreteElementCreator
                .FactoryMethod(
                    DiscreteElementDto
                        .Get(centerNodeId,
                            getRadius(GetCenterNodeInElement(elementNodes,centerNodeId),elementNodes[0]),
                            centerNodeId,
                            groupId)
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
    }
}