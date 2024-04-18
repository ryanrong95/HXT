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
    /// 客户发票
    /// </summary>
    [Serializable]
    public class ClientInvoice : IUnique, IPersist
    {
        string id;
        public string ID
        {
            get
            {
                //根据发票及客户信息确认发票的唯一性
                return this.id ?? string.Concat(this.ClientID, this.Title, this.TaxCode, this.Address, this.Tel, this.BankName, this.BankAccount).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public string ClientID { get; set; } = string.Empty;

        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxCode { get; set; } = string.Empty;

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; } = string.Empty;

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; } = string.Empty;

        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankAccount { get; set; } = string.Empty;

        internal Admin Admin;

        public void SetAdmin(Admin admin)
        {
            Admin = admin;
        }

        /// <summary>
        /// 发票交付方式
        /// </summary>
        public Enums.InvoiceDeliveryType DeliveryType { get; set; }

        /// <summary>
        /// 发票日志状态：1 已标注，0 未标注
        /// 用于通知开票人员客户发票信息修改，注意修改开票系统的缓存信息
        /// </summary>
        public Enums.ClientInvoiceStatus InvoiceStatus { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; } = string.Empty;

        public ClientInvoice()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
            this.InvoiceStatus = Enums.ClientInvoiceStatus.UnMarked;
        }

        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        //protected virtual void OnEnter()
        //{
        //    if (this != null && this.EnterSuccess != null)
        //    {
        //        //成功后触发事件
        //        this.EnterSuccess(this, new SuccessEventArgs(this));
        //    }
        //}
        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientInvoices>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    //新增数据及代表修改了发票信息，需要通知到开票人员在系统中提示客户发票信息已经修改。
                    //注意修改开票系统的缓存信息。
                    //将客户的发票日志信息的status变更为 Enums.Status.Delete
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientInvoices>(new { Status = (int)Enums.Status.Delete }, item => item.ClientID == this.ClientID);
                    try
                    {
                        reponsitory.Insert(this.ToLinq());
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                   
                }
                else
                {
                    try
                    {
                        reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                   
                }
            }

            this.OnEnter();
        }

        private Models.ClientInvoice _oldClientInvoice;

        public Models.ClientInvoice OldClientInvoice
        {
            get { return _oldClientInvoice; }
        }

        private Models.Client _logClient;

        public Models.Client LogClient
        {
            get { return _logClient; }
        }

        public void Enter(Models.ClientInvoice oldClientInvoice, Models.Client logClient)
        {
            this._oldClientInvoice = oldClientInvoice;
            this._logClient = logClient;
            this.Enter();
        }
    }
}