using TauCode.Db.Model;

namespace TauCode.Db
{
    public interface IScriptBuilderLab : IUtility
    {
        char? CurrentOpeningIdentifierDelimiter { get; set; }
        string BuildCreateTableScript(TableMold table, bool includeConstraints);
    }
}
