using System;
using System.Collections.Generic;
using TauCode.Db.Model;

namespace TauCode.Db.Utils.Dialects
{
    public interface IDialect
    {
        string Name { get; }

        IReadOnlyList<string> ReservedWords { get; }

        IReadOnlyList<string> DataTypeNames { get; }

        string ClauseTerminator { get; }

        string UnicodeTextLiteralPrefix { get; }

        bool IsClauseTerminatorMandatory { get; }

        DbTypeFamily GetTypeFamily(string typeName);

        DbTypeNameCategory GetTypeNameCategory(string typeName);

        IReadOnlyList<Tuple<char, char>> IdentifierDelimiters { get; }

        IReadOnlyList<char> AcceptableIdentifierFirstChars { get; }

        IReadOnlyList<char> AcceptableIdentifierInnerChars { get; }

        bool CanDecorateTypeIdentifier { get; }

        string DecorateIdentifier(DbIdentifierType identifierType, string identifier, char? openingDelimiter);

        string ValueToSqlValueString(DbTypeMold type, object value);

        DbTypeMold ResolveType(string typeName, int? size, int? precision, int? scale);

        string StringToSqlString(string value, bool isUnicode);
    }
}
