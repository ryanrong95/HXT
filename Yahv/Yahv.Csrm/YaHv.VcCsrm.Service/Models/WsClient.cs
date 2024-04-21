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
using YaHv.VcCsrm.Service.Extends;

namespace YaHv.VcCsrm.Service.Models
{
    public class WsClient : Yahv.Linq.IUnique
    {
        public WsClient()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = ApprovalStatus.UnComplete;
        }
        #region 属性 
        /// <summary>
        /// 合作关系ID
        /// </summary>
        public string ID { set; get; }
        string clientid;
        public string ClientID
        {
            get
            {
                return this.clientid ?? this.Name.MD5();

            }
            set
            {
                this.clientid = value;
            }
        }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string Name { set; get; }
        public string Corperation { set; get; }
        public string Uscc { set; get; }
        public string RegAddress { set; get; }
        string companyid;
        /// <summary>
        /// 合作公司ID
        /// </summary>
        public string CompanyID { set; get; }
        /// <summary>
        /// 合作公司名称
        /// </summary>
        public string CompanyName
        {
            get
            {
                return this.companyid ?? this.CompanyName.MD5();

            }
            set
            {
                this.companyid = value;
            }
        }
        /// <summary>
        /// 合作类型
        /// </summary>
        public Business Type { set; get; }

        public bool Vip { set; get; }
        public int Grade { set; get; }
        /// <summary>
        /// 入仓号
        /// </summary>
        public string EnterCode { set; get; }
        /// <summary>
        /// 海关编码
        /// </summary>
        public string CustomsCode { set; get; }
        /// <summary>
        /// 性质：
        /// </summary>
        public ClientType Nature { set; get; }
        /// <summary>
        /// 国家/地区
        /// </summary>
        public string Origin { set; get; }



        /// <summary>
        /// 合作类型/业务类型：代仓储-采购，代仓储-销售
        /// </summary>
        public Business CooperType { set; get; }
        public DateTime CreateDate { set; get; }
        public DateTime UpdateDate { set; get; }
        public string CreatorID { set; get; }
        public Admin Creator { set; get; }
        public string Summary { set; get; }
        public ApprovalStatus Status { set; get; }
        #endregion

        #region 子项信息
        //Admin serviceManager;

        ///// <summary>
        ///// 业务员
        ///// </summary>
        //public Admin ServiceManager
        //{
        //    get
        //    {
        //        using (var view = new Views.Rolls.TrackerAdmin(MapsID, MapsType.ServiceManager))
        //        {
        //            return view.FirstOrDefault();
        //        }
        //    }
        //    set
        //    {
        //        this.serviceManager = value;
        //    }
        //}

        ///// <summary>
        ///// 跟单员
        ///// </summary>
        //Admin merchandiser;
        //public Admin Merchandiser
        //{
        //    get
        //    {
        //        using (var view = new Views.Rolls.TrackerAdmin(MapsID, MapsType.Merchandiser))
        //        {
        //            return view.FirstOrDefault();
        //        };
        //    }
        //    set
        //    {
        //        this.merchandiser = value;
        //    }
        //}
        Views.Rolls.WsSuppliersRoll wssuppliers;
        /// <summary>
        /// 代仓储客户的供应商
        /// </summary>
        public Views.Rolls.WsSuppliersRoll WsSuppliers
        {
            get
            {
                if (this.wssuppliers == null || this.wssuppliers.Disposed)
                {
                    this.wssuppliers = new Views.Rolls.WsSuppliersRoll(this.ID);
                }
                return this.wssuppliers;
            }
        }

        Views.Rolls.InvoicesRoll invoices;
        /// <summary>
        /// 客户企业的发票
        /// </summary>
        public Views.Rolls.InvoicesRoll Invoices
        {
            get
            {
                if (this.invoices == null || this.Invoices.Disposed)
                {
                    this.invoices = new Views.Rolls.InvoicesRoll(this.clientid);
                }
                return this.invoices;
            }
        }
        //联系人
        //收件地址
        //业务员
        //跟单员

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
        virtual public event ErrorHanlder Repeat;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvcCrmReponsitory>.Create())
            {
                //客户企业
                //ShipPartners
                //WsClients
                (new Enterprise
                {
                    Name = this.Name,
                    Corporation = this.Corperation,
                    RegAddress = this.RegAddress,
                    Uscc = this.Uscc,
                    AdminCode = ""
                }).Enter();
                if (!repository.ReadTable<Layers.Data.Sqls.PvcCrm.ShipsPartner>().Any(item => item.Type == (int)this.Type && item.MainID == this.ClientID && item.SubID == this.companyid))
                {
                    this.ID = PKeySigner.Pick(Services.PKeyType.WsClient);

                    repository.Insert(new Layers.Data.Sqls.PvcCrm.ShipsPartner
                    {
                        ID = this.ID,
                        Type = (int)this.Type,
                        MainID = this.ClientID,
                        SubID = this.CompanyID
                    });
                }
                if (repository.ReadTable<Layers.Data.Sqls.PvcCrm.WsClients>().Any(item => item.ID == this.ID))
                {
                    #region 入仓号
                    if (this.EnterCode == "WL")
                    {
                        this.EnterCode = PKeySigner.Pick(Services.PKeyType.WL);
                    }
                    else if (this.EnterCode == "XL")
                    {
                        this.EnterCode = PKeySigner.Pick(Services.PKeyType.XL);
                    }
                    else
                    {
                        this.EnterCode = this.EnterCode;
                    }
                    #endregion
                    repository.Insert(this.ToLinq());
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PvcCrm.WsClients>(new
                    {
                        Nature = (int)this.Nature,
                        Vip = this.Vip,
                        Grade = (int)this.Grade,
                        UpdateDate = this.UpdateDate,
                        CustomsCode = this.CustomsCode,
                        Summary = this.Summary,
                        Status = (int)this.Status
                    }, item => item.ID == this.ID);
                }
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        public void Abandon()
        {
            using (var repository = LinqFactory<PvcCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvcCrm.WsClients>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvcCrm.WsClients>(new
                    {
                        Status = (int)GeneralStatus.Deleted
                    }, item => item.ID == this.ID);
                }
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }
}
