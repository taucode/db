using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Extensions;

namespace TauCode.Db
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DbDialectAttribute : Attribute
    {
        #region Constructor

        public DbDialectAttribute(
            Type targetType,
            string reservedWordsResourceName,
            string identifierDelimitersString)
        {
            this.ReservedWords = targetType.Assembly.GetResourceLines(reservedWordsResourceName, true);
            this.IdentifierDelimiters = BuildIdentifierDelimiters(identifierDelimitersString);
        }

        #endregion

        #region Private

        private static Tuple<char, char>[] BuildIdentifierDelimiters(string identifierDelimitersString)
        {
            var pairs = identifierDelimitersString.Split(',');
            var tuples = new List<Tuple<char, char>>();
            foreach (var pair in pairs)
            {
                if (pair.Length != 2)
                {
                    throw new ArgumentException("Bad format for identifier delimiters", nameof(identifierDelimitersString));
                }

                var tuple = Tuple.Create(pair.First(), pair.Last());
                tuples.Add(tuple);
            }

            return tuples.ToArray();
        }

        #endregion

        #region Public

        public string[] ReservedWords { get; }
        public Tuple<char, char>[] IdentifierDelimiters { get; }

        #endregion
    }
}

