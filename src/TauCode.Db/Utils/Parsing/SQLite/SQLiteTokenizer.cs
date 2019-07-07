using TauCode.Db.Utils.Dialects.SQLite;

namespace TauCode.Db.Utils.Parsing.SQLite
{
    public class SQLiteTokenizer : TokenizerBase
    {
        public SQLiteTokenizer()
            : base(SQLiteDialect.Instance)
        {
        }
    }
}
