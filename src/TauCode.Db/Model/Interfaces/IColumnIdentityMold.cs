namespace TauCode.Db.Model.Interfaces;

public interface IColumnIdentityMold : IMold
{
    public string? Seed { get; set; }
    public string Increment { get; set; }
}