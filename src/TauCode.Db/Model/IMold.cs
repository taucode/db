using System.Collections.Generic;

namespace TauCode.Db.Model
{
    public interface IMold
    {
        IDictionary<string, string> Properties { get; set; }
        IMold Clone(bool includeProperties = false);
    }
}
