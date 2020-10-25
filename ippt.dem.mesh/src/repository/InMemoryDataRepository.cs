using System;
using System.Collections.Generic;
using System.Linq;
using ippt.dem.mesh.entities.discrete.element;
using ippt.dem.mesh.entities.finite.element;
using ippt.dem.mesh.entities.nodes;
using Microsoft.Extensions.Logging;

namespace ippt.dem.mesh.repository
{
    public class InMemoryDataRepository : IDataRepository
    {
    
        private readonly Dictionary<long,INode> _nodes;
        private readonly Dictionary<long, HashSet<long>> _nodeNeighbourElements;
        private readonly Dictionary<long, IElement> _elements;
        private readonly Dictionary<long, HashSet<long>> _elementNeighbourElements;
        private readonly Dictionary<long, List<long>> _groupElementIds;
        
        private readonly Dictionary<long, IDiscreteElement> _discreteElements;
        private readonly Dictionary<long, List<long>> _groupDiscreteElementIds;
        private readonly Dictionary<long,INode> _discreteElementNodes;
        private readonly Dictionary<long, long> _discreteElementNodesRelatedToFe;
        private readonly Dictionary<long, long> _feIdRelatedToDiscreteElementNodes;
        private readonly Dictionary<double, long> _remeshProperties;
        
        
        private readonly ILogger _log;

        public InMemoryDataRepository(ILogger<InMemoryDataRepository> log)
        {
            _log = log;
            _remeshProperties = new Dictionary<double, long>();
            _nodeNeighbourElements = new Dictionary<long, HashSet<long>>();
            _nodes = new Dictionary<long, INode>(); 
            _elements = new Dictionary<long, IElement>();
            _elementNeighbourElements = new Dictionary<long, HashSet<long>>();
            _groupElementIds = new Dictionary<long, List<long>>();
            _discreteElements = new Dictionary<long, IDiscreteElement>();
            _groupDiscreteElementIds = new Dictionary<long, List<long>>();
            _discreteElementNodes = new Dictionary<long, INode>();
            _discreteElementNodesRelatedToFe = new Dictionary<long, long>();
            _feIdRelatedToDiscreteElementNodes = new Dictionary<long, long>();
            
        }
        
        public void AddNode(INode node)
        {
            _nodes.Add(node.GetId(),node);
        }

        public IDiscreteElement GetDiscreteElementById(long id)
        {
            return  DiscreteSphereElement.Get(_discreteElements[id]);
        }

        public IElement GetElementById(long id)
        {
            return _elements[id];
        }

        public Dictionary<long, INode> GetElementNodes(long id)
        {
            var elementVerticies = new Dictionary<long, INode>();
            int i = 0;
            _elements[id].GetVerticesId().ForEach(verticeId => elementVerticies.Add(i++, _nodes[verticeId]));
            return elementVerticies;
        }

        public void AddElement(IElement element)
        {
            _elements.Add(element.GetId(),element);
            _groupElementIds[element.GetGroup()].Add(element.GetId());
            element.GetVerticesId().ForEach(nodeId =>_nodeNeighbourElements[nodeId].Add(element.GetId()));
        }

        public void InitializeGroupElementIds(List<int> groupsId)
        {
            foreach (var id in groupsId)
            {
               _groupElementIds.Add(id, new List<long>()); 
               _groupDiscreteElementIds.Add(id, new List<long>());
            }
        }

        public void AddSimpleSphere(IDiscreteElement discreteElement, INode node, long finiteElementId)
        {
            _discreteElements.Add(discreteElement.GetId(),discreteElement);
            _groupDiscreteElementIds[discreteElement.GetGroupId()].Add(discreteElement.GetId());
            _discreteElementNodes.Add(node.GetId(),node);
            _discreteElementNodesRelatedToFe.Add(node.GetId(), finiteElementId);
            _feIdRelatedToDiscreteElementNodes.Add(finiteElementId,node.GetId());
        }

        public string GetSphereNodeToString(long id, FileFormat format)
        {
            return _discreteElementNodes[id].ToString(format);
        }

        public string GetSphereElementToString(long id, FileFormat format)
        {
            return _discreteElements[id].ToString(format);
        }

        public Dictionary<long, List<long>> GetDiscreteElementGroup()
        {
            return _groupDiscreteElementIds;
        }

        public List<IElement> GetFiniteElements()
        {
            return _elements.Values.ToList();
        }

        public Dictionary<long, INode> GetDiscreteElementNodes()
        {
            return _discreteElementNodes;
        }

        public long GetFiniteElementIdByDiscreteElementCenterNodeId(long discreteElementCenterNodeId)
        {
            return _discreteElementNodesRelatedToFe[discreteElementCenterNodeId];
        }

        public void InitNodeNeighbourElement()
        {
            foreach (var node in _nodes)
            {
                _nodeNeighbourElements.Add(node.Key, new HashSet<long>());
            }
        }

        public void LogVolumeInformation()
        {
            foreach (var (groupId, elements) in _groupElementIds)
            {
                var V_element = _elements[elements[1]].GetVolume();
                var V_total = elements.Count * V_element;
                _log.LogInformation("Application: {applicationEvent} at {dateTime}",
                    $"Group Id:{groupId.ToString()} N_of_elements={elements.Count.ToString()} V_element={V_element.ToString()} V_group={V_total.ToString()}",
                    DateTime.UtcNow.ToString());
            }
        }

        public void CleanUpFiniteElementDataInformation()
        {
            _nodes.Clear();
            _nodeNeighbourElements.Clear();
            _elements.Clear();
            _elementNeighbourElements.Clear();
            _groupElementIds.Clear();
        }

        public void AddReMeshInputData(long numberOfParticles, double particleRadius)
        {
            _remeshProperties.Add(particleRadius, numberOfParticles);
        }

        public Dictionary<double, long> GetRemeshProperties()
        {
            return _remeshProperties;
        }
        
        public List<long> GetDiscreteElementGroup(long groupId)
        {
            return _groupDiscreteElementIds[groupId];
        }

        public INode GetDiscreteElementNode(long centerNodeId)
        {
            return _discreteElementNodes[centerNodeId];
        }

        public Dictionary<long, IDiscreteElement> GetDiscreteElements()
        {
            return _discreteElements;
        }
    }
}