using System.Collections.Generic;
using System.Linq;

namespace TauCode.Db.Model
{
    internal static class ModelExtensions
    {
        internal static IDictionary<string, string> ClonePropertiesIfNeeded(this IDbMold dbMold, bool needed)
        {
            return needed ? dbMold.Properties.ToDictionary(x => x.Key, x => x.Value) : new Dictionary<string, string>();
        }

        internal static ColumnMold CloneColumn(this ColumnMold column, bool includeProperties)
        {
            return (ColumnMold)column.Clone(includeProperties);
        }

        internal static PrimaryKeyMold ClonePrimaryKey(this PrimaryKeyMold primaryKey, bool includeProperties)
        {
            return (PrimaryKeyMold)primaryKey?.Clone(includeProperties);
        }

        internal static IndexColumnMold CloneIndexColumn(this IndexColumnMold indexColumn, bool includeProperties)
        {
            return (IndexColumnMold)indexColumn.Clone(includeProperties);
        }

        internal static IndexMold CloneIndex(this IndexMold index, bool includeProperties)
        {
            return (IndexMold)index.Clone(includeProperties);
        }

        internal static ForeignKeyMold CloneForeignKey(this ForeignKeyMold foreignKey, bool includeProperties)
        {
            return (ForeignKeyMold)foreignKey.Clone(includeProperties);
        }

        internal static ColumnIdentityMold CloneColumnIdentity(
            this ColumnIdentityMold columnIdentity,
            bool includeProperties)
        {
            return (ColumnIdentityMold)columnIdentity?.Clone(includeProperties);
        }

        internal static DbTypeMold CloneType(this DbTypeMold type, bool includeProperties)
        {
            return (DbTypeMold)type.Clone(includeProperties);
        }
    }
}
