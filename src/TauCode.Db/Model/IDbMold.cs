using System.Collections.Generic;

namespace TauCode.Db.Model
{
    public interface IDbMold
    {
        IDictionary<string, string> Properties { get; set; }
    }
}
