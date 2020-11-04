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

            _neighboursId = new Dictionary<PositionInQuadElement, long>()
            {
                {PositionInQuadElement.Bottom, 0},
                {PositionInQuadElement.Top, 0},
                {PositionInQuadElement.Xz, 0},
                {PositionInQuadElement.Yz, 0},
                {PositionInQuadElement.MinusXz, 0},
                {PositionInQuadElement.MinusYz, 0}
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
                case PositionInQuadElement.Xz:
                    return GetXz(verticesId);
                case PositionInQuadElement.Yz:
                    return GetYz(verticesId);
                case PositionInQuadElement.MinusXz:
                    return GetMinusXz(verticesId);
                case PositionInQuadElement.MinusYz:
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
                PositionInQuadElement.Xz);
        }
        
        private QuadSurface GetYz(List<long> verticesId)
        {
            return new QuadSurface(new List<long> {verticesId[1], verticesId[2], verticesId[6], verticesId[5]},
                PositionInQuadElement.Yz);
        }
        
        private QuadSurface GetMinusXz(List<long> verticesId)
        {
            return new QuadSurface(new List<long> {verticesId[3], verticesId[2], verticesId[6], verticesId[7]},
                PositionInQuadElement.MinusXz);
        }
        
        private QuadSurface GetMinusYz(List<long> verticesId)
        {
            return new QuadSurface(new List<long> {verticesId[0], verticesId[3], verticesId[7], verticesId[4]},
                PositionInQuadElement.MinusYz);
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

        public void FindNeighbourElements()
        {
           FindContact(PositionInQuadElement.Bottom, PositionInQuadElement.Top);
           FindContact(PositionInQuadElement.Top, PositionInQuadElement.Bottom);
           FindContact(PositionInQuadElement.Xz, PositionInQuadElement.MinusXz);
           FindContact(PositionInQuadElement.Yz, PositionInQuadElement.MinusYz);
           FindContact(PositionInQuadElement.MinusXz, PositionInQuadElement.Xz);
           FindContact(PositionInQuadElement.MinusYz, PositionInQuadElement.Yz);
           
            _contactSearched = true;
        }

        public double  GetVolume()
        {
            var nodes = _dataRepository.GetElementNodes(_id);
            return Math.Pow(Vector.Length(nodes[0], nodes[1]), 3);
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