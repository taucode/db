using NUnit.Framework;
using TauCode.Db.Utils.Parsing;
using TauCode.Db.Utils.Parsing.Ansi;
using TauCode.Db.Utils.Parsing.Core.Tokens;

namespace TauCode.Db.Test.Utils.Parsing.Ansi
{
    [TestFixture]
    public class AnsiTokenizerTest
    {
        [Test]
        public void Tokenize_ValidArguments_ReturnsValidTokens()
        {
            // Arrange
            ITokenizer tokenizer = new AnsiTokenizer();

            // Act
            var input = " CREATE TABLE\tuser(\rID int PRIMARY KEY,\nname nvarchar(200))\r\n;\r\t\n";
            var tokens = tokenizer.Tokenize(input);

            // Assert
            Assert.That(tokens, Has.Length.EqualTo(16));

            var token = tokens[0];
            Assert.That(token, Is.EqualTo(new WordToken("CREATE")));

            token = tokens[1];
            Assert.That(token, Is.EqualTo(new WordToken("TABLE")));

            token = tokens[2];
            Assert.That(token, Is.EqualTo(new WordToken("user")));

            token = tokens[3];
            Assert.That(token, Is.EqualTo(SymbolToken.OpeningRoundBracketSymbol));

            token = tokens[4];
            Assert.That(token, Is.EqualTo(new WordToken("ID")));

            token = tokens[5];
            Assert.That(token, Is.EqualTo(new WordToken("int")));

            token = tokens[6];
            Assert.That(token, Is.EqualTo(new WordToken("PRIMARY")));

            token = tokens[7];
            Assert.That(token, Is.EqualTo(new WordToken("KEY")));

            token = tokens[8];
            Assert.That(token, Is.EqualTo(SymbolToken.CommaSymbol));

            token = tokens[9];
            Assert.That(token, Is.EqualTo(new WordToken("name")));

            token = tokens[10];
            Assert.That(token, Is.EqualTo(new WordToken("nvarchar")));

            token = tokens[11];
            Assert.That(token, Is.EqualTo(SymbolToken.OpeningRoundBracketSymbol));

            token = tokens[12];
            Assert.That(token, Is.EqualTo(new NumberToken("200")));

            token = tokens[13];
            Assert.That(token, Is.EqualTo(SymbolToken.ClosingRoundBracketSymbol));

            token = tokens[14];
            Assert.That(token, Is.EqualTo(SymbolToken.ClosingRoundBracketSymbol));

            token = tokens[15];
            Assert.That(token, Is.EqualTo(SymbolToken.SemicolonSymbol));
        }

        [Test]
        public void Tokenize_InputContainsIdentifiers_ReturnsIdentifierTokens()
        {
            // Arrange
            ITokenizer tokenizer = new AnsiTokenizer();

            // Act
            var input =
@"
CREATE TABLE ""user""(
    ""ID"" ""int"" PRIMARY KEY,
    name ""nvarchar""(200))
";

            var tokens = tokenizer.Tokenize(input);

            // Assert
            Assert.That(tokens, Has.Length.EqualTo(15));

            var token = tokens[0];
            Assert.That(token, Is.EqualTo(new WordToken("CREATE")));

            token = tokens[1];
            Assert.That(token, Is.EqualTo(new WordToken("TABLE")));

            token = tokens[2];
            Assert.That(token, Is.EqualTo(new IdentifierToken("user")));

            token = tokens[3];
            Assert.That(token, Is.EqualTo(SymbolToken.OpeningRoundBracketSymbol));

            token = tokens[4];
            Assert.That(token, Is.EqualTo(new IdentifierToken("ID")));

            token = tokens[5];
            Assert.That(token, Is.EqualTo(new IdentifierToken("int")));

            token = tokens[6];
            Assert.That(token, Is.EqualTo(new WordToken("PRIMARY")));

            token = tokens[7];
            Assert.That(token, Is.EqualTo(new WordToken("KEY")));

            token = tokens[8];
            Assert.That(token, Is.EqualTo(SymbolToken.CommaSymbol));

            token = tokens[9];
            Assert.That(token, Is.EqualTo(new WordToken("name")));

            token = tokens[10];
            Assert.That(token, Is.EqualTo(new IdentifierToken("nvarchar")));

            token = tokens[11];
            Assert.That(token, Is.EqualTo(SymbolToken.OpeningRoundBracketSymbol));

            token = tokens[12];
            Assert.That(token, Is.EqualTo(new NumberToken("200")));

            token = tokens[13];
            Assert.That(token, Is.EqualTo(SymbolToken.ClosingRoundBracketSymbol));

            token = tokens[14];
            Assert.That(token, Is.EqualTo(SymbolToken.ClosingRoundBracketSymbol));
        }
    }
}
