using System.Collections.Generic;
using System.Linq;

namespace TauCode.Db.Model
{
    public static class ModelExtensions
    {
        public static IDictionary<string, string> ClonePropertiesIfNeeded(this IMold mold, bool needed)
        {
            return needed ? mold
                .Properties
                .Where(x => !x.Key.StartsWith("#"))
                .ToDictionary(
                    x => x.Key,
                    x => x.Value)
                : new Dictionary<string, string>();
        }

        public static ColumnMold CloneColumn(this ColumnMold column, bool includeProperties)
        {
            return (ColumnMold)column.Clone(includeProperties);
        }

        public static PrimaryKeyMold ClonePrimaryKey(this PrimaryKeyMold primaryKey, bool includeProperties)
        {
            return (PrimaryKeyMold)primaryKey?.Clone(includeProperties);
        }

        public static IndexColumnMold CloneIndexColumn(this IndexColumnMold indexColumn, bool includeProperties)
        {
            return (IndexColumnMold)indexColumn.Clone(includeProperties);
        }

        public static IndexMold CloneIndex(this IndexMold index, bool includeProperties)
        {
            return (IndexMold)index.Clone(includeProperties);
        }

        public static ForeignKeyMold CloneForeignKey(this ForeignKeyMold foreignKey, bool includeProperties)
        {
            return (ForeignKeyMold)foreignKey.Clone(includeProperties);
        }

        public static ColumnIdentityMold CloneColumnIdentity(
            this ColumnIdentityMold columnIdentity,
            bool includeProperties)
        {
            return (ColumnIdentityMold)columnIdentity?.Clone(includeProperties);
        }

        public static DbTypeMold CloneType(this DbTypeMold type, bool includeProperties)
        {
            return (DbTypeMold)type.Clone(includeProperties);
        }

        public static TableMold CloneTable(this TableMold table, bool includeProperties)
        {
            return (TableMold)table.Clone(includeProperties);
        }
    }
}
