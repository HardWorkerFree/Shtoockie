using System;

using Shtoockie.Matematika;

namespace Shtoockie.Fizika
{
    public class World2DEdge : Body
    {
        public override int Kind => World2D.World2DEdgeKind;
    
        private readonly Vector2N _normal;
        public Vector2N Normal => _normal;

        private readonly Numerus _edge;
        public Numerus Edge => _edge;

        public World2DEdge(Vector2N position, Vector2N normal, Numerus edge) : base(Numerus.Max, position)
        {
            _normal = normal;
            _edge = edge;
        }
    }
}
