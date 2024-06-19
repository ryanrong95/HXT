using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Npoi;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 客户信息
    /// </summary>
    //[Needs.Underly.FactoryView(typeof(Views.ClientsView))]
    [Serializable]
    public class Client : IUnique, IPersist
    {
        #region 属性和拓展
        string id;
        public string ID
        {
            get
            {
                return this.id ?? this.Company.ID.MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 客户类型
        /// </summary>
        public Enums.ClientType ClientType { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        public Company Company { get; set; }
        public string CompanyID { get; set; }
        /// <summary>
        /// 业务经理
        /// </summary>
        Admin serviceManager;
        public Admin ServiceManager
        {
            get
            {
                if (serviceManager == null)
                {
                    using (var view = new Views.ClientAdminsView())
                    {
                        this.ServiceManager = view.Where(item => item.ClientID == this.ID && item.Status == Enums.Status.Normal
                            && item.Type == Enums.ClientAdminType.ServiceManager).Select(item => new Models.Admin
                            {
                                ID = item.Admin.ID,
                                RealName = item.Admin.RealName,
                                Tel = item.Admin.Tel,
                                Email = item.Admin.Email,
                                Mobile = item.Admin.Mobile
                            }).SingleOrDefault();
                    }
                }
                return this.serviceManager;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                this.serviceManager = value;
            }
        }

        /// <summary>
        /// 跟单员
        /// </summary>
        Admin merchandiser;
        public Admin Merchandiser
        {
            get
            {
                if (merchandiser == null)
                {
                    using (var view = new Views.ClientAdminsView())
                    {
                        this.Merchandiser = view.Where(item => item.ClientID == this.ID && item.Status == Enums.Status.Normal
                            && item.Type == Enums.ClientAdminType.Merchandiser).Select(item => new Models.Admin
                            {
                                ID = item.Admin.ID,
                                RealName = item.Admin.RealName,
                                Tel = item.Admin.Tel,
                                Email = item.Admin.Email,
                                Mobile = item.Admin.Mobile
                            }).SingleOrDefault();
                    }
                }
                return this.merchandiser;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                this.merchandiser = value;
            }
        }


        /// <summary>
        /// 引荐人
        /// </summary>
        //Admin referrer;
        //public Admin Referrer
        //{
        //    get
        //    {
        //        if (referrer == null)
        //        {
        //            using (var view = new Views.ClientAdminsView())
        //            {
        //                this.Referrer = view.Where(item => item.ClientID == this.ID && item.Status == Enums.Status.Normal
        //                    && item.Type == Enums.ClientAdminType.Referrer).Select(item => new Models.Admin
        //                    {
        //                        ID = item.Admin.ID,
        //                        RealName = item.Admin.RealName,
        //                        Tel = item.Admin.Tel,
        //                        Email = item.Admin.Email,
        //                        Mobile = item.Admin.Mobile
        //                    }).SingleOrDefault();
        //            }

        //        }
        //        return referrer;
        //    }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            return;
        //        }
        //        this.referrer = value;

        //    }

        //}
        /// <summary>
        /// 引荐人1
        /// </summary>
        //Admin referrer1;
        //public Admin Referrer1
        //{
        //    get
        //    {
        //        if (referrer == null)
        //        {
        //            this.Referrer1 = null;

        //        }
        //        return referrer1;
        //    }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            return;
        //        }
        //        this.referrer1 = value;

        //    }

        //}

        /// <summary>
        /// 客户编号/入仓号
        /// </summary>
        public string ClientCode { get; set; }

        /// <summary>
        /// 客户等级
        /// </summary>
        public Enums.ClientRank ClientRank { get; set; }

        private ClientFiles files;

        /// <summary>
        /// 附件
        /// </summary>
        public ClientFiles Files
        {
            get
            {
                if (files == null)
                {
                    using (var view = new Views.ClientFilesView())
                    {
                        var query = view.Where(t => t.ClientID == this.ID && t.Status == Enums.Status.Normal);
                        this.files = new ClientFiles(query);
                    }
                }
                return this.files;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                this.Files = value;
            }
        }

        /// <summary>
        /// 发票信息
        /// </summary>
        public ClientInvoice Invoice
        {
            get
            {
                using (var view = new Views.ClientInvoicesView())
                {
                    return view.Where(item => item.ClientID == this.ID && item.Status == Enums.Status.Normal).SingleOrDefault();
                }
            }
        }

        /// <summary>
        /// 发票收件地址
        /// </summary>
        public ClientInvoiceConsignee InvoiceConsignee
        {
            get
            {
                using (var view = new ClientInvoiceConsigneesView())
                {
                    return view.Where(item => item.ClientID == this.ID && item.Status == Enums.Status.Normal).SingleOrDefault();
                }
            }
        }

        /// <summary>
        /// 合同条款
        /// </summary>
        public ClientAgreement Agreement
        {
            get
            {
                using (var view = new Views.ClientAgreementsView())
                {
                    return view.Where(item => item.ClientID == this.ID && item.Status == Enums.Status.Normal).SingleOrDefault();
                }
            }
        }

        /// <summary>
        /// 供应商
        /// </summary>
        public Views.ClientSuppliers Suppliers
        {
            get
            {
                return new Views.ClientSuppliers(this);
            }
        }

        /// <summary>
        /// 会员账号
        /// </summary>
        public Views.ClientUsersView Users
        {
            get
            {
                return new Views.ClientUsersView(this);
            }
        }

        /// <summary>
        /// 客户信息添加人\操作人
        /// </summary>
        public Admin Admin { get; set; }

        public string AdminID { get; set; }
        /// <summary>
        /// 客户状态
        /// </summary>
        public Enums.ClientStatus ClientStatus { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public int? ClientNature { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public ServiceType ServiceType { get; set; }

        /// <summary>
        /// 代报关的客户信息是否已完善
        /// </summary>
        public bool? IsValid { get; set; }
        /// <summary>
        /// 代仓储的客户信息是否已完善
        /// </summary>

        public bool? IsStorageValid { get; set; }
        /// <summary>
        /// 代仓储客户类型
        /// </summary>
        public StorageType StorageType { get; set; }
        /// <summary>
        ///新添加的
        /// </summary>

        public ClientFile ClientFile { get; set; }
        /// <summary>
        /// 是否重新指定业务员
        /// </summary>
        public bool? IsSpecified { get; set; }

        /// <summary>
        /// 注册年，用于订单利润核算
        /// </summary>
        public string RegisterYear
        {
            get
            {
                return this.CreateDate.Year.ToString();
            }
        }

        public string IsModify
        {
            get
            {

                return System.Configuration.ConfigurationManager.AppSettings["AEO"];
            }

        }

        /// <summary>
        /// 人员的部门 DepartmentCode
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// 是否收取入仓费
        /// </summary>
        public ChargeWHType ChargeWH { get; set; }

        /// <summary>
        /// 是否是会员端编辑数据
        /// </summary>
        public bool IsApi { get; set; }
        /// <summary>
        /// 是否正常
        /// </summary>
        public bool IsNormal { get; set; }


        /// <summary>
        /// 是否合格客户 --新加字段2022-10-27
        /// </summary>
        public bool? IsQualified { get; set; }

        /// <summary>
        /// 评估日期
        /// </summary>
        public DateTime? AssessDate { get; set; }

        /// <summary>
        /// 是否可以下载海关缴款书
        /// </summary>
        public bool? IsDownloadDecTax { get; set; }

        /// <summary>
        /// 允许下载缴款书 宽限日期
        /// </summary>
        public string DecTaxExtendDate { get; set; }

        /// <summary>
        /// 是否可以申请开票
        /// </summary>
        public bool? IsApplyInvoice { get; set; }

        /// <summary>
        /// 申请开票 宽限日期
        /// </summary>
        public string InvoiceExtendDate { get; set; }

        public string Referrer { get; set; }
        /// <summary>
        ///收取方式
        /// </summary>
        public ChargeType? ChargeType { get; set; }

        /// <summary>
        /// 收取金额
        /// </summary>
        public decimal? AmountWH { get; set; }

        /// <summary>
        /// 超过90天未换汇美金金额
        /// </summary>
        public decimal? UnPayExchangeAmount { get; set; }

        /// <summary>
        /// 超过120天未换汇美金金额
        /// </summary>
        public decimal? UnPayExchangeAmount4M { get; set; }

        /// <summary>
        /// 近3个月内的报关金额
        /// </summary>
        public decimal? DeclareAmount { get; set; }

        /// <summary>
        /// 近3个月内的付汇金额
        /// </summary>
        public decimal? PayExchangeAmount { get; set; }

        /// <summary>
        /// 近1个月内的报关金额
        /// </summary>
        public decimal? DeclareAmountMonth { get; set; }

        /// <summary>
        /// 近1个月内的付汇金额
        /// </summary>
        public decimal? PayExchangeAmountMonth { get; set; }

        public Department Department { get; set; }
        #endregion
        /// <summary>
        /// 当客户等级改变时发生
        /// </summary>
        public event Hanlders.ClientRankChangedHanlder RankChanged;

        public Client()
        {
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.RankChanged += Client_RankChanged;
            this.IsValid = false;
            this.IsStorageValid = false;
            this.StorageType = StorageType.Unknown;
            this.IsApi = false;
            this.IsNormal = true;
            this.IsQualified = false;
            this.IsDownloadDecTax = true;
            this.IsApplyInvoice = true;
        }

        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;

        public void Enter()
        {
            this.Company.Enter();

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    var company = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>().FirstOrDefault(item => item.Name == this.Company.Name);
                    this.Company.ID = company.ID;
                    reponsitory.Insert(this.ToLinq());
                    if (this.Admin != null)
                    {
                        if (!IsApi)
                        {
                            this.Log("[" + this.Admin?.RealName + "]新增会员，默认[" + this.Admin.RealName + "]为业务员");
                        }
                        //新增会员，业务员默认为新增的人

                        ClientAdmin clientAdmin = new ClientAdmin();
                        clientAdmin.Type = Enums.ClientAdminType.ServiceManager;
                        clientAdmin.Admin = this.Admin;
                        clientAdmin.ClientID = this.ID;
                        clientAdmin.Summary = "新增会员";
                        clientAdmin.Enter();
                    }
                    var client = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>().FirstOrDefault(item => item.ID == this.ID);
                    if (client.ID != null && this.Admin != null)
                    {
                        var contact = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Contacts>().FirstOrDefault(item => item.ID == company.ContactID);

                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.Users
                        {
                            ID = Needs.Overall.PKeySigner.Pick(PKeyType.User),
                            AdminID = this.Admin.ID,
                            RealName = "",
                            ClientID = client.ID,
                            Email = contact.Email,
                            Mobile = contact.Mobile,
                            Name = company.Name,
                            Password = Needs.Utils.Converters.StringExtend.StrToMD5("HXT123"),
                            IsMain = true,
                            Status = (int)this.Status,
                            CreateDate = this.CreateDate,
                            UpdateDate = DateTime.Now,
                            Summary = this.Summary
                        });
                    }
                    //this.SetServiceManager(this.Admin, "新增会员");                    
                }
                else
                {
                    var logchange = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>().Where(item => item.ID == this.ID).Select(item => new { item.ClientRank }).FirstOrDefault().ClientRank;
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.Clients
                    {
                        ID = this.ID,
                        CompanyID = this.Company.ID,
                        ClientType = (int)this.ClientType,
                        ClientRank = (int)this.ClientRank,
                        ClientCode = this.ClientCode,
                        //AdminID = entity.Admin?.ID,
                        ClientStatus = (int)this.ClientStatus,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = DateTime.Now,
                        Summary = this.Summary,
                        ServiceType = (int)this.ServiceType,
                        ClientNature = this.ClientNature,
                        IsValid = this.IsValid,
                        IsStorageValid = this.IsStorageValid,
                        StorageType = (int)this.StorageType,
                        ChargeWH = (int)this.ChargeWH,
                        ChargeType = this.ChargeType != null ? (int?)this.ChargeType : null,
                        AmountWH = this.AmountWH,
                        IsNormal = this.IsNormal,
                        IsDownloadDecTax = this.IsDownloadDecTax,
                        DecTaxExtendDate = this.DecTaxExtendDate,
                        IsApplyInvoice = this.IsApplyInvoice,
                        InvoiceExtendDate = this.InvoiceExtendDate
                    }, item => item.ID == this.ID);
                    if (logchange != (int)this.ClientRank)
                    {
                        ChangeRank((Enums.ClientRank)logchange, this.ClientRank);
                    }
                    else
                    {
                        if (this.Admin != null)
                        {
                            if (!IsApi)
                            {
                                this.Log("操作人员[" + this.Admin.RealName + "]编辑会员");
                            }
                        }
                    }

                }
            }

            this.OnEnter();
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        #region 设置业务员和跟单
        /// <summary>
        /// 分配业务员
        /// </summary>
        /// <param name="admin"></param>
        public void SetServiceManager(Admin admin, string Summary)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                ClientAdmin clientAdmin = new ClientAdmin();
                clientAdmin.Type = Enums.ClientAdminType.ServiceManager;
                clientAdmin.Admin = admin;
                clientAdmin.ClientID = this.ID;
                clientAdmin.Summary = Summary;
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>().Count(item => item.ID == clientAdmin.ID);
                if (count == 0)
                {
                    this.Log("总经理[" + "张庆永" + "]分配了业务员:" + admin.RealName);
                }
                clientAdmin.Enter();
            }

        }


        /// <summary>
        /// 业务员认领客户
        /// </summary>
        /// <param name="admin"></param>
        public void ClientSetServiceManager(Admin admin, string Summary)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                ClientAdmin clientAdmin = new ClientAdmin();
                clientAdmin.Type = Enums.ClientAdminType.ServiceManager;
                clientAdmin.Admin = admin;
                clientAdmin.ClientID = this.ID;
                clientAdmin.Summary = Summary;
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>().Count(item => item.ID == clientAdmin.ID);
                if (count == 0)
                {
                    this.Admin = admin;
                    this.Log("业务员[" + this.Admin.RealName + "]认领了客户:" + this.Company.Name);
                }
                clientAdmin.Enter();
            }

        }

        #region  分配人员

        /// <summary>
        /// 分配跟单员
        /// </summary>
        /// <param name="admin"></param>
        public void SetMerchandiser(Admin admin, string Summary = null)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                ClientAdmin clientAdmin = new ClientAdmin();
                clientAdmin.Type = Enums.ClientAdminType.Merchandiser;
                clientAdmin.Admin = admin;
                clientAdmin.ClientID = this.ID;
                clientAdmin.Summary = Summary;
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>().Count(item => item.ID == clientAdmin.ID);
                if (count == 0)
                {
                    this.Log("总经理[" + "张庆永" + "]分配了跟单员:" + admin.RealName);
                }
                clientAdmin.Enter();
            }
        }

        /// <summary>
        /// 分配业务员
        /// </summary>
        /// <param name="admin"></param>
        public void SetServicer(Admin admin, Admin ServicerManager)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                ClientAdmin clientAdmin = new ClientAdmin();
                clientAdmin.Type = Enums.ClientAdminType.ServiceManager;
                clientAdmin.Admin = ServicerManager;
                clientAdmin.ClientID = this.ID;
                clientAdmin.Summary = "重新指定了业务员";

                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { IsSpecified = true }, item => item.ID == this.ID);

                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>().Count(item => item.ID == clientAdmin.ID);
                if (count == 0)
                {
                    this.Log("总经理[" + Admin.RealName + "]指定了业务员:" + serviceManager.RealName);
                }
                clientAdmin.Enter();

            }
        }


        public void SetReferrer(string referrer, string Summary = null)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {

                if (!string.IsNullOrEmpty(referrer))
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { Referrer = referrer }, item => item.ID == this.ID);

                if (!string.IsNullOrEmpty(referrer))
                {
                    this.Log("总经理[" + this.Admin.RealName + "]添加了引荐人:" + referrer);
                }

                //ClientAdmin clientAdmin = new ClientAdmin();
                //clientAdmin.Type = Enums.ClientAdminType.Referrer;
                //clientAdmin.Admin = admin;
                //clientAdmin.ClientID = this.ID;
                //clientAdmin.Summary = Summary;
                //int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>().Count(item => item.ID == clientAdmin.ID);
                //if (count == 0)
                //{
                //    this.Log("总经理[" + this.Admin.RealName + "]添加了引荐人:" + admin?.RealName);
                //}
                //clientAdmin.Enter();
            }
        }

        public void SetNormal(bool isNormal)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { IsNormal = isNormal }, item => item.ID == this.ID);
            }
        }
        #endregion

        #endregion

        #region 事件和日志

        /// <summary>
        /// 修改客户等级
        /// </summary>
        public void ChangeRank(Enums.ClientRank oldrank, Enums.ClientRank newrank)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { ClientRank = (int)newrank }, item => item.ID == this.ID);
            }

            //this.OnRankChanged(oldrank, newrank);
        }

        public void ChangeNature(Enums.ClientNature oldrank, Enums.ClientNature newrank)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { ClientNature = (int)newrank }, item => item.ID == this.ID);
            }
        }

        virtual protected void OnRankChanged(Enums.ClientRank oldrank, Enums.ClientRank newrank)
        {
            if (this != null && this.RankChanged != null)
            {
                this.RankChanged(this, new Hanlders.ClientRankChangedEventArgs(this, oldrank, newrank));
            }
        }

        private void Client_RankChanged(object sender, Hanlders.ClientRankChangedEventArgs e)
        {
            var client = e.Client;
            this.Log("操作人员[" + this.Admin.RealName + "]编辑会员,且将会员等级从[" + e.OldRank.GetDescription() + "]更改到[" + e.NewRank.GetDescription() + "]");
        }
        #endregion

        #region 方法
        /// <summary>
        /// TODO：此处需后期优化
        /// 会员信息全部完善
        /// 可以线上下单
        /// </summary>
        public void Confirm(string Summary = null)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var message = string.Empty;
                int count = 0;
                if (this.ServiceType == ServiceType.Both || this.ServiceType == ServiceType.Customs)
                {
                    //补充协议
                    count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>().Count(item => item.ClientID == this.ID
                     && item.Status == (int)Enums.Status.Normal);
                    if (count == 0)
                    {
                        message += "未添加补充协议 ";
                        //this.EnterError(this, new ErrorEventArgs("未添加补充协议"));
                    }

                    //开票信息
                    count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientInvoices>().Count(item => item.ClientID == this.ID
                     && item.Status == (int)Enums.Status.Normal);
                    if (count == 0)
                    {
                        message += "未添加开票信息 ";
                        //this.EnterError(this, new ErrorEventArgs("未添加开票信息"));
                    }
                }

                if (string.IsNullOrEmpty(message))
                {
                    this.SetServiceManager(this.ServiceManager, Summary);
                    this.SetMerchandiser(this.Merchandiser, Summary);
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { ClientStatus = Enums.ClientStatus.Confirmed, Referrer = this.Referrer }, item => item.ID == this.ID);
                    this.OnEnter();
                }
                else
                {
                    this.EnterError(this, new Linq.ErrorEventArgs(message));
                }
            }
        }




        public string ClientConfirm(string Summary = null)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var message = string.Empty;
                int count = 0;


                //补充协议
                count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>().Count(item => item.ClientID == this.ID
                 && item.Status == (int)Enums.Status.Normal);
                if (count == 0)
                {
                    message += "未添加补充协议 ";
                    //this.EnterError(this, new ErrorEventArgs("未添加补充协议"));
                }

                //开票信息
                count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientInvoices>().Count(item => item.ClientID == this.ID
                 && item.Status == (int)Enums.Status.Normal);
                if (count == 0)
                {
                    message += "未添加开票信息 ";
                    //this.EnterError(this, new ErrorEventArgs("未添加开票信息"));
                }

                if (string.IsNullOrEmpty(message))
                {
                    this.SetServiceManager(this.ServiceManager, Summary);
                    this.SetMerchandiser(this.Merchandiser, Summary);
                    //if (this.Referrer != null)
                    //{
                    //    this.SetReferrer(this.Referrer, Summary);
                    //}
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { ClientStatus = Enums.ClientStatus.Confirmed, Referrer = this.Referrer }, item => item.ID == this.ID);
                    this.OnEnter();
                }
                return message;
            }
        }


        /// <summary>
        /// 提交申请
        /// </summary>
        public void Submit(string admin)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                #region  代报关协议
                if (this.ServiceType == ServiceType.Customs)
                {

                    var message = string.Empty;
                    int count = 0;

                    var agreement = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>().Where(x => x.ClientID == this.ID && x.Status == (int)Enums.Status.Normal);
                    //补充协议
                    count = agreement.Count();
                    if (count == 0)
                    {
                        message += "未添加补充协议 ";
                    }

                    //开票信息
                    count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientInvoices>().Count(item => item.ClientID == this.ID
                     && item.Status == (int)Enums.Status.Normal);
                    if (count == 0)
                    {
                        message += "未添加开票信息 ";
                    }
                    //会员账号是否添加
                    count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>().Count(x => x.ClientID == this.ID && x.Status == (int)Enums.Status.Normal);
                    if (count == 0)
                    {
                        message += "未添加会员账号信息 ";
                    }

                    if (string.IsNullOrEmpty(message))
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { ClientStatus = Enums.ClientStatus.Verifying }, item => item.ID == this.ID);
                        this.OnEnter();
                    }
                    else
                    {
                        this.EnterError(this, new Linq.ErrorEventArgs(message));
                        return;
                    }
                    // 若未添加协议附件，系统自动生成协议草书，并上传中心
                    #region  若未添加协议附件，系统自动生成协议草书，并上传中心
                    var serviceFile = new CenterFilesTopView().FirstOrDefault(x => x.ClientID == id && x.Type == (int)Needs.Ccs.Services.Enums.FileType.ServiceAgreement);
                    if (serviceFile == null)
                    {
                        var entity = new ClientAgreementsView()[agreement.FirstOrDefault().ID];
                        //创建文件夹
                        var fileName = DateTime.Now.Ticks + "服务协议草书.docx";
                        FileDirectory file = new FileDirectory(fileName);
                        file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                        file.CreateDataDirectory();
                        //保存文件
                        entity.SaveAs(file.FilePath);
                        //上传中心
                        var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.ServiceAgreement;
                        var ErmAdminID = admin;
                        var dic = new { CustomName = fileName, ClientID = this.ID, AdminID = ErmAdminID };
                        var uploadFile = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(FileDirectory.Current.FilePath + @"\" + file.VirtualPath, centerType, dic);
                    }
                    //  this.Log("业务员[" + this.serviceManager.RealName + "]已提交：" + this.Company.Name);
                    #endregion

                }
                #endregion

                #region 待仓储协议
                //if (this.ServiceType == ServiceType.Warehouse)
                //{

                //    var message = string.Empty;
                //    var count = new CenterFilesTopView().Count(x => x.ClientID == this.ID && x.Type == (int)FileType.StorageAgreement && x.Status != FileDescriptionStatus.Delete);
                //    if (count==0)
                //    {
                //        message += "未上传香港本地协议 ";
                //    }
                //    if (string.IsNullOrEmpty(message))
                //    {
                //        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { ClientStatus = Enums.ClientStatus.WaitingApproval }, item => item.ID == this.ID);
                //        this.OnEnter();
                //    }
                //    else
                //    {
                //        this.EnterError(this, new Linq.ErrorEventArgs(message));
                //        return;
                //    }




                //}
                #endregion
                if (this.ServiceType == ServiceType.Both)
                {
                    #region  双服务

                    var message = string.Empty;
                    int count = 0;

                    // count = new CenterFilesTopView().Count(x => x.ClientID == this.ID && x.Type == (int)FileType.StorageAgreement && x.Status != FileDescriptionStatus.Delete);
                    //if (count == 0)
                    //{
                    //    message += "未上传香港本地协议 ";
                    //}

                    var agreement = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>().Where(x => x.ClientID == this.ID && x.Status == (int)Enums.Status.Normal);
                    //补充协议
                    count = agreement.Count();
                    if (count == 0)
                    {
                        message += "未添加补充协议 ";
                    }

                    //开票信息
                    count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientInvoices>().Count(item => item.ClientID == this.ID
                     && item.Status == (int)Enums.Status.Normal);
                    if (count == 0)
                    {
                        message += "未添加开票信息 ";
                    }
                    //会员账号是否添加
                    count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>().Count(x => x.ClientID == this.ID && x.Status == (int)Enums.Status.Normal);
                    if (count == 0)
                    {
                        message += "未添加会员账号信息 ";
                    }

                    if (string.IsNullOrEmpty(message))
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { ClientStatus = Enums.ClientStatus.Verifying }, item => item.ID == this.ID);
                        this.OnEnter();
                    }
                    else
                    {
                        this.EnterError(this, new Linq.ErrorEventArgs(message));
                        return;
                    }

                    //  this.Log("业务员[" + this.Admin.RealName + "]已提交：" + this.Company.Name);

                    // 若未添加协议附件，系统自动生成协议草书，并上传中心
                    #region  若未添加协议附件，系统自动生成协议草书，并上传中心
                    var serviceFile = new CenterFilesTopView().FirstOrDefault(x => x.ClientID == id && x.Type == (int)Needs.Ccs.Services.Enums.FileType.ServiceAgreement);
                    if (serviceFile == null)
                    {
                        var entity = new ClientAgreementsView()[agreement.FirstOrDefault().ID];
                        //创建文件夹
                        var fileName = DateTime.Now.Ticks + "服务协议草书.docx";
                        FileDirectory file = new FileDirectory(fileName);
                        file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                        file.CreateDataDirectory();
                        //保存文件
                        entity.SaveAs(file.FilePath);
                        //上传中心
                        var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.ServiceAgreement;
                        var ErmAdminID = admin;
                        var dic = new { CustomName = fileName, ClientID = this.ID, AdminID = ErmAdminID };
                        var uploadFile = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(FileDirectory.Current.FilePath + @"\" + file.VirtualPath, centerType, dic);
                    }

                    #endregion


                    #endregion

                }
                this.Log("业务员[" + this.serviceManager.RealName + "]已提交：" + this.Company.Name);

            }
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        public void Audit()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var message = string.Empty;
                try
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { ClientStatus = Enums.ClientStatus.WaitingApproval }, item => item.ID == this.ID);

                    //this.Log("风控人员[" + this.Admin.RealName + "]审核通过：" + this.Company.Name);
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientControls
                    {
                        ID = ChainsGuid.NewGuidUp(),
                        ClientID = this.ID,
                        CompanyID = this.Company.ID,
                        ClientType = (int)this.ClientType,
                        ClientCode = this.ClientCode,
                        ClientRank = (int)this.ClientRank,
                        ClientStatus = (int)ClientControlStatus.Audited,
                        Status = (int)this.Status,
                        AdminID = this.Admin.ID,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary
                    });

                    this.OnEnter();
                }
                catch (Exception ex)
                {

                    message = ex.ToString();
                    this.EnterError(this, new Linq.ErrorEventArgs(message));
                }
            }
        }


        /// <summary>
        /// 审核拒绝
        /// </summary>
        public void Refuse()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var message = string.Empty;
                try
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { ClientStatus = Enums.ClientStatus.Returned, Summary = this.Summary }, item => item.ID == this.ID);
                    //var RealName = this.Admin.RealName;
                    //if (this.Admin.RealName == "张令金" || this.Admin.RealName == "张庆永")
                    //{
                    //    RealName = "殷菲";
                    //}
                    this.Log("风控人员[" + this.Admin.RealName + "]风控审核不通过：" + this.Company.Name + ",备注：" + this.Summary);
                    ///审批退回的记录插入到客户风控表中
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientControls
                    {
                        ID = ChainsGuid.NewGuidUp(),
                        ClientID = this.ID,
                        CompanyID = this.Company.ID,
                        ClientType = (int)this.ClientType,
                        ClientCode = this.ClientCode,
                        ClientRank = (int)this.ClientRank,
                        ClientStatus = (int)ClientControlStatus.RefuseAudit,
                        Status = (int)this.Status,
                        AdminID = this.Admin.ID,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary
                    });
                    this.OnEnter();
                }
                catch (Exception ex)
                {

                    message = ex.ToString();
                    this.EnterError(this, new Linq.ErrorEventArgs(message));
                }
            }

        }

        /// <summary>
        /// 审批通过
        /// </summary>
        public void Approve(string referrersID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var message = string.Empty;
                try
                {
                    //设置跟单员
                    this.SetMerchandiser(this.Merchandiser);
                    //添加引荐人
                    if (!string.IsNullOrEmpty(referrersID))
                    {
                        this.SetReferrer(this?.Referrer);

                    }
                    switch (this.ServiceType)
                    {
                        case ServiceType.Customs:
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { ClientStatus = Enums.ClientStatus.Confirmed, IsValid = true }, item => item.ID == this.ID);
                            this.IsValid = true;
                            break;
                        case ServiceType.Warehouse:
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { ClientStatus = Enums.ClientStatus.Confirmed, IsStorageValid = true }, item => item.ID == this.ID);
                            this.IsStorageValid = true;
                            break;
                        case ServiceType.Both:
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { ClientStatus = Enums.ClientStatus.Confirmed, IsStorageValid = true, IsValid = true }, item => item.ID == this.ID);
                            this.IsStorageValid = true; this.IsValid = true;
                            break;
                        default:
                            break;
                    }
                    // if(this.StorageType==StorageType.Domestic)

                    //if (this.Admin != null)
                    //{
                    //    var RealName = this.Admin.RealName;
                    //    if (this.Admin.RealName == "张令金")
                    //    {
                    //        RealName = "张庆永";
                    //    }
                    //    this.Log("总经理[" + RealName + "]审批通过：" + this.Company.Name);
                    //}

                    this.Log("总经理[张庆永]审批通过：" + this.Company.Name);

                    this.OnEnter();
                }
                catch (Exception ex)
                {

                    message = ex.ToString();
                    this.EnterError(this, new Linq.ErrorEventArgs(message));
                }
            }

        }

        /// <summary>
        /// 审批拒绝
        /// </summary>
        public void ApprovalRefused(string summary)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var message = string.Empty;
                try
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { ClientStatus = Enums.ClientStatus.Returned, Summary = summary }, item => item.ID == this.ID);
                    //var RealName = this.Admin.RealName;
                    //if (this.Admin.RealName == "张令金")
                    //{
                    //    RealName = "张庆永";
                    //}
                    this.Log("总经理[张庆永]审批不通过：" + this.Company.Name + ",备注:" + summary);
                    this.OnEnter();
                }
                catch (Exception ex)
                {

                    message = ex.ToString();
                    this.EnterError(this, new Linq.ErrorEventArgs(message));
                }
            }

        }

        /// <summary>
        /// 设置服务协议未上交
        /// </summary>
        public void SetSAUnUpload()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new
                {
                    IsSAUpload = (int)Enums.SAUploadStatus.UnUpload,
                }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 设置服务协议已上交
        /// </summary>
        public void SetSAUploaded()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new
                {
                    IsSAUpload = (int)Enums.SAUploadStatus.Uploaded,
                }, item => item.ID == this.ID);
            }
        }



        /// <summary>
        ///  代仓储协议
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveStorageAgreement(string filePath)
        {
            XWPFDocument doc = this.ToStorageWord();
            FileStream file = new FileStream(filePath, FileMode.Create);
            doc.Write(file);
            file.Close();
        }

        public XWPFDocument ToStorageWord()
        {
            var tempPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\templates\\香港本地交货协议.docx");
            var npoi = new NPOIHelper(tempPath);
            var replaceText = new Dictionary<string, string>();
            string partB = "";
            switch (this.StorageType)
            {

                case Needs.Ccs.Services.Enums.StorageType.Domestic:
                    partB = "深圳市芯达通供应链管理有限公司";
                    break;
                case Needs.Ccs.Services.Enums.StorageType.HKCompany:
                    partB = "香港畅运国际物流有限公司";
                    break;
                case Needs.Ccs.Services.Enums.StorageType.Person:
                    partB = "深圳市芯达通供应链管理有限公司";
                    break;
                default:
                    break;
            }
            #region 填充内容
            replaceText.Add("{PartHeadB}", partB);
            replaceText.Add("{PartA}", this.Company.Name);
            replaceText.Add("{PartB}", partB);
            replaceText.Add("{PartEndA}", this.Company.Name);
            replaceText.Add("{PartEndB}", partB);
            replaceText.Add("{StartDate}", this.CreateDate.ToString("yyyy年MM月dd日"));
            replaceText.Add("{EndDate}", this.CreateDate.AddYears(2).ToString("yyyy年MM月dd日"));
            #endregion


            return npoi.GenerateWordByTempletePlus(replaceText);
        }



        /// <summary>
        /// 合格客户设置
        /// </summary>
        public void SetQualified()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { IsQualified = this.IsQualified }, item => item.ID == this.ID);
            }
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }


        /// <summary>
        /// 修改评估日期
        /// </summary>
        public void SetAssessDate()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { AssessDate = this.AssessDate }, item => item.ID == this.ID);
            }
        }

        #endregion

    }
}
