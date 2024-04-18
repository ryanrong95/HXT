using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Services.Models
{
    public class Picking
    {
        /// <summary>
        /// 四位年+2位月+2日+6位流水
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        ///  
        /// </summary>
        public string StorageID { get; set; }
        /// <summary>
        ///  
        /// </summary>
        public string NoticeID { get; set; }
        /// <summary>
        /// 装箱信息
        /// </summary>
        public string BoxCode { get; set; }
        /// <summary>
        ///  数量
        /// </summary>
        public decimal Quantity { get; set; }


      
        /// <summary>
        ///  
        /// </summary>
        public string AdminID { get; set; }
        /// <summary>
        /// CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? Weight { get; set; }

        private decimal? netWeight;
        /// <summary>
        /// 净重(默认逻辑NetWeight = Weight * 0.7d)
        /// </summary>
        public decimal? NetWeight
        {
            get
            {

                if (this.netWeight == null)
                {
                    if (this.Weight == null)
                    {
                        this.netWeight = null;
                    }
                    else
                    {
                        this.netWeight = this.Weight * (decimal)0.7d;
                    }
                }
                return this.netWeight;
            }
            set
            {
                this.netWeight = value;

            }
        }


        /// <summary>
        ///  
        /// </summary>
        public decimal? Volume { get; set; }


        public Notice [] Notices { get; set; }

    }
}
