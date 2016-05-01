using System;

namespace PCLExt.Database
{
    /// <summary>
    /// 
    /// </summary>
    public static class Database
    {
        private static Exception NotImplementedInReferenceAssembly() =>
            new NotImplementedException(@"This functionality is not implemented in the portable version of this assembly.
You should reference the PCLExt.Database NuGet package from your main application project in order to reference the platform-specific implementation.");


        public static BaseDatabase Create(string path, DatabaseType type)
        {
#if DESKTOP || MAC
            switch (type)
            {
                case DatabaseType.SQLiteDatabase:
                    return new SQLiteDatabase().Create(path);

                case DatabaseType.FileDBDatabase:
                    return new FileDBDatabase().Create(path);
            }
#elif ANDROID || __IOS__
            switch (type)
            {
                case DatabaseType.SQLiteDatabase:
                    return new SQLiteDatabase().Create(path);

                default:
                    return new SQLiteDatabase().Create(path);
            }
#endif

            throw NotImplementedInReferenceAssembly();
        }
    }
}
