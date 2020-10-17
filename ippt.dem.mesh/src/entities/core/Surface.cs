using System.Collections.Generic;
using System.Linq;

namespace ippt.dem.mesh.entities.core
{
    public enum SurfaceName
    {
        Quad
    }
    
    /*
        element created from nodes:
        1 2 3 4 5 6 7 8
        6 surfaces: 
        bottom: 1 2 3 4    -> _neighboursId[0]
        top: 5 6 7 8       -> _neighboursId[1]      
        XZ: 1 2 6 5        -> _neighboursId[2]
        YZ: 2 3 7 6        -> _neighboursId[3]
        MinusXZ : 4 3 7 8  -> _neighboursId[4]
        MinusYZ : 1 4 8 5  -> _neighboursId[5]
        https://abaqus-docs.mit.edu/2017/English/SIMACAEELMRefMap/simaelm-c-solidcont.htm
    */
    public enum PositionInQuadElement
    {
        Bottom,
        Top,
        Xz,
        Yz,
        MinusXz,
        MinusYz
    }
    
    public abstract class AbstractSurface
    {
        protected AbstractSurface(List<long> nodeIds, SurfaceName surfaceName)
        {
            this.NodeIds = nodeIds;
        }

        public List<long> NodeIds { get; }

        public override bool Equals(object? obj)
        {
            var other = obj as AbstractSurface;

            if (other == null)
                return false;
            var result = NodeIds.SequenceEqual(other.NodeIds);
            return result;
        }

        protected bool Equals(AbstractSurface other)
        {
            return Equals(NodeIds, other.NodeIds);
        }

        public override int GetHashCode()
        {
            return (NodeIds != null ? NodeIds.GetHashCode() : 0);
        }
    }

    public class QuadSurface : AbstractSurface
    {
        private PositionInQuadElement _position;
        public QuadSurface(List<long> nodeIds, PositionInQuadElement position) : base(nodeIds, SurfaceName.Quad)
        {
            _position = position;
        }

        public PositionInQuadElement Position => _position;
    }
}