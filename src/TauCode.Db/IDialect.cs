namespace TauCode.Db;

public interface IDialect : IUtility
{
    string Name { get; }

    IReadOnlyList<Tuple<char, char>> IdentifierDelimiters { get; }
    string Undelimit(string identifier);
}