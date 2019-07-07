using TauCode.Db.Utils.Dialects.Ansi;

namespace TauCode.Db.Utils.Parsing.Ansi
{
    public class AnsiTokenizer : TokenizerBase
    {
        public AnsiTokenizer()
            : base(AnsiDialect.Instance)
        {
        }
    }
}
