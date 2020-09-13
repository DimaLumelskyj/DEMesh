using System.Collections.Generic;
using ippt.dem.mesh.repository;

namespace ippt.dem.mesh.system.parser
{
    public interface IAbaqusParser
    {
        public void parse(List<string> data);
    }
}