using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using Yahv.Utils.Serializers;

namespace Yahv.PsWms.DappForm.Services
{
    /// <summary>
    /// 打印机配置
    /// </summary>
    public class PrinterConfig
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 使用模版的地址
        /// </summary>
        /// <remarks>
        /// 可以为相对地址
        /// </remarks>
        public string Url { get; set; }

        /// <summary>
        /// 控制的宽
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// 控制的高
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        /// <remarks>
        /// 程序员备注
        /// </remarks>
        public string Summary { get; set; }

        /// <summary>
        /// 打印机
        /// </summary>
        /// <remarks>
        /// 用于打印机的名称
        /// </remarks>
        public string PrinterName { get; set; }

        /// <summary>
        /// 是否连接？
        /// </summary>
        /// <returns></returns>
        public bool Connected()
        {
            return PrinterConfigs.Connected(this.PrinterName);
        }
    }


    /// <summary>
    /// 打印机配置类
    /// </summary>
    public class PrinterConfigs : IEnumerable<PrinterConfig>
    {
        List<PrinterConfig> data;
        string fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "printer.config.json");

        PrinterConfigs()
        {
            if (System.IO.File.Exists(this.fileName))
            {
                string txt = System.IO.File.ReadAllText(this.fileName, Encoding.UTF8);
                var arry = txt.JsonTo<PrinterConfig[]>();
                if (arry.Length > 0)
                {
                    this.data = new List<PrinterConfig>(arry);
                    return;
                }
            }

            #region 初始化配置文件

            var data = this.data = new List<PrinterConfig>();

            data.Add(new PrinterConfig
            {
                Name = "库存标签",
                Url = "/PrintLable/html/Surpluslabel.html",
                
                Width = 353,
                Height = 351,
                Summary = "",
                PrinterName = null
            });

            data.Add(new PrinterConfig
            {
                Name = "库位标签",
                Url = "/PrintLable/html/Shelvelabel.html",
                Width = 353,
                Height = 351,
                Summary = "",
                PrinterName = null
            });

            //data.Add(new PrinterConfig
            //{
            //    Name = "出库单",
            //    Url = "/Print/html/Detailedlist.html",
            //    Summary = "",
            //    PrinterName = null
            //});

            data.Add(new PrinterConfig
            {
                Name = "文档打印",
                Url = "/Print/html/Detailedlist.html",
                Summary = "一般文档：Word，Pdf，图片，等文件的自动打印配置",
                PrinterName = null
            });
            //data.Add(new PrinterConfig
            //{
            //    Name = "清单打印",
            //    Url = "/Print/html/Detailedlist.html",
            //    Summary = "",
            //    PrinterName = null
            //});
            //data.Add(new PrinterConfig
            //{
            //    Name = "图片打印",
            //    Url = "/Print/html/Imgprint.html",
            //    Summary = "",
            //    PrinterName = null
            //});
            data.Add(new PrinterConfig
            {
                Name = "箱签打印",
                Url = "/Print/html/Boxlabel.html",
                Summary = "",
                PrinterName = null
            });

            data.Add(new PrinterConfig
            {
                Name = "预出库单打印",
                Url = "/PrintLable/html/YCKD.html",
                Summary = "只适合预出库单打印功能",
                PrinterName = null
            });

            data.Add(new PrinterConfig
            {
                Name = "入库标签打印",
                Url = "/PrintLable/html/InNoticelabel.html",//做到winform里去
                Summary = "只适合入库标签打印功能",
                PrinterName = null
            });

           
            data.Add(new PrinterConfig
            {
                Name = "出库标签打印",
                Url = "/PrintLable/html/OutNoticelabel.html",//做到winform里去（目前没有）
                Summary = "只适合出库标签打印功能",
                PrinterName = null
            });

            data.Add(new PrinterConfig
            {
                Name = "顺丰打印",
                Url = "",
                Summary = "只适合大陆顺丰面单打印功能",
                PrinterName = null
            });

            data.Add(new PrinterConfig
            {
                Name = "跨越速运打印",
                Url = "",
                Summary = "只适合大陆跨越速运面单打印功能",
                PrinterName = null
            });

            data.Add(new PrinterConfig
            {
                Name = "送货单打印",
                Url = "/PrintLable/html/DeliveryList_sc.html",//做到winform里去
                Summary = "只适合送货单打印功能",
                //PrinterName = "Lenovo M7675DXF Printer"
                PrinterName = null
            });

          

            this.Save(data);

            #endregion
        }

        public const string 跨越速运打印 = nameof(跨越速运打印);
        public const string 顺丰打印 = nameof(顺丰打印);
        public const string 预出库单打印 = nameof(预出库单打印);
        public const string 送货单打印 = nameof(送货单打印);
        public const string 入库标签打印 = nameof(入库标签打印);
        public const string 出库标签打印 = nameof(出库标签打印);
        //public const string 入库单打印 = nameof(入库单打印);


        /// <summary>
        /// 配置索引
        /// </summary>
        /// <param name="index">配置名称</param>
        /// <returns>配置</returns>
        public PrinterConfig this[string index]
        {
            get { return this.data.SingleOrDefault(item => item.Name == index); }
        }

        public IEnumerator<PrinterConfig> GetEnumerator()
        {
            //未实现
            //throw new Exception("8");

            return this.data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Save(IEnumerable<PrinterConfig> data)
        {
            //未实现
            //throw new Exception("8");
            this.data = new List<PrinterConfig>(data);
            System.IO.File.WriteAllText(this.fileName, this.data.Json(Formatting.Indented), Encoding.UTF8);
        }

        static PrinterConfigs current;
        static object locker = new object();
        static public PrinterConfigs Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new PrinterConfigs();
                        }
                    }
                }
                return current;
            }
        }

        /// <summary>
        /// 打印机是否连接？
        /// </summary>
        /// <param name="printerName">打印机名称</param>
        /// <returns>连接情况</returns>
        static public bool Connected(string printerName)
        {
            //throw new Exception("9");
            if (string.IsNullOrWhiteSpace(printerName))
            {
                return false;
            }

            ManagementScope scope = new ManagementScope(@"\root\cimv2");
            scope.Connect();

            // Select Printers from WMI Object Collections
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");

            var view = searcher.Get().Cast<ManagementObject>();

            ManagementObject printer = view.SingleOrDefault(item => item["Name"].ToString() == printerName);

            return printer != null && !printer["WorkOffline"].ToString().ToLower().Equals("true");
        }

    }
}
