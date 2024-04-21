using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Wms.Services.Extends;
using Yahv.Linq;
using Yahv.Linq.Persistence;
using Yahv.Underly;
using Yahv.Usually;

namespace Wms.Services.Models
{
    public class WayChargos : IUnique, IPersisting
    {
        #region 事件
        //Enter成功
        public event SuccessHanlder WayChargoSuccess;
        //Enter失败
        public event ErrorHanlder WayChargoFailed;

        #endregion

        #region 属性

        /// <summary>
        /// 同Waybill.ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 代付货款、代收货款、忽略
        /// </summary>
        public Payer Payer { get; set; }

        /// <summary>
        /// 货款支付/收取方式
        /// </summary>
        public PayMethod PayMethod { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalPrice { get; set; }
        #endregion

        #region 扩展属性

        /// <summary>
        /// Payer的枚举描述
        /// </summary>
        public string PayerDes
        {
            get
            {
                return this.Payer.GetDescription();
            }
        }

        /// <summary>
        /// 货款支付/收取方式的枚举描述
        /// </summary>
        public string PayMethodDes
        {
            get
            {
                return this.PayMethod.GetDescription();
            }
        }

        /// <summary>
        /// 币种的枚举描述
        /// </summary>
        public string CurrencyDes
        {
            get
            {
                return this.Currency.GetDescription();
            }
        }
        #endregion

        #region 方法
        public void Enter()
        {
            try
            {
                using (var repository = new PvWmsRepository())
                {
                    repository.Insert(this.ToLinq());
                }
                this.WayChargoSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
            catch 
            {
                this.WayChargoFailed?.Invoke(this, new ErrorEventArgs("Failed!!"));
            }
           
        }

        public void Abandon()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
