using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using ippt.dem.mesh.entities.core;
using ippt.dem.mesh.entities.nodes;
using ippt.dem.mesh.repository;

namespace ippt.dem.mesh.system.parser
{
    public class ConcreteAbaqusParser:IAbaqusParser
    {
        private const string NodesBegin = @"^\*\*NODE DATA BEGIN";
        private const string NodesEnd = @"^\*\*NODE DATA END";
        private const string ElementsMask = @"^\*\*ELEMENTS \(HEXAHEDRA\) \- Part\: Mask*";
        private const int NumberOfElementsInNodeLineString = 4; // 3 coordinates + 1 node id
        private const int NumberOfElementsInHexahedronElementLineString = 9; // 8 vertices + 1 element id
        private const int NumberOfElementsInTetrahedronElementLineString = 5; // 4 vertices + 1 element id
       
        private readonly DataRepository _dataRepository;
        private readonly NodeCreator _nodeCreator;

        public ConcreteAbaqusParser(DataRepository dataRepository, NodeCreator nodeCreator)
        {
            _dataRepository = dataRepository;
            _nodeCreator = nodeCreator;
        }

        public void parse(List<string> data)
        {
            Position nodesPosition = GetNodePositions(data);
            List<Position> elementsPositions = GetElementPositions(data);
            ParseNodes(data.GetRange(nodesPosition.GetBegin(),nodesPosition.GetRange()));
            foreach (var position in elementsPositions)
            {
                ParseElementsSet(data.GetRange(position.GetBegin()+1,position.GetRange()-1));
            }
        }

        private void ParseElementsSet(List<string> elements)
        {
            var nodesID = new List<long>();
            foreach (var line in elements)
            {
                nodesID.Clear();
                try
                {
                    var elementData = line.Split(',').ToList();
                    bool a = elementData.Count == NumberOfElementsInHexahedronElementLineString && elementData.Count == NumberOfElementsInTetrahedronElementLineString;
                    if (a)
                    {
                        throw new InvalidDataException($"Wrong elements data read: {line}");
                    }

                    var id = long.Parse(elementData[0]);
                    for (var i = 1; i < elementData.Capacity; i++)
                    {
                        nodesID.Add(long.Parse(elementData[i]));
                    }
                   // _dataRepository.AddElement();
                    
                }
                catch (Exception e)
                {
                    throw new InvalidDataException($"Wrong nodes data read: {line}");
                }
            }
        }
        
        private void ParseNodes(List<string> nodes)
        {
            var coordinates = new List<double>();
            foreach (var line in nodes)
            {
                coordinates.Clear();
                try
                {
                    var nodeData = line.Split(',').ToList();
                    if (nodeData.Count != NumberOfElementsInNodeLineString)
                    {
                        throw new InvalidDataException($"Wrong nodes data read: {line}");
                    }

                    var id = long.Parse(nodeData[0]);
                    for (var i = 1; i < 4; i++)
                    {
                        coordinates.Add(double.Parse(nodeData[i]));
                    }
                    _dataRepository.AddNode(_nodeCreator.FactoryMethod(NodeDto.Get(id,coordinates)));
                    
                }
                catch (Exception e)
                {
                    throw new InvalidDataException($"Wrong nodes data read: {line}");
                }
            }
        }
        
        private List<Position> GetElementPositions(List<string> data)
        {
             /*
                 Abaqus format:
                 offset for begin is 1 lines
                 offset for end is 0 line
             */
            List<Position> result = new List<Position>();

            List<long> positions = new List<long>();
            
            for (var i = 0; i < data.Count; i++)
            {
                // Match the start of a string.
                if (Regex.IsMatch(data[(int) i], ElementsMask))
                {
                    positions.Add(i);
                }
            }
            
            for (var i = 0; i < positions.Count/2; i++)
            {
                result.Add(new Position(positions[2*i]+1,positions[2*i+1]));
            }
            
            return result;
        }
 
        private Position GetNodePositions(List<string> data)
        {
            /*
                Abaqus format:
                offset for begin is 5 lines
                offset for end is -1 line
            */
            
            var begin = SearchLine(data, 0, NodesBegin) + 6;
            var end = SearchLine(data, begin + 1, NodesEnd) - 1;
            return new Position(begin,end);
        }

        private long SearchLine(List<string> data, long offset, string pattern)
        {
            try
            {
                for (var i = offset; i < data.Count; i++)
                { 
                    if (Regex.IsMatch(data[(Index) i], pattern))
                    {
                        return i;
                    }
                }
            }
            catch (Exception e)
            {
                throw new RegexMatchTimeoutException(e.Message);
            }
            

            return 0;
        }

      
    }

    internal class Position
    {
        private long begin;
        private long end;

        public Position(long begin, long end)
        {
            this.begin = begin;
            this.end = end;
        }

        public int GetBegin()
        {
            return (int) begin;
        }
        
        public int GetRange()
        {
            return (int) (end-begin);
        }
    }
}
