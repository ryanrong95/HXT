using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;

namespace Layers.SqlCLR
{
    /// <summary>
    /// ת����sql����
    /// </summary>
    public class SqlFunctions
    {
        /// <summary>
        /// ��ѯ���ݽ��ת��json�ṹ
        /// </summary>
        /// <param name="sqlSelect">��ѯ���</param>
        /// <param name="jsonFormattor">json �ṹ���</param>
        /// <param name="objects">����</param>
        /// <returns>Json��Ϣ</returns>
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
