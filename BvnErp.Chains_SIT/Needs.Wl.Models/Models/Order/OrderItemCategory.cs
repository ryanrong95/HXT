using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using Needs.Utils.Converters;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 代理订单项商品归类
    /// </summary>
    public class OrderItemCategory : ModelBase<Layer.Data.Sqls.ScCustoms.OrderItems, ScCustomsReponsitory>, IUnique, IPersistence
    {
        #region 属性

        string id;
        public new string ID
        {
            get
            {
                //主键ID（OrderItemID）.MD5 -- 一个OrderItem对应一条归类信息
                //最好使用OrderItemID
                return this.id ?? string.Concat(this.OrderItemID).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public string OrderItemID { get; set; }

        /// <summary>
        /// 产品归类：普通，商检，3C，原产地证明、禁运、检疫等
        /// </summary>
        public Enums.ItemCategoryType Type { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 海关编码\商品编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 申报要素
        /// </summary>
        public string Elements { get; set; }

        /// <summary>
        /// 法定第一单位
        /// </summary>
        public string Unit1 { get; set; }

        /// <summary>
        /// 法定第二单位（可空）
        /// </summary>
        public string Unit2 { get; set; }

        /// <summary>
        /// 法定第一数量（必填）
        /// </summary>
        public decimal? Qty1 { get; set; }

        /// <summary>
        /// 法定第二数量（非必填）
        /// </summary>
        public decimal? Qty2 { get; set; }

        /// <summary>
        /// 检验检疫编码
        /// </summary>
        public string CIQCode { get; set; }

        /// <summary>
        /// 预归类一人员
        /// </summary>
        public Admin Admin1 { get; set; }

        /// <summary>
        /// 预归类二人员
        /// </summary>
        public Admin Admin2 { get; set; }

        #endregion

        public OrderItemCategory()
        {
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
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>().Count(item => item.ID == this.ID);

            if (count == 0)
            {
                this.Reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderItemCategories
                {
                    ID = this.ID,
                    OrderItemID = this.OrderItemID,
                    ClassifyFirstOperator = this.Admin1.ID,
                    ClassifySecondOperator = this.Admin2.ID,
                    Type = (int)this.Type,
                    TaxCode = this.TaxCode,
                    TaxName = this.TaxName,
                    HSCode = this.HSCode,
                    Unit1 = this.Unit1,
                    Unit2 = this.Unit2,
                    Name = this.Name,
                    Elements = this.Elements,
                    Qty1 = this.Qty1,
                    Qty2 = this.Qty2,
                    CIQCode = this.CIQCode,
                    Status = (int)this.Status,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = this.Summary
                });
            }
            else
            {
                this.Reponsitory.Update(new Layer.Data.Sqls.ScCustoms.OrderItemCategories
                {
                    ID = this.ID,
                    OrderItemID = this.OrderItemID,
                    ClassifyFirstOperator = this.Admin1.ID,
                    ClassifySecondOperator = this.Admin2.ID,
                    Type = (int)this.Type,
                    TaxCode = this.TaxCode,
                    TaxName = this.TaxName,
                    HSCode = this.HSCode,
                    Unit1 = this.Unit1,
                    Unit2 = this.Unit2,
                    Name = this.Name,
                    Elements = this.Elements,
                    Qty1 = this.Qty1,
                    Qty2 = this.Qty2,
                    CIQCode = this.CIQCode,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = DateTime.Now,
                    Summary = this.Summary
                }, item => item.ID == this.ID);
            }

            this.OnEnterSuccess();
        }

        virtual public void OnEnterSuccess()
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
            this.Reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderItemCategories>(item => item.ID == this.ID);

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