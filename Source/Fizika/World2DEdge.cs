using System;

using Shtoockie.Matematika;

namespace Shtoockie.Fizika
{
    public class World2DEdge : Body2D
    {
        public override int Code => World2D.EdgeCode;
    
        private readonly Vector2N _normal;
        public Vector2N Normal => _normal;

        private readonly Numerus _edge;
        public Numerus Edge => _edge;

        public World2DEdge(Vector2N position, Vector2N normal, Numerus edge) : base(Numerus.Max, position, Numerus.Max)
        {
            _normal = normal;
            _edge = edge;
        }
    }
}
