using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services.Extends;

namespace YaHv.Csrm.Services.Models.Origins
{
    /// <summary>
    /// 发票信息
    /// </summary>
    public class vInvoice : Yahv.Linq.IUnique
    {
        public vInvoice()
        {
            this.Status = GeneralStatus.Normal;
            this.CreateDate = this.ModifyDate = DateTime.Now;
        }
        #region 属性
        string id;
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { set; get; }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 是否个人发票，否是企业发票
        /// </summary>
        public bool IsPersonal { set; get; }

        /// <summary>
        /// 发票类型 1 普通发票 2 增值税发票 3 海关发票
        /// </summary>
        public InvoiceType Type { get; set; }
        /// <summary>
        /// 发票抬头
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxNumber { set; get; }
        /// <summary>
        /// 企业注册地址
        /// </summary>
        public string RegAddress { set; get; }
        /// <summary>
        /// 企业电话
        /// </summary>
        public string Tel { set; get; }
        /// <summary>
        /// 开户银行
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankAccount { set; get; }
        /// <summary>
        /// 收票地址
        /// </summary>
        public string PostAddress { get; set; }
        /// <summary>
        /// 收票人
        /// </summary>
        public string PostRecipient { set; get; }
        /// <summary>
        /// 收票人联系电话
        /// </summary>
        public string PostTel { set; get; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string PostZipCode { set; get; }

        /// <summary>
        /// 交付方式
        /// </summary>
        public InvoiceDeliveryType DeliveryType { set; get; }

        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; internal set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime ModifyDate { get; internal set; }

        public string CreatorID { set; get; }
        /// <summary>
        /// 添加人
        /// </summary>
        public string CreatorRealName { get; internal set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }

        #endregion

        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        virtual public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        virtual public event SuccessHanlder AbandonSuccess;
        #endregion


        #region 持久化
        virtual public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (this.IsDefault)
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.vInvoices>(new
                    {
                        IsDefault = false
                    }, item => item.EnterpriseID == this.EnterpriseID);
                }
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.vInvoices>().Any(item => item.ID == this.ID))
                {
                    //修改
                    repository.Update<Layers.Data.Sqls.PvbCrm.vInvoices>(new
                    {
                        EnterpriseID = this.EnterpriseID,
                        IsPersonal = this.IsPersonal,
                        Type = (int)this.Type,
                        Title = this.Title,
                        TaxNumber = this.TaxNumber,
                        RegAddress = this.RegAddress,
                        Tel = this.Tel,
                        BankName = this.BankName,
                        BankAccount = this.BankAccount,
                        PostAddress = this.PostAddress,
                        PostRecipient = this.PostRecipient,
                        PostTel = this.PostTel,
                        PostZipCode = this.PostZipCode,
                        DeliveryType = (int)this.DeliveryType,
                        Status = (int)this.Status,
                        ModifyDate = this.CreateDate,
                        IsDefault = this.IsDefault
                    }, item => item.ID == this.ID);
                }
                else
                {
                    this.ID = PKeySigner.Pick(PKeyType.vInvoice);
                    //录入
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.vInvoices
                    {
                        ID = this.ID,
                        EnterpriseID = this.EnterpriseID,
                        IsPersonal = this.IsPersonal,
                        Type = (int)this.Type,
                        Title = this.Title,
                        TaxNumber = this.TaxNumber,
                        RegAddress = this.RegAddress,
                        Tel = this.Tel,
                        BankName = this.BankName,
                        BankAccount = this.BankAccount,
                        PostAddress = this.PostAddress,
                        PostRecipient = this.PostRecipient,
                        PostTel = this.PostTel,
                        PostZipCode = this.PostZipCode,
                        DeliveryType = (int)this.DeliveryType,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        ModifyDate = this.CreateDate,
                        CreatorID = this.CreatorID,
                        IsDefault = this.IsDefault
                    });

                }

                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        virtual public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.vInvoices>(new
                {
                    Status = GeneralStatus.Deleted
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }

}