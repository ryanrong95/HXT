using System;


namespace Needs.Ccs.Services.Models
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

        private decimal? netWeigh;
        /// <summary>
        /// 净重(默认逻辑NetWeight = Weight * 0.7d)
        /// </summary>
        public decimal? NetWeight
        {
            get
            {
                return this.netWeigh;
            }

            set
            {
                this.netWeigh = value;
                if (value == null)
                {
                    if (this.Weight == null)
                    {
                        value = null;
                    }
                    else
                    {
                        value = this.Weight * (decimal)0.7d;
                    }
                    this.netWeigh = value;
                }
            }
        }
        /// <summary>
        ///  
        /// </summary>
        public decimal? Volume { get; set; }


        public Notice[] notices { get; set; }

    }
}
