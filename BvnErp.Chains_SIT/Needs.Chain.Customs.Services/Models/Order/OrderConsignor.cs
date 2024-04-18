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
    /// 代理订单交货信息
    /// </summary>
    [Serializable]
    public class OrderConsignor : IUnique, IPersist, IFulError, IFulSuccess
    {
        #region 属性
        string id;
        public string ID
        {
            get
            {
                //主键ID（OrderID）.MD5 -- 
                return this.id ?? string.Concat(this.OrderID).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public string OrderID { get; set; }

        /// <summary>
        /// 订单交货方式：自提、送货上门、快递
        /// </summary>
        public Enums.SZDeliveryType Type { get; set; }

        /// <summary>
        /// 收件单位名称（送货上门/快递时填写）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 联系人（送货上门/快递时），提货人（客户自提时）
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 联系人手机号码（送货上门/快递时），提货人手机号码（客户自提时）
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 联系人电话（送货上门/快递时），提货人电话（客户自提时），可空
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 收件地址（送货上门/快递时填写）
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 提货人证件类型：身份证、驾驶证（客户自提时填写）
        /// </summary>
        public string IDType { get; set; }

        /// <summary>
        /// 提货人证件号码（客户自提时填写）
        /// </summary>
        public string IDNumber { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }
        #endregion

        public OrderConsignor()
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
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignors>().Count(item => item.ID == this.ID);

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
