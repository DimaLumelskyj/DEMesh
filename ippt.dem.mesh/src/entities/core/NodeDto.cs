using System.Collections.Generic;
using ippt.dem.mesh.entities.nodes;

namespace ippt.dem.mesh.entities.core
{
    public class NodeDto
    {
        private readonly long _id;
        private readonly List<double> _listOfCoordinates;

        private NodeDto(List<double> listOfCoordinates, long id)
        {
            _listOfCoordinates = listOfCoordinates;
            _id = id;
        }

        public static NodeDto Get(long id, List<double> coordinates)
        {
            return new NodeDto(coordinates, id);
        }

        public long GetId()
        {
            return _id;
        }
        
        public List<double> GetCoordinates()
        {
            return _listOfCoordinates;
        }
    }
}