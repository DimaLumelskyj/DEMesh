using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ippt.dem.mesh.repository;
using Microsoft.Extensions.Logging;

namespace ippt.dem.mesh.system.parser
{
    public class ComaSeparatedDataParser : IComaSeparatedDataParser
    {
        private readonly IDataRepository _dataRepository;
        private readonly ILogger _log;
     
        public ComaSeparatedDataParser(ILogger log, IDataRepository dataRepository)
        {
            _log = log;
            _dataRepository = dataRepository;
        }

        public void ParseReMeshData(List<string> data)
        {
            foreach (var line in data)
            {
                try
                {
                    var rawData = Array.ConvertAll(line.Split(','), double.Parse).ToList();
                    _dataRepository.AddReMeshInputData( (long) rawData[0] ,rawData[1]);
                }
                catch (Exception e)
                {
                    throw new InvalidDataException($"Wrong data read: {line}" + e.Message);
                }
            }
        }
    }
}