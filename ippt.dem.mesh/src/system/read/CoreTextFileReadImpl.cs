using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using Microsoft.Extensions.Logging;

namespace ippt.dem.mesh.system
{
    public class CoreTextFileReadImpl : ICoreTextFileRead
    {
        private readonly ILogger log;

        public CoreTextFileReadImpl(ILogger<CoreTextFileReadImpl> log)
        {
            this.log = log;
        }

        public List<string> ReadFromFile(string path)
        {
            List<string> result = new List<string>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            try
            {
                log.LogInformation($"Reading file: {path}");

                using (StreamReader streamReader = new StreamReader(path))
                {
                    while (streamReader.Peek() >= 0)
                    {
                        result.Add(streamReader.ReadLine());
                    }
                }
                
                stopwatch.Stop();
                log.LogInformation(
                    $"File successfully read into memory, elapsed={TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds).TotalSeconds.ToString()} [seconds]");
                return result;
            }
            catch (Exception e)
            {
                log.LogError($"Reading file: {path} error");
                log.LogError($"Stacktrace:\n {e}");
                throw new FileLoadException(e.Message);
            }
        }

    }
}