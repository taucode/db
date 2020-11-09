using TauCode.Extensions;
using TauCode.Lab.Db.SQLite.Parsing.TokenProducers;
using TauCode.Parsing.Lexing;
using TauCode.Parsing.Lexing.StandardProducers;

namespace TauCode.Lab.Db.SQLite.Parsing
{
    public class SQLiteLexer : LexerBase
    {
        protected override ITokenProducer[] CreateProducers()
        {
            // todo: support sql comments: /* some comment */, --some comment
            return new ITokenProducer[]
            {
                new WhiteSpaceProducer(),
                new WordTokenProducer(),
                new SqlPunctuationTokenProducer(),
                new IntegerProducer(IsAcceptableIntegerTerminator), // todo: ...TokenProducer, here & anywhere?
                new SqlStringTokenProducer(),
                new SqlIdentifierTokenProducer(),
            };
        }

        private bool IsAcceptableIntegerTerminator(char c)
        {
            if (LexingHelper.IsInlineWhiteSpaceOrCaretControl(c))
            {
                return true;
            }

            if (c.IsIn('(', ')', ','))
            {
                return true;
            }

            return false;
        }
    }
}