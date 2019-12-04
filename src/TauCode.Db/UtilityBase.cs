using System;
using System.Data;

namespace TauCode.Db
{
    public abstract class UtilityBase : IUtility
    {
        protected UtilityBase(IDbConnection connection, bool connectionShouldBeOpened, bool connectionCanBeNull)
        {
            if (connectionCanBeNull && connectionShouldBeOpened)
            {
                throw new ArgumentException(
                    $"Controversial conditions: both '{nameof(connectionCanBeNull)}' and '{nameof(connectionShouldBeOpened)}' are true.",
                    nameof(connectionShouldBeOpened));
            }

            if (connection == null)
            {
                if (connectionCanBeNull)
                {
                    // no problem
                }
                else
                {
                    throw new ArgumentNullException(nameof(connection));
                }
            }
            else
            {
                if (connectionShouldBeOpened && (connection.State & ConnectionState.Open) == 0)
                {
                    throw new ArgumentException("Connection should be opened.", nameof(connection));
                }
            }

            this.Connection = connection;
        }

        protected IDbConnection Connection { get; }

        public abstract IUtilityFactory Factory { get; }
    }
}
