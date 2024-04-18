using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class Notice
    {
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 通知类型：入库通知、出库通知、分拣通知、检测通知、捡货通知、客户自提通知、装箱通知
        /// </summary>
        public CgNoticeType Type { get; set; }
        /// <summary>
        /// 仓库编号
        /// </summary>
        public string WareHouseID { get; set; }
        /// <summary>
        /// 运单编号
        /// </summary>
        public string WaybillID { get; set; }
        /// <summary>
        /// 进项编号
        /// </summary>
        public string InputID { get; set; }
        /// <summary>
        /// 销项编号
        /// </summary>
        public string OutputID { get; set; }
        /// <summary>
        /// 产品编号
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Supplier { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string DateCode { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public decimal StockQuantity { get; set; }


        /// <summary>
        /// 剩余数量
        /// </summary>
        public decimal SurplusQuantity { get { return this.StockQuantity - this.Quantity; } }


        /// <summary>
        /// 条件
        /// </summary>
        public NoticeCondition Conditions { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 状态：等待Waiting、关闭Closed、（货物）丢失Lost、完成Completed
        /// </summary>
        public NoticesStatus Status { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public NoticeSource Source { get; set; }
        /// <summary>
        /// 目标
        /// </summary>
        public NoticesTarget Target { get; set; }
        /// <summary>
        /// 箱号
        /// </summary>
        public string BoxCode { get; set; }
        /// <summary>
        /// 重量(毛重)
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
        /// 体积
        /// </summary>
        public decimal? Volume { get; set; }
        /// <summary>
        /// 货架编号
        /// </summary>
        public string ShelveID { get; set; }


        public CenterFileDescription[] Files { get; set; }

        /// <summary>
        /// 可见
        /// </summary>
        public bool Visable { get; set; } = true;

        /// <summary>
        /// 复核
        /// </summary>
        public bool Checked { get; set; } = false;


        public Input Input { get; set; }

        /// <summary>
        /// 深圳处理通知专用
        /// </summary>
        public string StorageID { get; set; }
        public string Origin { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        /// <remarks>
        /// 为内单展示特殊增加
        /// </remarks>
        public string CustomsName { get; set; }
    }
}
