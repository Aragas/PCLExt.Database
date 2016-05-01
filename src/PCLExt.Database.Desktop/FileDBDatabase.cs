using System;
using System.Linq;
using System.Linq.Expressions;

using FileDbNs;


namespace PCLExt.Database
{
    /// <summary>
    /// NoSQL Database, only Primitive Types.
    /// </summary>
    public sealed class FileDBDatabase : BaseDatabase
    {
        public override string FileExtension => ".fdb";

        private FileDb Database { get; set; }
        private string Path { get; set; }


        public override BaseDatabase Create(string path)
        {
            Database = new FileDb();
            Path = path;

            return this;
        }

        public override void CreateTable<T>()
        {
            Database.Create(CombinePath(typeof(T).Name), CreateFields(new T()));
            Database.Close();
        }

        public override void Insert<T>(T obj)
        {
            Database.Open(CombinePath(typeof(T).Name), false);
            Database.AddRecord(CreateFieldValues(obj));
            Database.Close();
        }

        public override void Update<T>(T obj)
        {
            Database.Open(CombinePath(typeof(T).Name), false);

            var idProp = obj.GetType().GetProperties().FirstOrDefault(property => property.Name == "Id");
            if (idProp != null)
                Database.UpdateRecords(new FilterExpression("Id", idProp.GetValue(obj), ComparisonOperatorEnum.Equal), CreateFieldValues(obj));

            Database.Close();
        }

        public override T Find<T>(Expression<Func<T, bool>> predicate)
        {
            Database.Open(CombinePath(typeof(T).Name), true);

            var table = Database.SelectAllRecords();
            if (table.Count > 0)
            {
                var function = predicate.Compile();
                var result = table.Select(CreateT<T>).FirstOrDefault(function);
                Database.Close();
                return result;
            }
            else
            {
                Database.Close();
                return null;
            }
        }


        private string CombinePath(string fileName) { return System.IO.Path.Combine(Path, fileName + FileExtension); }


        private static Fields CreateFields(object obj)
        {
            var fields = new Fields();

            foreach (var info in obj.GetType().GetProperties())
            {
                DataTypeEnum dataType;

                if (info.PropertyType.IsEnum || info.PropertyType == typeof(Int16) || info.PropertyType == typeof(UInt16))
                    dataType = DataTypeEnum.Int32;
                else if (info.PropertyType == typeof(bool))
                    dataType = DataTypeEnum.Bool;
                else if (info.PropertyType == typeof(Guid))
                    dataType = DataTypeEnum.String;
                else
                    if (!Enum.TryParse(info.PropertyType.Name, out dataType))
                    throw new Exception("Only Primitive Types supported!");


                var field = new Field(info.Name, dataType);
                if (info.Name == "Id")
                {
                    if (Attribute.IsDefined(info, typeof(PrimaryKeyAttribute)))
                        field.IsPrimaryKey = true;

                    if (Attribute.IsDefined(info, typeof(AutoIncrementAttribute)))
                        field.AutoIncStart = 1;
                }

                fields.Add(field);
            }
            return fields;
        }

        private static FieldValues CreateFieldValues(object obj)
        {
            var fieldValues = new FieldValues();

            foreach (var propertyInfo in obj.GetType().GetProperties())
                fieldValues.Add(propertyInfo.Name, propertyInfo.GetValue(obj));

            return fieldValues;
        }

        private static T CreateT<T>(Record record) where T : class, new()
        {
            var instance = new T();

            foreach (var info in instance.GetType().GetProperties())
                info.SetValue(instance, record[info.Name]);

            return instance;
        }
    }
}
