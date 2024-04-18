using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Yahv.Linq.Extends
{
    /// <summary>
    /// Sql with 信息
    /// </summary>
    public enum With
    {
        Nolock
    }

    /// <summary>
    /// 数据扩展
    /// </summary>
    static public class DataContextWithExentions
    {
        static Regex regex_withNoLock = new Regex(@"(\] AS \[t\d+\])", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        static DataContextWithExentions()
        {

            //Yahv.Linq.Extends.With.Nolock
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        /// <param name="context"></param>
        static void openConnection(this DataContext context)
        {
            if (context.Connection.State == ConnectionState.Closed)
            {
                context.Connection.Open();
            }
        }

        /// <summary>
        /// 获取当前连接
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        static DataContext getDataContext(this IQueryable query)
        {
            var provoider = query.Provider;
            var context = provoider.GetType().GetField("context", BindingFlags.NonPublic
                   | BindingFlags.Instance).GetValue(provoider) as DataContext;

            if (context == null)
            {
                throw new NotSupportedException("不支持您的调用！");
            }
            return context;
        }

        /// <summary>
        /// 将Sql语句修改为with nolock
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        static string AddWithNoLock(string context)
        {
            IEnumerable<Match> matches = regex_withNoLock.Matches(context).Cast<Match>()
                .OrderByDescending(m => m.Index);
            foreach (Match m in matches)
            {
                int splitIndex = m.Index + m.Value.Length;
                context =
                    context.Substring(0, splitIndex) + " WITH (NOLOCK)" +
                    context.Substring(splitIndex);
            }

            return context;
        }

        /// <summary>
        /// 扩展GetCommend方法，允许设置WithNoLick
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="query"></param>
        /// <param name="with"></param>
        /// <returns></returns>
        static SqlCommand getCommand(DataContext context, IQueryable query, With with)
        {
            SqlCommand command = context.GetCommand(query) as SqlCommand;

            switch (with)
            {
                case Extends.With.Nolock:
                    command.CommandText = AddWithNoLock(command.CommandText);
                    break;
                default:
                    throw new NotSupportedException("不支持您的调用");
            }

            return command;
        }

        [Obsolete("发现微软做了系统级管控，暂时放弃相关开发。如果需要转为视图开发")]
        static public IEnumerable<TResult> Invoke<TSource, TResult>(this IQueryable<TSource> query, Expression<Func<TSource, TResult>> selector, With with)
        {
            var context = query.getDataContext();
            SqlCommand command = getCommand(context, query.Select(selector), with) as SqlCommand;
            context.openConnection();
            var reader = command.ExecuteReader();

            var compile = selector.Compile();

            var k = selector.Body;

            while (reader.Read())
            {
                //dynamic obj = new System.Dynamic.ExpandoObject();

                //var index = 0;
                //foreach (var property in properties)
                //{
                //    ((IDictionary<string, object>)obj)[property.Name] = reader[index++];
                //}
                //list.Add(obj);
            }


            return null;

            //var data = context.Translate<T>(reader);

            //return data as IEnumerable<T>;
        }

        //static public T Invoke<T>(this IQueryable query, With with) where T : class, IEnumerable
        //{
        //    var context = query.getDataContext();
        //    DbCommand command = getCommand(context, query, with);
        //    context.openConnection();

        //    //var first = ((Type[])((TypeInfo)(query)).ImplementedInterfaces)[0].GetGenericArguments()[0];

        //    return context.Translate(command.ExecuteReader()).GetResult<dynamic>() as T;
        //}


        static object Excute(string cSharpCode)
        {
            // 1.CSharpCodePrivoder
            CSharpCodeProvider provider = new CSharpCodeProvider();

            // 2.ICodeComplier
            //ICodeCompiler objICodeCompiler = objCSharpCodePrivoder.CreateCompiler();

            // 3.CompilerParameters
            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.Add("System.dll");
            objCompilerParameters.GenerateExecutable = false;
            objCompilerParameters.GenerateInMemory = true;

            // 4.CompilerResults
            CompilerResults result = provider.CompileAssemblyFromSource(objCompilerParameters, cSharpCode);

            if (result.Errors.HasErrors)
            {
                throw new Exception(string.Join("\r\n", result.Errors.Cast<CompilerError>().Select(item => item.ErrorText)));
            }
            else
            {
                // 通过反射，调用HelloWorld的实例
                Assembly assembly = result.CompiledAssembly;

                assembly.GetType("Excutor");

                object instance = assembly.CreateInstance("DynamicCodeGenerate.HelloWorld");
                MethodInfo objMI = instance.GetType().GetMethod("OutPut");

                return objMI.Invoke(instance, null);
            }
        }
    }

    class Excutor
    {
        static public object run(SqlDataReader reader)
        {
            //var list = new List<T>();
            //var properties = typeof(T).GetProperties();
            //while (reader.Read())
            //{
            //    dynamic obj = new System.Dynamic.ExpandoObject();

            //    var index = 0;
            //    foreach (var property in properties)
            //    {
            //        ((IDictionary<string, object>)obj)[property.Name] = reader[index++];
            //    }
            //    list.Add(obj);
            //}

            //return list;

            return null;

        }
    }
}
