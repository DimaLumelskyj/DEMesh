using ippt.dem.mesh.entities.core;

namespace ippt.dem.mesh.entities.nodes
{
    public abstract class NodeCreator
    {
        public abstract INode FactoryMethod(NodeDto nodeDto);
    }
}