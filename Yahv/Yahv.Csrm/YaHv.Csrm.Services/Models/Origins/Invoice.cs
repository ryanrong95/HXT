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
    public class Invoice : Yahv.Linq.IUnique
    {
        public Invoice()
        {
            this.Status = ApprovalStatus.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }
        #region 属性
        string id;
        /// <summary>
        /// 唯一码
        /// </summary>
        virtual public string ID
        {
            get
            {
                return this.id ?? string.Join("",
                    this.EnterpriseID,
                    this.Type,
                    this.Bank,
                    this.BankAddress,
                    this.Account,
                    this.TaxperNumber
                    ).MD5();
            }
            set { this.id = value; }
        }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 企业电话
        /// </summary>
        public string CompanyTel { set; get; }
        /// <summary>
        /// 发票类型 1 普通发票 2 增值税发票 3 海关发票
        /// </summary>
        public InvoiceType Type { get; set; }
        /// <summary>
        /// 开户银行
        /// </summary>
        public string Bank { get; set; }
        /// <summary>
        /// 开户行地址
        /// </summary>
        public string BankAddress { get; set; }
        /// <summary>
        /// 银行账号
        /// </summary>
        public string Account { set; get; }
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxperNumber { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 联系人手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 联系人邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 收货地区
        /// </summary>
        public District District { set; get; }
        /// <summary>
        /// 省
        /// </summary>
        public string Province { set; get; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { set; get; }
        /// <summary>
        /// 地
        /// </summary>
        public string Land { set; get; }
        /// <summary>
        /// 收货地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string Postzip { set; get; }
        /// <summary>
        /// 交付方式
        /// </summary>
        public InvoiceDeliveryType DeliveryType { set; get; }

        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus Status { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; internal set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; internal set; }
        /// <summary>
        /// 公司基本信息
        /// </summary>
        public Enterprise Enterprise { set; get; }
        public string CreatorID { set; get; }
        /// <summary>
        /// 添加人
        /// </summary>
        public Admin Creator { get; internal set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }
        /// <summary>
        /// 发票地址
        /// </summary>
        public string InvoiceAddress { set; get; }
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
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Invoices>().Any(item => item.ID == this.ID))
                {
                    //修改
                    repository.Update<Layers.Data.Sqls.PvbCrm.Invoices>(new
                    {
                        Bank = this.Bank,
                        BankAddress = this.BankAddress,
                        Type = (int)this.Type,
                        Account = this.Account,
                        TaxperNumber = this.TaxperNumber,
                        District = (int)this.District,
                        Address = this.Address,
                        Postzip = this.Postzip,
                        Name = this.Name,
                        Tel = this.Tel,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        Status = (int)this.Status,
                        UpdateDate = this.UpdateDate,
                        Province = this.Province,
                        City = this.City,
                        Land = this.Land,
                        DeliveryType = (int)this.DeliveryType,
                        InvoiceAddress=this.InvoiceAddress
                    }, item => item.ID == this.ID);
                }
                else
                {
                    //录入
                    repository.Insert(this.ToLinq());
                    this.Creator = new Admin
                    {
                        ID = this.CreatorID
                    };
                    this.Creator.Binding(this.EnterpriseID, this.ID, MapsType.Invoice, IsDefault);
                }
                //Consignee录入联系人
                //联系人
                if (!string.IsNullOrWhiteSpace(this.Name))
                {
                    new Contact
                    {
                        EnterpriseID = this.EnterpriseID,
                        Type = ContactType.Pruchaser,
                        Name = this.Name,
                        Tel = this.Tel,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        Status = Services.Status.Normal,//联系人的状态
                        CreatorID = this.CreatorID
                    }.Enter();
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
                repository.Update<Layers.Data.Sqls.PvbCrm.Invoices>(new
                {
                    Status = ApprovalStatus.Deleted
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }

    public class WsInvoice : Invoice
    {
        string mapsid;
        public string MapsID
        {
            get
            {
                return "WsInvoice_" + this.Enterprise.ID;
            }

            set
            {
                this.mapsid = value;
            }
        }
        public override event SuccessHanlder AbandonSuccess;
        public override event SuccessHanlder EnterSuccess;
        public override void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                //发票是否存在
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.Invoices>().Any(item => item.ID == this.ID))
                {
                    this.UpdateDate = DateTime.Now;
                    repository.Update<Layers.Data.Sqls.PvbCrm.Invoices>(new
                    {
                        Bank = this.Bank,
                        CompanyTel = this.CompanyTel,
                        BankAddress = this.BankAddress,
                        Type = (int)this.Type,
                        Account = this.Account,
                        TaxperNumber = this.TaxperNumber,
                        District = (int)this.District,
                        Address = this.Address,
                        Postzip = this.Postzip,
                        Name = this.Name,
                        Tel = this.Tel,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        Status = (int)this.Status,
                        UpdateDate = this.UpdateDate,
                        Province = this.Province,
                        City = this.City,
                        Land = this.Land,
                        DeliveryType = (int)this.DeliveryType,
                        InvoiceAddress = this.InvoiceAddress
                    }, item => item.ID == this.ID);
                }
                else
                {
                    /* repository.Update<Layers.Data.Sqls.PvbCrm.Invoices>(new { Status = GeneralStatus.Deleted }, item => item.EnterpriseID == this.Enterprise.ID);*///只保留一个正常状态的发票
                    repository.Insert(this.ToLinq());
                }
                //是否默认
                //if (this.IsDefault)
                //{
                //    repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                //    {
                //        IsDefault = false
                //    }, item => item.EnterpriseID == this.EnterpriseID && item.Type == (int)MapsType.Invoices && item.Bussiness == (int)Business.WarehouseServicing);
                //}
                //关系是否存在
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == MapsID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                    {
                        IsDefault = this.IsDefault,
                        SubID = base.ID
                    }, item => item.ID == this.MapsID);
                }
                else
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
                    {
                        ID = this.MapsID,
                        Bussiness = (int)Business.WarehouseServicing,
                        Type = (int)MapsType.Invoice,
                        EnterpriseID = this.EnterpriseID,
                        SubID = this.ID,
                        CreateDate = DateTime.Now,
                        CtreatorID = this.CreatorID,
                        IsDefault = this.IsDefault
                    });
                }
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        //删除关系
        public override void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == this.MapsID))
                {
                    repository.Delete<Layers.Data.Sqls.PvbCrm.MapsBEnter>(item => item.ID == this.MapsID);
                }
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
    }

    /// <summary>
    /// 某业务服务关系中的发票（传统贸易）
    /// </summary>
    public class TradingInvoice : Invoice
    {
        public string MapsID
        {
            get
            {
                return string.Join("", Business.Trading, MapsType.Invoice, "_", base.ID, this.CreatorID).MD5();
            }

            set
            {
                base.ID = value;
            }
        }
        public override event SuccessHanlder AbandonSuccess;
        public override event SuccessHanlder EnterSuccess;
        public override void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                //发票是否存在
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.Invoices>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Invoices>(new
                    {
                        Bank = this.Bank,
                        CompanyTel = this.CompanyTel,
                        BankAddress = this.BankAddress,
                        Type = (int)this.Type,
                        Account = this.Account,
                        TaxperNumber = this.TaxperNumber,
                        District = (int)this.District,
                        Address = this.Address,
                        Postzip = this.Postzip,
                        Name = this.Name,
                        Tel = this.Tel,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        Status = (int)this.Status,
                        UpdateDate = this.UpdateDate,
                        Province = this.Province,
                        City = this.City,
                        Land = this.Land,
                        DeliveryType = (int)this.DeliveryType,
                        InvoiceAddress=this.InvoiceAddress

                    }, item => item.ID == this.ID);
                }
                else
                {
                    repository.Insert(this.ToLinq());
                }
                //传统贸易客户与发票的关系
                //是否默认
                if (this.IsDefault)
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                    {
                        IsDefault = false
                    }, item => item.EnterpriseID == this.EnterpriseID && item.Type == (int)MapsType.Invoice);
                }
                //关系是否存在
                if (!repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == MapsID))
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
                    {
                        ID = this.MapsID,
                        Bussiness = (int)Business.Trading,
                        Type = (int)MapsType.Invoice,
                        EnterpriseID = this.EnterpriseID,
                        SubID = this.ID,
                        CreateDate = DateTime.Now,
                        CtreatorID = this.CreatorID,
                        IsDefault = this.IsDefault
                    });
                }
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        //删除关系
        public override void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == this.MapsID))
                {
                    repository.Delete<Layers.Data.Sqls.PvbCrm.MapsBEnter>(item => item.ID == this.MapsID);
                }
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
    }
}