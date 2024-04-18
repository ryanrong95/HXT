using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Enums;
using Yahv.Services.Views;
using Yahv.Underly;

namespace Yahv.Services.Models
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
        /// 供应商
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
        /// 已发生的数量
        /// </summary>
        public decimal HappenedQuantity { get; set; }

        /// <summary>
        /// 已发生的数量
        /// </summary>
        public decimal UnHappenedQuantity { get { return this.Quantity - this.HappenedQuantity; } }

        /// <summary>
        /// 库存数量
        /// </summary>
        public decimal StockQuantity { get; set; }

        /// <summary>
        /// 剩余库存数量
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
        public CgNoticeSource Source { get; set; }
        
        /// <summary>
        /// 目标
        /// </summary>
        public NoticesTarget Target { get; set; }
        
        /// <summary>
        /// 箱号
        /// </summary>
        public string BoxCode { get; set; }

        /// <summary>
        /// 装箱日期
        /// </summary>
        public DateTime BoxDate { get; set; }
        
        /// <summary>
        /// 重量(毛重)
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

        public int? BoxingSpecs { get; set; }
        public Sorting Sorting { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }
        public string StorageID { get; set; }

        #region 扩展属性
        public CenterProduct Product { get; set; }
        #endregion

    }

    public class PickingNotice : Notice
    {      
        public Picking Picking { get; set; }
     
        public Output Output { get; set; }

        public CgPickingExcuteStatus ExcuteStatus { get; set; }

        /// <summary>
        /// 总件数
        /// </summary>
        public decimal TotalPieces { get; set; }

        public string ExcuteStatusDes
        {
            get
            {
                return this.ExcuteStatus.GetDescription();
            }
        }

        public string BoxingSpecsDescription
        {
            get
            {
                try
                {
                    if (this.BoxingSpecs == null)
                    {
                        return null;
                    }
                    return ((BoxingSpecs)this.BoxingSpecs).GetDescription();
                }
                catch
                {
                    return null;
                }
            }
        }
    }

    public class SortingNotice : Notice
    {

        public Storage Storage { get; set; }
    }
}
