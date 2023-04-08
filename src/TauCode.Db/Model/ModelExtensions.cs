using TauCode.Db.Model.Interfaces;

namespace TauCode.Db.Model;

internal static class ModelExtensions
{
    internal static void CopyPropertiesFrom(this IMold target, IMold source)
    {
        foreach (var pair in source.Properties)
        {
            target.Properties.Add(pair.Key, pair.Value);
        }
    }
}