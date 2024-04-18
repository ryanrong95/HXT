using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单接货信息
    /// </summary>
    [Serializable]
    public class OrderConsignee : IUnique, IPersist, IFulError, IFulSuccess
    {
        #region 属性
        string id;
        public string ID
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

        public string ClientSupplierID { get; set; }

        public ClientSupplier ClientSupplier { get; set; }

        /// <summary>
        /// 香港接货方式：供应商送货、自提
        /// </summary>
        public Enums.HKDeliveryType Type { get; set; }

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

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }
        #endregion

        #region 新增字段 库房那边信息
        public string CarrierID { get; set; }
        public string DriverID { get; set; }
        public string CarNumber { get; set; }
        #endregion

        public OrderConsignee()
        {
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
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
