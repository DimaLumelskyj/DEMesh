   
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;

namespace ippt.dem.mesh.app
{
    public class MeshApp
    {
        private readonly ILogger _logger;

        public MeshApp(ILogger<MeshApp> logger)
        {
            _logger = logger;
        }

        internal void Run()
        {
            string path = "C:\\Log\\1_100.inp";
            string readContents;
            using (StreamReader streamReader = new StreamReader(path, Encoding.UTF8))
            {
                readContents = streamReader.ReadToEnd();
            }
            
            _logger.LogInformation("Application {applicationEvent} at {dateTime}", "Started",
                DateTime.UtcNow.ToString());
            _logger.LogInformation("Application {applicationEvent} at {dateTime}", "Ended", DateTime.UtcNow.ToString());
        }
    }
}