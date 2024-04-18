using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class SFConfig
    {
        public string MonthlyCard { get; set; }
        public string PartnerID { get; set; }
        public string Checkword { get; set; }
        public string ReqURL { get; set; }
        public string CheckServiceCode { get; set; }
        public string OrderServiceCode { get; set; }
        public string ImageURL { get; set; }
        public string Language { get; set; }

        static object locker = new object();
        static SFConfig current;

        public static SFConfig Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if(current == null)
                        {
                            current = new SFConfig();
                        }
                    }
                }

                return current;
            }
        }

        private SFConfig()
        {
            this.MonthlyCard = System.Configuration.ConfigurationManager.AppSettings["MonthlyCard"];
            this.PartnerID = System.Configuration.ConfigurationManager.AppSettings["PartnerID"];
            this.Checkword = System.Configuration.ConfigurationManager.AppSettings["Checkword"];
            this.ReqURL = System.Configuration.ConfigurationManager.AppSettings["ReqURL"];
            this.CheckServiceCode = System.Configuration.ConfigurationManager.AppSettings["CheckServiceCode"];
            this.OrderServiceCode = System.Configuration.ConfigurationManager.AppSettings["OrderServiceCode"];
            this.ImageURL = System.Configuration.ConfigurationManager.AppSettings["ImageURL"];
            this.Language = "zh-CN";
        }
    }
}
