using System;
using System.IO;
using ippt.dem.mesh.repository;
using Microsoft.Extensions.Logging;

namespace ippt.dem.mesh.system.write
{
    public class WriteOutputResults : IWriteOutputResults
    {
        private readonly IDataRepository _dataRepository;
        private string _fileName;
        private string _fileDirectory;
        private string _outputDatPath;
        private string _outputMshPath;
        private string _outputDatFileName;
        private string _outputMshFileName;
        private readonly ILogger _log;
        
        public WriteOutputResults(IDataRepository dataRepository,
            ILogger<WriteOutputResults> log)
        {
            _dataRepository = dataRepository;
            _log = log;
        }
        
        public void WriteOutput(string path)
        {
           SetPath(path);
           WritingToFiles();
        }

        private void SetPath(string path)
        {
             _fileName = Path.GetFileName(path);
             _fileDirectory = Path.GetDirectoryName(path);
             
             string[] fileNameParsed = _fileName.Split('.');
             if (fileNameParsed.Length == 2 || fileNameParsed.Length == 1 )
             {
                _outputDatFileName = $"{fileNameParsed[0]}.dat";
                _outputDatPath = @$"{_fileDirectory}\{_outputDatFileName}";
                _outputMshFileName = $"{fileNameParsed[0]}.msh";
                _outputMshPath = @$"{_fileDirectory}\{_outputMshFileName}";
             }
             else
             {
                 _log.LogError($"Parsing file name: {_fileName} error");
                 throw new InvalidDataException($"Parsing file name: {_fileName} error");
             }
        }

        private void WriteNodeDataInGroup(StreamWriter streamWriter, long id, FileFormat format)
        {
            streamWriter.WriteLine(_dataRepository.GetSphereNodeToString(
                _dataRepository.GetDiscreteElementById(id).GetCenterNodeId(),
                format));
        }
        
        private void WriteDiscreteElementSphereDataInGroup(StreamWriter streamWriter, long id, FileFormat format)
        {
            streamWriter.WriteLine(_dataRepository.GetSphereElementToString(
                _dataRepository.GetDiscreteElementById(id).GetCenterNodeId(),
                format));
        }
        
        private void WriteMshFile(StreamWriter  mshStreamWriter)
        {
            foreach (var (groupId, elements) in _dataRepository.GetDiscreteElementGroup())
            {
                mshStreamWriter.WriteLine($"MESH \"Group_{groupId.ToString()}\" dimension = 3 ElemType Sphere        Nnode =  1");
                mshStreamWriter.WriteLine("Coordinates");
                elements.ForEach(id => WriteNodeDataInGroup(mshStreamWriter, id, FileFormat.Msh));
                mshStreamWriter.WriteLine("End coordinates");
                mshStreamWriter.WriteLine("Elements");
                elements.ForEach(id => WriteDiscreteElementSphereDataInGroup(mshStreamWriter, id, FileFormat.Msh));
                mshStreamWriter.WriteLine("End Elements");
            }
        }

        private void writeDatFile(StreamWriter datStreamWriter)
        {
            datStreamWriter.WriteLine("GEOMETRY_DEFINITION");
            datStreamWriter.WriteLine("        GENERAL:    GSCALE =  1.0");
            
            foreach (var (key, value) in _dataRepository.GetDiscreteElementGroup())
            {
                datStreamWriter.WriteLine($"        SET Group_{key.ToString()}");
                value.ForEach(id => WriteNodeDataInGroup(datStreamWriter, id, FileFormat.Dat));
                datStreamWriter.WriteLine($"        END_SET Group_{key.ToString()}");
            }
            datStreamWriter.WriteLine("END_GEOMETRY_DEFINITION");
            datStreamWriter.WriteLine("$-----------------------------------------------------------------------------");

            foreach (var group in _dataRepository.GetDiscreteElementGroup())
            {
                datStreamWriter.WriteLine($"SET_DEFINITION");
                datStreamWriter.WriteLine($"$");
                datStreamWriter.WriteLine($"ELS_NAME= Group_{group.Key.ToString()}    ELM_TYPE: DISTINCT");
                datStreamWriter.WriteLine("        CTOL=0.5E-06 SCALE=1.0 RISCAL=10.0");
                group.Value.ForEach(id => WriteDiscreteElementSphereDataInGroup(datStreamWriter, id, FileFormat.Dat));
                datStreamWriter.WriteLine($"    END_ELEMENT_DEFINITION");
                datStreamWriter.WriteLine($"$");
                datStreamWriter.WriteLine($"END_SET_DEFINITION");
            }
        }
        private void WritingToFiles()
        {
            try
            {
                StreamWriter mshStreamWriter = new StreamWriter(_outputMshPath);
                StreamWriter datStreamWriter = new StreamWriter(_outputDatPath);

                WriteMshFile(mshStreamWriter);
                writeDatFile(datStreamWriter);

                mshStreamWriter.Close();
                datStreamWriter.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }
        
    }
}