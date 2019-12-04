//using System;
//using System.Collections.Generic;
//using System.Reflection;

//namespace TauCode.Db.Migrations
//{
//    public interface IMigrator
//    {
//        string ConnectionString { get; set; }
//        DbProviderName ProviderName { get; set; }
//        Assembly MigrationsAssembly { get; set; }
//        void AddSingleton(Type serviceType, object serviceImplementation);
//        IReadOnlyDictionary<Type, object> Singletons { get; }
//        void Migrate();
//    }
//}
