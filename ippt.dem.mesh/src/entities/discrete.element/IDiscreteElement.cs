using System.Collections.Generic;
using ippt.dem.mesh.entities.core;
using ippt.dem.mesh.repository;

namespace ippt.dem.mesh.entities.discrete.element
{
    public interface IDiscreteElement
    {
        public long GetId();

        public long GetCenterNodeId();

        public double GetRadius();

        public int GetGroupId();

        public string ToString(FileFormat format);
        
        long GetFiniteElementId();
        
        public void UpdateBoundaryData(HashSet<long> neighboursElementsId,
            bool isOnInterface);

        public bool IsOnInterface();
        
        public HashSet<long> GetNeighbours();
        
        public void SetMaxRadius(in int iteration);
        
        public double GetMaxRadius();

        public int GetNOfLayersAroundTheElement();
        public void SetNOfLayersAroundTheElement(int nOfLayersAroundTheElement);
    }
}