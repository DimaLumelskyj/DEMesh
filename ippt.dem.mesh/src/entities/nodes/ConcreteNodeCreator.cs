using System.Collections.Generic;
using ippt.dem.mesh.entities.core;

namespace ippt.dem.mesh.entities.nodes
{
    public class ConcreteNodeCreator : NodeCreator
    {
        public override INode FactoryMethod(NodeDto nodeDto)
        {
            return new Node(nodeDto);
        }
    }

    public class Node : INode
    {
        private long Id;
        private List<double> ListOfCoordinates; 
        
        public Node(NodeDto nodeDto)
        {
            this.Id = nodeDto.Id;
            this.ListOfCoordinates = nodeDto.ListOfCoordinates;
        }

        public long GetId()
        {
            return Id;
        }

        public List<double> GetCoordinates()
        {
            return ListOfCoordinates;
        }
    }
}