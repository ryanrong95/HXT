using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.DataImport.Service.Extends;

namespace Yahv.IcgooData.Import.Models
{
    /// <summary>
    /// Icgoo Eccn数据
    /// </summary>
    public class Icgoo_Eccn
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// Eccn编码
        /// </summary>
        public string Code { get; set; }
    }

    public class Icgoo_Eccns
    {
        string[] files;
        public Icgoo_Eccns()
        {
            this.files = new string[3] { "2020-06-24-eccn.csv", "2020-6-23eccn.csv", "eccn_2020-7-3.csv" };
        }

        public void Import()
        {
            List<Icgoo_Eccn> eccns = new List<Icgoo_Eccn>();

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/icgoo20200703/weipan", file));
                StringBuilder builder = new StringBuilder();
                using (StreamReader sr = new StreamReader(fileInfo.FullName, Encoding.Default))
                {

                    //标题行
                    sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        var datas = new string[3];
                        int quotesCount = 0;
                        for (int i = 0; i < line.Length; i++)
                        {
                            char c = line[i];

                            if (c == ',' && quotesCount % 2 == 0)
                            {
                                for (int j = 0; j < datas.Length; j++)
                                {
                                    if (datas[j] == null)
                                    {
                                        datas[j] = builder.ToString();
                                        builder.Clear();
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (c == '"')
                                    quotesCount++;
                                builder.Append(line[i]);

                                if (datas[1] != null)
                                {
                                    datas[2] = builder.ToString();
                                    builder.Clear();
                                }
                            }
                        }

                        if (datas[0] == null || datas[1] == null || datas[2] == null)
                        {
                            datas = line.Split(',');
                        }

                        for (int i = 0; i < datas.Length; i++)
                        {
                            datas[i] = datas[i].Trim();
                            if (datas[i].StartsWith("\""))
                            {
                                //去除首尾特殊字符
                                datas[i] = datas[i].TrimStart('"').TrimEnd('"');
                            }
                        }
                        //品牌处理
                        if (datas[2] == "--")
                            datas[2] = string.Empty;

                        eccns.Add(new Icgoo_Eccn()
                        {
                            ID = string.Concat(datas[1], datas[2].ToLower()).MD5(),
                            PartNumber = datas[1],
                            Manufacturer = datas[2],
                            Code = datas[0]
                        });
                    }
                }
            }

            //数据排重
            eccns = eccns.GroupBy(item => item.ID).Select(item => item.First()).ToList();

            int count = 0, step = 10000;
            using (var conn = new PvDataReponsitory().CreateConnection())
            {
                while (count < eccns.Count)
                {
                    conn.BulkInsert(eccns.Skip(count).Take(step));
                    count += step;
                }
            }
        }
    }
}
