using TauCode.Parsing.Tokens;

namespace TauCode.Parsing.Lab
{
    // todo: move to taucode.parsing.
    public class BackQuoteTextDecorationLab : ITextDecoration
    {
        public static readonly BackQuoteTextDecorationLab Instance = new BackQuoteTextDecorationLab();

        private BackQuoteTextDecorationLab()
        {   
        }
    }
}
