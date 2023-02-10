namespace TauCode.Db.Model.Interfaces;

public interface IMold
{
    IDictionary<string, string> Properties { get; }
    IMold Clone(bool includeProperties = false);
}