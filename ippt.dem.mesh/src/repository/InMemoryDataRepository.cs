using System.Collections.Generic;
using ippt.dem.mesh.entities.nodes;

namespace ippt.dem.mesh.repository
{
    public class InMemoryDataRepository : DataRepository
    {
    
        private Dictionary<long,INode> Nodes;
        
        public InMemoryDataRepository()
        {
            Nodes = new Dictionary<long, INode>();                      
        }
        
        public void addNode(INode node)
        {
            Nodes.Add(node.GetId(),node);
        }

        public INode getById(long id)
        {
            return Nodes[id];
        }
    }
}