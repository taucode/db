using System;
using System.Collections.Generic;

namespace TauCode.Db
{
    public interface IDbDialect : IDbUtility
    {
        string Name { get; }

        IReadOnlyList<string> ReservedWords { get; }

        string ClauseTerminator { get; }

        string UnicodeTextLiteralPrefix { get; }

        bool IsClauseTerminatorMandatory { get; }

        IReadOnlyList<Tuple<char, char>> IdentifierDelimiters { get; }

        IReadOnlyList<char> AcceptableIdentifierFirstChars { get; }

        IReadOnlyList<char> AcceptableIdentifierInnerChars { get; }

        bool CanDecorateTypeIdentifier { get; }

        string DecorateIdentifier(DbIdentifierType identifierType, string identifier, char? openingDelimiter);
    }
}
