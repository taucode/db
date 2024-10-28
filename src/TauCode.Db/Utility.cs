namespace TauCode.Db;

public abstract class Utility : IUtility
{
    #region ctor

    protected Utility()
    {
    }

    #endregion

    #region IUtility Members

    public abstract IUtilityFactory Factory { get; }

    #endregion
}
