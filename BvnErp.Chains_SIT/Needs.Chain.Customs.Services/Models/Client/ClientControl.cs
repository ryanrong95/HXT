using Needs.Ccs.Services.Views;
using Needs.Linq;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 客户信息
    /// </summary>
    public class ClientControl : IUnique
    {
        public string ID
        {
            get;set;
        }

        /// <summary>
        /// 客户类型
        /// </summary>
        public Enums.ClientType ClientType { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        public Company Company { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }
        
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
                        this.ServiceManager = view.Where(item => item.ClientID == this.ClientID && item.Status == Enums.Status.Normal
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
        /// 代仓储业务经理
        /// </summary>
        Admin StorageserviceManager;
        public Admin StorageServiceManager
        {
            get
            {
                if (StorageserviceManager == null)
                {
                    using (var view = new Views.ClientAdminsView())
                    {
                        this.StorageServiceManager = view.Where(item => item.ClientID == this.ClientID && item.Status == Enums.Status.Normal
                            && item.Type == Enums.ClientAdminType.StorageServiceManager).Select(item => new Models.Admin
                            {
                                ID = item.Admin.ID,
                                RealName = item.Admin.RealName,
                                Tel = item.Admin.Tel,
                                Email = item.Admin.Email,
                                Mobile = item.Admin.Mobile
                            }).SingleOrDefault();
                    }
                }
                return this.StorageserviceManager;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                this.StorageserviceManager = value;
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
        /// 客户信息添加人\操作人
        /// </summary>
        public Admin Admin { get; set; }

        public string AdminID { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public Enums.ClientControlStatus ClientControlStatus { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }
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

        public ClientControl()
        {
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientControls>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    this.ID = ChainsGuid.NewGuidUp();
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientControls
                    {
                        ID = this.ID,
                        ClientID=this.ClientID,
                        CompanyID = this.CompanyID,
                        ClientType = (int)this.ClientType,
                        ClientCode = this.ClientCode,
                        ClientRank = (int)this.ClientRank,
                        ClientStatus = (int)this.ClientControlStatus,
                        Status = (int)this.Status,
                        AdminID = this.Admin.ID,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.ClientControls
                    {
                        ID = this.ID,
                        ClientID = this.ClientID,
                        CompanyID = this.CompanyID,
                        ClientType = (int)this.ClientType,
                        ClientCode = this.ClientCode,
                        ClientRank = (int)this.ClientRank,
                        ClientStatus = (int)this.ClientControlStatus,
                        Status = (int)this.Status,
                        AdminID = this.Admin.ID,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary
                    }, item => item.ID == this.ID);
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
        
        
      
    }
}