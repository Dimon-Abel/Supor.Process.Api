using Supor.Process.Entity.Attributies;
using System;
using System.Reflection;
using System.Text;

namespace Supor.Process.Common.SentenceGenerate
{
    public static class SqlGenerate
    {
        public static string GenerateSelect<T>() where T : class
        {
            var type = typeof(T);

            var table = type.GetCustomAttribute<TableNameAttribute>();
            var tableName = table.Name ?? type.Name;

            return $"select * from {tableName} where 1 = 1 ";
        }

        public static string GenerateInsert<T>() where T : class
        {
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var table = type.GetCustomAttribute<TableNameAttribute>();
            var tableName = table.Name ?? type.Name;

            var columns = new StringBuilder();
            var values = new StringBuilder();

            foreach (var prop in properties)
            {
                if (prop.GetCustomAttribute<SqlIgnoreAttribute>() != null)
                    continue;

                columns.Append($"{prop.Name},");
                values.Append($"@{prop.Name},");
            }

            // 移除末尾逗号
            if (columns.Length > 0) columns.Length--;
            if (values.Length > 0) values.Length--;

            return $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
        }

        public static string GenerateUpdate<T>(T obj = null) where T : class
        {
            var type = obj.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var table = type.GetCustomAttribute<TableNameAttribute>();
            var tableName = table.Name ?? type.Name;

            var setBuilder = new StringBuilder();
            var whereBuilder = new StringBuilder();

            foreach (var prop in properties)
            {
                object value = null;

                if (obj != null) {
                    value = prop.GetValue(obj, null);
                }

                var primaryKey = prop.GetCustomAttribute<PrimaryKeyAttribute>();

                if (primaryKey != null)
                {
                    whereBuilder.Append($"{primaryKey.Name} = @{prop.Name} AND ");
                }
                else if (value != null)
                {
                    setBuilder.Append($"{prop.Name} = @{prop.Name}, ");
                }
            }

            if (setBuilder.Length == 0 || whereBuilder.Length == 0)
                throw new ArgumentException("必须包含主键字段和至少一个更新字段");

            // 移除末尾多余的逗号和AND
            setBuilder.Length -= 2;
            whereBuilder.Length -= 5;

            return $"UPDATE {tableName} SET {setBuilder} WHERE {whereBuilder}";
        }
    }
}
