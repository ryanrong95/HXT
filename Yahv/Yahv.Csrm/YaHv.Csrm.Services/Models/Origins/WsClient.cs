using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Npoi;
using YaHv.Csrm.Services.Extends;

namespace YaHv.Csrm.Services.Models.Origins
{
    public class WsClient : Yahv.Linq.IUnique
    {
        public WsClient()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.WsClientStatus = ApprovalStatus.UnComplete;
        }
        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;
        /// <summary>
        /// 状态异常
        /// </summary>
        public event ErrorHanlder StatusUnnormal;
        #endregion

        #region 属性
        public string ID { set; get; }
        /// <summary>
        /// 内部公司
        /// </summary>
        public Enterprise Company { set; get; }
        /// <summary>
        /// 基本信息
        /// </summary>
        public Enterprise Enterprise { set; get; }
        /// <summary>
        /// 客户性质
        /// </summary>
        public ClientType Nature { set; get; }
        /// <summary>
        /// 等级
        /// </summary>
        public ClientGrade Grade { set; get; }
        /// <summary>
        /// 是否Vip
        /// </summary>
        public bool Vip { set; get; }
        /// <summary>
        /// 入仓号
        /// </summary>
        public string EnterCode { set; get; }
        /// <summary>
        /// 海关编码
        /// </summary>
        public string CustomsCode { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus WsClientStatus { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        public string CreatorID { set; get; }
        /// <summary>
        /// 国家、地区，（Enum.Origin的简称）
        /// </summary>
        public string Place { set; get; }
        /// <summary>
        /// 录入人信息
        /// </summary>

        public Admin Admin { internal set; get; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { set; get; }
        public DateTime UpdateDate { set; get; }

        /// <summary>
        /// 是否收取入仓费
        /// </summary>
        public ChargeWHType ChargeWHType { get; set; }
        string mapsid;
        public string MapsID
        {
            get
            {
                return string.Join("", this.Company.ID, this.Enterprise.ID).MD5();
            }
            set
            {
                this.mapsid = value;
            }
        }
        /// <summary>
        /// 服务类型
        /// </summary>
        public ServiceType ServiceType { set; get; }
        /// <summary>
        /// 代仓储审批
        /// </summary>
        public bool IsStorageService { set; get; }
        /// <summary>
        /// 代报关审批
        /// </summary>
        public bool IsDeclaretion { set; get; }
        /// <summary>
        /// 客户身份
        /// </summary>
        public WsIdentity StorageType { set; get; }
        #endregion

        #region 扩展(logo,营业执照、业务员、跟单员、发票，到货地址，联系人,付款人)
        #region 付款人
        Views.Rolls.PayersRoll payers;
        /// <summary>
        /// 付款人
        /// </summary>
        public Views.Rolls.PayersRoll Payers
        {
            get
            {
                if (payers == null)
                {
                    using (var view = new Views.Rolls.PayersRoll(this.Enterprise.ID))
                    {
                        return payers = view;
                    }
                }
                else
                {
                    return payers;
                }
            }
            set
            {

                this.payers = value;
            }
        }
        #endregion

        #region 企业Log
        CenterFileDescription logo;
        /// <summary>
        /// 企业Logo
        /// </summary>
        public CenterFileDescription Logo
        {
            get
            {
                if (logo == null)
                {
                    using (var view = new Views.Rolls.CenterFiles(FileType.EnterpriseLogo, this.ID))
                    {
                        return view.FirstOrDefault();
                    }
                }
                else
                {
                    return logo;
                }
            }
            set
            {

                this.logo = value;
            }
        }
        #endregion

        #region 营业执照
        //FileDescription businessLicense;
        ///// <summary>
        ///// 营业执照
        ///// </summary>
        //public FileDescription BusinessLicense
        //{
        //    get
        //    {
        //        if (businessLicense == null)
        //        {
        //            using (var view = new Views.Rolls.FilesRoll(this.Enterprise))
        //            {
        //                return businessLicense = view.FirstOrDefault(item => item.Type == FileType.BusinessLicense && item.Status == ApprovalStatus.Normal);
        //            }
        //        }
        //        else
        //        {
        //            return businessLicense;
        //        }
        //    }
        //    set
        //    {

        //        this.businessLicense = value;
        //    }
        //}
        /// <summary>
        /// 中心库-营业执照
        /// </summary>
        CenterFileDescription businesslicense;
        public CenterFileDescription BusinessLicense
        {
            get
            {
                if (this.businesslicense == null)
                {
                    this.businesslicense = new Views.Rolls.CenterFiles(FileType.BusinessLicense, this.ID).FirstOrDefault();
                }
                return this.businesslicense;
            }
            set
            {
                this.businesslicense = value;
            }
        }
        CenterFileDescription hkbusinesslicense;
        public CenterFileDescription HKBusinessLicense
        {
            get
            {
                if (this.hkbusinesslicense == null)
                {
                    this.hkbusinesslicense = new Views.Rolls.CenterFiles(FileType.HKBusinessLicense, this.ID).FirstOrDefault();
                }
                return this.hkbusinesslicense;
            }
            set
            {
                this.businesslicense = value;
            }
        }
        /// <summary>
        /// 代仓储协议
        /// </summary>
        CenterFileDescription storageAgreement;
        public CenterFileDescription StorageAgreement
        {
            get
            {
                if (this.storageAgreement == null)
                {
                    this.storageAgreement = new Views.Rolls.CenterFiles(FileType.StorageAgreement, this.ID).FirstOrDefault();
                }
                return this.storageAgreement;
            }
            set
            {
                this.storageAgreement = value;
            }
        }
        #endregion

        #region 业务员
        Admin serviceManager;

        /// <summary>
        /// 业务员
        /// </summary>
        public Admin ServiceManager
        {
            get
            {
                using (var view = new Views.Rolls.TrackerAdmin(MapsID, MapsType.ServiceManager))
                {
                    return view.FirstOrDefault();
                }
            }
            set
            {
                this.serviceManager = value;
            }
        }
        #endregion

        #region 跟单员
        /// <summary>
        /// 跟单员
        /// </summary>
        Admin merchandiser;
        public Admin Merchandiser
        {
            get
            {
                using (var view = new Views.Rolls.TrackerAdmin(MapsID, MapsType.Merchandiser))
                {
                    return view.FirstOrDefault();
                };
            }
            set
            {
                this.merchandiser = value;
            }
        }

        #endregion

        #region 引荐人
        /// <summary>
        /// 引荐人
        /// </summary>
        Admin referrer;
        public Admin Referrer
        {
            get
            {
                using (var view = new Views.Rolls.TrackerAdmin(MapsID, MapsType.Referrer))
                {
                    return view.FirstOrDefault();
                };
            }
            set
            {
                this.referrer = value;
            }
        }
        #endregion

        #region  代仓储客户的默认发票信息
        /// <summary>
        /// /// <summary>
        ///
        /// </summary>
        /// </summary>
        Invoice invoice;
        public Invoice Invoice
        {
            get
            {
                using (var view = new Views.Rolls.WsInvoicesRoll(this.Enterprise))
                {
                    return view.FirstOrDefault(item => item.IsDefault == true);
                };
            }
            set
            {
                this.invoice = value;
            }
        }
        #endregion

        #region 客户的默认联系人
        WsContact contact;
        /// <summary>
        /// 客户的默认联系人
        /// </summary>
        public WsContact Contact
        {
            get
            {
                using (var view = new Views.Rolls.WsContactsRoll(this.Enterprise))
                {
                    return view.FirstOrDefault(item => item.IsDefault == true);
                };
            }
            set
            {
                this.contact = value;
            }
        }
        #endregion




        #region 服务协议

        ///// <summary>
        ///// 合同信息
        ///// </summary>
        Contract contract;
        public Contract Contract
        {
            get
            {
                using (var view = new Views.Rolls.ContractsRoll(this.Enterprise, this.Company.ID))
                {
                    return view.Where(item => item.Status == GeneralStatus.Normal).FirstOrDefault();
                }
            }
            set
            {
                this.contract = value;
            }
        }
        #endregion

        #region 代仓储合同信息
        WsContract wscontract;
        /// <summary>
        /// 代仓储合同信息
        /// </summary>
        /// //WsContract wscontract;
        public WsContract WsContract
        {
            get
            {
                using (var view = new Views.Rolls.WsContractsRoll(this.Enterprise.ID))
                {
                    return view.FirstOrDefault(item => item.Status == GeneralStatus.Normal);
                }
            }
            set
            {
                this.wscontract = value;
            }
        }

        #endregion

        #region 私有供应商
        //Views.Rolls.XdtWsSuppliersView wssuppliers;
        ///// <summary>
        ///// 代仓储客户的供应商
        ///// </summary>
        //public Views.Rolls.XdtWsSuppliersView WsSuppliers
        //{
        //    get
        //    {
        //        if (this.wssuppliers == null || this.wssuppliers.Disposed)
        //        {
        //            this.wssuppliers = new Views.Rolls.XdtWsSuppliersView(this.Enterprise);
        //        }
        //        return this.wssuppliers;
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        Views.Rolls.nSuppliersRoll nsuppliers;
        public Views.Rolls.nSuppliersRoll nSuppliers
        {
            get
            {
                if (this.nsuppliers == null || this.nsuppliers.Disposed)
                {
                    this.nsuppliers = new Views.Rolls.nSuppliersRoll(this.Enterprise.ID);
                }
                return this.nsuppliers;
            }
        }
        #endregion


        #region 客户会员信息
        /// <summary>
        /// 客户会员信息
        /// </summary>
        Views.Rolls.SiteUsersXdtRoll siterusers;
        public Views.Rolls.SiteUsersXdtRoll SiteUsers
        {
            get
            {
                if (this.siterusers == null || this.siterusers.Disposed)
                {
                    this.siterusers = new Views.Rolls.SiteUsersXdtRoll(this.Enterprise);
                }
                return this.siterusers;
            }
        }
        #endregion



        #region 客户的所有到货地址
        /// <summary>
        /// 客户的所有到货地址
        /// </summary>
        Views.Rolls.WsConsigneesRoll consignees;
        public Views.Rolls.WsConsigneesRoll Consignees
        {
            get
            {
                if (this.consignees == null || this.consignees.Disposed)
                {
                    this.consignees = new Views.Rolls.WsConsigneesRoll(this.Enterprise);
                }
                return this.consignees;
            }
        }
        #endregion



        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                this.Enterprise.Enter();

                var client = new Views.Origins.WsClientsOrigin()[this.Enterprise.ID];
                //关系不存在
                if (client == null)
                {
                    //1.WsClients存在
                    if (repository.GetTable<Layers.Data.Sqls.PvbCrm.WsClients>().Any(item => item.ID == this.Enterprise.ID))
                    {
                        this.MapsCompany();
                        repository.Update<Layers.Data.Sqls.PvbCrm.WsClients>(new
                        {
                            ServiceType = (int)this.ServiceType,
                            Place = this.Place,
                            Grade = (int)this.Grade,
                            Nature = (int)this.Nature,
                            Vip = this.Vip,
                            EnterCode = this.EnterCode,
                            CustomsCode = this.CustomsCode,
                            UpdateDate = this.UpdateDate,
                            Summary = this.Summary,
                            Status = (int)this.WsClientStatus,
                            IsDeclaretion = this.IsDeclaretion,
                            IsStorageService = this.IsStorageService,
                            StorageType = (int)this.StorageType,
                            ChargeWH = this.ChargeWHType
                        }, item => item.ID == this.ID);
                        if (this != null && this.EnterSuccess != null)
                        {
                            this.EnterSuccess(this, new SuccessEventArgs(this));
                        }
                    }
                    //2.WsClients不存在
                    else
                    {
                        this.ID = this.Enterprise.ID;
                        #region 入仓号
                        if (this.EnterCode == "WL")
                        {
                            this.EnterCode = PKeySigner.Pick(PKeyType.WL);
                        }
                        else if (this.EnterCode == "XL")
                        {
                            this.EnterCode = PKeySigner.Pick(PKeyType.XL);
                        }
                        //else if (this.EnterCode == "ICG")
                        //{
                        //    this.EnterCode = PKeySigner.Pick(PKeyType.ICGO);
                        //}
                        else
                        {
                            this.EnterCode = this.EnterCode;
                        }
                        #endregion
                        repository.Insert(this.ToLinq());
                        //和内部公司关系
                        this.MapsCompany();
                        ////业务员或业务员负责人
                        if (!string.IsNullOrWhiteSpace(this.CreatorID))
                        {
                            var serviceM = new Views.Rolls.AdminsAllRoll()[this.CreatorID];
                            if (serviceM.RoleID == FixedRole.ServiceManager.GetFixedID() || serviceM.RoleID == FixedRole.ServiceManagerLeader.GetFixedID())
                            {
                                this.Assin(this.CreatorID, MapsType.ServiceManager);
                            }
                        }

                        if (this != null && this.EnterSuccess != null)
                        {
                            this.EnterSuccess(this, new SuccessEventArgs(this));
                        }
                    }
                }
                //关系存在
                else
                {
                    if (string.IsNullOrWhiteSpace(this.ID))
                    {
                        this.WsClientStatus = client.WsClientStatus;
                        if (this != null && this.StatusUnnormal != null)
                        {
                            this.StatusUnnormal(this, new ErrorEventArgs());
                        }
                    }
                    else
                    {
                        repository.Update<Layers.Data.Sqls.PvbCrm.WsClients>(new
                        {
                            ServiceType = (int)this.ServiceType,
                            Place = this.Place,
                            Grade = (int)this.Grade,
                            Nature = (int)this.Nature,
                            Vip = this.Vip,
                            EnterCode = this.EnterCode,
                            CustomsCode = this.CustomsCode,
                            UpdateDate = this.UpdateDate,
                            Summary = this.Summary,
                            Status = (int)this.WsClientStatus,
                            IsDeclaretion = this.IsDeclaretion,
                            IsStorageService = this.IsStorageService,
                            StorageType = (int)this.StorageType,
                            ChargeWH = this.ChargeWHType
                        }, item => item.ID == this.ID);

                        if (this != null && this.EnterSuccess != null)
                        {
                            this.EnterSuccess(this, new SuccessEventArgs(this));
                        }
                    }
                }


            }
        }
        public void Abandon()
        {
            //using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            //{
            //    repository.Update<Layers.Data.Sqls.PvbCrm.WsClients>(new
            //    {
            //        Status = ApprovalStatus.Deleted
            //    }, item => item.ID == this.ID);
            //    if (this != null && this.AbandonSuccess != null)
            //    {
            //        this.AbandonSuccess(this, new SuccessEventArgs(this));
            //    }
            //}
            ///删除关系
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == this.MapsID))
                {
                    repository.Delete<Layers.Data.Sqls.PvbCrm.MapsBEnter>(item => item
                    .ID == this.MapsID);
                }
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        string gettrackerid(MapsType type, string newid)
        {
            return string.Join("",
                     this.Company.ID,
                     newid,
                     Business.WarehouseServicing,
                     type
                 ).MD5();
        }
        string getbenterrid(string newid)
        {
            return string.Join("", this.Company.ID, newid).MD5();
        }
        /// <summary>
        /// 仅芯达通网站用户修改企业ID是使用
        /// </summary>
        /// <param name="oldname"></param>
        /// <returns></returns>
        public bool UpdateEnterpriseID(string newid, string newname = null)
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                bool result = false;
                if (string.IsNullOrWhiteSpace(newid))
                {
                    newid = repository.GetTable<Layers.Data.Sqls.PvbCrm.Enterprises>().First(item => item.Name == newname).ID;
                }
                var newenterprise = new Views.Rolls.EnterprisesRoll()[newid];
                if (newenterprise != null)
                {

                    using (var tsql = new PvbCrmReponsitory().TSql)
                    {
                        tsql.Update<Layers.Data.Sqls.PvbCrm.WsClients>(new
                        {
                            ID = newid
                        }, item => item.ID == this.Enterprise.ID);

                        tsql.Update<Layers.Data.Sqls.PvbCrm.SiteUsersXdt>(new
                        {
                            EnterpriseID = newid
                        },
                            item => item.EnterpriseID == this.Enterprise.ID);

                        string mapid = getbenterrid(newid);//企业关系（客户与深圳市芯达通的关系）ID

                        //MapsBEnter 和MapsTracker
                        string old_sid = gettrackerid(MapsType.ServiceManager, this.Enterprise.ID);//业务员
                        if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsTracker>().Any(item => item.ID == old_sid))
                        {
                            string new_sid = gettrackerid(MapsType.ServiceManager, newid);
                            tsql.Update<Layers.Data.Sqls.PvbCrm.MapsTracker>(new
                            {
                                ID = new_sid,
                                RealID = mapid
                            },
                                         item => item.ID == old_sid);
                        }
                        string old_mid = gettrackerid(MapsType.Merchandiser, this.Enterprise.ID);//跟单员
                        if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsTracker>().Any(item => item.ID == old_mid))
                        {
                            string new_mid = gettrackerid(MapsType.Merchandiser, newid);
                            tsql.Update<Layers.Data.Sqls.PvbCrm.MapsTracker>(new
                            {
                                ID = new_mid,
                                RealID = mapid
                            },
                                         item => item.ID == old_mid);
                        }
                        var mapstracker = repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsTracker>().Where(item => item.RealID == this.Enterprise.ID).ToArray();
                        //企业关系
                        var mapsbenter = repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().FirstOrDefault(item => item.EnterpriseID == this.Enterprise.ID && item.Type == (int)MapsType.WsClient);
                        if (mapsbenter != null)
                        {
                            tsql.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
                            {
                                ID = mapid,
                                EnterpriseID = newid
                            },
                           item => item.ID == mapsbenter.ID);
                        }
                    }
                    result = true;
                }
                return result;
            }
        }
        #endregion


        #region 模板导出
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void SaveAs(string filePath, bool toHtml = false)
        {
            var tempPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "templates\\服务协议模板(书签).docx");
            Aspose.Words.Document doc = new Aspose.Words.Document(tempPath);//新建一个空白的文档
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);

            Dictionary<string, string> dic = ContractDic();

            foreach (var key in dic.Keys)   //循环键值对
            {
                builder.MoveToBookmark(key);  //将光标移入书签的位置
                builder.Write(dic[key]);   //填充值
            }
            DeleteFolder(AppDomain.CurrentDomain.BaseDirectory + @"\Files\Dowload\Contracts");
            if (toHtml)
            {
                doc.Save(filePath, Aspose.Words.SaveFormat.Html);//保存html
            }
            else
            {
                doc.Save(filePath); //保存word
            }
        }

        void DeleteFolder(string dir)
        {
            foreach (string d in System.IO.Directory.GetFileSystemEntries(dir))
            {
                if (System.IO.File.Exists(d))
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = System.IO.FileAttributes.Normal;
                    System.IO.File.Delete(d);//直接删除其中的文件  
                }
                else
                {
                    System.IO.DirectoryInfo d1 = new System.IO.DirectoryInfo(d);
                    if (d1.GetFiles().Length != 0)
                    {
                        DeleteFolder(d1.FullName);////递归删除子文件夹
                    }
                    System.IO.Directory.Delete(d);
                }
            }
        }
        public Dictionary<string, string> ContractDic()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();   //创建键值对   第一个string 为书签名称 第二个string为要填充的数据
            #region 组装内容
            dic.Add("ClientName", this.Enterprise.Name);
            dic.Add("ClientName1", this.Enterprise.Name);
            dic.Add("A_RegAddress", this.Enterprise.RegAddress);
            dic.Add("A_Corporation", this.Enterprise.Corporation);

            dic.Add("MinimumAgent", this.Contract.MinAgencyFee.ToString());
            dic.Add("ServiceChargePoint", this.Contract.AgencyRate.ToString());


            dic.Add("StartDate", this.Contract.StartDate.ToString("yyyy年MM月dd日"));
            dic.Add("StartDate1", this.Contract.StartDate.ToString("yyyy年MM月dd日"));
            dic.Add("EndDate", this.Contract.EndDate.ToString("yyyy年MM月dd日"));
            dic.Add("EndDate1", this.Contract.EndDate.ToString("yyyy年MM月dd日"));
            dic.Add("Grade", this.Grade.GetDescription());

            //开票点位
            dic.Add("InvoicePoint", (this.Contract.InvoiceTaxRate + 1).ToString());
            dic.Add("InvoicePoint1", (this.Contract.InvoiceTaxRate + 1).ToString());

            //dic.Add("ExchangeMode", this.Contract.ExchangeMode.GetDescription());
            //换汇
            dic.Add("GoodsPayExchange90", this.Contract.ExchangeMode == ExchangeMode.LimitNinetyDays ? "☑" : "□");
            dic.Add("GoodsPayExchangePre", this.Contract.ExchangeMode == ExchangeMode.PrePayExchange ? "☑" : "□");
            //乙方信息：内部公司
            string regaddress = string.IsNullOrWhiteSpace(this.Company.RegAddress) ? "" : this.Company.RegAddress;
            string corporation = string.IsNullOrWhiteSpace(this.Company.Corporation) ? "" : this.Company.Corporation;
            dic.Add("B_CompanyName", this.Company.Name);
            dic.Add("B_CompanyName1", this.Company.Name);
            dic.Add("B_OfficeAddress", regaddress);
            dic.Add("B_RegAddress", regaddress);
            dic.Add("B_Corporation", corporation);

            //开票
            dic.Add("InvoiceType", this.Contract.InvoiceType == BillingType.Full ? "☑" : "□");
            dic.Add("InvoiceTypeService", this.Contract.InvoiceType == BillingType.Service ? "☑" : "□");

            #region 计算年份
            int year = this.Contract.EndDate.Year - this.Contract.StartDate.Year;
            var y = "";
            string[] parm = new string[] { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖", "拾" };
            if (year < 10 || year == 10)
            {
                y = parm[year];
            }
            else if (year < 100)
            {
                y = year / 10 == 1 ? "" : parm[year / 10] + '拾' + parm[year % 10];
            }

            dic.Add("Years", y);
            #endregion

            var debtTerms = new YaHv.Csrm.Services.Views.Rolls.DebtTermsTopView();      //账期条款
            var credits = new YaHv.Csrm.Services.Views.Rolls.CreditsStatisticsView();   //信用批复
            string payer = this.Company.ID;
            string payee = this.ID;

            #region 货款
            var goods = debtTerms[payer, payee, "货款"];

            dic.Add("GoodsIsMonth", goods?.SettlementType == SettlementType.Month ? "☑" : "□");      //是否月结
            dic.Add("GoodsIsCustom", goods?.SettlementType == SettlementType.DueTime ? "☑" : "□");   //是否约定期限

            dic.Add("GoodsCredits1", goods?.SettlementType == SettlementType.Month ? credits[payer, payee, "货款", Currency.CNY]?.Total.ToString("N") : "");   //信用上限
            dic.Add("GoodsCredits2", goods?.SettlementType == SettlementType.DueTime ? credits[payer, payee, "货款", Currency.CNY]?.Total.ToString("N") : "");   //信用上限

            dic.Add("GoodsMonth", goods?.SettlementType == SettlementType.Month ? goods?.Months.ToString() : string.Empty);   //月结月数
            dic.Add("GoodsMonthDay", goods?.SettlementType == SettlementType.Month ? goods?.Days.ToString() : string.Empty);   //月结天数
            dic.Add("GoodsCustomMonth", goods?.SettlementType == SettlementType.DueTime ? goods?.Months.ToString() : string.Empty);   //约定月数
            dic.Add("GoodsCustomDay", goods?.SettlementType == SettlementType.DueTime ? (this.Contract.StartDate.AddMonths(goods.Months).AddDays(goods.Days) - this.Contract.StartDate).Days.ToString() : string.Empty);   //约定天数

            dic.Add("GoodsIsCustoms", goods?.ExchangeType == ExchangeType.Customs ? "☑" : "□");   //是否海关汇率
            dic.Add("GoodsIsFloating", goods?.ExchangeType == ExchangeType.Floating ? "☑" : "□");   //是否实时汇率
            dic.Add("GoodsIsFixed", goods?.ExchangeType == ExchangeType.Fixed ? "☑" : "□");   //是否固定汇率
            dic.Add("GoodsIsPreset", goods?.ExchangeType == ExchangeType.Preset ? "☑" : "□");   //是否预设汇率
            #endregion

            #region 税款
            var taxes = debtTerms[payer, payee, "税款"];

            dic.Add("TaxesIsMonth", taxes?.SettlementType == SettlementType.Month ? "☑" : "□");      //是否月结
            dic.Add("TaxesIsCustom", taxes?.SettlementType == SettlementType.DueTime ? "☑" : "□");   //是否约定期限

            dic.Add("TaxesCredits1", taxes?.SettlementType == SettlementType.Month ? credits[payer, payee, "税款", Currency.CNY]?.Total.ToString("N") : "");   //信用上限
            dic.Add("TaxesCredits2", taxes?.SettlementType == SettlementType.DueTime ? credits[payer, payee, "税款", Currency.CNY]?.Total.ToString("N") : "");   //信用上限

            dic.Add("TaxesMonth", taxes?.SettlementType == SettlementType.Month ? taxes?.Months.ToString() : string.Empty);   //月结月数
            dic.Add("TaxesMonthDay", taxes?.SettlementType == SettlementType.Month ? taxes?.Days.ToString() : string.Empty);   //月结天数
            dic.Add("TaxesCustomMonth", taxes?.SettlementType == SettlementType.DueTime ? taxes?.Months.ToString() : string.Empty);   //约定月数
            dic.Add("TaxesCustomDay", taxes?.SettlementType == SettlementType.DueTime ? (this.Contract.StartDate.AddMonths(taxes.Months).AddDays(taxes.Days) - this.Contract.StartDate).Days.ToString() : string.Empty);   //约定天数

            dic.Add("TaxesIsCustoms", taxes?.ExchangeType == ExchangeType.Customs ? "☑" : "□");   //是否海关汇率
            dic.Add("TaxesIsFloating", taxes?.ExchangeType == ExchangeType.Floating ? "☑" : "□");   //是否实时汇率
            dic.Add("TaxesIsFixed", taxes?.ExchangeType == ExchangeType.Fixed ? "☑" : "□");   //是否固定汇率
            dic.Add("TaxesIsPreset", taxes?.ExchangeType == ExchangeType.Preset ? "☑" : "□");   //是否预设汇率
            #endregion

            #region 代理费
            var agent = debtTerms[payer, payee, "代理费"];

            dic.Add("AgentIsMonth", agent?.SettlementType == SettlementType.Month ? "☑" : "□");      //是否月结
            dic.Add("AgentIsCustom", agent?.SettlementType == SettlementType.DueTime ? "☑" : "□");   //是否约定期限

            dic.Add("AgentCredits1", agent?.SettlementType == SettlementType.Month ? credits[payer, payee, "代理费", Currency.CNY]?.Total.ToString("N") : "");   //信用上限
            dic.Add("AgentCredits2", agent?.SettlementType == SettlementType.DueTime ? credits[payer, payee, "代理费", Currency.CNY]?.Total.ToString("N") : "");   //信用上限

            dic.Add("AgentMonth", agent?.SettlementType == SettlementType.Month ? agent?.Months.ToString() : string.Empty);   //月结月数
            dic.Add("AgentMonthDay", agent?.SettlementType == SettlementType.Month ? agent?.Days.ToString() : string.Empty);   //月结天数
            dic.Add("AgentCustomMonth", agent?.SettlementType == SettlementType.DueTime ? agent?.Months.ToString() : string.Empty);   //约定月数
            dic.Add("AgentCustomDay", agent?.SettlementType == SettlementType.DueTime ? (this.Contract.StartDate.AddMonths(agent.Months).AddDays(agent.Days) - this.Contract.StartDate).Days.ToString() : string.Empty);   //约定天数

            dic.Add("AgentIsCustoms", agent?.ExchangeType == ExchangeType.Customs ? "☑" : "□");   //是否海关汇率
            dic.Add("AgentIsFloating", agent?.ExchangeType == ExchangeType.Floating ? "☑" : "□");   //是否实时汇率
            dic.Add("AgentIsFixed", agent?.ExchangeType == ExchangeType.Fixed ? "☑" : "□");   //是否固定汇率
            dic.Add("AgentIsPreset", agent?.ExchangeType == ExchangeType.Preset ? "☑" : "□");   //是否预设汇率
            #endregion

            #region 杂费
            var extras = debtTerms[payer, payee, "杂费"];

            dic.Add("ExtrasIsMonth", extras?.SettlementType == SettlementType.Month ? "☑" : "□");     //是否月结
            dic.Add("ExtrasIsCustom", extras?.SettlementType == SettlementType.DueTime ? "☑" : "□");   //是否约定期限

            dic.Add("ExtrasCredits1", extras?.SettlementType == SettlementType.Month ? credits[payer, payee, "杂费", Currency.CNY]?.Total.ToString("N") : "");   //信用上限
            dic.Add("ExtrasCredits2", extras?.SettlementType == SettlementType.DueTime ? credits[payer, payee, "杂费", Currency.CNY]?.Total.ToString("N") : "");   //信用上限

            dic.Add("ExtrasMonth", extras?.SettlementType == SettlementType.Month ? extras?.Months.ToString() : string.Empty);   //月结月数
            dic.Add("ExtrasMonthDay", extras?.SettlementType == SettlementType.Month ? extras?.Days.ToString() : string.Empty);   //月结天数
            dic.Add("ExtrasCustomMonth", extras?.SettlementType == SettlementType.DueTime ? extras?.Months.ToString() : string.Empty);   //约定月数
            dic.Add("ExtrasCustomDay", extras?.SettlementType == SettlementType.DueTime ? (this.Contract.StartDate.AddMonths(extras.Months).AddDays(extras.Days) - this.Contract.StartDate).Days.ToString() : string.Empty);   //约定天数

            dic.Add("ExtrasIsCustoms", extras?.ExchangeType == ExchangeType.Customs ? "☑" : "□");   //是否海关汇率
            dic.Add("ExtrasIsFloating", extras?.ExchangeType == ExchangeType.Floating ? "☑" : "□");   //是否实时汇率
            dic.Add("ExtrasIsFixed", extras?.ExchangeType == ExchangeType.Fixed ? "☑" : "□");   //是否固定汇率
            dic.Add("ExtrasIsPreset", extras?.ExchangeType == ExchangeType.Preset ? "☑" : "□");   //是否预设汇率
            #endregion


            #endregion
            return dic;
        }
        #endregion


    }



}
