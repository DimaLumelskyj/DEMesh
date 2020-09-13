using System.Collections.Generic;
using ippt.dem.mesh.entities.core;

namespace ippt.dem.mesh.system.parser
{
    public class AbaqusDataDTO
    {
        public List<NodeDto> NodesDto { get; set; }
        public List<ElementDto> ElementsDto { get; set; } 
    }
}