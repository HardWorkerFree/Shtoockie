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
        public record class Impact(AscPair<Body> BodyPair, Numerus OneSpeed, Numerus OtherSpeed);

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
        private readonly List<Body> _outOfBoundsBodies;
        private readonly ConcurrentBag<Body> _unhandledAnnihilatedBodies;
        public ConcurrentBag<Body> UnhandledAnnihilatedBodies => _unhandledAnnihilatedBodies;

        private readonly Dictionary<AscPair<Body>, Impact> _impacts;
        private readonly ConcurrentBag<Impact> _unhandledImpacts;
        public ConcurrentBag<Impact> UnhandledImpacts => _unhandledImpacts;

        public World2D(Numerus sectionLength, int gridXLength, int gridYLength)
        {
            _allBodies = new Dictionary<Body, GridCoords>();
            _xLength = gridXLength;
            _yLength = gridYLength;
            _grid = new HashSet<Body>[_xLength, _yLength];
            _sectionLength = sectionLength;
            _boundsStart = new Vector2N(_sectionLength);
            _boundsEnd = new Vector2N((Numerus)(_xLength - 1), (Numerus)(_yLength - 1)) * _sectionLength;
            _outOfBoundsBodies = new List<Body>();
            _unhandledAnnihilatedBodies = new ConcurrentBag<Body>();
            _impacts = new Dictionary<AscPair<Body>, Impact>();
            _unhandledImpacts = new ConcurrentBag<Impact>();

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
                _unhandledAnnihilatedBodies.Add(body);
            }
        }

        public override void Observe(Numerus deltaTime)
        {
            AddElasticForce(deltaTime);
            AddFrictionForce(deltaTime);
            Move(deltaTime);
        }

        private void AddElasticForce(Numerus deltaTime)
        {
            foreach (var pair in _allBodies)
            {
                Body one = pair.Key;

                //eanote игнорируем стены
                if (pair.Key.Code == EdgeCode)
                {
                    continue;
                }

                GridCoords oneGridCoords = pair.Value;

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

                            AscPair<Body> bodyPair = new AscPair<Body>(one, other);
                            ClalculateCollision(bodyPair, deltaTime);
                        }
                    }
                }
            }
        }

        private void ClalculateCollision(AscPair<Body> bodyPair, Numerus deltaTime)
        {
            bool hasImpact = _impacts.ContainsKey(bodyPair);
            bool isCollided = false;

            switch (bodyPair.OrCode)
            {
                case World2D.EdgeCode:
                    throw new InvalidOperationException();
                case World2D.RoundCode:
                    isCollided = CollideRounds(bodyPair, deltaTime);
                    break;
                case World2D.EdgeOrRound:
                    isCollided = CollideEdgeWithRound(bodyPair, deltaTime);
                    break;
                default:
                    break;
            }

            if (hasImpact && !isCollided) //eanote столкновение закончилось - можно обработать
            {
                _unhandledImpacts.Add(_impacts[bodyPair]);
                _impacts.Remove(bodyPair);
            }
        }

        private bool CollideRounds(AscPair<Body> bodyPair, Numerus deltaTime)
        {
            RoundBody one = (RoundBody)bodyPair.One;
            RoundBody other = (RoundBody)bodyPair.Other;

            Vector2N collisionLine = one.Position - other.Position;

            Numerus distanceSquared = collisionLine.LengthSquared();
            Numerus minimalDistance = one.Radius + other.Radius;

            if ((distanceSquared - (minimalDistance * minimalDistance)) >= Numerus.Zero)
            {
                return false;
            }

            if (distanceSquared == Numerus.Zero) //eanote исключительное условие
            {
                return false;
            }

            Numerus distance = distanceSquared.Sqrt();
            Vector2N normal = collisionLine / distance;

            //eanote dl1=k2*dl/2(k1+k2)

            Numerus squeeze = minimalDistance - distance;

            if (one.Elasticity == other.Elasticity)
            {
                squeeze = squeeze.Halve();
            }
            else
            {
                squeeze = (other.Elasticity / (one.Elasticity + other.Elasticity)) * squeeze;
            }

            if (!_impacts.ContainsKey(bodyPair))
            {
                Numerus oneSpeed = Vector2N.ProjectToNormal(one.Movement, -normal);
                Numerus otherSpeed = Vector2N.ProjectToNormal(other.Movement, normal);
                _impacts.Add(bodyPair, new Impact(bodyPair, oneSpeed, otherSpeed));
            }

            //eanote Fупр=-kx
            //следует учесть что сила упругости возникает от центра
            Vector2N elasticForce = one.Elasticity * squeeze * normal;
            one.AddForce(elasticForce, deltaTime);

            return true;
        }

        private bool CollideEdgeWithRound(AscPair<Body> bodyPair, Numerus deltaTime)
        {
            World2DEdge one = (World2DEdge)bodyPair.One;
            RoundBody other = (RoundBody)bodyPair.Other;

            Numerus squeeze = Numerus.Zero;

            if (one.Normal.X > Numerus.Zero)
            {
                squeeze = one.Edge + other.Radius - other.Position.X;

                if (squeeze <= Numerus.Zero) //eanote позиция дальше чем начинается сжатие
                {
                    return false;
                }
            }
            else
            {
                if (one.Normal.X < Numerus.Zero)
                {
                    squeeze = other.Position.X + other.Radius - one.Edge;

                    if (squeeze <= Numerus.Zero) //eanote позиция ближе чем начинается сжатие
                    {
                        return false;
                    }
                }
                else
                {
                    if (one.Normal.Y > Numerus.Zero)
                    {
                        squeeze = one.Edge + other.Radius - other.Position.Y;

                        if (squeeze <= Numerus.Zero) //eanote позиция дальше чем начинается сжатие
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (one.Normal.Y < Numerus.Zero)
                        {
                            squeeze = other.Position.Y + other.Radius - one.Edge;

                            if (squeeze <= Numerus.Zero) //eanote позиция ближе чем начинается сжатие
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            if (!_impacts.ContainsKey(bodyPair))
            {
                Numerus oneSpeed = Numerus.Zero;
                Numerus otherSpeed = Vector2N.ProjectToNormal(other.Movement, one.Normal);
                _impacts.Add(bodyPair, new Impact(bodyPair, oneSpeed, otherSpeed));
            }

            //eanote Fупр=-kx
            //следует учесть что сила упругости возникает от стены
            Vector2N elasticForce = other.Elasticity * squeeze * one.Normal;
            other.AddForce(elasticForce, deltaTime);

            return true;
        }

        private void AddFrictionForce(Numerus deltaTime)
        {
            foreach (var pair in _allBodies)
            {
                Body body = pair.Key;

                if (body.IsStatic)
                {
                    continue;
                }

                //eanote Fтр=-uN;
                Vector2N frictionForce = -body.Direction * body.NormalReaction;
                body.AddForce(frictionForce, deltaTime);
            }
        }

        private void Move(Numerus deltaTime)
        {
            foreach (var pair in _allBodies)
            {
                if (pair.Key.IsStatic) //eanote edges always static
                {
                    continue;
                }

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
    }
}
