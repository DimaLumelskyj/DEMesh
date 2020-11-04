namespace ippt.dem.mesh.entities.core
{
    public class DiscreteElementDto
    {
        private readonly long _id;
        private readonly long _finiteElementId;
        private readonly double _radius;
        private readonly long _nodeId;
        private readonly int _groupId;

        private DiscreteElementDto(long id,
            double radius,
            long nodeId,
            int groupId,
            long finiteElementId)
        {
            _id = id;
            _radius = radius;
            _nodeId = nodeId;
            _groupId = groupId;
            _finiteElementId = finiteElementId;
        }

        public static DiscreteElementDto Get(long id,
            double radius,
            long nodeId,
            int groupId,
            long finiteElementId)
        {
            return new DiscreteElementDto(id,
                radius,
                nodeId,
                groupId,
                finiteElementId);
        }

        public long Id => _id;

        public double Radius => _radius;

        public long NodeId => _nodeId;

        public int GroupId => _groupId;
        public long FiniteElementId => _finiteElementId;
    }
}