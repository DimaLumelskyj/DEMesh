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
}