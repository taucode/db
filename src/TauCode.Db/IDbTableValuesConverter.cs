namespace TauCode.Db
{
    public interface IDbTableValuesConverter
    {
        IDbValueConverter GetColumnConverter(string columnName);
        void SetColumnConverter(string columnName, IDbValueConverter dbValueConverter);
    }
}
