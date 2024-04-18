using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services
{
    /// <summary>
    /// 读取配置信息
    /// </summary>
    static public class PvClientConfig
    {
        /// <summary>
        /// 默认平台公司
        /// </summary>
        static public string CompanyID
        {
            get
            {
                return ConfigurationManager.AppSettings["CompanyID"];
            }
        }

        /// <summary>
        /// 畅运物流公司
        /// </summary>
        static public string CYCompanyID => ConfigurationManager.AppSettings["CYCompanyID"];

        /// <summary>
        /// 默认平台公司
        /// </summary>
        static public string CompanyName
        {
            get
            {
                return ConfigurationManager.AppSettings["CompanyName"];
            }
        }

        /// <summary>
        /// 默认香港库房
        /// </summary>
        static public string WareHouseID
        {
            get
            {
                return ConfigurationManager.AppSettings["WareHouseID"];
            }
        }

        /// <summary>
        /// 默认深圳库房
        /// </summary>
        static public string SZWareHouseID
        {
            get
            {
                return ConfigurationManager.AppSettings["SZWareHouseID"];
            }
        }

        /// <summary>
        /// 文件上传路径
        /// </summary>
        static public string FileServerUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["FileServerUrl"];
            }
        }

        /// <summary>
        /// 库房公司ID
        /// </summary>
        static public string ThirdCompanyID
        {
            get
            {
                return ConfigurationManager.AppSettings["ThirdCompanyID"];
            }
        }

        static public string DomainUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["DomainUrl"];
            }
        }

        static public string Domain
        {
            get
            {
                return ConfigurationManager.AppSettings["Domain"];
            }
        }

        /// <summary>
        /// 香港库房英文名
        /// </summary>
        static public string WareHouseEnglishName
        {
            get
            {
                return ConfigurationManager.AppSettings["WareHouseEnglishName"];
            }
        }
    }
}
