   
using Microsoft.Extensions.Logging;
using System;
using ippt.dem.mesh.system;
using ippt.dem.mesh.system.parser;
using ippt.dem.mesh.system.write;

namespace ippt.dem.mesh.app
{
    public class MeshApp
    {
        private readonly ILogger _log;
        private readonly ICoreTextFileRead _coreTextFileRead;
        private readonly IAbaqusParser _abaqusParser;
        private readonly IWriteOutputResults _writeOutputResults;
        public MeshApp(ILogger<MeshApp> log,
            ICoreTextFileRead coreTextFileRead,
            IAbaqusParser abaqusParser,
            IWriteOutputResults writeOutputResults)
        {
            this._log = log;
            this._coreTextFileRead = coreTextFileRead;
            this._abaqusParser = abaqusParser;
            _writeOutputResults = writeOutputResults;
        }

        internal void Run(string path)
        {
            _log.LogInformation("Application {applicationEvent} at {dateTime}", "Started", DateTime.UtcNow.ToString());
            _abaqusParser.parse(_coreTextFileRead.ReadFromFile(path));
            _writeOutputResults.WriteOutput(path);
            _log.LogInformation("Application {applicationEvent} at {dateTime}", "Ended", DateTime.UtcNow.ToString());
        }
    }
}