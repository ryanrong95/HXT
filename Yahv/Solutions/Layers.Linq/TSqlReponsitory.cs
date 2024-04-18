using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;


namespace Layers.Linq
{
    /// <summary>
    /// T-Sql支持者
    /// </summary>
    /// <remarks>
    /// 没有考虑线程安全性，请在自己的线程中增加创建支持者
    /// </remarks>
    public class TSqlReponsitory : IDisposable
    {
        LinqReponsitory reponsitory;
        /// <summary>
        /// 构造器
        /// </summary>
        internal TSqlReponsitory(LinqReponsitory reponsitory)
        {
            this.reponsitory = reponsitory;
        }

        /// <summary>
        /// 插入目标对象表数据
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="entities">目标对象数组</param>
        public void Insert<T>(params T[] entities) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            var type = typeof(T);
            var properties = type.GetProperties().Where(item =>
            {
                var attribute = item.GetCustomAttribute<System.Data.Linq.Mapping.ColumnAttribute>(false);
                if (item == null)
                {
                    return true;
                }
                //过滤掉自动生成的字段

                if (attribute == null)
                {
                    return false;
                }

                return !attribute.IsDbGenerated;
            }).ToArray();

            //获取属性型名称
            string propertyNames = $"{string.Join(",", properties.Select(item => $"[{item.Name}]"))}";
            //获取属性型名称值
            string propertyValues = $"{string.Join(",", properties.Select((item) => $"@{item.Name}_{{0}}"))}";

            //记录sql
            StringBuilder sqlBuilder = new StringBuilder();
            //记录参数
            List<SqlParameter> parameters = new List<SqlParameter>();
            List<Task<int>> tasks = new List<Task<int>>();

            for (int index = 0; index < entities.Length; index++)
            {
                //参数不能大于2000，否则会报错
                if (parameters.Count + properties.Length > 2000 || (index > 0 && index % 20 == 0))
                {
                    tasks.Add(this.ExecuteNonQueryAsync(sqlBuilder.ToString(), parameters.ToArray()));
                    sqlBuilder.Clear();
                    parameters.Clear();
                }

                sqlBuilder.Append($"INSERT INTO [{type.Name}]");
                sqlBuilder.Append($"({propertyNames})");
                sqlBuilder.Append("VALUES");
                sqlBuilder.Append('(');
                sqlBuilder.AppendFormat(propertyValues, index);
                sqlBuilder.Append(')');
                sqlBuilder.Append(";").AppendLine();

                parameters.AddRange(properties.Select(item => SqlParameterBuilder.Create($"@{item.Name}_{index}", item.GetValue(entities[index]) ?? DBNull.Value)));
            }

            var sql = sqlBuilder.ToString();
            tasks.Add(this.ExecuteNonQueryAsync(sql, parameters.ToArray()));
            Task.WaitAll(tasks.ToArray());
        }


        /// <summary>
        /// 插入目标对象表数据
        /// [同步]
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="entities">目标对象数组</param>
        public void InserSync<T>(params T[] entities) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            var type = typeof(T);
            var properties = type.GetProperties().Where(item =>
            {
                var attribute = item.GetCustomAttribute<System.Data.Linq.Mapping.ColumnAttribute>(false);
                if (item == null)
                {
                    return true;
                }
                //过滤掉自动生成的字段

                if (attribute == null)
                {
                    return false;
                }

                return !attribute.IsDbGenerated;
            }).ToArray();

            //获取属性型名称
            string propertyNames = $"{string.Join(",", properties.Select(item => $"[{item.Name}]"))}";
            //获取属性型名称值
            string propertyValues = $"{string.Join(",", properties.Select((item) => $"@{item.Name}_{{0}}"))}";

            //记录sql
            StringBuilder sqlBuilder = new StringBuilder();
            //记录参数
            List<SqlParameter> parameters = new List<SqlParameter>();

