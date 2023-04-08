namespace TauCode.Db;

public interface IUtilityFactory
{
    IDialect Dialect { get; }
    IScriptBuilder CreateScriptBuilder();
    IExplorer CreateExplorer();
    ICruder CreateCruder();
}
