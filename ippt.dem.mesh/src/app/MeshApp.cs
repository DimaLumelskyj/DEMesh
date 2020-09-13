   
using Microsoft.Extensions.Logging;
using System;
using ippt.dem.mesh.system;
using ippt.dem.mesh.system.parser;

namespace ippt.dem.mesh.app
{
    public class MeshApp
    {
        private readonly ILogger log;
        private readonly ICoreTextFileRead coreTextFileRead;
        private readonly IAbaqusParser abaqusParser;
        public MeshApp(ILogger<MeshApp> log,
            ICoreTextFileRead coreTextFileRead,
            IAbaqusParser abaqusParser)
        {
            this.log = log;
            this.coreTextFileRead = coreTextFileRead;
            this.abaqusParser = abaqusParser;
        }

        internal void Run(string path)
        {
            log.LogInformation("Application {applicationEvent} at {dateTime}", "Started", DateTime.UtcNow.ToString());
            abaqusParser.parse(coreTextFileRead.ReadFromFile(path));
            log.LogInformation("Application {applicationEvent} at {dateTime}", "Ended", DateTime.UtcNow.ToString());
        }
    }
}