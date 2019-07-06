using System;
using TauCode.Db.Utils.Parsing.Core.Tokens;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Utils.Parsing.Core
{
    public static class ParsingExtensions
    {
        public static string GetSymbolNameCaption(this SymbolName symbolName)
        {
            switch (symbolName)
            {
                case SymbolName.OpeningRoundBracket:
                    return "(";

                case SymbolName.ClosingRoundBracket:
                    return ")";

                case SymbolName.Comma:
                    return ",";

                case SymbolName.Semicolon:
                    return ";";

                default:
                    return $"<unknown> ({symbolName})";
            }
        }

        public static SymbolName ParseSymbol(string symbol)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            switch (symbol)
            {
                case "(":
                    return SymbolName.OpeningRoundBracket;

                case ")":
                    return SymbolName.ClosingRoundBracket;

                case ",":
                    return SymbolName.Comma;

                case ";":
                    return SymbolName.Semicolon;

                default:
                    throw new ArgumentException($"Could not parse string '{symbol}' as symbol.", nameof(symbol));
            }
        }

        public static string GetTokenIdentifier(this Token token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (token is WordToken wordToken)
            {
                return wordToken.Word;
            }
            else if (token is IdentifierToken identifierToken)
            {
                return identifierToken.Identifier;
            }
            else
            {
                throw new ArgumentException($"Could not get identifier from token '{token}' of type '{token.GetType().FullName}'.");
            }
        }

        public static int GetTokenInt32(this Token token)
        {
            if (token is NumberToken numberToken)
            {
                return numberToken.Number.ToInt32();
            }
            else
            {
                throw new ArgumentException("'NumberToken' was expected.", nameof(token));
            }
        }
    }
}

