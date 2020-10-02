using System;
using System.Collections.Generic;
using ippt.dem.mesh.entities.core;
using ippt.dem.mesh.repository;

namespace ippt.dem.mesh.entities.nodes
{
    public class Node : INode
    {
        private readonly long _id;
        private readonly List<double> _listOfCoordinates;

        public Node(NodeDto nodeDto)
        {
            this._id = nodeDto.GetId();
            this._listOfCoordinates = nodeDto.GetCoordinates();
        }

        public long GetId()
        {
            return _id;
        }

        public List<double> GetCoordinates()
        {
            return _listOfCoordinates;
        }

        public string ToString(FileFormat format)
        {
            string nodeToString = $"            {_id.ToString()}" +
                                  $" {_listOfCoordinates[0].ToString()}" +
                                  $" {_listOfCoordinates[1].ToString()}" +
                                  $" {_listOfCoordinates[2].ToString()}";
            switch (format)
            {
                case FileFormat.Dat:
                    return nodeToString;
                case FileFormat.Msh:
                    return nodeToString;
                default:
                    throw new Exception($"unknown file format: {format}");
            }
        }
    }
}