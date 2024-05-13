using Gecko;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Serializers;
using System.IO;
using Web.Services;
using Needs.Utils.Descriptions;
using WinApp.Models;

namespace WinApp
{
    /// <summary>
    /// 打印管理者
    /// </summary>
    public class PrintingManager : ConfigManger
    {
        static List<NameValue> list = null;

        /// <summary>
        /// 打印对象对应的打印机
        /// </summary>
        /// <param name="key">打印对象编号</param>
        /// <returns></returns>
        public static string Printer(string id)
        {
            var entity = LocalConfig().Where(item => item.Name == id).FirstOrDefault();
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
                list.Add(new NameValue {Name = name, Value = value });
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
            string apiPath = $"{FromType.Scheme.GetDescription()}://{FromType.WebApi.GetDescription()}/printings";
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
                StatusDes=item.StatusDes,
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


        static List<NameValue> LocalConfig()
        {
            if (list == null)
            {
                list = new List<NameValue>();
                if (File.Exists(FilePath))
                {
                    list = File.ReadAllText(FilePath).JsonTo<List<NameValue>>();
                }
                else
                {
                    list = new List<NameValue>() { };
                }
            }

            return list;

        }


        static string[] Printers()
        {
            return PrinterSettings.InstalledPrinters.Cast<string>().ToArray();
        }

    }

   
}
