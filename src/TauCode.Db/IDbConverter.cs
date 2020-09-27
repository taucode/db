using System.Collections.Generic;
using TauCode.Db.Model;

namespace TauCode.Db
{
    public interface IDbConverter : IDbUtility
    {
        DbTypeMold ConvertType(DbTypeMold originType, string originProviderName);
        ColumnIdentityMold ConvertIdentity(ColumnIdentityMold originIdentity, string originProviderName);
        string ConvertDefault(string originDefault, string originProviderName);
        PrimaryKeyMold ConvertPrimaryKey(TableMold originTable, string originProviderName);
        IList<ForeignKeyMold> ConvertForeignKeys(TableMold originTable, string originProviderName);
        IList<IndexMold> ConvertIndexes(TableMold originTable, string originProviderName);
        TableMold ConvertTable(TableMold originTable, string originProviderName);
        DbMold ConvertDb(DbMold originDb, IReadOnlyDictionary<string, string> options = null);
    }
}
