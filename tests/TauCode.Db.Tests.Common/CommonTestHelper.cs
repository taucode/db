using System;

namespace TauCode.Db.Tests.Common
{
    public static class CommonTestHelper
    {
        public const string NonExistingGuidString = "eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee";
        public static readonly Guid NonExistingGuid = new Guid(NonExistingGuidString);
    }
}
