using System.Collections.Generic;
using ippt.dem.mesh.entities.discrete.element;
using ippt.dem.mesh.entities.finite.element;
using ippt.dem.mesh.entities.nodes;

namespace ippt.dem.mesh.repository
{
    public class InMemoryDataRepository : DataRepository
    {
    
        private readonly Dictionary<long,INode> _nodes;
        private readonly Dictionary<long, IElement> _elements;
        private readonly Dictionary<long, List<long>> _groupElementIds;
        
        private readonly Dictionary<long, IDiscreteElement> _discreteElements;
        private readonly Dictionary<long, List<long>> _groupDiscreteElementIds;
        private readonly Dictionary<long,INode> _discreteElementNodes;
        
        public InMemoryDataRepository()
        {
            _nodes = new Dictionary<long, INode>(); 
            _elements = new Dictionary<long, IElement>();
            _groupElementIds = new Dictionary<long, List<long>>();
            _discreteElements = new Dictionary<long, IDiscreteElement>();
            _groupDiscreteElementIds = new Dictionary<long, List<long>>();
            _discreteElementNodes = new Dictionary<long, INode>();
        }
        
        public void AddNode(INode node)
        {
            _nodes.Add(node.GetId(),node);
        }

        public INode GetFiniteElementNodeById(long id)
        {
            return _nodes[id];
        }

        public INode GetDiscreteElementNodeById(long id)
        {
            return _discreteElementNodes[id];
        }

        public IDiscreteElement GetDiscreteElementById(long id)
        {
            return _discreteElements[id];
        }

        public IElement GetElementById(long id)
        {
            return _elements[id];
        }

        public Dictionary<long, INode> GetElementNodes(long id)
        {
            var elementVerticies = new Dictionary<long, INode>();
            var nodesList = GetElementById(id).GetVerticesId();
            for (var i = 0; i < nodesList.Count; i++)
            {
                elementVerticies.Add(i,GetFiniteElementNodeById(nodesList[i]));
            }
            return elementVerticies;
        }

        public void AddElement(IElement element)
        {
            _elements.Add(element.GetId(),element);
            _groupElementIds[element.GetGroup()].Add(element.GetId());
        }

        public void InitializeGroupElementIds(List<int> groupsId)
        {
            foreach (var id in groupsId)
            {
               _groupElementIds.Add(id, new List<long>()); 
               _groupDiscreteElementIds.Add(id, new List<long>());
            }
        }

        public void AddSimpleSphere(IDiscreteElement discreteElement, INode node)
        {
            _discreteElements.Add(discreteElement.GetId(),discreteElement);
            _groupDiscreteElementIds[discreteElement.GetGroupId()].Add(discreteElement.GetId());
            _discreteElementNodes.Add(node.GetId(),node);
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
    }
}