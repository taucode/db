namespace TauCode.Db
{
    public interface IDbValueConverter
    {
        object ToDbValue(object value);
        object FromDbValue(object dbValue);
    }
}
