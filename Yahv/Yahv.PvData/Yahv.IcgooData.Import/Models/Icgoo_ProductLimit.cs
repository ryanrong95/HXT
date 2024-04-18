using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yahv.DataImport.Service.Extends;
using Yahv.DataImport.Service.Utils;

namespace Yahv.IcgooData.Import.Models
{
    /// <summary>
    /// Icgoo型号和管制类型对照表
    /// </summary>
    public class Icgoo_ProductLimit
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
        /// 数据来源
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 管制类型
        /// </summary>
        public string RealType { get; set; }

        /// <summary>
        /// 数据的更新时间
        /// </summary>
        public DateTime WriteTime { get; set; }

        /// <summary>
        /// 是否超重
        /// </summary>
        public bool IsBig { get; set; }
    }

    public class Icgoo_ProductLimits
    {
        string file;
        public Icgoo_ProductLimits()
        {
            this.file = "product_limit_20200618.csv";
        }

        public void Import()
        {
            List<Icgoo_ProductLimit> productLimits = new List<Icgoo_ProductLimit>();
            StringBuilder builder = new StringBuilder();

            FileInfo fileInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/icgoo20200703/weipan", file));
            using (StreamReader sr = new StreamReader(fileInfo.FullName, Encoding.UTF8))
            {
                //标题行
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var datas = new string[6];
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

                            if (datas[4] != null)
                            {
                                datas[5] = builder.ToString();
                                builder.Clear();
                            }
                        }
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

                    productLimits.Add(new Icgoo_ProductLimit()
                    {
                        ID = GuidUtil.NewGuidUp(),
                        PartNumber = datas[0],
                        Manufacturer = datas[1],
                        Note = datas[2],
                        RealType = datas[3],
                        WriteTime = DateTime.Parse(datas[4]),
                        IsBig = datas[5] == "0" ? false : true
                    });
                }
            }

            int count = 0, step = 10000;
            using (var conn = new PvDataReponsitory().CreateConnection())
            {
                while (count < productLimits.Count)
                {
                    conn.BulkInsert(productLimits.Skip(count).Take(step));
                    count += step;
                }
            }
        }
    }
}
