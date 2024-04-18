using System.Configuration;

namespace Yahv.Settings.Models
{
    /// <summary>
    /// 财务配置
    /// </summary>
    class PaysSettings : IPaysSettings
    {
        public PaysSettings()
        {
            ClosedDay = int.Parse(ConfigurationManager.AppSettings["ClosedDay"]);
        }

        /// <summary>
        /// 封账日
        /// </summary>
        public int ClosedDay { get; set; }
    }
}