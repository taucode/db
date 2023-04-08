using TauCode.Db.Model.Enums;
using TauCode.Db.Model.Interfaces;
using TauCode.Extensions;

namespace TauCode.Db.Model.Molds
{
    public class ForeignKeyMold : ConstraintMold, IForeignKeyMold
    {
        #region IForeignKeyMold Members

        public IList<string> ColumnNames { get; } = new List<string>();

        public string? ReferencedTableSchemaName { get; set; }
        public string ReferencedTableName { get; set; } = null!;
        public IList<string> ReferencedColumnNames { get; } = new List<string>();

        #endregion

        #region Overridden

        public override ConstraintType Type => ConstraintType.ForeignKey;

        public override IMold Clone(bool includeProperties = false)
        {
            var clonedForeignKeyMold = new ForeignKeyMold
            {
                Name = this.Name,
                SchemaName = this.SchemaName,
                TableName = this.TableName,

                ReferencedTableSchemaName = this.ReferencedTableSchemaName,
                ReferencedTableName = this.ReferencedTableName,
            };

            clonedForeignKeyMold.ColumnNames.AddMany(this.ColumnNames);
            clonedForeignKeyMold.ReferencedColumnNames.AddMany(this.ReferencedColumnNames);

            if (includeProperties)
            {
                clonedForeignKeyMold.CopyPropertiesFrom(this);
            }

            return clonedForeignKeyMold;
        }

        #endregion

        // todo deal with this
        //public string GetDefaultCaption()
        //{
        //    var sb = new StringBuilder();
        //    var columnNames = string.Join(", ", this.ColumnNames);
        //    var referencedColumnNames = string.Join(", ", this.ReferencedColumnNames);

        //    sb.Append("CONSTRAINT ");
        //    if (!string.IsNullOrEmpty(this.Name))
        //    {
        //        sb.Append(this.Name);
        //        sb.Append(" ");
        //    }

        //    sb.Append($"FOREIGN KEY({columnNames}) REFERENCES {this.ReferencedTableName}({referencedColumnNames})");
        //    return sb.ToString();
        //}
    }
}
