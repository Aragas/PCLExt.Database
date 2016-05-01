using System;
using System.Linq.Expressions;

namespace PCLExt.Database
{
    public enum DatabaseType { SQLiteDatabase, FileDBDatabase }

    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class AutoIncrementAttribute : Attribute { }


    public abstract class DatabaseTable { }
    public abstract class DatabaseTable<TKeyType> : DatabaseTable
    {
        [PrimaryKey]
        public abstract TKeyType Id { get; protected set; }
    }

    public abstract class BaseDatabase
    {
        public abstract string FileExtension { get; }


        public abstract BaseDatabase Create(string path);

        public abstract void CreateTable<T>() where T : DatabaseTable, new();
        public abstract void Insert<T>(T obj) where T : DatabaseTable, new();
        public abstract void Update<T>(T obj) where T : DatabaseTable, new();
        public abstract T Find<T>(Expression<Func<T, bool>> predicate) where T : DatabaseTable, new();
    }
}