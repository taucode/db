using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db.Model;
using TauCode.Extensions;

namespace TauCode.Db
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DialectAttribute : Attribute
    {
        #region Fields

        private readonly Dictionary<string, DbTypeFamily> _families;
        private readonly Dictionary<string, DbTypeNameCategory> _categories;

        #endregion

        #region Constructor

        public DialectAttribute(
            Type targetType,
            string reservedWordsResourceName,
            string dataTypeNamesResourceName,
            string identifierDelimitersString)
        {
            this.ReservedWords = targetType.Assembly.GetResourceLines(reservedWordsResourceName, true);

            _families = new Dictionary<string, DbTypeFamily>();
            _categories = new Dictionary<string, DbTypeNameCategory>();

            var typeNameLines = targetType.Assembly.GetResourceLines(dataTypeNamesResourceName, true)
                .Select(x => x.Trim())
                .Where(x => x != string.Empty);

            foreach (var typeNameLine in typeNameLines)
            {
                var parts = typeNameLine.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var typeName = parts[0];
                var category = parts[1].ToEnum<DbTypeNameCategory>();
                var family = parts[2].ToEnum<DbTypeFamily>();

                _families.Add(typeName, family);
                _categories.Add(typeName, category);
            }

            this.IdentifierDelimiters = BuildIdentifierDelimiters(identifierDelimitersString);
            this.DataTypeNames = _families.Keys
                .OrderBy(x => x)
                .ToArray();
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
        public string[] DataTypeNames { get; }
        public Tuple<char, char>[] IdentifierDelimiters { get; }
        public Dictionary<string, DbTypeFamily> Families => _families;
        public Dictionary<string, DbTypeNameCategory> Categories => _categories;


        #endregion
    }
}

