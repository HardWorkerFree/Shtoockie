using System.Collections.Generic;

using Shtoockie.Matematika;

namespace Shtoockie.Fizika
{
    public class World2D : BaseWorld
    {
        public const int PointKind = 0;
        public const int World2DEdgeKind = 1;
        public const int RoundBodyKind = 2;

        record class GridCoords(int X, int Y);

        private readonly int _xLength;
        private readonly int _yLength;

        private readonly Dictionary<Body, GridCoords> _allBodies;
        private readonly HashSet<Body>[,] _grid;

        private readonly Numerus _sectionLength;
        public Numerus SectionLength => _sectionLength;

        private readonly Vector2N _boundsStart;
        private readonly Vector2N _boundsEnd;
        private readonly HashSet<Body> _outOfBoundsBodies;
        private readonly Dictionary<Body, Body> _intersectedBodies;

        public World2D(Numerus sectionLength, int gridXLength, int gridYLength)
        {
            _allBodies = new Dictionary<Body, GridCoords>();
            _xLength = gridXLength;
            _yLength = gridYLength;
            _grid = new HashSet<Body>[_xLength, _yLength];
            _sectionLength = sectionLength;
            _boundsStart = new Vector2N(_sectionLength);
            _boundsEnd = new Vector2N((Numerus)(_xLength - 1), (Numerus)(_yLength - 1)) * _sectionLength;
            _outOfBoundsBodies = new HashSet<Body>();
            _intersectedBodies = new Dictionary<Body, Body>();

            FillGrid();
        }

        private void FillGrid()
        {
            for (int x = 0; x < _xLength; x++)
            {
                for (int y = 0; y < _yLength; y++)
                {
                    _grid[x, y] = new HashSet<Body>();

                    Numerus edge = _sectionLength;
                    Vector2N edgeNormal = Vector2N.Zero;

                    if (x == 0)
                    {
                        edgeNormal = new Vector2N(Numerus.One, Numerus.Zero);
                    }
                    else if (x == (_xLength - 1))
                    {
                        edge = (Numerus)x * _sectionLength;
                        edgeNormal = new Vector2N(-Numerus.One, Numerus.Zero);
                    }
                    else if (y == 0)
                    {
                        edgeNormal = new Vector2N(Numerus.Zero, Numerus.One);
                    }
                    else if (y == (_yLength - 1))
                    {
                        edge = (Numerus)y * _sectionLength;
                        edgeNormal = new Vector2N(Numerus.Zero, -Numerus.One);
                    }

                    if (edgeNormal != Vector2N.Zero)
                    {
                        Vector2N edgePosition = new Vector2N(_sectionLength.Halve() + (Numerus)x * _sectionLength, _sectionLength.Halve() + (Numerus)y * _sectionLength);
                        World2DEdge worldEdge = new World2DEdge(edgePosition, edgeNormal, edge);
                        Materialize(worldEdge);
                    }
                }
            }
        }

        private bool CheckOutOfBounds(Body body)
        {
            return (body.Position.X < _boundsStart.X) || (body.Position.Y < _boundsStart.Y) || (body.Position.X > _boundsEnd.X) || (body.Position.Y > _boundsEnd.Y);
        }

        private GridCoords ConvertToCoords(Vector2N position)
        {
            return new GridCoords((int)(position.X / _sectionLength), (int)(position.Y / _sectionLength));
        }

        public override void Materialize(Body body)
        {
            if (CheckOutOfBounds(body))
            {
                if (body.Kind != World2DEdgeKind)
                {
                    return;
                }
            }

            GridCoords coords = ConvertToCoords(body.Position);
            _grid[coords.X, coords.Y].Add(body);
            _allBodies[body] = coords;
        }

        public override void Annihilate(Body body)
        {
            if (_allBodies.ContainsKey(body))
            {
                GridCoords coords = _allBodies[body];
                _grid[coords.X, coords.Y].Remove(body);
                _allBodies.Remove(body);
            }
        }

        public override void Observe(Numerus deltaTime)
        {
            Move(deltaTime);
            Intersect();
            SolveCollisions();
        }

