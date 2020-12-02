using System;
using System.Collections.Generic;
using System.Globalization;
using ippt.dem.mesh.entities.core;
using ippt.dem.mesh.repository;

namespace ippt.dem.mesh.entities.nodes
{
    public class Node : INode
    {
        private readonly long _id;
        private readonly List<double> _listOfCoordinates;
        private readonly HashSet<long> _elements;
        private readonly HashSet<long> _groups;
   
        public Node(NodeDto nodeDto)
        {
            _id = nodeDto.GetId();
            _listOfCoordinates = nodeDto.GetCoordinates();
            _elements = new HashSet<long>();
            _groups = new HashSet<long>();
        }

        public HashSet<long> GetGroupSet()
        {
            return _groups;
        }

        public HashSet<long> GetElementSet()
        {
            return _elements;
        }

        public void AddToGroupSet(long id)
        {
            _groups.Add(id);
        }

        public void AddToElementSet(long id)
        {
            _elements.Add(id);
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
                                  $" {_listOfCoordinates[0].ToString(CultureInfo.InvariantCulture)}" +
                                  $" {_listOfCoordinates[1].ToString(CultureInfo.InvariantCulture)}" +
                                  $" {_listOfCoordinates[2].ToString(CultureInfo.InvariantCulture)}";
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

        public void AddElementInformation(in long elementId, in int groupId)
        {
            AddToGroupSet(groupId);
            AddToElementSet(elementId);
        }
    }
}