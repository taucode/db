using TauCode.Utils.Extensions;

namespace TauCode.Db.Tests.Common
{
    public static class CommonTestHelper
    {
        public static string GetResourceText(string fileName) =>
            typeof(CommonTestHelper).Assembly.GetResourceText(fileName, true);
    }
}
