using System.Security.Cryptography;
using ippt.dem.mesh.entities.nodes;

namespace ippt.dem.mesh.repository
{
    public interface DataRepository
    {
        public void addNode(INode node);
        public INode getById(long id);
    }
}