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
    /// Icgoo型号和额外关税对照表
    /// </summary>
    public class Icgoo_ProductTax
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
        /// 1 表示没有额外关税， 数值比 1 大的是有额外关税。
        /// </summary>
        public decimal Tax { get; set; }

        /// <summary>
        /// 数据收录的时间
        /// </summary>
        public DateTime JoinDate { get; set; }

        /// <summary>
        /// 产地
        /// </summary>
        public string Coo { get; set; }

        /// <summary>
        /// 数据来源， YJX 表示 ‘英捷讯’  HY 表示’恒远‘， XDT 表示’芯达通‘
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 这里只有三种可能的值：   额外关税 ， 产地税 ， 额外关税+产地税 
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; }
    }

    public class Icgoo_ProductTaxes
    {
        string file;
        public Icgoo_ProductTaxes()
        {
            this.file = "product_tax_20200617-1.csv";
        }

        public void Import()
        {
            List<Icgoo_ProductTax> productTaxes = new List<Icgoo_ProductTax>();
            StringBuilder builder = new StringBuilder();

            FileInfo fileInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/icgoo20200703/weipan", file));
            using (StreamReader sr = new StreamReader(fileInfo.FullName, Encoding.UTF8))
            {
                //标题行
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var datas = new string[10];
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

                            if (datas[8] != null)
                            {
                                datas[9] = builder.ToString();
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

                    productTaxes.Add(new Icgoo_ProductTax()
                    {
                        ID = datas[0],
                        PartNumber = datas[1],
                        Manufacturer = datas[2],
                        Tax = decimal.Parse(datas[3]),
                        JoinDate = DateTime.Parse(datas[4]),
                        Coo = datas[5],
                        Source = datas[6],
                        Note = datas[7],
                        TaxName = datas[8],
                        HSCode = datas[9]
                    });
                }
            }

            int count = 0, step = 10000;
            using (var conn = new PvDataReponsitory().CreateConnection())
            {
                while (count < productTaxes.Count)
                {
                    conn.BulkInsert(productTaxes.Skip(count).Take(step));
                    count += step;
                }
            }
        }
    }
}
