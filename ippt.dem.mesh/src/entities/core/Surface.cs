using System.Collections.Generic;
using System.Linq;

namespace ippt.dem.mesh.entities.finite.element
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
        XZ,
        YZ,
        MinusXZ,
        MinusYZ
    }
    
    public abstract class AbstractSurface
    {
        private List<long> _nodeIds;
        private SurfaceName _surfaceName;
        
        protected AbstractSurface(List<long> nodeIds, SurfaceName surfaceName)
        {
            this._nodeIds = nodeIds;
            _surfaceName = surfaceName;
        }

        public List<long> NodeIds => _nodeIds;

        public SurfaceName SurfaceName => _surfaceName;
        
        public override bool Equals(object? obj)
        {
            var other = obj as AbstractSurface;

            if (other == null)
                return false;
            var result = _nodeIds.SequenceEqual(other._nodeIds);
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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