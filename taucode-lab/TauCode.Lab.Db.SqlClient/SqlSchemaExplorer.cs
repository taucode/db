using Microsoft.Data.SqlClient;
using TauCode.Db.Schema;

namespace TauCode.Lab.Db.SqlClient
{
    public class SqlSchemaExplorer : DbSchemaExplorerBase
    {
        public SqlSchemaExplorer(SqlConnection connection)
            : base(connection)
        {
        }
    }
}
