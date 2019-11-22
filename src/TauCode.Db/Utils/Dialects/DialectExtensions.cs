//using System;
//using System.Linq;
//using TauCode.Db.Model;

//namespace TauCode.Db.Utils.Dialects
//{
//    public static class DialectExtensions
//    {
//        public static bool IsDialectType(this IDialect dialect, string typeName)
//        {
//            if (dialect == null)
//            {
//                throw new ArgumentNullException(nameof(dialect));
//            }

//            if (typeName == null)
//            {
//                throw new ArgumentNullException(nameof(typeName));
//            }

//            var typeNameLower = typeName.ToLower();
//            return dialect.DataTypeNames.Contains(typeNameLower);
//        }

//        public static bool IsDialectSingleWordTypeName(this IDialect dialect, string typeName)
//        {
//            return 
//                dialect.IsDialectType(typeName) &&
//                dialect.GetTypeNameCategory(typeName) == DbTypeNameCategory.SingleWord;
//        }

//        public static bool IsDialectSizedTypeName(this IDialect dialect, string typeName)
//        {
//            return
//                dialect.IsDialectType(typeName) &&
//                dialect.GetTypeNameCategory(typeName) == DbTypeNameCategory.Sized;
//        }

//        public static bool IsDialectPreciseNumberTypeName(this IDialect dialect, string typeName)
//        {
//            return
//                dialect.IsDialectType(typeName) &&
//                dialect.GetTypeNameCategory(typeName) == DbTypeNameCategory.PreciseNumber;
//        }

//        public static string[] GetDialectSingleWordTypeNames(this IDialect dialect)
//        {
//            if (dialect == null)
//            {
//                throw new ArgumentNullException(nameof(dialect));
//            }

//            return dialect.DataTypeNames
//                .Where(x => dialect.GetTypeNameCategory(x) == DbTypeNameCategory.SingleWord)
//                .ToArray();
//        }

//        public static string[] GetDialectSizedTypeNames(this IDialect dialect)
//        {
//            if (dialect == null)
//            {
//                throw new ArgumentNullException(nameof(dialect));
//            }

//            return dialect.DataTypeNames
//                .Where(x => dialect.GetTypeNameCategory(x) == DbTypeNameCategory.Sized)
//                .ToArray();
//        }

//        public static string[] GetDialectPreciseNumberTypeNames(this IDialect dialect)
//        {
//            if (dialect == null)
//            {
//                throw new ArgumentNullException(nameof(dialect));
//            }

//            return dialect.DataTypeNames
//                .Where(x => dialect.GetTypeNameCategory(x) == DbTypeNameCategory.PreciseNumber)
//                .ToArray();
//        }
//    }
//}
