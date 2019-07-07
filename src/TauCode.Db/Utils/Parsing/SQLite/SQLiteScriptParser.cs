using TauCode.Db.Model;
using TauCode.Db.Utils.Dialects.SQLite;
using TauCode.Db.Utils.Parsing.Core;
using TauCode.Db.Utils.Parsing.Core.Fluent.Impl;
using TauCode.Db.Utils.Parsing.Core.Gallery;

namespace TauCode.Db.Utils.Parsing.SQLite
{
    public class SQLiteScriptParser : ScriptParserBase
    {
        #region Constructor

        public SQLiteScriptParser()
            : base(SQLiteDialect.Instance, new SQLiteTokenizer())
        {
        }

        #endregion

        #region Overridden

        protected override ParsingBlock CreateIdentityBlock()
        {
            var syntax = new NodeSyntax();
            syntax
                .UseWord(@"AUTOINCREMENT", (token, context) => context.GetColumn().Identity = new ColumnIdentityMold())
                .Idle("end");

            return new ParsingBlock(syntax.Root, syntax.GetNode("end").Node);
        }

        #endregion
    }
}
