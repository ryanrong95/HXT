using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Yahv.Services
{
    /// <summary>
    /// 更新pk
    /// </summary>
    public class CheckPrimaryKey
    {
        static public void CheckPkey<T>()
        {
            var type = typeof(T);
            Regex regex_tableName = new Regex(@"FROM(.*?)where", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            foreach (var name in Enum.GetNames(type))
            {
                var member = type.GetMember(name).Single();
                var rpstoryAttr = member.GetCustomAttribute<Layers.Linq.RepositoryAttribute>() as Layers.Linq.RepositoryAttribute;
                var primkeyAttr = member.GetCustomAttribute<Layers.Data.PKeyAttribute>() as Layers.Data.PKeyAttribute;

                if (string.IsNullOrWhiteSpace(primkeyAttr.CheckSql))
                {
                    continue;
                }

                try
                {
                    using (var rpstory = rpstoryAttr.Create())
                    {
                        string sql;
                        switch (primkeyAttr.Type)
                        {
                            case Layers.Data.PKeySigner.Mode.Date:
                                sql = primkeyAttr.CheckSql.Replace("{datetime}", DateTime.Now.ToString("yyyyMMdd"));
                                break;
                            case Layers.Data.PKeySigner.Mode.Normal:
                            default:
                                sql = primkeyAttr.CheckSql;
                                break;
                        }
                        string tableName = regex_tableName.Match(sql).Groups[1].Value.Trim();

                        if (rpstory.Query<int>($"SELECT COUNT(1) FROM {tableName.Split('.')[0]}.sys.objects WHERE object_id = OBJECT_ID({{0}})", tableName).Single() == 1)
                        {
                            int current = rpstory.Query<int?>(sql).Single() ?? 0;

                            if (rpstory.Query<int>($"select count(*) from PrimaryKeys where Name='{primkeyAttr.Name}'").Single() == 0)
                            {
                                rpstory.Command($"INSERT INTO dbo.PrimaryKeys ( Name, Type, Length, Value, Day ) values('{primkeyAttr.Name}',{(int)primkeyAttr.Type},{primkeyAttr.Length},{current},{DateTime.Now.Day})");
                                Console.WriteLine($"同步ID：{rpstoryAttr.Type.Name}.{primkeyAttr.Name}");
                            }
                            else
                            {
                                rpstory.Command("update PrimaryKeys Set [Value] = {0},Day={1} where Name={2}", current, DateTime.Now.Day, primkeyAttr.Name);
                                Console.WriteLine($"更正ID：{rpstoryAttr.Type.Name}.{primkeyAttr.Name}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"异常：{rpstoryAttr.Type.Name}.{primkeyAttr.Name}" + ex.Message);
                }
            }

        }

        static void Main(string[] args)
        {
            var type = typeof(Yahv.Underly.PKeyType);
            Regex regex_tableName = new Regex(@"FROM(.*?)where", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            foreach (var name in Enum.GetNames(typeof(Yahv.Underly.PKeyType)))
            {
                try
                {
                    var member = type.GetMember(name).Single();
                    var rpstoryAttr = member.GetCustomAttribute<Layers.Linq.RepositoryAttribute>() as Layers.Linq.RepositoryAttribute;
                    var primkeyAttr = member.GetCustomAttribute<Layers.Data.PKeyAttribute>() as Layers.Data.PKeyAttribute;

                    if (string.IsNullOrWhiteSpace(primkeyAttr.CheckSql))
                    {
                        continue;
                    }

                    using (var rpstory = rpstoryAttr.Create())
                    {
                        string sql;
                        switch (primkeyAttr.Type)
                        {
                            case Layers.Data.PKeySigner.Mode.Date:
                                sql = primkeyAttr.CheckSql.Replace("{datetime}", DateTime.Now.ToString("yyyyMMdd"));
                                break;
                            case Layers.Data.PKeySigner.Mode.Normal:
                            default:
                                sql = primkeyAttr.CheckSql;
                                break;
                        }
                        string tableName = regex_tableName.Match(sql).Groups[1].Value.Trim();

                        if (rpstory.Query<int>($"SELECT COUNT(1) FROM {tableName.Split('.')[0]}.sys.objects WHERE object_id = OBJECT_ID({{0}})", tableName).Single() == 1)
                        {
                            int current = rpstory.Query<int?>(sql).Single() ?? 0;
                            rpstory.Command("update PrimaryKeys Set [Value] = {0},Day={1} where Name={2}", current, DateTime.Now.Day, primkeyAttr.Name);
                            Console.WriteLine($"更正ID：{rpstoryAttr.Type.Name}.{primkeyAttr.Name}");
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            Console.Read();
        }
    }
}