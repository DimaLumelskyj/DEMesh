using System.IO;
using ippt.dem.mesh.repository;

namespace ippt.dem.mesh.system.write
{
    public class WriteOutputResults : IWriteOutputResults
    {
        private readonly DataRepository _dataRepository;
        private string _fileName;
        private string _filePath;

        public WriteOutputResults(DataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }
        
        public void WriteOutput(string path)
        {
            _fileName = Path.GetFileName(path);
            _filePath = Path.GetFullPath(path);

        }
    }
}