﻿using System;
using System.Linq.Expressions;

using SQLite.Net;
using SQLite.Net.Interop;


namespace PCLExt.Database
{
    /// <summary>
    /// SQL Database, only Primitive Types.
    /// </summary>
    public sealed class SQLiteDatabase : BaseDatabase
    {
        public override string FileExtension => ".sqlite3";

        private SQLiteConnection Connection { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Path without file extension</param>
        /// <returns></returns>
        public override BaseDatabase Create(string path)
        {
#if DESKTOP || MAC
            Connection = new SQLiteConnection(new SQLite.Net.Platform.Generic.SQLitePlatformGeneric(), path + FileExtension);
#elif ANDROID
            Connection = new SQLiteConnection(new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid(), path + FileExtension);
#elif __IOS__
            Connection = new SQLiteConnection(new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS(), path + FileExtension);
#endif

            return this;
        }

        public override void CreateTable<T>()
        {
            var flags = CreateFlags.None;

            var prop = typeof(T).GetProperty("Id");
            if (Attribute.IsDefined(prop, typeof (PrimaryKeyAttribute)))
                flags |= CreateFlags.ImplicitPK;
            if (Attribute.IsDefined(prop, typeof (AutoIncrementAttribute)))
                flags |= CreateFlags.AutoIncPK;

            Connection.CreateTable<T>(flags);
        }

        public override void Insert<T>(T obj) { Connection.Insert(obj); }

        public override void Update<T>(T obj) { Connection.Update(obj); }

        public override T Find<T>(Expression<Func<T, bool>> predicate) => Connection.Find(predicate);
    }
}
