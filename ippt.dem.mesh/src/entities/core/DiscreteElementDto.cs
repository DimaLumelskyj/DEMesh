namespace ippt.dem.mesh.entities.core
{
    public class DiscreteElementDto
    {
        private readonly long _id;
        private readonly double _radius;
        private readonly long _nodeId;
        private readonly int _groupId;

        private DiscreteElementDto(long id, double radius, long nodeId, int groupId)
        {
            _id = id;
            _radius = radius;
            _nodeId = nodeId;
            _groupId = groupId;
        }

        public static DiscreteElementDto Get(long id, double radius, long nodeId, int groupId)
        {
            return new DiscreteElementDto(id,radius,nodeId,groupId);
        }

        public long Id => _id;

        public double Radius => _radius;

        public long NodeId => _nodeId;

        public int GroupId => _groupId;
    }
}