using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Wms.Services.chonggous.Models
{
    /// <summary>
    /// 通知类 ---重构
    /// </summary>
    public class CgNotice : IUnique
    {
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 通知类型：入库通知、出库通知、检测通知、拣货通知、装箱通知
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
        /// 原产地
        /// </summary>
        public string Origin { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }
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
        /// 通知的业务来源
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
        /// <summary>
        /// 装箱规格
        /// </summary>
        public int? BoxingSpecs { get; set; }

        public string StorageID { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        /// <remarks>
        /// 为内单展示特殊增加
        /// </remarks>
        public string CustomsName { get; set; }

        #endregion

        #region 扩展属性
        /// <summary>
        /// 库存数量
        /// </summary>
        public decimal StockQuantity { get; set; }

        /// <summary>
        /// 剩余数量
        /// </summary>
        public decimal SurplusQuantity { get { return this.StockQuantity - this.Quantity; } }

        /// <summary>
        /// 装箱日期
        /// </summary>
        public DateTime BoxDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CenterFileDescription[] Files { get; set; }

        /// <summary>
        /// 可见
        /// </summary>
        public bool Visable { get; set; } = true;

        /// <summary>
        /// 复核
        /// </summary>
        public bool Checked { get; set; } = false;

        /// <summary>
        /// 进项
        /// </summary>
        public CgLog_Declare Input { get; set; }

        /// <summary>
        /// 分拣
        /// </summary>
        public CgSorting Sorting { get; set; }

        /// <summary>
        /// 中心产品信息
        /// </summary>
        public CenterProduct Product { get; set; }

        /// <summary>
        /// 库存信息
        /// </summary>
        public CgStorage Storage { get; set; }

        /// <summary>
        /// 销项信息
        /// </summary>
        public Output Output { get; set; }

        #endregion

        public CgNotice()
        {
            this.Conditions = new NoticeCondition();
            this.Status = NoticesStatus.Waiting;
            this.CreateDate = DateTime.Now;
            this.Target = NoticesTarget.Default;
        }

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = new Layers.Data.Sqls.PvWmsRepository())
            {
                //保存产品数据到中心
                Yahv.Services.Views.ProductsTopView<Layers.Data.Sqls.PvWsOrderReponsitory>.Enter(this.Product);
                this.ProductID = this.Product.ID;
                //保存通知
                var Count = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(item => item.ID == this.ID).Count();
                if (Count == 0)
                {
                    this.ID = Layers.Data.PKeySigner.Pick(PkeyType.Notices);
                    reponsitory.Insert(new Layers.Data.Sqls.PvWms.Notices()
                    {
                        ID = this.ID,
                        Type = (int)this.Type,
                        WareHouseID = this.WareHouseID,
                        WaybillID = this.WaybillID,
                        InputID = this.InputID,
                        OutputID = this.OutputID,
                        ProductID = this.ProductID,
                        DateCode = this.DateCode,
                        Origin = this.Origin,
                        Quantity = this.Quantity,
                        Conditions = this.Conditions.Json(),
                        Status = (int)this.Status,
                        Source = (int)this.Source,
                        Target = (int)this.Target,
                        Weight = this.Weight,
                        NetWeight = this.NetWeight,
                        Volume = this.Volume,
                        BoxCode = this.BoxCode,
                        ShelveID = this.ShelveID,
                        BoxingSpecs = this.BoxingSpecs,
                        CreateDate = DateTime.Now,
                        StorageID = this.StorageID,
                    });
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvWms.Notices>(new
                    {
                        ID = this.ID,
                        Type = (int)this.Type,
                        WareHouseID = this.WareHouseID,
                        WaybillID = this.WaybillID,
                        InputID = this.InputID,
                        OutputID = this.OutputID,
                        ProductID = this.ProductID,
                        DateCode = this.DateCode,
                        Origin = this.Origin,
                        Quantity = this.Quantity,
                        Conditions = this.Conditions.Json(),
                        Status = (int)this.Status,
                        Source = (int)this.Source,
                        Target = (int)this.Target,
                        Weight = this.Weight,
                        NetWeight = this.NetWeight,
                        Volume = this.Volume,
                        BoxCode = this.BoxCode,
                        ShelveID = this.ShelveID,
                        BoxingSpecs = this.BoxingSpecs,
                        StorageID = this.StorageID,
                    }, item => item.ID == this.ID);
                }
            }
        }

        #endregion
    }
}
