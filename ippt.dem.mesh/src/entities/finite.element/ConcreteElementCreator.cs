using System.ComponentModel.DataAnnotations;
using ippt.dem.mesh.entities.core;
using ippt.dem.mesh.entities.discrete.element;
using ippt.dem.mesh.repository;
using ippt.dem.mesh.system.parser;

namespace ippt.dem.mesh.entities.finite.element
{
    public class ConcreteElementCreator : ElementCreator
    {
        private const int NumberOfVerticesInHexahedron = 8;
        private const int NumberOfVerticesInTetrahaedr = 4;
        private readonly DiscreteElementCreator _discreteElementCreator;
        private readonly IDataRepository _dataRepository;

        public ConcreteElementCreator(DiscreteElementCreator discreteElementCreator,
            IDataRepository dataRepository)
        {
            _discreteElementCreator = discreteElementCreator;
            _dataRepository = dataRepository;
        }

        public override IElement FactoryMethod(ElementDto elementDto)
        {
            switch (elementDto.GetNodesId().Count)
            {
                case NumberOfVerticesInHexahedron: 
                    return new HexahedronElement(elementDto, _discreteElementCreator, _dataRepository);
                case NumberOfVerticesInTetrahaedr:
                    return new TetrahaedrElement(elementDto);
                default:
                    throw new ValidationException(
                        $"Invalid number of available vertices: {elementDto.GetNodesId().Count}{elementDto}");
            }
        }
    }
}