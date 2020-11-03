using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ippt.dem.mesh.entities.nodes;
using ippt.dem.mesh.repository;
using Microsoft.Extensions.Logging;

namespace ippt.dem.mesh.entities.discrete.element
{
    public class ReMeshDiscreteElement
    {
        private readonly IDataRepository _dataRepository;
        private readonly ILogger _log;
        private Dictionary<double, long> _properties;
        private Dictionary<long, IDiscreteElement> _elements;
        private Dictionary<long, INode> _nodes;
        private const double Delta = 1e-6;

        private Dictionary<double, List<long>> _nodeIdWhereRadiusPossible;

        public ReMeshDiscreteElement(IDataRepository dataRepository,
            ILogger<ReMeshDiscreteElement> log)
        {
            _dataRepository = dataRepository;
            _log = log;
            _elements = new Dictionary<long, IDiscreteElement>();
            _nodes = new Dictionary<long, INode>();
            _nodeIdWhereRadiusPossible = new Dictionary<double, List<long>>();
        }

        public void Run()
        {
            int groupId = 2;
            DataSetup(groupId);
            //GetRemeshRadiusSorted()
            //    .ForEach(r => UpdateMesh(r, _properties[r]));
            Parallel.ForEach(GetRemeshRadiusSorted(), r => UpdateMesh(r, _properties[r]));
        }

        private void DataSetup(int groupId)
        {
            _dataRepository.CleanUpFiniteElementDataInformation();
            _properties = _dataRepository.GetRemeshProperties();
            _dataRepository
                .GetDiscreteElementGroup()[groupId]
                .ForEach(id =>
                    _elements.Add(id, DiscreteSphereElement.Get(_dataRepository.GetDiscreteElementById(id))));
            _elements
                .Values
                .ToList()
                .ForEach(element => _nodes
                    .Add(element.GetCenterNodeId(),
                        _dataRepository.GetDiscreteElementNode(element.GetCenterNodeId())));
            //SetupBoxes();
            _properties.Keys.ToList().ForEach(r => _nodeIdWhereRadiusPossible.Add(r, new List<long>()));
        }

        private void SetupBoxes()
        {
            List<double> xCoordinate = new List<double>();
            List<double> yCoordinate = new List<double>();
            List<double> zCoordinate = new List<double>();
            _nodes.Values.ToList().ForEach(node =>
            {
                if (node == null) throw new ArgumentNullException(nameof(node));
                xCoordinate.Add(node.GetCoordinates()[0]);
                yCoordinate.Add(node.GetCoordinates()[1]);
                zCoordinate.Add(node.GetCoordinates()[2]);
            });
            xCoordinate.Sort();
            yCoordinate.Sort();
            zCoordinate.Sort();
        }


        private List<double> GetRemeshRadiusSorted()
        {
            List<double> remeshRadius = _properties.Keys.ToList();
            remeshRadius.Sort((a, b) => b.CompareTo(a));
            return remeshRadius;
        }

        private void UpdateMesh(double radius, long numberOfElements)
        {
            _nodes.Values.ToList()
                .ForEach(node => SearchMaxRadiusPossible(node.GetId(), node.GetCoordinates(), radius));
        }

        private List<INode> filterXYNodes(double x, double y)
        {
            return _nodes
                .Values
                .ToList()
                .Where(n => Math.Abs(n.GetCoordinates()[0] - x) < Delta &&
                            Math.Abs(n.GetCoordinates()[1] - y) < Delta)
                .OrderByDescending(node => node.GetCoordinates()[2])
                .ToList();
        }

        private List<INode> filterZGreaterThanRange(List<INode> nodes, double radius, double z)
        {
            return nodes
                .Where(n =>
                {
                    return Math.Abs(n.GetCoordinates()[2] - radius) > z ||
                           Math.Abs(n.GetCoordinates()[2] + radius) < z;
                }).ToList()
                .OrderByDescending(node => node.GetCoordinates()[2])
                .ToList();
        }

        private List<INode> filterXZNodes(double x, double z)
        {
            return _nodes
                .Values
                .ToList()
                .Where(n => Math.Abs(n.GetCoordinates()[0] - x) < Delta &&
                            Math.Abs(n.GetCoordinates()[2] - z) < Delta)
                .OrderByDescending(node => node.GetCoordinates()[2])
                .ToList();
        }

        private List<INode> filterYGreaterThanRange(List<INode> nodes, double radius, double y)
        {
            return nodes
                .Where(n =>
                {
                    return Math.Abs(n.GetCoordinates()[1] - radius) > y ||
                           Math.Abs(n.GetCoordinates()[1] + radius) < y;
                }).ToList()
                .OrderByDescending(node => node.GetCoordinates()[1])
                .ToList();
        }

        private List<INode> filterYZNodes(double y, double z)
        {
            return _nodes
                .Values
                .ToList()
                .Where(n => Math.Abs(n.GetCoordinates()[1] - y) < Delta &&
                            Math.Abs(n.GetCoordinates()[2] - z) < Delta)
                .OrderByDescending(node => node.GetCoordinates()[0])
                .ToList();
        }

        private List<INode> filterXGreaterThanRange(List<INode> nodes, double radius, double x)
        {
            return nodes
                .Where(n =>
                {
                    return Math.Abs(n.GetCoordinates()[0] - radius) > x ||
                           Math.Abs(n.GetCoordinates()[0] + radius) < x;
                }).ToList()
                .OrderByDescending(node => node.GetCoordinates()[0])
                .ToList();
        }

        private void SearchMaxRadiusPossible(long nodeId, List<double> coordinates, double radius)
        {
            if (_nodeIdWhereRadiusPossible[radius].Count > 100)
                return;
            
            var nodesWithSameXyAndZGreaterThanRange
                = filterZGreaterThanRange(filterXYNodes(coordinates[0], coordinates[1]), radius, coordinates[2]);
            INode nodeZPlus =
                nodesWithSameXyAndZGreaterThanRange.FirstOrDefault(node => node.GetCoordinates()[2] > coordinates[2]);
            INode nodeZMinus =
                nodesWithSameXyAndZGreaterThanRange.FirstOrDefault(node => node.GetCoordinates()[2] < coordinates[2]);
            if (nodeZPlus == null || nodeZMinus == null)
                return;

            var nodesWithSameXzAndYGreaterThanRange
                = filterYGreaterThanRange(filterXZNodes(coordinates[0], coordinates[2]), radius, coordinates[1]);
            INode nodeYPlus =
                nodesWithSameXzAndYGreaterThanRange.FirstOrDefault(node => node.GetCoordinates()[1] > coordinates[1]);
            INode nodeYMinus =
                nodesWithSameXzAndYGreaterThanRange.FirstOrDefault(node => node.GetCoordinates()[1] < coordinates[1]);
            if (nodeYPlus == null || nodeYMinus == null)
                return;

            var nodesWithSameYzAndXGreaterThanRange
                = filterXGreaterThanRange(filterYZNodes(coordinates[1], coordinates[2]), radius, coordinates[0]);
            INode nodeXPlus =
                nodesWithSameYzAndXGreaterThanRange.FirstOrDefault(node => node.GetCoordinates()[0] > coordinates[0]);
            INode nodeXMinus =
                nodesWithSameYzAndXGreaterThanRange.FirstOrDefault(node => node.GetCoordinates()[0] < coordinates[0]);
            if (nodeXPlus == null || nodeXMinus == null)
                return;
            
            _nodeIdWhereRadiusPossible[radius].Add(nodeId);
        }
    }

}