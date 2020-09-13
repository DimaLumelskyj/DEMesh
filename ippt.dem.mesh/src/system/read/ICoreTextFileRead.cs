using System.Collections.Generic;

namespace ippt.dem.mesh.system
{
    public interface ICoreTextFileRead
    {
        public List<string> ReadFromFile(string path);
    }
}