using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Shtoockie.Kod;
using Shtoockie.Matematika;

namespace Shtoockie.Fizika
{
    public class World2D : BaseWorld
    {
        record class GridCoords(int X, int Y);

        public const int PointCode = 1;
        public const int EdgeCode = 2;
        public const int RoundCode = 4;
        public const int EdgeOrRound = EdgeCode | RoundCode;

        private readonly int _xLength;
        private readonly int _yLength;

        private readonly Dictionary<Body, GridCoords> _allBodies;
        private readonly HashSet<Body>[,] _grid;

        private readonly Numerus _sectionLength;
        public Numerus SectionLength => _sectionLength;

        private readonly Vector2N _boundsStart;
        private readonly Vector2N _boundsEnd;
        private readonly HashSet<Body> _outOfBoundsBodies;
        private readonly Dictionary<Body, AscPair<Body>> _intersectedBodies;

        private readonly ConcurrentQueue<Vector2N> _unhandledImpacts;
        private readonly Dictionary<Body, AscPair<Body>> _impactedBodies;

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
            _intersectedBodies = new Dictionary<Body, AscPair<Body>>();

            _unhandledImpacts = new ConcurrentQueue<Vector2N>();
            _impactedBodies = new Dictionary<Body, AscPair<Body>>();

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
                if (body.Code != EdgeCode)
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
            foreach (var pair in _allBodies)
            {
                Body one = pair.Key;
                AscPair<Body> impactedPair;
                bool isOneImpacted = _impactedBodies.TryGetValue(one, out impactedPair);

                //eanote игнорируем тела без движения
                if (pair.Key.IsStatic)
                {
                    //eanote оба тела бездвижны - значит удар закончился
                    if (isOneImpacted && impactedPair.One.IsStatic && impactedPair.Other.IsStatic)
                    {
                        _impactedBodies.Remove(impactedPair.One);
                        _impactedBodies.Remove(impactedPair.Other);
                    }

                    continue;
                }

                GridCoords oneGridCoords = pair.Value;
                bool isBodiesIntersected = false;

                for (int x = oneGridCoords.X - 1; x <= oneGridCoords.X + 1; x++)
                {
                    for (int y = oneGridCoords.Y - 1; y <= oneGridCoords.Y + 1; y++)
                    {
                        foreach (var other in _grid[x, y])
                        {
                            if (one == other)
                            {
                                continue;
                            }

                            //eanote диагональные не смотрим, т.к. ближе будут те, что по краям.
                            if (other.Code == EdgeCode
                                && x != oneGridCoords.X
                                && y != oneGridCoords.Y)
                            {
                                continue;
                            }

                            AscPair<Body> ascPair = new AscPair<Body>(one, other);
                            bool isOneImpactedOther = isOneImpacted && (impactedPair.One == ascPair.One) && (impactedPair.Other == ascPair.Other);

                            if (CheckIntersection(ascPair))
                            {
                                if (isOneImpactedOther)
                                {
                                    continue;
                                }

                                //eanote рассчитываем не более одного пересечения за такт
                                if (_intersectedBodies.ContainsKey(other))
                                {
                                    continue;
                                }

                                //eanote взаимное пересечение тел
                                isBodiesIntersected = true;
                                _intersectedBodies.Add(one, ascPair);
                                _intersectedBodies.Add(other, ascPair);

                                break;
                            }
                            else if (isOneImpactedOther) //eanote если пересечение закончилось, то считаем удар завершённым
                            {
                                _impactedBodies.Remove(one);
                                _impactedBodies.Remove(other);
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

        private bool CheckIntersection(AscPair<Body> bodyPair)
        {
            switch (bodyPair.OrCode)
            {
                case World2D.EdgeCode:
                    throw new InvalidOperationException();
                case World2D.RoundCode:
                    return CheckIntersection((RoundBody)bodyPair.One, (RoundBody)bodyPair.Other);
                case World2D.EdgeOrRound:
                    return CheckIntersection((World2DEdge)bodyPair.One, (RoundBody)bodyPair.Other);
                default:
                    return false;
            }
        }

        private bool CheckIntersection(World2DEdge one, RoundBody other)
        {
            if (one.Normal.X > Numerus.Zero)
            {
                return (other.Position.X - other.Radius) <= one.Edge;
            }

            if (one.Normal.X < Numerus.Zero)
            {
                return one.Edge <= other.Position.X + other.Radius;
            }

            if (one.Normal.Y > Numerus.Zero)
            {
                return (other.Position.Y - other.Radius) <= one.Edge;
            }

            if (one.Normal.Y < Numerus.Zero)
            {
                return one.Edge <= other.Position.Y + other.Radius;
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
            foreach (var pair in _intersectedBodies)
            {
                if (_impactedBodies.ContainsKey(pair.Key))
                {
                    continue;
                }

                Vector2N reflectedMovement = Vector2N.Zero;
                AscPair<Body> intersectedPair = pair.Value;

                switch (intersectedPair.OrCode)
                {
                    case World2D.EdgeCode:
                        throw new InvalidOperationException();
                    case World2D.RoundCode:
                        Impact((RoundBody)intersectedPair.One, (RoundBody)intersectedPair.Other);
                        break;
                    case World2D.EdgeOrRound:
                        Impact((World2DEdge)intersectedPair.One, (RoundBody)intersectedPair.Other);
                        break;
                    default:
                        continue;
                }

                //eanote для защиты от двойного рассчета пересечений
                _impactedBodies[pair.Value.One] = pair.Value;
                _impactedBodies[pair.Value.Other] = pair.Value;
            }

            _intersectedBodies.Clear();
        }

        private void Impact(World2DEdge one, RoundBody other)
        {
            //eanote r=n*2*|a.n|+a
            Numerus reflectedXMovement = other.Movement.X;
            Numerus reflectedYMovement = other.Movement.Y;

            if (one.Normal.X > Numerus.Zero)
            {
                reflectedXMovement = reflectedXMovement.Abs();
            }

            if (one.Normal.X < Numerus.Zero)
            {
                reflectedXMovement = -reflectedXMovement.Abs();
            }

            if (one.Normal.Y > Numerus.Zero)
            {
                reflectedYMovement = reflectedYMovement.Abs();
            }

            if (one.Normal.Y < Numerus.Zero)
            {
                reflectedYMovement = -reflectedYMovement.Abs();
            }

            Vector2N reflectedMovement = new Vector2N(reflectedXMovement, reflectedYMovement);
            other.Redirect(reflectedMovement);
            _unhandledImpacts.Enqueue(reflectedMovement);
        }

        private void Impact(RoundBody one, RoundBody other)
        {
            _unhandledImpacts.Enqueue(one.Movement + other.Movement);

            one.Redirect(Vector2N.Zero);
            other.Redirect(Vector2N.Zero);
        }
    }
}
