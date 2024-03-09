using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Shtoockie.Kod;
using Shtoockie.Matematika;

namespace Shtoockie.Fizika
{
    public class World2D
    {
        record class GridCoords(int X, int Y);
        public record class Impact(AscPair<Body2D> BodyPair, Numerus OneSpeed, Numerus OtherSpeed, Vector2N CollisionLine);

        private static readonly Numerus _gravitation = (Numerus)9_806_650L;
        public static Numerus Gravitation => _gravitation;

        private static readonly Numerus _frictionMultiplier = (Numerus)(900_000L);
        public static Numerus FrictionMultiplier => _frictionMultiplier;

        public const int PointCode = 1;
        public const int EdgeCode = 2;
        public const int RoundCode = 4;
        public const int EdgeOrRound = EdgeCode | RoundCode;

        private readonly int _xLength;
        private readonly int _yLength;

        private readonly Dictionary<Body2D, GridCoords> _allBodies;
        private readonly HashSet<Body2D>[,] _grid;

        private readonly Numerus _sectionLength;
        public Numerus SectionLength => _sectionLength;

        private readonly Vector2N _boundsStart;
        private readonly Vector2N _boundsEnd;
        private readonly List<Body2D> _outOfBoundsBodies;
        private readonly ConcurrentBag<Body2D> _unhandledAnnihilatedBodies;
        public ConcurrentBag<Body2D> UnhandledAnnihilatedBodies => _unhandledAnnihilatedBodies;

        private readonly HashSet<AscPair<Body2D>> _calculatedPairs;
        private readonly Dictionary<AscPair<Body2D>, Impact> _impacts;
        private readonly ConcurrentBag<Impact> _unhandledImpacts;
        public ConcurrentBag<Impact> UnhandledImpacts => _unhandledImpacts;

        public World2D(Numerus sectionLength, int gridXLength, int gridYLength)
        {
            _allBodies = new Dictionary<Body2D, GridCoords>();
            _xLength = gridXLength;
            _yLength = gridYLength;
            _grid = new HashSet<Body2D>[_xLength, _yLength];
            _sectionLength = sectionLength;
            _boundsStart = new Vector2N(_sectionLength);
            _boundsEnd = new Vector2N((Numerus)(_xLength - 1), (Numerus)(_yLength - 1)) * _sectionLength;
            _outOfBoundsBodies = new List<Body2D>();
            _unhandledAnnihilatedBodies = new ConcurrentBag<Body2D>();
            _calculatedPairs = new HashSet<AscPair<Body2D>>();
            _impacts = new Dictionary<AscPair<Body2D>, Impact>();
            _unhandledImpacts = new ConcurrentBag<Impact>();

            FillGrid();
        }

        private void FillGrid()
        {
            for (int x = 0; x < _xLength; x++)
            {
                for (int y = 0; y < _yLength; y++)
                {
                    _grid[x, y] = new HashSet<Body2D>();

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

        private bool CheckOutOfBounds(Body2D body)
        {
            return (body.Position.X < _boundsStart.X) || (body.Position.Y < _boundsStart.Y) || (body.Position.X > _boundsEnd.X) || (body.Position.Y > _boundsEnd.Y);
        }

        private GridCoords ConvertToCoords(Vector2N position)
        {
            return new GridCoords((int)(position.X / _sectionLength), (int)(position.Y / _sectionLength));
        }

        public void Materialize(Body2D body)
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

        public void Annihilate(Body2D body)
        {
            if (_allBodies.ContainsKey(body))
            {
                GridCoords coords = _allBodies[body];
                _grid[coords.X, coords.Y].Remove(body);
                _allBodies.Remove(body);
                _unhandledAnnihilatedBodies.Add(body);
            }
        }

        public void Observe(Numerus deltaTime)
        {
            AddElasticForce(deltaTime);
            AddFrictionForce(deltaTime);
            Move(deltaTime);
        }

        private void AddElasticForce(Numerus deltaTime)
        {
            _calculatedPairs.Clear();

            foreach (var pair in _allBodies)
            {
                Body2D one = pair.Key;

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

                            AscPair<Body2D> bodyPair = new AscPair<Body2D>(one, other);

                            if (!_calculatedPairs.Contains(bodyPair))
                            {
                                ClalculateCollision(bodyPair, deltaTime);
                            }
                        }
                    }
                }
            }
        }

        private void ClalculateCollision(AscPair<Body2D> bodyPair, Numerus deltaTime)
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

            if (isCollided)
            {
                _calculatedPairs.Add(bodyPair);
            }

            if (hasImpact && !isCollided) //eanote столкновение закончилось - можно обработать
            {
                _unhandledImpacts.Add(_impacts[bodyPair]);
                _impacts.Remove(bodyPair);
            }
        }

        private bool CollideRounds(AscPair<Body2D> bodyPair, Numerus deltaTime)
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
            Numerus squeezeOne;
            Numerus squeezeOther;

            if (one.Elasticity == other.Elasticity)
            {
                squeezeOne = squeeze.Halve();
                squeezeOther = squeezeOne;
            }
            else
            {
                squeezeOne = (other.Elasticity / (one.Elasticity + other.Elasticity)) * squeeze;
                squeezeOther = (one.Elasticity / (one.Elasticity + other.Elasticity)) * squeeze;
            }

            if (!_impacts.ContainsKey(bodyPair))
            {
                Numerus oneSpeed = Vector2N.ProjectToNormal(one.Movement, -normal);
                Numerus otherSpeed = Vector2N.ProjectToNormal(other.Movement, normal);
                _impacts.Add(bodyPair, new Impact(bodyPair, oneSpeed, otherSpeed, collisionLine));
            }

            //eanote Fупр=-kx
            //следует учесть что сила упругости возникает от центра
            Vector2N elasticForce = one.Elasticity * squeezeOne * normal;
            one.AddForce(elasticForce, deltaTime);
            elasticForce = other.Elasticity * squeezeOther * (-normal);
            other.AddForce(elasticForce, deltaTime);

            return true;
        }

        private bool CollideEdgeWithRound(AscPair<Body2D> bodyPair, Numerus deltaTime)
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
                _impacts.Add(bodyPair, new Impact(bodyPair, oneSpeed, otherSpeed, one.Normal));
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
                Body2D body = pair.Key;

                if (body.IsStatic)
                {
                    continue;
                }

                body.AddFrictionForce(FrictionMultiplier, deltaTime);
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
