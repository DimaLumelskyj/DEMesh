using System.Text;
using ippt.dem.mesh.repository;
using Microsoft.Extensions.Logging;

namespace ippt.dem.mesh.entities.discrete.element
{
    public class ReMeshDiscreteElement
    {
        private readonly IDataRepository _dataRepository;
        private readonly ILogger _log;

        public ReMeshDiscreteElement(IDataRepository dataRepository, ILogger log)
        {
            _dataRepository = dataRepository;
            _log = log;
        }

        public void Run()
        {
            _dataRepository.CleanUpFiniteElementDataInformation();
            
        }
    }
}