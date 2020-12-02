using System;
using System.IO;
using System.Linq;
using ippt.dem.mesh.entities.discrete.element;
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
        private string _outputResPath;
        private string _outputDatFileName;
        private string _outputMshFileName;
        private string _outputResFileName;
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
                _outputResFileName = $"{fileNameParsed[0]}.res";
                _outputResPath = @$"{_fileDirectory}\{_outputResFileName}";
             }
             else
             {
                 _log.LogError($"Parsing file name: {_fileName} error");
                 throw new InvalidDataException($"Parsing file name: {_fileName} error");
             }
        }
        private void WritingToFiles()
        {
            StreamWriter mshStreamWriter = new StreamWriter(_outputMshPath);
            StreamWriter datStreamWriter = new StreamWriter(_outputDatPath);
            StreamWriter resStreamWriter = new StreamWriter(_outputResPath);
            try
            {
                WriteMshFile(mshStreamWriter);
                WriteDatFile(datStreamWriter);
                WriteResFile(resStreamWriter);
            }
            catch(Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }
            finally
            {
                mshStreamWriter.Close();
                datStreamWriter.Close();
                resStreamWriter.Close();
                Console.WriteLine("Executing finally block.");
            }
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
        
        private void WriteNodeDataInGroup(StreamWriter streamWriter, long id, FileFormat format)
        {
            streamWriter.WriteLine(_dataRepository.GetSphereNodeToString(
                _dataRepository.GetDiscreteElementById(id).GetCenterNodeId(),
                format));
        }
        
        private void WriteDiscreteElementSphereDataInGroup(StreamWriter streamWriter, long id, FileFormat format)
        {
            streamWriter.WriteLine(_dataRepository.GetSphereElementToString(_dataRepository.GetDiscreteElementById(id).GetCenterNodeId(), format));
        }

        private void WriteDatFile(StreamWriter datStreamWriter)
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
        
        private void WriteResFile(StreamWriter resStreamWriter)
        {
            String[] header = 
            {
                "GiD Post Results File 1.0"
            };
            header.ToList().ForEach(resStreamWriter.WriteLine);
            WriteExistingInterfaceSpheres(resStreamWriter);
            WriteMaxNumberOfGrowthLayesrsAroudSphere(resStreamWriter);
            //WriteMaxRemeshRadiusSpheres(resStreamWriter);
        }

        private void WriteExistingInterfaceSpheres(StreamWriter resStreamWriter)
        {
            String[] resultsHeader = 
            {
                "Result \"Is Interface\" \"Time Step \"   0 Scalar OnNodes",
                "Values"
            };
            String[] resultsEnd = { "End Values" };
            resultsHeader.ToList().ForEach(resStreamWriter.WriteLine);
            _dataRepository
                .GetDiscreteElements()
                .Values
                .ToList()
                .ForEach(element => WriteLineOfInterfaceResult(element, resStreamWriter));
            resultsEnd.ToList().ForEach(resStreamWriter.WriteLine);
        }

        private void WriteLineOfInterfaceResult(IDiscreteElement element, StreamWriter resStreamWriter)
        {
            if (element.GetGroupId() == 1)
            {
                resStreamWriter.WriteLine($"{element.GetId().ToString()} -1");
                return;
            }

            resStreamWriter.WriteLine(
                $"{element.GetId().ToString()} {Convert.ToInt32(element.IsOnInterface()).ToString()}");
        }

        private void WriteMaxNumberOfGrowthLayesrsAroudSphere(StreamWriter resStreamWriter)
        {
            String[] resultsHeader = 
            {
                "Result \"Max Number Of Possible Growth\" \"Time Step \"   0 Scalar OnNodes",
                "Values"
            };
            String[] resultsEnd = { "End Values" };
            resultsHeader.ToList().ForEach(resStreamWriter.WriteLine);
            _dataRepository
                .GetDiscreteElements()
                .Values
                .ToList()
                .ForEach(element => WriteLineOfLayerGrowthResult(element, resStreamWriter));
            resultsEnd.ToList().ForEach(resStreamWriter.WriteLine);
        }

        private void WriteLineOfLayerGrowthResult(IDiscreteElement element, StreamWriter resStreamWriter)
        {
            if (element.GetGroupId() == 1)
            {
                resStreamWriter.WriteLine($"{element.GetId().ToString()} -1");
                return;
            }

            resStreamWriter.WriteLine($"{element.GetId().ToString()} {element.GetNOfLayersAroundTheElement().ToString()}");
        }

        private void WriteMaxRemeshRadiusSpheres(StreamWriter resStreamWriter)
        {
            String[] resultsHeader = 
            {
                "Result \"Max Remesh Radius\" \"Time Step \"   0 Scalar OnNodes",
                "Values"
            };
            String[] resultsEnd = { "End Values" };
            resultsHeader.ToList().ForEach(resStreamWriter.WriteLine);
            _dataRepository
                .GetDiscreteElements()
                .Values
                .ToList()
                .ForEach(element => resStreamWriter
                    .WriteLine($"{element.GetId().ToString()} {element.GetMaxRadius().ToString()}"));
            resultsEnd.ToList().ForEach(resStreamWriter.WriteLine);
        }
    }
    

}