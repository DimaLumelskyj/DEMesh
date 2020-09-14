using System.Collections.Generic;
using ippt.dem.mesh.entities.finite.element;
using ippt.dem.mesh.entities.nodes;

namespace ippt.dem.mesh.repository
{
    public class InMemoryDataRepository : DataRepository
    {
    
        private readonly Dictionary<long,INode> _nodes;
        private readonly Dictionary<long, IElement> _elements;
        
        public InMemoryDataRepository()
        {
            _nodes = new Dictionary<long, INode>(); 
            _elements = new Dictionary<long, IElement>();
        }
        
        public void AddNode(INode node)
        {
            _nodes.Add(node.GetId(),node);
        }

        public INode GetById(long id)
        {
            return _nodes[id];
        }

        public void AddElement(IElement element)
        {
            _elements.Add(element);
        }
    }
}