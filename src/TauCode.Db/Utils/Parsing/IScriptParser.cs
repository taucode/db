using TauCode.Db.Utils.Dialects;

namespace TauCode.Db.Utils.Parsing
{
    public interface IScriptParser
    {
        IDialect Dialect { get; }

        object[] Parse(string script);
    }
}
