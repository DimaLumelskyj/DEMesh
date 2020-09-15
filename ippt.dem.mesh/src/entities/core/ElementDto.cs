using System.Collections.Generic;

namespace ippt.dem.mesh.entities.core
{
    public class ElementDto
    {
        private readonly long _id;
        private readonly List<long> _listOfNodes;
        private readonly int _group;

        private ElementDto(long id, List<long> listOfNodes, int group)
        {
            _id = id;
            _listOfNodes = listOfNodes;
            _group = group;
        }
        
        public static ElementDto Get(long id, List<long> listOfNodes, int group)
        {
            return new ElementDto(id, listOfNodes, group );
        }

        public long GetId()
        {
            return _id;
        }
        
        public List<long> GetNodesId()
        {
            return _listOfNodes;
        }

        public override string ToString()
        {
            return $"Element with id={_id.ToString()}\nnode vertices list: {_listOfNodes}";
        }
    }
}