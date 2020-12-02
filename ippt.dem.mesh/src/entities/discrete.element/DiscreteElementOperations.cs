using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using ippt.dem.mesh.entities.discrete.element;
using ippt.dem.mesh.repository;

namespace ippt.dem.mesh.entities.core
{
    public class DiscreteElementOperations
    {
        public static void UpdateBoundaryData(IDataRepository data)
        {
            data.GetDiscreteElements().Values.ToList().ForEach(element => UpdateBoundaryDataForElement(element, data));
        }

        private static void UpdateBoundaryDataForElement(IDiscreteElement element, IDataRepository data)
        {
            HashSet<long>setOfNeighboursElements = new HashSet<long>();
            
            data
                .GetElementById(element.GetFiniteElementId())
                .GetVerticesId()
                .ToList()
                .ForEach(verticeId => 
                    setOfNeighboursElements
                        .UnionWith(data
                            .GetNodes()[verticeId]
                            .GetElementSet()));
            
            setOfNeighboursElements.Remove(element.GetId());
            
            data.UpdateBoundaryDataForDiscreteElement(
                    element.GetId(),
                    setOfNeighboursElements,
                    data.GetElementById(element.GetFiniteElementId()).IsInterfaceBoundary());
        }

        public static void UpdateGrowthAbilityOfDiscreteElementInGroup(IDataRepository data, int groupId)
        {
            data.GetDiscreteElementGroup()[groupId].ForEach(id => UpdateGrowthAbilityOfDiscreteElement(id, data));
        }

        private static void UpdateGrowthAbilityOfDiscreteElement(in long id, IDataRepository data)
        {
            int nofLayersAroundElement = -1;
            long maxIterations = (long) (data.GetRemeshMaxRadius() / data.GetDiscreteElementById(id).GetRadius());

            HashSet<long> previousElementsIdInLayers = new HashSet<long>();
            HashSet<long> additionalElementsIdInLayers = new HashSet<long>();
            HashSet<long> nextLayerElementIds = new HashSet<long>();
            bool isEndOfLayerSearch = false;
            while (++nofLayersAroundElement <= maxIterations)
            {
                
                if (nofLayersAroundElement == 0)
                {
                    additionalElementsIdInLayers.UnionWith(data.GetDiscreteElementById(id).GetNeighbours());
                }
                
                foreach (var elementId in additionalElementsIdInLayers)
                {
                    if (data.GetDiscreteElementById(elementId).IsOnInterface())
                    {
                        isEndOfLayerSearch = true;
                        break;
                    }
                }

                if (isEndOfLayerSearch)
                { 
                    break;
                }
                
                previousElementsIdInLayers.UnionWith(additionalElementsIdInLayers);

                nextLayerElementIds.Clear();
                
                additionalElementsIdInLayers
                    .ToList()
                    .ForEach(id => 
                        nextLayerElementIds
                            .UnionWith(data.GetDiscreteElementById(id).GetNeighbours()));
                
                var nextStepTotalElements = new List<long>(previousElementsIdInLayers);
                
                var setOfNextStepTotalElements = nextStepTotalElements.ToHashSet();
                
                setOfNextStepTotalElements.UnionWith(nextLayerElementIds);
                
                additionalElementsIdInLayers.Clear();

                additionalElementsIdInLayers = setOfNextStepTotalElements
                    .ToList()
                    .Except(previousElementsIdInLayers.ToList())
                    .ToHashSet();
            }

            data.UpdateMaxDiscreteElementPossibleRemeshNumberOfLayers(id, nofLayersAroundElement);
        }
    }
}