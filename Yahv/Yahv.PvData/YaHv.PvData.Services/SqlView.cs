using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.PvData.Services
{
    /// <summary>
    /// 利用Sql获取数据
    /// </summary>
    public class SqlView
    {
        static string sqlClassifiedInfo;
        static string sqlStandardPastQuotes;
        static string sqlStandardPartnumbersForShow;

        static string sqlUpdateClassifiedHistories;
        static string sqlUpdatePreProductCategories;
        static string sqlCountClassifiedHistories;
        static string sqlQueryClassifiedHistories;

        static SqlView()
        {
            sqlClassifiedInfo = SqlConstructor("Query_ClassifiedInfo.sql");
            sqlStandardPastQuotes = SqlConstructor("Query_StandardPastQuotes.sql");
            sqlStandardPartnumbersForShow = SqlConstructor("Query_StandardPartnumbers.sql");

            sqlUpdateClassifiedHistories = SqlConstructor("Update_ClassifiedHistories.sql");
            sqlUpdatePreProductCategories = SqlConstructor("Update_PreProductCategories.sql");
            sqlCountClassifiedHistories = SqlConstructor("Count_ClassifiedHistories.sql");
            sqlQueryClassifiedHistories = SqlConstructor("Query_ClassifiedHistories.sql");
        }

        /// <summary>
        /// 读取sql文件，构造sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        static private string SqlConstructor(string sql)
        {
            //建造语句
            StringBuilder builder = new StringBuilder();
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DB_Script", sql);
            if (!File.Exists(file))
                return null;

            using (var reader = new StreamReader(file, Encoding.UTF8))
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    builder.AppendLine(line);
                    if (line == null)
                    {
                        break;
                    }
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// 查询归类信息
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <param name="manufacturer">品牌</param>
        /// <returns></returns>
        static public Dictionary<string, object> ClassifiedInfo(string partNumber, string manufacturer)
        {
            using (var conn = new PvDataReponsitory().CreateConnection())
            {
                SqlCommand sqlcmd = conn.CreateCommand();
                sqlcmd.CommandText = sqlClassifiedInfo;
                sqlcmd.Parameters.AddWithValue("@paramPN", partNumber);
                sqlcmd.Parameters.AddWithValue("@paramMfr", manufacturer ?? "");

                //从数据库查询数据
                using (var reader = sqlcmd.ExecuteReader())
                {
                    reader.Read();
                    //查询结果
                    int code = reader.GetInt32(0);

                    //查询到数据
                    if (code == 1)
                    {
                        var dic = new Dictionary<string, object>();
                        for (int index = 1; index < reader.FieldCount; index++)
                        {
                            dic.Add(reader.GetName(index), reader[index]);
                        }
                        return dic;
                    }

                    return null;
                }
            }
        }

        /// <summary>
        /// 查询历史价格
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <param name="manufacturer">品牌</param>
        /// <returns></returns>
        static public List<Models.StandardPastQuote> StandardPastQuotes(string partNumber, string manufacturer)
        {
            using (var conn = new PvDataReponsitory().CreateConnection())
            {
                SqlCommand sqlcmd = conn.CreateCommand();
                sqlcmd.CommandText = sqlStandardPastQuotes;
                sqlcmd.Parameters.AddWithValue("@paramPN", partNumber);
                sqlcmd.Parameters.AddWithValue("@paramMfr", manufacturer ?? "");

                List<Models.StandardPastQuote> data = new List<Models.StandardPastQuote>();

                //从数据库查询数据
                using (var reader = sqlcmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var quote = new Models.StandardPastQuote()
                        {
                            PartNumber = reader.GetFieldValue<string>(0),
                            Manufacturer = reader.GetFieldValue<string>(1),
                            Currency = reader.GetFieldValue<string>(2),
                            UnitPrice = reader.GetDecimal(3),
                            Quantity = reader.GetDecimal(4),
                            CreateDate = reader.GetDateTime(5)
                        };
                        data.Add(quote);
                    }
                }

                return data;
            }
        }

        /// <summary>
        /// 查询标准型号
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static public IEnumerable<Models.IStandardPartnumberForShow> StandardPartnumbersForShow(string name)
        {
            var myCachers = new MyCachers();

            //缓存中有需要的数据，直接返回
            var cache = myCachers.StandardPartnumberCaches[name];
            if (cache != null)
                return cache.Partnumbers;

            var data = new List<Models.IStandardPartnumberForShow>();

            #region 数据库查询标准型号数据
            using (var conn = new PvDataReponsitory().CreateConnection())
            {
                SqlCommand sqlcmd = conn.CreateCommand();
                sqlcmd.CommandText = sqlStandardPartnumbersForShow;
                sqlcmd.Parameters.AddWithValue("@paramPN", name);

                using (var reader = sqlcmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Models.IStandardPartnumberForShow sp = new Models.StandardPartnumberForShow()
                        {
                            Partnumber = reader.GetFieldValue<string>(0),
                            HSCode = reader.GetFieldValue<string>(1),
                            TaxCode = reader.GetFieldValue<string>(2),
                            TaxName = reader.GetFieldValue<string>(3),
                            IsCcc = reader.GetBoolean(4),
                            IsEmbargo = reader.GetBoolean(5),
                            CIQprice = reader.GetDecimal(7),
                            Eccn = reader.GetFieldValue<string>(8),

                            TariffRate = reader.GetDecimal(9),
                            VATRate = reader.GetDecimal(10),
                            AddedTariffRate = reader.GetDecimal(11)
                        };
                        data.Add(sp);
                    }
                }

                if (data.Count < 50)
                {
                    sqlcmd.CommandText = $"SELECT TOP ({50 - data.Count}) NAME AS PartNumber FROM [PvData].[dbo].[StandardPartnumbers] WHERE NAME LIKE '{name}%';";
                    using (var reader = sqlcmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Models.IStandardPartnumberForShow sp = new Models.StandardPartnumberOnly()
                            {
                                Partnumber = reader.GetFieldValue<string>(0)
                            };
                            data.Add(sp);
                        }
                    }
                }
            }
            #endregion

            #region 内存拼接大赢家冷偏型号
            string key = name.Substring(0, 2);
            var dyjUnpopulars = myCachers.DyjUnpopularCaches[key];
            var partnumbers = dyjUnpopulars?.Partnumbers.Where(item => item.StartsWith(name));
            if (partnumbers != null && partnumbers.Count() > 0)
            {
                int count = data.Count;
                if (count > 0)
                {
                    data = (from entity in data
                            join partnumber in partnumbers on entity.Partnumber equals partnumber into _partnumbers
                            from partnumber in _partnumbers.DefaultIfEmpty()
                            let isUnpopular = entity.IsUnpopular = partnumber != null
                            select entity).ToList();
                }
                if (count < 50)
                {
                    data.AddRange((from partnumber in partnumbers
                                   select new Models.StandardPartnumberOnly
                                   {
                                       Partnumber = partnumber,
                                       IsUnpopular = true
                                   }).Take(50 - count));
                }
            }
            #endregion

            //加入缓存
            myCachers.StandardPartnumberCaches.Add(name, new StandardPartnumberCache
            {
                Key = name,
                Partnumbers = data,
                ExpireDate = DateTime.Now.AddSeconds(5)
            });

            return data;
        }

        /// <summary>
        /// 修改申报要素品牌后，修正归类历史记录
        /// </summary>
        /// <param name="manufacturer"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        static public int UpdateClassifiedHistories(string manufacturer, string from, string to)
        {
            using (var conn = new PvDataReponsitory().CreateConnection())
            {
                SqlCommand sqlcmd = conn.CreateCommand();
                sqlcmd.CommandText = sqlUpdateClassifiedHistories;
                sqlcmd.Parameters.AddWithValue("@paramMfr", manufacturer);
                sqlcmd.Parameters.AddWithValue("@paramFrom", from.TrimEnd('牌'));
                sqlcmd.Parameters.AddWithValue("@paramTo", to);

                return sqlcmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 修正申报要素品牌后，修正产品预归类数据
        /// </summary>
        /// <param name="manufacturer"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        static public int UpdatePreProductCategories(string manufacturer, string from, string to)
        {
            using (var conn = new PvDataReponsitory().CreateConnection())
            {
                SqlCommand sqlcmd = conn.CreateCommand();
                sqlcmd.CommandText = sqlUpdatePreProductCategories;
                sqlcmd.Parameters.AddWithValue("@paramMfr", manufacturer);
                sqlcmd.Parameters.AddWithValue("@paramFrom", from.TrimEnd('牌'));
                sqlcmd.Parameters.AddWithValue("@paramTo", to);

                return sqlcmd.ExecuteNonQuery();
            }
        }

        static public int GetClassifiedHistoriesCount(string partNumber, string manufacturer, string hsCode, string name)
        {
            using (var conn = new PvDataReponsitory().CreateConnection())
            using (var sqlcmd = conn.CreateCommand())
            {
                var queryCriteria = new List<string>();
                if (!string.IsNullOrEmpty(partNumber))
                {
                    queryCriteria.Add($"[ch].[PartNumber] LIKE @partNumber + '%'");
                    sqlcmd.Parameters.AddWithValue("@partNumber", partNumber.Trim());
                }
                if (!string.IsNullOrEmpty(manufacturer))
                {
                    queryCriteria.Add($"[ch].[Manufacturer] LIKE @manufacturer + '%'");
                    sqlcmd.Parameters.AddWithValue("@manufacturer", manufacturer.Trim());
                }
                if (!string.IsNullOrEmpty(hsCode))
                {
                    queryCriteria.Add($"[ch].[HSCode] LIKE @hsCode + '%'");
                    sqlcmd.Parameters.AddWithValue("@hsCode", hsCode.Trim());
                }
                if (!string.IsNullOrEmpty(name))
                {
                    queryCriteria.Add($"[ch].[Name] LIKE @name + '%'");
                    sqlcmd.Parameters.AddWithValue("@name", name.Trim());
                }

                if (queryCriteria.Count > 0)
                {
                    var condition = string.Join(" AND ", queryCriteria);
                    sqlcmd.CommandText = sqlCountClassifiedHistories.Replace("--{QueryCriteria}", $"WHERE {condition}");
                }
                else
                    sqlcmd.CommandText = sqlCountClassifiedHistories;

                using (var reader = sqlcmd.ExecuteReader())
                {
                    reader.Read();
                    return reader.GetInt32(0);
                }
            }
        }

        static public IEnumerable<Models.ClassifiedHistory> ClassifiedHistories(int pageIndex, int pageSize,
            string partNumber, string manufacturer, string hsCode, string name)
        {
            using (var conn = new PvDataReponsitory().CreateConnection())
            {
                SqlCommand sqlcmd = conn.CreateCommand();
                sqlcmd.Parameters.AddWithValue("@pageIndex", pageIndex);
                sqlcmd.Parameters.AddWithValue("@pageSize", pageSize);

                var queryCriteria = new List<string>();
                if (!string.IsNullOrEmpty(partNumber))
                {
                    queryCriteria.Add($"[ch].[PartNumber] LIKE @partNumber + '%'");
                    sqlcmd.Parameters.AddWithValue("@partNumber", partNumber.Trim());
                }
                if (!string.IsNullOrEmpty(manufacturer))
                {
                    queryCriteria.Add($"[ch].[Manufacturer] LIKE @manufacturer + '%'");
                    sqlcmd.Parameters.AddWithValue("@manufacturer", manufacturer.Trim());
                }
                if (!string.IsNullOrEmpty(hsCode))
                {
                    queryCriteria.Add($"[ch].[HSCode] LIKE @hsCode + '%'");
                    sqlcmd.Parameters.AddWithValue("@hsCode", hsCode.Trim());
                }
                if (!string.IsNullOrEmpty(name))
                {
                    queryCriteria.Add($"[ch].[Name] LIKE @name + '%'");
                    sqlcmd.Parameters.AddWithValue("@name", name.Trim());
                }

                if (queryCriteria.Count > 0)
                {
                    var condition = string.Join(" AND ", queryCriteria);
                    sqlcmd.CommandText = sqlQueryClassifiedHistories.Replace("--{QueryCriteria}", $"WHERE {condition}");
                }
                else
                    sqlcmd.CommandText = sqlQueryClassifiedHistories;

                List<Models.ClassifiedHistory> data = new List<Models.ClassifiedHistory>();

                //从数据库查询数据
                using (var reader = sqlcmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var ch = new Models.ClassifiedHistory()
                        {
                            ID = reader.GetString(0),

                            PartNumber = reader.GetString(1),
                            Manufacturer = reader.GetString(2),
                            HSCode = reader.GetString(3),
                            TariffName = reader.GetString(4),
                            TaxCode = reader.GetString(5),
                            TaxName = reader.GetString(6),
                            LegalUnit1 = reader.GetString(7),
                            LegalUnit2 = reader.GetValue(8) == DBNull.Value ? null : reader.GetString(8),
                            VATRate = reader.GetDecimal(9),
                            ImportPreferentialTaxRate = reader.GetDecimal(10),
                            ImportControlTaxRate = reader.GetValue(11) == DBNull.Value? null : (decimal?)reader.GetDecimal(11),
                            ExciseTaxRate = reader.GetDecimal(12),
                            CIQCode = reader.GetString(13),
                            Elements = reader.GetString(14),

                            SupervisionRequirements = reader.GetValue(15) == DBNull.Value ? null : reader.GetString(15),
                            CIQC = reader.GetValue(16) == DBNull.Value ? null : reader.GetString(16),
                            OrderDate = reader.GetDateTime(17),

                            Ccc = reader.GetBoolean(18),
                            Embargo = reader.GetBoolean(19),
                            HkControl = reader.GetBoolean(20),
                            Coo = reader.GetBoolean(21),
                            CIQ = reader.GetBoolean(22),
                            CIQprice = reader.GetDecimal(23),
                            Summary = reader.GetValue(24) == DBNull.Value ? null : reader.GetString(24),
                        };
                        data.Add(ch);
                    }
                }

                return data;
            }
        }
    }
}
