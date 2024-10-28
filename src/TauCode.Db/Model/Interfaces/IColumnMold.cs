namespace TauCode.Db.Model.Interfaces;

public interface IColumnMold : INamedMold
{
    IDbTypeMold Type { get; set; }
    bool IsNullable { get; set; }
    IColumnIdentityMold? Identity { get; set; }
}