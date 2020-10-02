using System;
using System.IO;
using ippt.dem.mesh.repository;
using Microsoft.Extensions.Logging;

namespace ippt.dem.mesh.system.write
{
    public class WriteOutputResults : IWriteOutputResults
    {
        private readonly DataRepository _dataRepository;
        private string _fileName;
        private string _fileDirectory;
        private string _outputDatPath;
        private string _outputMshPath;
        private string _outputDatFileName;
        private string _outputMshFileName;
        private readonly ILogger _log;
        
        public WriteOutputResults(DataRepository dataRepository,
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

        private void writeNodeDataInGroup(StreamWriter streamWriter, long id, FileFormat format)
        {
            streamWriter.WriteLine(_dataRepository.GetSphereNodeToString(
                _dataRepository.GetDiscreteElementById(id).GetCenterNodeId(),
                format));
        }
        
        private void writeDiscreteElementSphereDataInGroup(StreamWriter streamWriter, long id, FileFormat format)
        {
            streamWriter.WriteLine(_dataRepository.GetSphereElementToString(
                _dataRepository.GetDiscreteElementById(id).GetCenterNodeId(),
                format));
        }
        
        private void writeMshFile(StreamWriter  mshStreamWriter)
        {
            foreach (var group in _dataRepository.GetDiscreteElementGroup())
            {
                mshStreamWriter.WriteLine($"MESH \"Group_{group.Key.ToString()}\" dimension = 3 ElemType Sphere        Nnode =  1");
                mshStreamWriter.WriteLine("Coordinates");
                group.Value.ForEach(id => writeNodeDataInGroup(mshStreamWriter, id, FileFormat.Msh));
                mshStreamWriter.WriteLine("End coordinates");
                mshStreamWriter.WriteLine("Elements");
                group.Value.ForEach(id => writeDiscreteElementSphereDataInGroup(mshStreamWriter, id, FileFormat.Msh));
                mshStreamWriter.WriteLine("End Elements");
            }
        }

        private void writeDatFile(StreamWriter datStreamWriter)
        {
            datStreamWriter.WriteLine("GEOMETRY_DEFINITION");
            datStreamWriter.WriteLine("        GENERAL:    GSCALE =  1.0");
            
            foreach (var group in _dataRepository.GetDiscreteElementGroup())
            {
                datStreamWriter.WriteLine($"        SET Group_{group.Key.ToString()}");
                group.Value.ForEach(id => writeNodeDataInGroup(datStreamWriter, id, FileFormat.Dat));
                datStreamWriter.WriteLine($"        END_SET Group_{group.Key.ToString()}");
            }
            datStreamWriter.WriteLine("END_GEOMETRY_DEFINITION");
            datStreamWriter.WriteLine("$-----------------------------------------------------------------------------");

            foreach (var group in _dataRepository.GetDiscreteElementGroup())
            {
                datStreamWriter.WriteLine($"SET_DEFINITION");
                datStreamWriter.WriteLine($"$");
                datStreamWriter.WriteLine($"ELS_NAME= Group_{group.Key.ToString()}    ELM_TYPE: DISTINCT");
                datStreamWriter.WriteLine("        CTOL=0.5E-06 SCALE=1.0 RISCAL=10.0");
                group.Value.ForEach(id => writeDiscreteElementSphereDataInGroup(datStreamWriter, id, FileFormat.Dat));
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

                writeMshFile(mshStreamWriter);
                writeDatFile(datStreamWriter);

                mshStreamWriter.Close();
                datStreamWriter.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }
        
    }
}