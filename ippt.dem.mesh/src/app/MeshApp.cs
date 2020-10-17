   
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using ippt.dem.mesh.entities.discrete.element;
using ippt.dem.mesh.system;
using ippt.dem.mesh.system.parser;
using ippt.dem.mesh.system.write;
using static ippt.dem.mesh.app.DemMeshProcessCase;

namespace ippt.dem.mesh.app
{
    enum DemMeshProcessCase
    {
        StoreRawDemMesh,
        StoreReMeshedDemMesh
    }
    public class MeshApp
    {
        private readonly ILogger _log;
        private readonly ICoreTextFileRead _coreTextFileRead;
        private readonly IAbaqusParser _abaqusParser;
        private readonly IWriteOutputResults _writeOutputResults;
        private readonly ReMeshDiscreteElement _reMeshDiscreteElement;
        private readonly IComaSeparatedDataParser _comaSeparatedDataParser;
        public MeshApp(ILogger<MeshApp> log,
            ICoreTextFileRead coreTextFileRead,
            IAbaqusParser abaqusParser,
            IWriteOutputResults writeOutputResults, ReMeshDiscreteElement reMeshDiscreteElement, IComaSeparatedDataParser comaSeparatedDataParser)
        {
            this._log = log;
            this._coreTextFileRead = coreTextFileRead;
            this._abaqusParser = abaqusParser;
            _writeOutputResults = writeOutputResults;
            _reMeshDiscreteElement = reMeshDiscreteElement;
            _comaSeparatedDataParser = comaSeparatedDataParser;
        }

        internal void Run(string path, string reMeshDataPath)
        {
            
            _log.LogInformation("Application {applicationEvent} at {dateTime}", "Started", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            _abaqusParser.Parse(_coreTextFileRead.ReadFromFile(path));
            var processDataCase = StoreReMeshedDemMesh;
            switch (processDataCase)
            {
                case StoreRawDemMesh:
                    _writeOutputResults.WriteOutput(path);
                    break;
                case StoreReMeshedDemMesh:
                    _comaSeparatedDataParser.ParseReMeshData(_coreTextFileRead.ReadFromFile(reMeshDataPath));
                    break;
            }
            _log.LogInformation("Application {applicationEvent} at {dateTime}", "Ended", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
        }
    }
}