using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Views;

namespace Yahv.Services.Models
{
    public class Sorting : IUnique
    {
        #region 属性
        /// <summary>
        /// 唯一码，四位年+2位月+2日+6位流水
        /// </summary>
        public string ID { get; set; }

        public string InputID { get; set; }

        /// <summary>
        ///通知编号 
        /// </summary>
        public string NoticeID { get; set; }

        /// <summary>
        ///运单编号 
        /// </summary>
        public string WaybillID { get; set; }

        /// <summary>
        /// 装箱信息（箱号）
        /// </summary>
        public string BoxCode { get; set; }

        /// <summary>
        /// 分拣数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 分拣人
        /// </summary>
        public string AdminID { get; set; }



        /// <summary>
        /// 创建时间(发生时间)
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 重量
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
        /// 体积
        /// </summary>
        public decimal? Volume { get; set; }



        #endregion

        #region 扩展属性

        public string AdminName
        {
            get
            {
                if (string.IsNullOrEmpty(this.AdminID))
                {
                    return "";
                }
                else
                {
                    return new AdminsView().Where(item => item.ID == this.AdminID).FirstOrDefault()?.RealName ?? "";
                }


            }
        }

        private CenterFileDescription[] fileInfos;

        public CenterFileDescription[] FileInfos
        {
            set { fileInfos = value; }
        }

        #endregion
    }
}
