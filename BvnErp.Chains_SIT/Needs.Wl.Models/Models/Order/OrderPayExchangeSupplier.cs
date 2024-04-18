using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using Needs.Utils.Converters;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 下单时的付汇供应商
    /// </summary>
    [Serializable]
    public class OrderPayExchangeSupplier : ModelBase<Layer.Data.Sqls.ScCustoms.OrderPayExchangeSuppliers, ScCustomsReponsitory>, IUnique, IPersistence
    {
        #region 属性

        string id;
        public new string ID
        {
            get
            {
                //主键ID（OrderID+ClientSupplierID）.MD5
                return this.id ?? string.Concat(this.OrderID, this.ClientSupplierID).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 付汇供应商ID
        /// </summary>
        public string ClientSupplierID { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 付汇供应商中文名称
        /// </summary>
        public string ChineseName { get; set; }

        #endregion

        public OrderPayExchangeSupplier()
        {
            this.Status = (int)Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 持久化
        /// </summary>
        public override void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPayExchangeSuppliers>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderPayExchangeSuppliers
                    {
                        ID = this.ID,
                        OrderID = this.OrderID,
                        ClientSupplierID = this.ClientSupplierID,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = DateTime.Now,
                        Summary = this.Summary
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.OrderPayExchangeSuppliers
                    {
                        ID = this.ID,
                        OrderID = this.OrderID,
                        ClientSupplierID = this.ClientSupplierID,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = DateTime.Now,
                        Summary = this.Summary
                    }, item => item.ID == this.ID);
                }
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
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderPayExchangeSuppliers>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

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
