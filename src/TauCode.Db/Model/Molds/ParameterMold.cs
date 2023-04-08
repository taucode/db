using System.Data;
using TauCode.Db.Model.Interfaces;

namespace TauCode.Db.Model.Molds;

public class ParameterMold : NamedMold, IParameterMold
{
    public override IMold Clone(bool includeProperties = false)
    {
        throw new NotImplementedException();
    }

    public IColumnMold? Column { get; set; }
    public IDbDataParameter? Parameter { get; set; }
}