using System.Collections.Generic;
using ippt.dem.mesh.entities.core;
using ippt.dem.mesh.entities.discrete.element;
using ippt.dem.mesh.entities.nodes;

namespace ippt.dem.mesh.entities.finite.element
{
    public interface IElement
    {
        public long GetId();

        public ElementType GetType();
        
        public List<long> GetVerticesId();
        
        public IDiscreteElement GetSimpleFilledSphereDiscreteElement(
            Dictionary<long, INode> elementNodes,
            long centerNodeId,
            int groupId);

        public int GetGroup();
        public NodeDto GetCenterNodeInElement(Dictionary<long, INode> getElementNodes, long id);
    }
}