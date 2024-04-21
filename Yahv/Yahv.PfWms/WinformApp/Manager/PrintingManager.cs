using Gecko;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Yahv.Underly;
using Yahv.Utils.Serializers;
using WinApp.Models;
using WinApp.Printers;

namespace WinApp
{
    /// <summary>
    /// 打印管理者
    /// </summary>
    public class PrintingManager : ConfigManger
    {
        static List<NameValuePair> list = null;
        static List<Setting> list1 = null;

        /// <summary>
        /// 打印对象对应的打印机
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Printer(string name)
        {
            var entity = LocalConfig().Where(item => item.Name == name).FirstOrDefault();
            return entity != null ? entity.Value : "";
        }

        /// <summary>
        /// 打印配置和当前系统打印机对象
        /// </summary>
        /// <returns></returns>
        public static string List()
        {
            return new { config = Printings().ToArray(), printers = Printers() }.Json();
        }

     

        /// <summary>
        /// 保存打印配置
        /// </summary>
        /// <param name="name">对象编号</param>
        /// <param name="value">打印机名称</param>
        public static void Enter(string name, string value)
        {


            var entity = LocalConfig().Where(item => item.Name == name).FirstOrDefault();
            if (entity == null)
            {
                list.Add(new NameValuePair { Name = name, Value = value });
            }
            else
            {
                entity.Value = value;
            }
            using (var writer = File.CreateText(FilePath))
            {
                writer.Write(list.Json());
            }

        }

        class Obj
        {
            public List<Printing> obj { get; set; }
        }

        static Printing[] Printings()
        {
            string apiPath = $"{WebApp.Services.FromType.Scheme.GetDescription()}://{WebApp.Services.FromType.WebApi.GetDescription()}/printings";


            string json = RemoteManger.Read(apiPath);
            var os = json.JsonTo<Obj>();
            var lc = LocalConfig();
            return os.obj.Select(item => new Printing
            {
                ID = item.ID,
                Name = item.Name,
                TypeDes = item.TypeDes,
                Width = item.Width,
                Height = item.Height,
                Url = item.Url,
                Summary = item.Summary,
                Printer = lc.Select(tem => tem.Name).Contains(item.ID) ? lc.Where(tem => tem.Name == item.ID).First().Value : "请选择"
            }).ToArray();

        }

        private static string FilePath
        {
            get
            {
                return ConfigPath + "\\printset.config";
            }
        }


        static List<NameValuePair> LocalConfig()
        {
            if (list == null)
            {
                list = new List<NameValuePair>();
                if (File.Exists(FilePath))
                {
                    list = File.ReadAllText(FilePath).JsonTo<List<NameValuePair>>();
                }
                else
                {
                    list = new List<NameValuePair>() { };
                }
            }

            return list;

        }

        static string[] Printers()
        {
            return PrinterSettings.InstalledPrinters.Cast<string>().ToArray();
        }

        /// <summary>
        /// 打印配置和当前系统打印机对象
        /// </summary>
        /// <returns></returns>
        public static string List1()
        {
            return new { config = Printings1().ToArray(), printers = Printers() }.Json();
        }

        /// <summary>
        /// 保存打印配置
        /// </summary>
        /// <param name="name">对象编号</param>
        /// <param name="value">打印机名称</param>
        public static void Enter(Setting setting)
        {
            var entity = LocalConfig1().Where(item => item.ID == setting.ID).FirstOrDefault();
            if (entity == null)
            {
                list1.Add(new Setting { ID = setting.ID, Height = setting.Height, Name = setting.Name, Printer = setting.Printer, Url = setting.Url, Width = setting.Width });
            }
            else
            {
                entity.Height = setting.Height;
                entity.Name = setting.Name;
                entity.Printer = setting.Printer;
                entity.Url = setting.Url;
                entity.Width = setting.Width;
            }
            using (var writer = File.CreateText(FilePath))
            {
                writer.Write(list1.Json());
            }

        }

        static Printing[] Printings1()
        {
            string apiPath = $"{WebApp.Services.FromType.Scheme.GetDescription()}://{WebApp.Services.FromType.WebApi.GetDescription()}/printings";


            string json = RemoteManger.Read(apiPath);
            var os = json.JsonTo<Obj>();
            var lc = LocalConfig1();
            return os.obj.Select(item => new Printing
            {
                ID = item.ID,
                Name = item.Name,
                TypeDes = item.TypeDes,
                Width = item.Width,
                Height = item.Height,
                Url = item.Url,
                Summary = item.Summary,
                Printer = lc.Select(tem => tem.ID).Contains(item.ID) ? lc.Where(tem => tem.ID == item.ID).First().Printer : "请选择"
            }).ToArray();

        }

        static List<Setting> LocalConfig1()
        {
            if (list1 == null)
            {
                list1 = new List<Setting>();
                if (File.Exists(FilePath))
                {
                    list1 = File.ReadAllText(FilePath).JsonTo<List<Setting>>();
                }
                else
                {
                    list1 = new List<Setting>() { };
                }
            }

            return list1;

        }

        /// <summary>
        /// 打印对象对应的打印机
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Printer1(string name)
        {
            var entity = LocalConfig1().Where(item => item.ID == name).FirstOrDefault();
            return entity != null ? entity.Printer : "";
        }

    }


}
