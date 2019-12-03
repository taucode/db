using TauCode.Db.Utils.Inspection.SQLite.TokenExtractors;
using TauCode.Parsing.Lexing;
using TauCode.Parsing.Lexing.StandardTokenExtractors;

namespace TauCode.Db.Utils.Inspection.SQLite
{
    public class SQLiteLexer : LexerBase
    {
        public SQLiteLexer()
        {
        }

        protected override void InitTokenExtractors()
        {
            // word
            var wordExtractor = new WordExtractor(this.Environment);
            this.AddTokenExtractor(wordExtractor);

            // punctuation
            var punctuationExtractor = new SQLitePunctuationExtractor();
            this.AddTokenExtractor(punctuationExtractor);

            // integer
            var integerExtractor = new IntegerExtractor(this.Environment);
            this.AddTokenExtractor(integerExtractor);

            // identifier
            var identifierExtractor = new SQLiteIdentifierExtractor();
            this.AddTokenExtractor(identifierExtractor);

            // string
            var stringExtractor = new SQLiteStringExtractor();
            this.AddTokenExtractor(stringExtractor);

            // *** Links ***
            wordExtractor.AddSuccessors(
                punctuationExtractor);

            punctuationExtractor.AddSuccessors(
                punctuationExtractor,
                wordExtractor,
                integerExtractor,
                identifierExtractor,
                stringExtractor);

            integerExtractor.AddSuccessors(
                punctuationExtractor);

            identifierExtractor.AddSuccessors(
                punctuationExtractor);

            stringExtractor.AddSuccessors(
                punctuationExtractor);
        }
    }
}
