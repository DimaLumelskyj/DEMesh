using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ippt.dem.mesh.repository;

namespace ippt.dem.mesh.system.parser
{
    public class ConcreteAbaqusParser:IAbaqusParser
    {
        private readonly string NODES_BEGIN = @"^\*\*NODE DATA BEGIN";
        private readonly string NODES_END = @"^\*\*NODE DATA END";
        private readonly string ELEMENTS_MASK = @"^\*\*ELEMENTS \(HEXAHEDRA\) \- Part\: Mask*";
        
        private DataRepository dataRepository;

        public ConcreteAbaqusParser(DataRepository dataRepository)
        {
            this.dataRepository = dataRepository;
        }

        public void parse(List<string> data)
        {
            var nodesPosition = GetNodePositions(data);
            List<Position> elementsPositions = GetElementPositions(data);

        }
        
        private List<Position> GetElementPositions(List<string> data)
        {
            /*
                offset for begin is 1 lines
                offset for end is 0 line
            */
            List<Position> result = new List<Position>();

            List<long> positions = new List<long>();
            
            for (var i = 0; i < data.Count; i++)
            {
                // Match the start of a string.
                if (Regex.IsMatch(data[(int) i], ELEMENTS_MASK))
                {
                    positions.Add(i);
                }
            }
            
            for (var i = 0; i < positions.Count/2; i++)
            {
                result.Add(new Position(positions[i]+1,positions[i+1]));
            }
            
            return result;
        }
 
        private Position GetNodePositions(List<string> data)
        {
            /*
            offset for begin is 5 lines
            offset for end is -1 line
            
            **NODE DATA BEGIN
            **==============================================================================
            *NODE
            **------------------------------------------------------------------------------
            **INDEX, X COORD, Y COORD, Z COORD
            **------------------------------------------------------------------------------
             
            .................................................................................
            
            **==============================================================================
            **NODE DATA END
            **==============================================================================
            */
            
            var begin = SearchLine(data, 0, NODES_BEGIN) + 5;
            var end = SearchLine(data, begin + 1, NODES_END) - 1;
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
        long begin { get; }
        long end { get; }

        public Position(long begin, long end)
        {
            this.begin = begin;
            this.end = end;
        }
    }
}
