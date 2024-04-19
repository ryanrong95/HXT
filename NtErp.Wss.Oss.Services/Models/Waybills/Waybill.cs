using Needs.Linq;
using NtErp.Wss.Oss.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Models
{
    public class WaybillKit
    {
        /// <summary>
        /// 承运商
        /// </summary>
        public string Carrier { get; set; }
        /// <summary>
        /// 重量 [标准单位:千克(kg)]
        /// </summary>
        public decimal Weight { get; set; }
    }

    /// <summary>
    /// 运单
    /// </summary>
    public class Waybill : WaybillKit, IUnique, IPersistence, IFulSuccess, IFulError
    {
        public Waybill()
        {

        }

        #region 属性

        public string ID { get; set; }
      
        #endregion

        #region 扩展属性

        WayItems items;
        /// <summary>
        /// 运单项
        /// </summary>
        public WayItems Items
        {
            get
            {
                return this.items;
            }
            internal set
            {
                this.items = value;
            }
        }

        #endregion

        #region 持久化
        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;

        public void Enter()
        {
            try
            {
                using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
                {
                    if (string.IsNullOrWhiteSpace(this.ID))
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.Waybill);
                        reponsitory.Insert(this.ToLinq());
                    }
                }

                Items.Enter();
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
            catch (Exception ex)
            {
                if (this != null && this.EnterError != null)
                {
                    this.EnterError(this, new ErrorEventArgs(ex.Message, ErrorType.System));
                }
            }
        }

        public void Abandon()
        {
            throw new Exception("This method does not support!");
        }

        #endregion
    }
}
