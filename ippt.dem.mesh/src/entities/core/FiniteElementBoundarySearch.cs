using System.Collections.Generic;
using System.Linq;
using ippt.dem.mesh.entities.finite.element;
using ippt.dem.mesh.repository;

namespace ippt.dem.mesh.entities.core
{
    public class FiniteElementBoundarySearch
    {
        private static IDataRepository _data;
        

        public static void SetBoundaryElements(IDataRepository data, long groupToSetBoundary)
        {
            _data = data;
            _data
                .GetFiniteElementGroups()[groupToSetBoundary]
                .ForEach(id => 
                    SearchBoundary(
                        _data
                            .GetFiniteElementGroups()
                            .Keys
                            .Where(id => !id.Equals(groupToSetBoundary))
                            .ToList(),
                        id));
         }

        private static void SearchBoundary(List<long> groupsToVerify, in long id)
        {
            foreach (var groupId in groupsToVerify)
            {
                if(SearchBoundaryInGroup(groupId, _data.GetElementById(id)))
                    return;
            }
        }

        private static bool SearchBoundaryInGroup(in long groupId, IElement elementToCheck)
        {
            foreach (var elementId in _data.GetFiniteElementGroups()[groupId])
            {
                foreach (var nodeId in _data.GetElementById(elementId).GetVerticesId())
                {
                    if (elementToCheck.GetVerticesId().Contains(nodeId))
                    {
                        _data.SetInterfaceBoundary(elementToCheck.GetId());
                        return true;  
                    }    
                }
            }
            return false;
        }
    }
}