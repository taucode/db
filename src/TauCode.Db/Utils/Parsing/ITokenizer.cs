using TauCode.Db.Utils.Dialects;
using TauCode.Db.Utils.Parsing.Core;

namespace TauCode.Db.Utils.Parsing
{
    public interface ITokenizer
    {
        IDialect Dialect { get; }

        int? TabSize { get; set; }
        
        Token[] Tokenize(string input);
    }
}
