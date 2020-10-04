using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ippt.dem.mesh.entities.core;
using ippt.dem.mesh.entities.discrete.element;
using ippt.dem.mesh.entities.nodes;
using ippt.dem.mesh.repository;
using Microsoft.Extensions.Logging;

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

        private Boolean _contactSearched = false;
         
        private readonly long _id;
        
        private readonly List<long> _verticesId;
        
        private Dictionary<PositionInQuadElement, long> _neighboursId;

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

            _neighboursId = new Dictionary<PositionInQuadElement, long>()
            {
                {PositionInQuadElement.Bottom, 0},
                {PositionInQuadElement.Top, 0},
                {PositionInQuadElement.XZ, 0},
                {PositionInQuadElement.YZ, 0},
                {PositionInQuadElement.MinusXZ, 0},
                {PositionInQuadElement.MinusYZ, 0}
            };
        }

        private QuadSurface GetSurface(PositionInQuadElement position, List<long> verticesId)
        {
            switch (position)
            {
                case PositionInQuadElement.Bottom:
                    return GetBottom(verticesId);
                case PositionInQuadElement.Top:
                    return GetTop(verticesId);
                case PositionInQuadElement.XZ:
                    return GetXz(verticesId);
                case PositionInQuadElement.YZ:
                    return GetYz(verticesId);
                case PositionInQuadElement.MinusXZ:
                    return GetMinusXz(verticesId);
                case PositionInQuadElement.MinusYZ:
                    return GetMinusYz(verticesId);
                default:
                    throw new Exception("wrong position");
            }
        }
        private QuadSurface GetBottom(List<long> verticesId)
        {
            return new QuadSurface(new List<long> {verticesId[0], verticesId[1], verticesId[2], verticesId[3]},
                PositionInQuadElement.Bottom);
        }

        private QuadSurface GetTop(List<long> verticesId)
        {
            return new QuadSurface(new List<long> {verticesId[4], verticesId[5], verticesId[6], verticesId[7]},
                PositionInQuadElement.Top);
        }

        private QuadSurface GetXz(List<long> verticesId)
        {
            return new QuadSurface(new List<long> {verticesId[0], verticesId[1], verticesId[5], verticesId[4]},
                PositionInQuadElement.XZ);
        }
        
        private QuadSurface GetYz(List<long> verticesId)
        {
            return new QuadSurface(new List<long> {verticesId[1], verticesId[2], verticesId[6], verticesId[5]},
                PositionInQuadElement.YZ);
        }
        
        private QuadSurface GetMinusXz(List<long> verticesId)
        {
            return new QuadSurface(new List<long> {verticesId[3], verticesId[2], verticesId[6], verticesId[7]},
                PositionInQuadElement.MinusXZ);
        }
        
        private QuadSurface GetMinusYz(List<long> verticesId)
        {
            return new QuadSurface(new List<long> {verticesId[0], verticesId[3], verticesId[7], verticesId[4]},
                PositionInQuadElement.MinusYZ);
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
           FindContact(PositionInQuadElement.Bottom, PositionInQuadElement.Top);
           FindContact(PositionInQuadElement.Top, PositionInQuadElement.Bottom);
           FindContact(PositionInQuadElement.XZ, PositionInQuadElement.MinusXZ);
           FindContact(PositionInQuadElement.YZ, PositionInQuadElement.MinusYZ);
           FindContact(PositionInQuadElement.MinusXZ, PositionInQuadElement.XZ);
           FindContact(PositionInQuadElement.MinusYZ, PositionInQuadElement.YZ);
           
            _contactSearched = true;
        }

        private void FindContact(PositionInQuadElement surface, PositionInQuadElement contactSurface)
        {
            foreach (var element in _dataRepository.GetFiniteElements())
            {
                if(_id == element.GetId())
                    continue;
                if (CheckContact(element, surface, contactSurface)) { break; }
            }
        }

        private Boolean CheckContact(IElement elementInContact,
            PositionInQuadElement surfacePosition,
            PositionInQuadElement contactSurfacePosition)
        {
            if (elementInContact.GetType() != ElementType.Hexahedron)
            {
                throw new Exception("wrong element type");
            }
            QuadSurface surface = GetSurface(surfacePosition, _verticesId);
            QuadSurface contactSurface = GetSurface(contactSurfacePosition, elementInContact.GetVerticesId());
            if (surface.Equals(contactSurface))
            {
                _neighboursId[surfacePosition] = elementInContact.GetId();
                return true;
            }
            return false;
        }
    }
}