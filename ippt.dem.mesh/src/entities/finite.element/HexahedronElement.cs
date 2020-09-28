using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ippt.dem.mesh.entities.core;
using ippt.dem.mesh.entities.discrete.element;
using ippt.dem.mesh.entities.nodes;
using ippt.dem.mesh.repository;

namespace ippt.dem.mesh.entities.finite.element
{
    public class HexahedronElement : IElement
    {
        /*
         element created from nodes:
         1 2 3 4 5 6 7 8
         6 surfaces: 
         bottom: 1 2 3 4    -> _neighboursId[0]
         top: 5 6 7 8       -> _neighboursId[1]      
         XZ: 1 2 6 5        -> _neighboursId[2]
         YZ: 2 3 7 6        -> _neighboursId[3]
         MinusXZ : 4 3 7 8  -> _neighboursId[4]
         MinusYZ : 1 4 8 5  -> _neighboursId[5]
         https://abaqus-docs.mit.edu/2017/English/SIMACAEELMRefMap/simaelm-c-solidcont.htm
         */
        
        private readonly long _id;
        
        private readonly List<long> _verticesId;
        
        private readonly List<long> _neighboursId;
        
        private readonly List<long> _topSurface;
        
        private readonly List<long> _bottomSurface;
        
        private readonly List<long> _xzSurface;
        
        private readonly List<long> _yzSurface;

        private readonly List<long> _minusXzSurface;
        
        private readonly List<long> _minusYzSurface;
        
        private const ElementType Type = ElementType.Hexahedron;
        
        private readonly int _groupId;

        private readonly DiscreteElementCreator _discreteElementCreator;
        
        private readonly DataRepository _dataRepository;

        public HexahedronElement(ElementDto elementDto,
            DiscreteElementCreator discreteElementCreator,
            DataRepository dataRepository)
        {
            _discreteElementCreator = discreteElementCreator;
            _dataRepository = dataRepository;
            _verticesId = elementDto.GetNodesId();
            _id = elementDto.GetId();
            _groupId = elementDto.GetGroupId();
            _neighboursId = new List<long>{ 0, 0, 0, 0, 0 , 0 };
            _topSurface = new List<long>{ _verticesId[4], _verticesId[5], _verticesId[6], _verticesId[7] };
            _bottomSurface = new List<long>{ _verticesId[0], _verticesId[1], _verticesId[2], _verticesId[3] };
            _xzSurface = new List<long>{  _verticesId[0], _verticesId[1], _verticesId[5], _verticesId[4] };
            _yzSurface = new List<long>{  _verticesId[1], _verticesId[2], _verticesId[6], _verticesId[5] };
            _minusXzSurface = new List<long>{  _verticesId[3], _verticesId[2], _verticesId[6], _verticesId[7] };
            _minusYzSurface = new List<long>{  _verticesId[0], _verticesId[3], _verticesId[7], _verticesId[4] };
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

        public void FindNeighbourElements()
        {
            
        }
    }
}