using System.Collections.Generic;
using System.Linq;
using ippt.dem.mesh.entities.finite.element;
using ippt.dem.mesh.repository;

namespace ippt.dem.mesh.entities.core
{
    public class FiniteElementBoundarySearch
    {
        public static void SetBoundaryElements(IDataRepository data)
        {
            var nodesWithBoundary = data
                .GetNodes()
                .Values
                .ToList()
                .Where(node => node.GetGroupSet().Count > 1)
                .ToList();
            nodesWithBoundary.ForEach(node => node
                    .GetElementSet()
                    .ToList()
                    .ForEach(data.SetInterfaceBoundary));
        }
    }
}