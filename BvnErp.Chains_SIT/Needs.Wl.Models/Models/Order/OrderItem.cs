using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 代理订单项
    /// </summary>
    [Serializable]
    public class OrderItem : ModelBase<Layer.Data.Sqls.ScCustoms.OrderItems, ScCustomsReponsitory>, IUnique, IPersistence
    {
        #region 属性

        public string OrderID { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string Batch { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 已申报数量（分批申报时填写）
        /// DeclaredQuantity为空，表示该订单项未申报
        /// DeclaredQuantity小于Quantity，表示该订单项已经部分申报
        /// DeclaredQuantity等于Quantity，表示该订单项已申报
        /// </summary>
        public decimal? DeclaredQuantity { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? GrossWeight { get; set; }

        /// <summary>
        /// 是否抽检
        /// 对于四级客户，分拣时需要选择是否抽检
        /// </summary>
        public bool IsSampllingCheck { get; set; }

        /// <summary>
        /// 归类状态：未归类、首次归类、归类完成，归类异常
        /// </summary>
        public Enums.ClassifyStatus ClassifyStatus { get; set; }

        /// <summary>
        /// 客户物料号
        /// </summary>
        public string ProductUniqueCode { get; set; }

        /// <summary>
        /// 商检费
        /// </summary>
        public decimal? InspectionFee { get; set; }

        #endregion

        public OrderItem()
        {
            this.IsSampllingCheck = false;
            this.ClassifyStatus = Enums.ClassifyStatus.Unclassified;
            this.Status = (int)Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        /// <summary>
        /// 持久化
        /// </summary>
        public override void Enter()
        {
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Count(item => item.ID == this.ID);
            if (count == 0)
            {
                //主键ID（OrderItem +8位年月日+10位流水号）
                var prefix = System.Configuration.ConfigurationManager.AppSettings["Purchaser"];
                this.ID = prefix + Needs.Overall.PKeySigner.Pick(PKeyType.OrderItem);
                this.Reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderItems
                {
                    ID = this.ID,
                    OrderID = this.OrderID,
                    Name = this.Name,
                    Model = this.Model,
                    Manufacturer = this.Manufacturer,
                    Batch = this.Batch,
                    Origin = this.Origin,
                    Quantity = this.Quantity,
                    DeclaredQuantity = this.DeclaredQuantity,
                    Unit = this.Unit,
                    UnitPrice = this.UnitPrice,
                    TotalPrice = this.TotalPrice,
                    GrossWeight = this.GrossWeight,
                    IsSampllingCheck = this.IsSampllingCheck,
                    ClassifyStatus = (int)this.ClassifyStatus,
                    Status = (int)this.Status,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = this.Summary,
                    ProductUniqueCode = this.ProductUniqueCode,
                });
            }
            else
            {
                this.Reponsitory.Update(new Layer.Data.Sqls.ScCustoms.OrderItems
                {
                    ID = this.ID,
                    OrderID = this.OrderID,
                    Name = this.Name,
                    Model = this.Model,
                    Manufacturer = this.Manufacturer,
                    Batch = this.Batch,
                    Origin = this.Origin,
                    Quantity = this.Quantity,
                    DeclaredQuantity = this.DeclaredQuantity,
                    Unit = this.Unit,
                    UnitPrice = this.UnitPrice,
                    TotalPrice = this.TotalPrice,
                    GrossWeight = this.GrossWeight,
                    IsSampllingCheck = this.IsSampllingCheck,
                    ClassifyStatus = (int)this.ClassifyStatus,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = DateTime.Now,
                    Summary = this.Summary,
                    ProductUniqueCode = this.ProductUniqueCode,
                }, item => item.ID == this.ID);
            }

            this.OnEnterSuccess();
        }

        virtual protected void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public override void Abandon()
        {
            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);

            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}