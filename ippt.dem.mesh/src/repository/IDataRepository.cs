using System.Collections.Generic;
using System.Security.Cryptography;
using ippt.dem.mesh.entities.discrete.element;
using ippt.dem.mesh.entities.finite.element;
using ippt.dem.mesh.entities.nodes;

namespace ippt.dem.mesh.repository
{
    public interface IDataRepository
    {
        public void AddNode(INode node);
        public IDiscreteElement GetDiscreteElementById(long id);
        public IElement GetElementById(long id);
        public Dictionary<long, INode> GetElementNodes(long id);
        public void AddElement(IElement element);
        public void InitializeGroupElementIds(List<int> groupsId);
        public void AddSimpleSphere(IDiscreteElement discreteElement, INode node, long finiteElementId);
        public string GetSphereNodeToString(long id, FileFormat format);
        public string GetSphereElementToString(long id, FileFormat format);
        public Dictionary<long, List<long>> GetDiscreteElementGroup();
        public List<IElement> GetFiniteElements();
        public Dictionary<long, INode> GetDiscreteElementNodes();
        public long GetFiniteElementIdByDiscreteElementCenterNodeId(long discreteElementCenterNodeId);
        public void InitNodeNeighbourElement();
        public void LogVolumeInformation();
        public void CleanUpFiniteElementDataInformation();
        public void AddReMeshInputData(long numberOfParticles, double particleRadius);
        public Dictionary<double, long> GetRemeshProperties();
        public List<long> GetDiscreteElementGroup(long groupId);
        public Dictionary<long, List<long>> GetFiniteElementGroups();
        INode GetDiscreteElementNode(long getCenterNodeId);
        public Dictionary<long, IDiscreteElement> GetDiscreteElements();
        void SetInterfaceBoundary(long elementId);
        void UpdateNode(in long id, long getId, int getGroup);
        public Dictionary<long, INode> GetNodes();
        void UpdateBoundaryDataForDiscreteElement(long id,
            HashSet<long> setOfNeighboursElements,
            bool isInterfaceBoundary);
        void UpdateMaxDiscreteElementPossibleRemeshNumberOfLayers(long getId, in int iteration);
        public double GetRemeshMaxRadius();
    }

    public enum FileFormat
    {
        Msh,
        Dat
    }
}