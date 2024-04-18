using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using Needs.Utils.Converters;
using Needs.Wl.Models.Enums;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 报关订单的香港交货方式
    /// </summary>
    [Serializable]
    public class OrderConsignee : ModelBase<Layer.Data.Sqls.ScCustoms.OrderConsignees, ScCustomsReponsitory>, IUnique, IPersistence
    {
        #region 属性

        string id;
        public new string ID
        {
            get
            {
                //主键ID（OrderID）.MD5 -- 默认香港宏图接货
                return this.id ?? string.Concat(this.OrderID).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public string OrderID { get; set; }

        public ClientSupplier ClientSupplier { get; set; }

        /// <summary>
        /// 香港接货方式：供应商送货、上门自提
        /// </summary>
        public HKDeliveryType Type { get; set; }

        /// <summary>
        /// 联系人, 交货方式为自提时填写
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 联系人手机号码, 交货方式为自提时填写
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 联系人电话, 交货方式为自提时填写（可空）
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 交货地址, 交货方式为自提时填写
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 提货时间，交货方式为自提时填写
        /// </summary>
        public DateTime? PickUpTime { get; set; }

        /// <summary>
        /// 物流单号，供应商送货时填写
        /// </summary>
        public string WayBillNo { get; set; }

        #endregion

        public OrderConsignee()
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
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>().Count(item => item.ID == this.ID);
            if (count == 0)
            {
                this.Reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderConsignees
                {
                    ID = this.ID,
                    OrderID = this.OrderID,
                    ClientSupplierID = this.ClientSupplier.ID,
                    Type = (int)this.Type,
                    Contact = this.Contact,
                    Mobile = this.Mobile,
                    Tel = this.Tel,
                    Address = this.Address,
                    PickUpTime = this.PickUpTime,
                    WayBillNo = this.WayBillNo,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = DateTime.Now,
                    Summary = this.Summary
                });
            }
            else
            {
                this.Reponsitory.Update(new Layer.Data.Sqls.ScCustoms.OrderConsignees
                {
                    ID = this.ID,
                    OrderID = this.OrderID,
                    ClientSupplierID = this.ClientSupplier.ID,
                    Type = (int)this.Type,
                    Contact = this.Contact,
                    Mobile = this.Mobile,
                    Tel = this.Tel,
                    Address = this.Address,
                    PickUpTime = this.PickUpTime,
                    WayBillNo = this.WayBillNo,
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
    }
}