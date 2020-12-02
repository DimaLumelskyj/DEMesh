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
        private long counter = 0;

        public ReMeshDiscreteElement(IDataRepository dataRepository,
            ILogger<ReMeshDiscreteElement> log)
        {
            _dataRepository = dataRepository;
            _log = log;
            _elements = new Dictionary<long, IDiscreteElement>();
            _nodes = new Dictionary<long, INode>();
        }

        public void Run()
        {
            int groupId = 2;
            DataSetup(groupId);
            SearchMaxPossibleRemeshRadius();
        }

        private void DataSetup(int groupId)
        {
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
        }

   

        private void SearchMaxPossibleRemeshRadius()
        {
            var maxRadius = _dataRepository.GetRemeshMaxRadius();
            long maxIterations = (long) (maxRadius / _elements[_elements.Keys.ToList()[0]].GetRadius());
            _elements.Values.ToList().ForEach(element => SearchMaxRadius(maxIterations, element));
        }

        private void SearchMaxRadius(in long maxIterations, IDiscreteElement element)
        {
            counter++;
            if (element.IsOnInterface())
            {
                _elements[element.GetId()].SetMaxRadius(1);
                return;
            }
            var iteration = 0;
            HashSet<long> setOfElementsToDelete = new HashSet<long>();
            HashSet<long> setOfElementsAddToDelete = new HashSet<long>();
            setOfElementsAddToDelete.UnionWith(element.GetNeighbours());
            while (++iteration <= maxIterations)
            {
                bool exit = false;
                var prevStepSetOfElementsToDelete = setOfElementsToDelete.ToList().ToHashSet();
                setOfElementsToDelete.UnionWith(setOfElementsAddToDelete);
                var idsToCheck = setOfElementsToDelete
                    .Where(id => !prevStepSetOfElementsToDelete.Contains(id))
                    .ToHashSet();
                foreach (var id in idsToCheck)
                {
                    if (_elements[id].IsOnInterface())
                    {
                        exit = true;
                        break;
                    }
                }

                if (exit)
                {
                    return;
                }
                
                setOfElementsAddToDelete.Clear();
                idsToCheck
                    .ToList()
                    .ForEach(id => setOfElementsAddToDelete.UnionWith(_elements[id].GetNeighbours()));
            }

            //_dataRepository.UpdateMaxDiscreteElementPossibleRemeshRadius(element.GetId(), iteration);
        }
    }

}