using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Utils.Serializers;

namespace Yahv.Erm.Services.Common
{
    public static class ErmConfig
    {
        /// <summary>
        /// 默认劳务公司
        /// </summary>
        static public string LabourEnterpriseID
        {
            get
            {
                return ConfigurationManager.AppSettings["LabourEnterpriseID"];
            }
        }

        static public string LabourEnterpriseName
        {
            get
            {
                return ConfigurationManager.AppSettings["LabourEnterpriseName"];
            }
        }

        static public string LabourEnterpriseID2
        {
            get
            {
                return ConfigurationManager.AppSettings["LabourEnterpriseID2"];
            }
        }

        static public string LabourEnterpriseName2
        {
            get
            {
                return ConfigurationManager.AppSettings["LabourEnterpriseName2"];
            }
        }
    }

    public static class XdtInfoHelper
    {
        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <returns></returns>
        public static Xdt_Infos GetConfig()
        {
            try
            {
                using (StreamReader reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "xdt_infos.json")))
                {
                    string json = reader.ReadToEnd();
                    return Utils.Serializers.JsonSerializerExtend.JsonTo<Xdt_Infos>(json);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 修改配置文件
        /// </summary>
        public static void SetConfig(Xdt_Infos info)
        {
            try
            {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "xdt_infos.json"), info.Json());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class Xdt_Infos
    {
        /// <summary>
        /// 岗位名称
        /// </summary>
        public List<string> Positions { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public List<string> Roles { get; set; }
    }
}
