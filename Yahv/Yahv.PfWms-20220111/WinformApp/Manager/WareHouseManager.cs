using Gecko;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Serializers;
using System.IO;
using WinApp.Models;
using Web.Services;
using Needs.Utils.Descriptions;

namespace WinApp
{
    /// <summary>
    /// 打印管理者
    /// </summary>
    public class WareHouseManager : ConfigManger
    {
        static List<NameValue> list = null;
        public static string WareHouseID
        {
            get
            {
                var entity = LocalConfig().FirstOrDefault();
                return entity != null ? entity.Value : "";
            }

        }

        public static string List()
        {
            return new { config = LocalConfig().ToArray(), storehourses = WareHouses() }.Json();
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">库房编号</param>
        /// <param name="value">库房名称</param>
        public static void Enter(string name, string value)
        {

            LocalConfig().Clear();

            list.Add(new NameValue { Name = name,Value=value });

            using (var writer = File.CreateText(FilePath))
            {
                writer.Write(list.Json());
            }
        }





        class Obj
        {
            public List<NameValue> obj { get; set; }
        }

        private static NameValue[] WareHouses()
        {
            string apiPath = $"{FromType.Scheme.GetDescription()}://{FromType.WebApi.GetDescription()}/warehourse";
            string json = RemoteManger.Read(apiPath);
            var os = json.JsonTo<Obj>();
            return os.obj.ToArray();

        }




        private static string FilePath
        {
            get
            {
                return ConfigPath + "warehouse.config";
            }
        }

        private static List<NameValue> LocalConfig()
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
                    list = new List<NameValue>();
                    list.Add(new NameValue { Name = "", Value = "请选择" });
                }

            }

            return list;

        }

    }


}
