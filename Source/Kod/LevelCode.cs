using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shtoockie.Kod
{
    public class LevelCode : IComparable<LevelCode>
    {
        private readonly int[] _levelPositions;

        public LevelCode(int position)
        {
            _levelPositions = new int[1] { position };
        }

        public LevelCode(LevelCode rootCode, int position)
        {
            this._levelPositions = new int[rootCode._levelPositions.Length + 1];
            Array.Copy(rootCode._levelPositions, 0, this._levelPositions, 0, rootCode._levelPositions.Length);
            this._levelPositions[rootCode._levelPositions.Length] = position;
        }

        public override int GetHashCode()
        {
            return this._levelPositions.GetHashCode();
        }

        public override string ToString()
        {
            return string.Join(",", this._levelPositions);
        }

        public int CompareTo(LevelCode other)
        {
            if (other == null)
            {
                return 1;
            }
            
            int comparableLenght = this._levelPositions.Length;
            int lenghtComparison = this._levelPositions.Length.CompareTo(other._levelPositions.Length);

            if (lenghtComparison > 0)
            {
                comparableLenght = other._levelPositions.Length;
            }

            for (int i = 0; i < comparableLenght; i++)
            {
                int comparison = this._levelPositions[i].CompareTo(other._levelPositions[i]);

                if (comparison != 0)
                {
                    return comparison;
                }
            }

            return lenghtComparison;
        }
    }
}
