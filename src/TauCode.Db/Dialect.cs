namespace TauCode.Db;

public abstract class Dialect : IDialect
{
    #region IUtility Members

    public abstract IUtilityFactory Factory { get; }

    #endregion

    #region IDialect Members

    public abstract string Name { get; }

    public IReadOnlyList<Tuple<char, char>> IdentifierDelimiters => throw new NotImplementedException();

    public abstract string Undelimit(string identifier);

    #endregion
}