        private void Move(Numerus deltaTime)
        {
            foreach (var pair in _allBodies)
            {
                if (pair.Key.IsStatic) //eanote edges always static
                {
                    continue;
                }

                pair.Key.AddForce(deltaTime, base.DefaultFriction);
                pair.Key.Move(deltaTime);

                if (CheckOutOfBounds(pair.Key))
                {
                    _outOfBoundsBodies.Add(pair.Key);
                }
                else
                {
                    GridCoords newCoords = ConvertToCoords(pair.Key.Position);

                    if (newCoords != pair.Value)
                    {
                        _grid[pair.Value.X, pair.Value.Y].Remove(pair.Key);
                        _grid[newCoords.X, newCoords.Y].Add(pair.Key);
                        _allBodies[pair.Key] = newCoords;
                    }
                }
            }

            foreach (var body in _outOfBoundsBodies)
            {
                Annihilate(body);
            }

            _outOfBoundsBodies.Clear();
        }

        private void Intersect()
        {
            bool isBodiesIntersected;
            foreach (var pair in _allBodies)
            {
                if (pair.Key.IsStatic)
                {
                    continue;
                }

                if (_intersectedBodies.ContainsKey(pair.Key))
                {
                    continue;
                }

                isBodiesIntersected = false;

                for (int x = pair.Value.X - 1; x <= pair.Value.X + 1; x++)
                {
                    for (int y = pair.Value.Y - 1; y <= pair.Value.Y + 1; y++)
                    {
                        foreach (var body in _grid[x, y])
                        {
                            if (body == pair.Key)
                            {
                                continue;
                            }

                            if (body.Kind == World2DEdgeKind
                                && x != pair.Value.X
                                && y != pair.Value.Y)
                            {
                                continue;
                            }

                            if (_intersectedBodies.ContainsKey(body))
                            {
                                continue;
                            }    

                            if (CheckIntersection(pair.Key, body))
                            {
                                isBodiesIntersected = true;
                                _intersectedBodies.Add(body, pair.Key);

                                break;
                            }
                        }

                        if (isBodiesIntersected)
                        {
                            break;
                        }
                    }

                    if (isBodiesIntersected)
                    {
                        break;
                    }
                }
            }
        }

        private bool CheckIntersection(Body one, Body other)
        {
            switch (one.Kind)
            {
                case World2D.World2DEdgeKind:
                    return false;
                case World2D.RoundBodyKind:
                    return CheckIntersection((RoundBody)one, other);
                default:
                    return false;
            }
        }

        private bool CheckIntersection(RoundBody one, Body other)
        {
            switch (other.Kind)
            {
                case World2D.World2DEdgeKind:
                    return CheckIntersection(one, (World2DEdge)other);
                case World2D.RoundBodyKind:
                    return CheckIntersection(one, (RoundBody)other);
                default:
                    return false;
            }
        }

        private bool CheckIntersection(RoundBody one, World2DEdge other)
        {
            if (other.Normal.X > Numerus.Zero)
            {
                return (one.Position.X - one.Radius) <= other.Edge;
            }

            if (other.Normal.X < Numerus.Zero)
            {
                return other.Edge <= one.Position.X + one.Radius;
            }

            if (other.Normal.Y > Numerus.Zero)
            {
                return (one.Position.Y - one.Radius) <= other.Edge;
            }

            if (other.Normal.Y < Numerus.Zero)
            {
                return other.Edge <= one.Position.Y + one.Radius;
            }

            return false;
        }

        private bool CheckIntersection(RoundBody one, RoundBody other)
        {
            Numerus distanceSquared = (one.Position - other.Position).LengthSquared();
            Numerus minimalDistance = one.Radius + other.Radius;

            return (distanceSquared - (minimalDistance * minimalDistance)) < Numerus.Zero;
        }

        private void SolveCollisions()
        {
            Numerus two = Numerus.One.Redouble();

            foreach (var pair in _intersectedBodies)
            {
                if (pair.Key.Kind == World2DEdgeKind)
                {
                    World2DEdge worldEdge = (World2DEdge)pair.Key;
                    //eanote r=n*2*|a.n|+a
                    Numerus doubleProjection = two * Vector2N.ProjectToNormal(pair.Value.Movement, worldEdge.Normal).Abs();
                    Vector2N reflectedMovement = worldEdge.Normal * doubleProjection + pair.Value.Movement;
                    pair.Value.Redirect(reflectedMovement);
                }
            }

            _intersectedBodies.Clear();
        }
    }
}
