using System.Linq;
using ippt.dem.mesh.entities.finite.element;
using ippt.dem.mesh.repository;

namespace ippt.dem.mesh.entities.core
{
    public class NodesToElementsMapper
    {
        private static IDataRepository _data;
        
        public static void MapElementsToNodes(IDataRepository data)
        {
            _data = data;
            _data.GetFiniteElements().ForEach(ConnectNodeWithElement);
        }

        private static void ConnectNodeWithElement(IElement element)
        {
            element.GetVerticesId().ForEach(id => _data
                .UpdateNode(id,
                            element.GetId(),
                            element.GetGroup()));
        }
    }
}