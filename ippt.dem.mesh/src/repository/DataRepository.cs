using System.Collections.Generic;
using System.Security.Cryptography;
using ippt.dem.mesh.entities.discrete.element;
using ippt.dem.mesh.entities.finite.element;
using ippt.dem.mesh.entities.nodes;

namespace ippt.dem.mesh.repository
{
    public interface DataRepository
    {
        public void AddNode(INode node);
        public INode GetNodeById(long id);
        public IElement GetElementById(long id);
        public Dictionary<long, INode> GetElementNodes(long id);
        public void AddElement(IElement element);
        public void InitializeGroupElementIds(List<int> groupsId);
        public void AddSimpleSphere(IDiscreteElement discreteElement, INode node);
    }
}