namespace TauCode.Db
{
    public interface ITableValuesConverter
    {
        IDbValueConverter GetColumnConverter(string columnName);
        void SetColumnConverter(string columnName, IDbValueConverter dbValueConverter);
    }
}
