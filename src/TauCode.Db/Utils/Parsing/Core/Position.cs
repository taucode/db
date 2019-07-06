using System;

namespace TauCode.Db.Utils.Parsing.Core
{
    public struct Position : IEquatable<Position>, IComparable<Position>
    {
        #region Fields

        public readonly int Line;
        public readonly int Column;

        #endregion

        #region Constructor

        public Position(int line, int column)
        {
            this.Line = line;
            this.Column = column;
        }

        #endregion

        #region IEquatable<Position> Members

        public bool Equals(Position other)
        {
            return
                this.Line == other.Line &&
                this.Column == other.Column;
        }

        #endregion

        #region IComparable<Position> Members

        public int CompareTo(Position other)
        {
            if (this.Line > other.Line)
            {
                return 1;
            }

            if (this.Line < other.Line)
            {
                return -1;
            }

            if (this.Column > other.Column)
            {
                return 1;
            }

            if (this.Column < other.Column)
            {
                return -1;
            }

            return 0;
        }

        #endregion

        #region Overridden

        public override string ToString()
        {
            return $"{this.Line};{this.Column}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Position otherPosition)
            {
                return this.Equals(otherPosition);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Line * 397) ^ Column;
            }
        }

        #endregion

        #region Operators

        public static bool operator ==(Position position1, Position position2)
        {
            return position1.Equals(position2);
        }

        public static bool operator !=(Position position1, Position position2)
        {
            return !(position1 == position2);
        }

        public static bool operator <(Position position1, Position position2)
        {
            return position1.CompareTo(position2) == -1;
        }

        public static bool operator >(Position position1, Position position2)
        {
            return position1.CompareTo(position2) == +1;
        }

        public static bool operator <=(Position position1, Position position2)
        {
            return position1.CompareTo(position2) <= 0;
        }

        public static bool operator >=(Position position1, Position position2)
        {
            return position1.CompareTo(position2) >= 0;
        }

        #endregion
    }
}
