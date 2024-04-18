using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yahv.FPGAData.Import.Attributes;

namespace Yahv.FPGAData.Import.Extends
{
    /// <summary>
    /// 批量插入扩展方法
    /// </summary>
    public static class SqlConnectionExtend
    {
        /// <summary>
        /// 直接插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn">数据库连接</param>
        /// <param name="entities">数据对象</param>
        /// <param name="bulkCopyTimeout">超时时间</param>
        /// <param name="transaction">事务</param>
        public static void BulkInsert<T>(this SqlConnection conn, IEnumerable<T> entities, int? bulkCopyTimeout = null, SqlTransaction transaction = null)
        {
            SqlBulkCopy sbc;
            if (transaction != null)
                sbc = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction);
            else
                sbc = new SqlBulkCopy(conn);

            using (sbc)
            {
                DataTable dt = entities.ToDataTable();

                sbc.DestinationTableName = typeof(T).Name;
                sbc.BatchSize = dt.Rows.Count;

                if (bulkCopyTimeout != null)
                    sbc.BulkCopyTimeout = bulkCopyTimeout.Value;

                if (conn.State != ConnectionState.Open)
                    conn.Open();

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sbc.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                }

                sbc.WriteToServer(dt);
            }
        }

        /// <summary>
        /// 通过临时表插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conn">数据库连接</param>
        /// <param name="entities">数据对象</param>
        public static void BulkInsertWithTempTable<T>(this SqlConnection conn, IEnumerable<T> entities)
        {
            string tableName = conn.CreateTempTable<T>();

            SqlBulkCopy sbc = new SqlBulkCopy(conn);
            using (sbc)
            {
                DataTable dt = entities.ToDataTable();

                sbc.DestinationTableName = tableName;
                sbc.BatchSize = dt.Rows.Count;

                if (conn.State != ConnectionState.Open)
                    conn.Open();

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sbc.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                }

                sbc.WriteToServer(dt);
            }

            conn.UpdateTargetTable<T>();
            conn.DropTempTable(tableName);
        }

        private static string CreateTempTable<T>(this SqlConnection conn)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            StringBuilder cmdText = new StringBuilder();
            var table = typeof(T).GetCustomAttribute<TableAttribute>();
            if (table == null)
            {
                throw new Exception($"未声明{nameof(TableAttribute)}属性");
            }
            cmdText.Append($"create table {table.Name} (");

            var propeties = typeof(T).GetProperties();
            foreach (var prop in propeties)
            {
                var column = prop.GetCustomAttribute<ColumnAttribute>();
                if (column == null)
                {
                    throw new Exception($"未声明{nameof(ColumnAttribute)}属性");
                }
                cmdText.Append($"{column.Name} {column.Definition},");
            }

            cmdText.Remove(cmdText.Length - 1, 1).Append(")");
            SqlCommand cmd = new SqlCommand(cmdText.ToString(), conn);
            cmd.ExecuteNonQuery();

            return table.Name;
        }

        private static void UpdateTargetTable<T>(this SqlConnection conn)
        {
            StringBuilder cmdText = new StringBuilder();
            var table = typeof(T).GetCustomAttribute<TableAttribute>();
            cmdText.Append($"update {table.Target} set ");

            var propeties = typeof(T).GetProperties();
            foreach (var prop in propeties)
            {
                var column = prop.GetCustomAttribute<ColumnAttribute>();
                cmdText.Append($"{column.Target} = tmp.{column.Name},");
            }
            cmdText = cmdText.Remove(cmdText.Length - 1, 1);
            cmdText.Append($" from {table.Target} as target join {table.Name} as tmp on target.ID = tmp.ID");

            SqlCommand cmd = new SqlCommand(cmdText.ToString(), conn);
            cmd.ExecuteNonQuery();
        }

        private static void DropTempTable(this SqlConnection conn, string tableName)
        {
            SqlCommand cmd = new SqlCommand($"drop table {tableName}", conn);
            cmd.ExecuteNonQuery();
        }

        private static DataTable ToDataTable<T>(this IEnumerable<T> entities)
        {
            DataTable dt = new DataTable();
            var propeties = typeof(T).GetProperties().Where(item => !item.PropertyType.FullName.Contains("Layers.Data.Sqls")).ToArray();
            foreach (var prop in propeties)
            {
                var propType = prop.PropertyType;
                if ((propType.IsGenericType) && (propType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    propType = propType.GetGenericArguments()[0];
                }
                dt.Columns.Add(new DataColumn(prop.GetCustomAttribute<ColumnAttribute>()?.Name ?? prop.Name, propType));
            }

            foreach (var entity in entities)
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < propeties.Count(); i++)
                {
                    PropertyInfo prop = propeties[i];
                    object value = prop.GetValue(entity);

                    dr[i] = value ?? DBNull.Value;
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}
