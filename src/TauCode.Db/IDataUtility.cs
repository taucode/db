using System.Data;

namespace TauCode.Db;

public interface IDataUtility : IUtility
{
    IDbConnection? Connection { get; set; }
}