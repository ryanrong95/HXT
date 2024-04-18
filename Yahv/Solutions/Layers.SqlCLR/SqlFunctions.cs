using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;

namespace Layers.SqlCLR
{
    /// <summary>
    /// 转换成sql函数
    /// </summary>
    public class SqlFunctions
    {
        /// <summary>
        /// 查询数据结果转换json结构
        /// </summary>
        /// <param name="sqlSelect">查询语句</param>
        /// <param name="jsonFormattor">json 结构语句</param>
        /// <param name="objects">参数</param>
        /// <returns>Json消息</returns>
        [SqlFunction(DataAccess = DataAccessKind.Read)]
        public static SqlString SqlToJson(string sqlSelect, string jsonFormattor, bool isArray = true)
        {
            using (SqlConnection connection = new SqlConnection("context connection=true"))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sqlSelect, connection);

                //command.Parameters.AddWithValue("@Name", "chenhan tester");


                SqlDataReader reader = command.ExecuteReader();

                string[] columnNames = new string[reader.FieldCount];
                for (int index = 0; index < reader.FieldCount; index++)
                {
                    columnNames[index] = reader.GetName(index);
                }

                List<string> jsonslist = new List<string>();
                while (reader.Read())
                {
                    string json = jsonFormattor;
                    for (int index = 0; index < columnNames.Length; index++)
                    {
                        json = json.Replace("@" + columnNames[index],
                            Utils.GetValue(reader.GetValue(index)));
                    }
                    jsonslist.Add(json);
                }

                reader.Close();

                if (jsonslist.Count == 0)
                {
                    return null;
                }
                if (isArray)
                {
                    return new SqlString("[" + string.Join(",", jsonslist.ToArray()) + "]");
                }
                else
                {
                    return new SqlString(string.Join(",", jsonslist.ToArray()));
                }
            }
        }

    }
}
