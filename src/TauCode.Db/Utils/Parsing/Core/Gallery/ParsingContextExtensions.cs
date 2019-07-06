using TauCode.Db.Model;

namespace TauCode.Db.Utils.Parsing.Core.Gallery
{
    public static class ParsingContextExtensions
    {
        public static TableMold GetTable(this ParsingContext context)
        {
            return context.GetProperty<TableMold>("table");
        }

        public static ColumnMold GetColumn(this ParsingContext context)
        {
            return context.GetProperty<ColumnMold>("column");
        }

        public static ForeignKeyMold GetForeignKey(this ParsingContext context)
        {
            return context.GetProperty<ForeignKeyMold>("foreign-key");
        }

        public static IndexMold GetIndex(this ParsingContext context)
        {
            return context.GetProperty<IndexMold>("index");
        }
    }
}
