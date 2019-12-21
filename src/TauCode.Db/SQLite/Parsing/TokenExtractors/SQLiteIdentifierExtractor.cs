using TauCode.Extensions;
using TauCode.Parsing;
using TauCode.Parsing.Lexing;
using TauCode.Parsing.Lexing.StandardTokenExtractors;
using TauCode.Parsing.Tokens;

namespace TauCode.Db.SQLite.Parsing.TokenExtractors
{
    public class SQLiteIdentifierExtractor : TokenExtractorBase
    {
        public SQLiteIdentifierExtractor()
            : base(StandardLexingEnvironment.Instance, x => x.IsIn('[', '`', '"'))
        {
        }

        protected override IToken ProduceResult()
        {
            var str = this.ExtractResultString();
            if (str.Length <= 2)
            {
                return null;
            }

            var identifier = str.Substring(1, str.Length - 2);
            return new IdentifierToken(identifier);
        }

        protected override CharChallengeResult ChallengeCurrentChar()
        {
            var c = this.GetCurrentChar();
            var pos = this.GetLocalPosition();

            if (pos == 0)
            {
                return CharChallengeResult.Continue; // how else?
            }

            if (WordExtractor.StandardInnerCharPredicate(c))
            {
                return CharChallengeResult.Continue;
            }

            if (c.IsIn(']', '`', '"'))
            {
                var openingDelimiter = this.GetLocalChar(0);
                if (GetClosingDelimiter(openingDelimiter) == c)
                {
                    this.Advance();
                    return CharChallengeResult.Finish;
                }
            }

            return CharChallengeResult.Error; // unexpected char within identifier.
        }

        private char GetClosingDelimiter(char openingDelimiter)
        {
            switch (openingDelimiter)
            {
                case '[':
                    return ']';

                case '`':
                    return '`';

                case '"':
                    return '"';

                default:
                    return '\0';
            }
        }

        protected override CharChallengeResult ChallengeEnd() => CharChallengeResult.Error; // met end while extracting identifier.
    }
}
