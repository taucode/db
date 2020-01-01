using TauCode.Extensions;
using TauCode.Parsing;
using TauCode.Parsing.Lab;
using TauCode.Parsing.Lexing;
using TauCode.Parsing.Lexing.StandardTokenExtractors;
using TauCode.Parsing.Tokens;
using TauCode.Parsing.Tokens.TextClasses;
using TauCode.Parsing.Tokens.TextDecorations;

namespace TauCode.Db.SQLite.Parsing.TokenExtractors
{
    public class SQLiteIdentifierExtractor : TokenExtractorBase
    {
        private ITextDecoration _textDecoration;
        private char? _expectedClosingDelimiter;

        public SQLiteIdentifierExtractor()
            : base(StandardLexingEnvironment.Instance, x => x.IsIn('[', '`', '"'))
        {
        }

        protected override void ResetState()
        {
            _textDecoration = null;
            _expectedClosingDelimiter = null;
        }

        protected override IToken ProduceResult()
        {
            var str = this.ExtractResultString();
            if (str.Length <= 2)
            {
                return null;
            }

            var identifier = str.Substring(1, str.Length - 2);
            return new TextToken(IdentifierTextClass.Instance, _textDecoration, identifier);
        }

        protected override CharChallengeResult ChallengeCurrentChar()
        {
            var c = this.GetCurrentChar();
            var pos = this.GetLocalPosition();

            if (pos == 0)
            {
                switch (c)
                {
                    case '[':
                        _expectedClosingDelimiter = ']';
                        _textDecoration = BracketsTextDecoration.Instance;
                        break;

                    case '"':
                        _expectedClosingDelimiter = '"';
                        _textDecoration = DoubleQuoteTextDecoration.Instance;
                        break;

                    case '`':
                        _expectedClosingDelimiter = '`';
                        _textDecoration = BackQuoteTextDecorationLab.Instance;
                        break;
                }

                return CharChallengeResult.Continue; // how else?
            }

            if (WordExtractor.StandardInnerCharPredicate(c))
            {
                return CharChallengeResult.Continue;
            }

            if (c.IsIn(']', '`', '"'))
            {
                if (c == _expectedClosingDelimiter.Value)
                {
                    this.Advance();
                    return CharChallengeResult.Finish;
                }
            }

            return CharChallengeResult.Error; // unexpected char within identifier.
        }
        
        protected override CharChallengeResult ChallengeEnd() => CharChallengeResult.Error; // met end while extracting identifier.
    }
}