            for (int index = 0; index < entities.Length; index++)
            {
                //参数不能大于2000，否则会报错
                if (parameters.Count + properties.Length > 2000 || (index > 0 && index % 20 == 0))
                {
                    this.ExecuteNonQuerySync(sqlBuilder.ToString(), parameters.ToArray());
                    sqlBuilder.Clear();
                    parameters.Clear();
                }

                sqlBuilder.Append($"INSERT INTO [{type.Name}]");
                sqlBuilder.Append($"({propertyNames})");
                sqlBuilder.Append("VALUES");
                sqlBuilder.Append('(');
                sqlBuilder.AppendFormat(propertyValues, index);
                sqlBuilder.Append(')');
                sqlBuilder.Append(";").AppendLine();

                parameters.AddRange(properties.Select(item => SqlParameterBuilder.Create($"@{item.Name}_{index}", item.GetValue(entities[index]) ?? DBNull.Value)));
            }

            var sql = sqlBuilder.ToString();
            this.ExecuteNonQuerySync(sql, parameters.ToArray());
        }


        /// <summary>
        /// 异步执行Sql
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>异步任务相应行数</returns>
        Task<int> ExecuteNonQueryAsync(string sql, params SqlParameter[] parameters)
        {
            var command = this.reponsitory.Connection.CreateCommand();

            if (this.reponsitory.DataContext.Transaction != null)
            {
                command.Transaction = this.reponsitory.DataContext.Transaction;
            }

            command.CommandText = sql;
            command.Parameters.AddRange(parameters);
            return command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// 异步执行Sql
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数数组</param>
        /// <returns>异步任务相应行数</returns>
        int ExecuteNonQuerySync(string sql, params SqlParameter[] parameters)
        {
            var command = this.reponsitory.Connection.CreateCommand();

            if (this.reponsitory.DataContext.Transaction != null)
            {
                command.Transaction = this.reponsitory.DataContext.Transaction;
            }

            command.CommandText = sql;
            command.Parameters.AddRange(parameters);
            return command.ExecuteNonQuery();
        }


        /// <summary>
        /// 插入目标对象表数据
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="entities">目标对象数组</param>
        public void Insert<T>(IEnumerable<T> entities) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            //由于需要计数因此，必须是有序操作
            this.Insert(entities.ToArray());
        }

        ///// <summary>
        ///// 跟新使用的Sqls
        ///// </summary>
        //List<string> addUpdateSqls = new List<string>();
        ///// <summary>
        ///// 更新使用的SqlParameters
        ///// </summary>
        //List<SqlParameter[]> addUpdateSqlParameters = new List<SqlParameter[]>();

        ///// <summary>
        ///// 增加更新操作
        ///// </summary>
        ///// <typeparam name="T">操作表类型</typeparam>
        ///// <param name="parameter">更新对象</param>
        ///// <param name="id">唯一码</param>
        ///// <param name="columnName">条件列名称</param>
        ///// <remarks>
        ///// 增加后需要提交
        ///// 暂时没有考虑线程安全性！
        ///// </remarks>
        //public void AddUpdate<T>(object parameter, object id, string columnName = null)
        //{
        //    columnName = (columnName ?? "ID").Trim(']', '[');

        //    var type = parameter.GetType();
        //    var properties = type.GetProperties();
        //    StringBuilder sqlBuilder = new StringBuilder();
        //    sqlBuilder.Append($"UPDATE [{typeof(T).Name}]");
        //    sqlBuilder.Append(" SET ");
        //    sqlBuilder.Append(string.Join(",", properties.Select(item => $"[{item.Name}]=@{item.Name}_{addUpdateSqls.Count}")));
        //    sqlBuilder.Append(" where ").Append($"[{columnName}]").Append("=").Append($"@{columnName}_{addUpdateSqls.Count}");

        //    var parameters = new List<SqlParameter>(properties.Length + 1);
        //    parameters.AddRange(properties.Select(item => SqlParameterBuilder.Create($"@{item.Name}_{addUpdateSqls.Count}", item.GetValue(parameter))));
        //    parameters.Add(SqlParameterBuilder.Create($"@{columnName}_{addUpdateSqls.Count}", id));

        //    addUpdateSqls.Add(sqlBuilder.ToString());
        //    addUpdateSqlParameters.Add(parameters.ToArray());
        //}

        ///// <summary>
        ///// 更新提交
        ///// </summary>
        //public void ExecuteUpdate()
        //{
        //    List<SqlParameter> parameters = new List<SqlParameter>();
        //    StringBuilder sqlBuilder = new StringBuilder();

        //    for (int index = 0; index < addUpdateSqls.Count; index++)
        //    {
        //        if (parameters.Count + addUpdateSqls[index].Length > 2000)
        //        {
        //            this.ExecuteNonQuery(sqlBuilder.ToString(), parameters.ToArray());
        //            sqlBuilder.Clear();
        //            parameters.Clear();
        //        }

        //        sqlBuilder.AppendLine(addUpdateSqls[index]);
        //        parameters.AddRange(addUpdateSqlParameters[index]);
        //    }

        //    var sql = sqlBuilder.ToString();
        //    this.ExecuteNonQuery(sql, parameters.ToArray());
        //}

        /// <summary>
        /// 增加更新操作
        /// 
        /// </summary>
        /// <typeparam name="T">操作表类型</typeparam>
        /// <param name="parameter">更新对象</param>
        /// <param name="id">唯一码</param>
        /// <param name="columnName">条件列名称</param>
        /// <remarks>
        /// 应多数人员要求 增加 lambda 条件更新
        /// </remarks>
        public void Update<T>(object parameter, Expression<Func<T, bool>> lambda) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            var type = parameter.GetType();
            var properties = type.GetProperties();

            var table = this.reponsitory.GetTable<T>() as Table<T>;
            var linq = table.Where(lambda).Select("ID");

            var command = table.Context.GetCommand(linq);
            var sql = command.CommandText;

            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"UPDATE [{typeof(T).Name}]");
            sqlBuilder.Append(" SET ");
            sqlBuilder.Append(string.Join(",", properties.Select(item => $"[{item.Name}]=@{item.Name}")));
            sqlBuilder.Append(" where ID in").Append($"(").Append(sql).Append(")");
            command.CommandText = sqlBuilder.ToString();
            command.Parameters.AddRange(properties.Select(item => SqlParameterBuilder.Create($"@{item.Name}", item.GetValue(parameter))).ToArray());

            if (table.Context.Connection.State != System.Data.ConnectionState.Open)
            {
                table.Context.Connection.Open();
            }

            if (this.reponsitory.DataContext.Transaction != null)
            {
                command.Transaction = this.reponsitory.DataContext.Transaction;
            }


            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除方法
        /// </summary>
        public void Delete<T>(Expression<Func<T, bool>> lambda) where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            var table = this.reponsitory.GetTable<T>() as Table<T>;
            var linq = table.Where(lambda).Select("ID");

            var command = table.Context.GetCommand(linq);
            var sql = command.CommandText;

            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"DELETE FROM [{typeof(T).Name}]");
            sqlBuilder.Append(" where ID in").Append($"(").Append(sql).Append(")");
            command.CommandText = sqlBuilder.ToString();

            if (table.Context.Connection.State != System.Data.ConnectionState.Open)
            {
                table.Context.Connection.Open();
            }

            if (this.reponsitory.DataContext.Transaction != null)
            {
                command.Transaction = this.reponsitory.DataContext.Transaction;
            }




            command.ExecuteNonQuery();
        }


        /// <summary>
        /// 实现释放
        /// </summary>
        public void Dispose()
        {
            this.reponsitory.Dispose();
        }
    }

    /// <summary>
    /// 对sql 参数做一层封装
    /// </summary>
    public class SqlParameterBuilder
    {
        /// <summary>
        /// 建议推算参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <returns>Sql 参数</returns>
        static public SqlParameter Create(string name, object value)
        {
            return new SqlParameter(name, value);
        }
    }
}
