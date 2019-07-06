using TauCode.Db.Utils.Dialects.Ansi;

namespace TauCode.Db.Utils.Parsing.Ansi
{
    public class AnsiScriptParser : ScriptParserBase
    {
        #region Constructor

        public AnsiScriptParser()
            : base(AnsiDialect.Instance, new AnsiTokenizer())
        {
        }

        #endregion

        #region Overridden

        //protected override ParsingBlock CreateTableCreationBlock()
        //{
        //    var block = new TableCreationBuilder(this.Dialect).Build();
        //    return block;
        //}

        //protected override void SetColumnIdentity(Token token, ParsingContext context)
        //{
        //    throw new NotSupportedException();
        //}

        //protected override ParsingBlock CreateIdentityBlock()
        //{
        //    return this.CreateIdleBlock();
        //}

        #endregion

    }
}
