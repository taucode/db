namespace TauCode.Db.Model.Interfaces;

public interface IMold
{
    IDictionary<string, string> Properties { get; set; }
    IMold Clone(bool includeProperties = false);
}