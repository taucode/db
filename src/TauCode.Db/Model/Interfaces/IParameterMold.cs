using System.Data;

namespace TauCode.Db.Model.Interfaces;

public interface IParameterMold : INamedMold
{
    IColumnMold? Column { get; set; }
    IDbDataParameter? Parameter { get; set; }
}