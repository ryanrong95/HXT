using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public  class NoticeSettings
    {
        static  object locker = new object();
        private static NoticeSettings current;

        public static NoticeSettings Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new NoticeSettings();
                        }
                    }
                }

                return current;
            }
        }

        /// <summary>
        /// 报关员菜单
        /// </summary>
        public string DeclareMenu { get; }
        /// <summary>
        /// 北京总部菜单
        /// </summary>
        public string HQMenu { get; set; }
        /// <summary>
        /// 财务菜单
        /// </summary>
        public string FinanceMenu { get; set; }
        /// <summary>
        /// 张庆永菜单
        /// </summary>
        public string ManagerMenu { get; set; }
        private NoticeSettings()
        {         
           this.DeclareMenu = System.Configuration.ConfigurationManager.AppSettings["DeclareMenu"];
           this.HQMenu = System.Configuration.ConfigurationManager.AppSettings["HQMenu"];
           this.FinanceMenu = System.Configuration.ConfigurationManager.AppSettings["FinanceMenu"];
           this.ManagerMenu = System.Configuration.ConfigurationManager.AppSettings["ManagerMenu"];
        }
    }
}
