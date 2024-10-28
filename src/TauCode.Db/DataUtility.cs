using System.Data;

namespace TauCode.Db;

public abstract class DataUtility : Utility, IDataUtility
{
    #region ctor

    protected DataUtility()
    {
    }

    protected DataUtility(IDbConnection? connection)
    {
        this.Connection = connection;
    }

    #endregion

    #region IDataUtility Members

    public IDbConnection? Connection { get; set; }

    #endregion
}