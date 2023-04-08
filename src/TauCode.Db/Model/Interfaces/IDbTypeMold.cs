namespace TauCode.Db.Model.Interfaces;

public interface IDbTypeMold : INamedMold
{
    public int? Size { get; set; }
    public int? Precision { get; set; }
    public int? Scale { get; set; }
}