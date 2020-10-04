using ippt.dem.mesh.repository;

namespace ippt.dem.mesh.entities.core
{
    public static class ContactElementSearch
    {
        private static DataRepository _data;

        public static void ContactSearchOfHexaElements(DataRepository data)
        {
            _data = data;
            foreach (var node in _data.GetDiscreteElementNodes())
            {
                var deCenterNodeId = node.Value.GetId();
                var feId = _data.GetFiniteElementIdByDiscreteElementCenterNodeId(deCenterNodeId);
                // https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-use-parallel-invoke-to-execute-parallel-operations
            }
        }
        
    }
}