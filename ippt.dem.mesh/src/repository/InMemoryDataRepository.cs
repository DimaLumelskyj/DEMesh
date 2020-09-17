using System.Collections.Generic;
using ippt.dem.mesh.entities.finite.element;
using ippt.dem.mesh.entities.nodes;

namespace ippt.dem.mesh.repository
{
    public class InMemoryDataRepository : DataRepository
    {
    
        private readonly Dictionary<long,INode> _nodes;
        private readonly Dictionary<long, IElement> _elements;
        private readonly Dictionary<long, List<long>> _groupElementIds;
        
        public InMemoryDataRepository()
        {
            _nodes = new Dictionary<long, INode>(); 
            _elements = new Dictionary<long, IElement>();
            _groupElementIds = new Dictionary<long, List<long>>();
        }
        
        public void AddNode(INode node)
        {
            _nodes.Add(node.GetId(),node);
        }

        public INode GetNodeById(long id)
        {
            return _nodes[id];
        }

        public IElement GetElementById(long id)
        {
            return _elements[id];
        }

        public Dictionary<long, INode> GetElementNodes(long id)
        {
            var elementVerticies = new Dictionary<long, INode>();
            long i = -1;
            GetElementById(id).GetVerticesId().ForEach(nodeId =>elementVerticies.Add(i++,GetNodeById(nodeId)));;
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
            }
        }
    }
}