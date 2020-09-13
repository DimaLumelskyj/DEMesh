using System.Collections.Generic;

namespace ippt.dem.mesh.entities.core
{
    public class ElementDto
    {
        public long Id { get; set; }
        public List<long> ListOfNodes { get; set; }
    }
}