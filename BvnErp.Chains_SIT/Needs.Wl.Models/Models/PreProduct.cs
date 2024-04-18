using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 预归类产品
    /// </summary>
    public class PreProduct : ModelBase<Layer.Data.Sqls.ScCustoms.Companies, ScCustomsReponsitory>, IUnique, IPersist
    {
        #region 属性

        public string ClientID { get; set; }

        /// <summary>
        /// 物料号
        /// </summary>
        public string ProductUnionCode { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal? Qty { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        public string Supplier { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNo { get; set; }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public PreProduct()
        {
            this.Status = (int)Enums.Status.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        /// <summary>
        /// 数据删除触发事件
        /// </summary>
        public override void Abandon()
        {
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                if (this != null && this.AbandonError != null)
                {
                    this.AbandonError(this, new ErrorEventArgs("主键ID不能为空！"));
                }
            }

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProducts>(new
                {
                    Status = Enums.Status.Delete
                }, item => item.ID == this.ID);
            }

            this.OnAbandon();
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        virtual protected void OnAbandon()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public override void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var oldID = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProducts>().Where(item => item.ProductUnionCode == this.ProductUnionCode).FirstOrDefault();

                //判断是否为新增
                if (oldID == null)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.PreProducts
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        ClientID = this.ClientID,
                        ProductUnionCode = this.ProductUnionCode,
                        Model = this.Model,
                        Manufacturer = this.Manufacturer,
                        Qty = this.Qty,
                        Price = this.Price,
                        Currency = this.Currency,
                        Supplier = this.Supplier,
                        BatchNo = this.BatchNo,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    this.ID = oldID.ID;
                }
            }

            this.OnEnter();
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}