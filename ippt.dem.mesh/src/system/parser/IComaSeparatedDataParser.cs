using System.Collections.Generic;

namespace ippt.dem.mesh.system.parser
{
    public interface IComaSeparatedDataParser
    {
        public void ParseReMeshData(List<string> data);
    }
}