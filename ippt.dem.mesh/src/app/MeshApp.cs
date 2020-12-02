   
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using ippt.dem.mesh.entities.core;
using ippt.dem.mesh.entities.discrete.element;
using ippt.dem.mesh.repository;
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
        private readonly IDataRepository _dataRepository;
        public MeshApp(ILogger<MeshApp> log,
            ICoreTextFileRead coreTextFileRead,
            IAbaqusParser abaqusParser,
            IWriteOutputResults writeOutputResults,
            ReMeshDiscreteElement reMeshDiscreteElement,
            IComaSeparatedDataParser comaSeparatedDataParser,
            IDataRepository dataRepository)
        {
            _log = log;
            _coreTextFileRead = coreTextFileRead;
            _abaqusParser = abaqusParser;
            _writeOutputResults = writeOutputResults;
            _reMeshDiscreteElement = reMeshDiscreteElement;
            _comaSeparatedDataParser = comaSeparatedDataParser;
            _dataRepository = dataRepository;
        }

        internal void Run(string path,
            string reMeshDataPath,
            DemMeshProcessCase mesherCase)
        {
            
            _log.LogInformation("Application {applicationEvent} at {dateTime}",
                "Started",
                DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            _abaqusParser.Parse(_coreTextFileRead.ReadFromFile(path));
            _dataRepository.LogVolumeInformation();
            NodesToElementsMapper.MapElementsToNodes(_dataRepository);
            FiniteElementBoundarySearch.SetBoundaryElements(_dataRepository);
            DiscreteElementOperations.UpdateBoundaryData(_dataRepository);
            //_dataRepository.SetElementNeighbourElement();
            _log.LogInformation("Application {applicationEvent} at {dateTime}", "parsing data from inp file ended", DateTime.UtcNow.ToString());
            _log.LogInformation("Application {applicationEvent} at {dateTime}", "searching elements in contact", DateTime.UtcNow.ToString());
            //ContactElementSearch.ContactSearchOfHexaElements(_dataRepository);
            _log.LogInformation("Application {applicationEvent} at {dateTime}", "searching elements in contact ended", DateTime.UtcNow.ToString());
            switch (mesherCase)
            {
                case StoreRawDemMesh:
                    _writeOutputResults.WriteOutput(path);
                    break;
                case StoreReMeshedDemMesh:
                    _comaSeparatedDataParser.ParseReMeshData(_coreTextFileRead.ReadFromFile(reMeshDataPath));
                    _dataRepository.CleanUpFiniteElementDataInformation();
                    DiscreteElementOperations.UpdateGrowthAbilityOfDiscreteElementInGroup(_dataRepository, 2);
                    //_reMeshDiscreteElement.Run();
                    _writeOutputResults.WriteOutput(path);
                    break;
            }
            _log.LogInformation("Application {applicationEvent} at {dateTime}",
                "Ended",
                DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
        }
    }
}