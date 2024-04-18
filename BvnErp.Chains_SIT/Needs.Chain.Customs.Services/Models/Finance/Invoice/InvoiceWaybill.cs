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
    /// 发票运单
    /// </summary>
    public class InvoiceWaybill : IUnique, IFulError, IFulSuccess
    {
        #region 数据库属性

        private string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.InvoiceNotice.ID).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 开票通知
        /// </summary>
        public InvoiceNotice InvoiceNotice { get; set; }

        public string CompanyName { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        public string WaybillCode { get; set; }

        public DateTime CreateDate { get; set; }

        #endregion

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public InvoiceWaybill()
        {
            this.CreateDate = DateTime.Now;
        }
        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceWaybills>().Count(item => item.ID == this.ID);
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
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
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
