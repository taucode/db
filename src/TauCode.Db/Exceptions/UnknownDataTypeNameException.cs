using System;
using System.Text;

namespace TauCode.Db.Exceptions
{
    [Serializable]
    public class UnknownDataTypeNameException : DialectException
    {
        public UnknownDataTypeNameException(string message = null, string typeName = null)
            : base(BuildMessage(message, typeName))
        {
        }

        private static string BuildMessage(string message, string typeName)
        {
            var sb = new StringBuilder();

            if (message == null)
            {
                message  = "Unknown data type name";
            }

            sb.Append(message);

            if (typeName != null)
            {
                sb.Append($": '{typeName}'");
            }

            sb.Append(".");

            return sb.ToString();
        }
    }
}
