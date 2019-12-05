using System.Data;

namespace TauCode.Db.Testing
{
    public abstract class DbTestBase
    {
        protected IDbConnection DbConnection { get; set; }
        
        protected virtual void OneTimeSetUpImpl()
        {
        }

        protected virtual void OneTimeTearDownImpl()
        {
        }

        protected virtual void SetUpImpl()
        {
        }

        protected virtual void TearDownImpl()
        {
        }
    }
}
