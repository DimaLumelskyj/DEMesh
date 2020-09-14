using System.Collections.Generic;
using ippt.dem.mesh.entities.core;

namespace ippt.dem.mesh.entities.nodes
{
    public class Node : INode
    {
        private readonly long _id;
        private readonly List<double> _listOfCoordinates;

        public Node(NodeDto nodeDto)
        {
            this._id = nodeDto.GetId();
            this._listOfCoordinates = nodeDto.GetCoordinates();
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