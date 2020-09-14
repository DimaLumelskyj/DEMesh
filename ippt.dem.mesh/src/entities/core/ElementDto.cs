using System.Collections.Generic;

namespace ippt.dem.mesh.entities.core
{
    public class ElementDto
    {
        private readonly long _id;
        private readonly List<long> _listOfNodes;

        private ElementDto(long id, List<long> listOfNodes)
        {
            _id = id;
            _listOfNodes = listOfNodes;
        }
        
        public static ElementDto Get(long id, List<long> listOfNodes)
        {
            return new ElementDto(id, listOfNodes);
        }

        public long GetId()
        {
            return _id;
        }
        
        public List<long> GetNodesId()
        {
            return _listOfNodes;
        }
    }
}