using System;

namespace TauCode.Db.Utils.Inspection.SQLite
{
    public class SQLiteParser
    {
        public static SQLiteParser Instance = new SQLiteParser();

        private SQLiteParser()
        {
            
        }

        public object[] Parse(string sql)
        {
            throw new NotImplementedException();
        }
    }
}
