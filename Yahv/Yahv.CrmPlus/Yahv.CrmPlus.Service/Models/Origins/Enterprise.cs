using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Extends;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Serializers;
using static Yahv.CrmPlus.Service.Models.Origins.Enterprise;

namespace Yahv.CrmPlus.Service.Models.Origins
{

    /// <summary>
    /// 企业
    /// </summary>
    /// <remarks>
    /// 建议
    /// 如果我们已经有审批过的客户的信息，直接使用
    /// 如果多人同时注册没有审批过的客户信息，任然使用接口
    /// </remarks>
    public class Enterprise : IUnique, IMyCloneable
    {
        public Enterprise()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
            this.Status = AuditStatus.Waiting;
        }
        #region  属性
        virtual public string ID { get; set; }
        string name;
        /// <summary>
        /// 企业名称
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set
            {
                Regex regex = new Regex(@"\s+", RegexOptions.Singleline);
                this.name = regex.Replace(value, " ").Trim();
            }
        }

        public EnterpriseRegister EnterpriseRegister { get; set; }
        /// <summary>
        /// 是否是草稿
        /// </summary>
        public bool IsDraft { get; set; }

        public AuditStatus Status { get; set; }

        /// <summary>
        /// 国别地区
        /// </summary>
        public string District
        {
            get; set;
        }
        /// <summary>
        /// 国家/地区
        /// </summary>

        public string Place { get; set; }
        /// <summary>
        /// 企业等级
        /// </summary>
        public int? Grade { get; set; }

        public string Summary { get; set; }

        virtual public DateTime CreateDate { get; set; }
        /// <summary>
        /// 大赢家编码
        /// </summary>
        public string DyjCode { set; get; }
        /// <summary>
        /// 修改时间
        /// </summary>
        virtual public DateTime ModifyDate { set; get; }
        #endregion

        #region 扩展属性
        /// <summary>
        /// District的描述
        /// </summary>
        public string DistrictDes
        {
            get
            {
                return EnumsDictionary<FixedArea>.Current[this.District];
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;


        public event SuccessHanlder AbandonSuccess;
        #endregion

        #region 持久化

        virtual public void Enter()
        {
            using (var reponsitory = new PvdCrmReponsitory())
            {
                this.Enter(reponsitory);
            }
        }

        protected void Enter(Layers.Data.Sqls.PvdCrm.Clients client, PvdCrmReponsitory reponsitory)
        {
            #region 保存客户数据

            if (reponsitory.GetTable<Layers.Data.Sqls.PvdCrm.Clients>().Any(item => item.ID == this.ID))
            {
                reponsitory.Update<Layers.Data.Sqls.PvdCrm.Clients>(new
                {
                    Grade = (int)client.Grade,
                    Type = (int)client.Type,
                    Vip = (int)client.Vip,
                    Source = client.Source,
                    IsMajor = client.IsMajor,
                    IsSpecial = client.IsSpecial,
                    IsSupplier = client.IsSupplier,
                    Industry = client.Industry,
                    Status = (int)client.Status,
                    CreateDate = client.CreateDate,
                    ModifyDate = client.ModifyDate,
                    ProfitRate = client.ProfitRate,
                }, item => item.ID == client.ID);
            }
            else
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.Clients()
                {
                    ID = client.ID,
                    Grade = (int)client.Grade,
                    Type = (int)client.Type,
                    Vip = (int)VIPLevel.NonVIP,
                    Source = client.Source,
                    IsMajor = client.IsMajor,
                    IsSpecial = client.IsSpecial,
                    IsSupplier = client.IsSupplier,
                    Industry = client.Industry,
                    Status = (int)client.Status,
                    CreateDate = client.CreateDate,
                    ModifyDate = client.ModifyDate,
                    ProfitRate = client.ProfitRate,
                });
            }

            #endregion
        }
        internal protected void Enter(PvdCrmReponsitory reponsitory)
        {
            if (reponsitory.GetTable<Layers.Data.Sqls.PvdCrm.Enterprises>().Any(item => item.ID == this.ID))
            {
                reponsitory.Update<Layers.Data.Sqls.PvdCrm.Enterprises>(new
                {
                    Name = this.Name,
                    //IsDraft = this.IsDraft,
                    Status = (int)this.Status,
                    District = this.District,
                    Place = this.Place,
                    Grade = this.Grade,
                    Summary = this.Summary,
                    DyjCode = this.DyjCode,
                    ModifyDate = this.ModifyDate
                }, item => item.ID == this.ID);
            }
            else
            {

                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Enterprise);
                }
                reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.Enterprises()
                {
                    ID = this.ID,
                    Name = this.Name,
                    IsDraft = this.IsDraft,
                    Status = (int)this.Status,
                    District = this.District,
                    Place = this.Place,
                    Grade = this.Grade,
                    Summary = this.Summary,
                    CreateDate = this.CreateDate,
                    DyjCode = this.DyjCode,
                    ModifyDate = this.ModifyDate
                });
            }
            if (this.EnterpriseRegister != null)
            {
                if (string.IsNullOrWhiteSpace(this.EnterpriseRegister.ID))
                {
                    this.EnterpriseRegister.ID = this.ID;
                }
                this.EnterpriseRegister.Enter();
            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
        }

        /// <summary>
        /// 加入黑名单
        /// </summary>
        public void JoinBlack()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.PvdCrm.Enterprises>(new
                {
                    Status = (int)AuditStatus.Black,
                    Summary = this.Summary
                }, item => item.ID == this.ID);
                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }
        /// <summary>
        /// 撤销黑名单
        /// </summary>
        public void CancelBlack()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.PvdCrm.Enterprises>(new
                {
                    Status = (int)AuditStatus.Normal,
                    Summary = this.Summary
                }, item => item.ID == this.ID);
                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }
        /// <summary>
        /// 供应商保存
        /// </summary>
        /// <param name="supplier"></param>
        /// <param name="reponsitory"></param>
        protected void Enter(Layers.Data.Sqls.PvdCrm.Suppliers supplier, PvdCrmReponsitory reponsitory)
        {
            if (reponsitory.GetTable<Layers.Data.Sqls.PvdCrm.Suppliers>().Any(item => item.ID == supplier.ID))
            {
                reponsitory.Update<Layers.Data.Sqls.PvdCrm.Suppliers>(new
                {
                    Products = supplier.Products,
                    Source = supplier.Products,
                    Type = (int)supplier.Type,
                    SettlementType = (int)supplier.SettlementType,
                    OrderType = (int)supplier.OrderType,
                    InvoiceType = (int)supplier.InvoiceType,
                    IsSpecial = supplier.IsSpecial,
                    IsClient = supplier.IsClient,
                    IsProtected = supplier.IsProtected,
                    IsAccount = supplier.IsAccount,
                    WorkTime = supplier.WorkTime,
                    IsFixed = supplier.IsFixed,
                    Status = supplier.Status,
                    Grade=supplier.Grade,
                    OrderCompanyID = supplier.OrderCompanyID
                }, item => item.ID == supplier.ID);
            }
            else
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.Suppliers
                {
                    ID = supplier.ID,
                    Grade = supplier.Grade,
                    Products = supplier.Products,
                    Source = supplier.Products,
                    Type = (int)supplier.Type,
                    SettlementType = (int)supplier.SettlementType,
                    OrderType = (int)supplier.OrderType,
                    InvoiceType = (int)supplier.InvoiceType,
                    IsSpecial = supplier.IsSpecial,
                    IsClient = supplier.IsClient,
                    IsProtected = supplier.IsProtected,
                    IsAccount = supplier.IsAccount,
                    WorkTime = supplier.WorkTime,
                    IsFixed = supplier.IsFixed,
                    Status = (int)supplier.Status,
                    CreateDate = supplier.CreateDate,
                    CreatorID = supplier.CreatorID
                });
            }
        }
        /// <summary>
        /// 内部公司保存
        /// </summary>
        /// <param name="company"></param>
        /// <param name="reponsitory"></param>
        protected void Enter(Layers.Data.Sqls.PvdCrm.Companies company, PvdCrmReponsitory reponsitory)
        {
            if (!reponsitory.GetTable<Layers.Data.Sqls.PvdCrm.Companies>().Any(item => item.ID == company.ID))
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.Companies()
                {
                    ID = this.ID,
                    Status = (int)company.Status,
                    CreateDate = company.CreateDate,
                    CreatorID = company.CreatorID
                });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enterprise"></param>
        /// <param name="reponsitory"></param>
        internal protected void Enter(Layers.Data.Sqls.PvdCrm.Enterprises enterprise, PvdCrmReponsitory reponsitory)
        {
            if (reponsitory.GetTable<Layers.Data.Sqls.PvdCrm.Enterprises>().Any(item => item.ID == this.ID))
            {
                reponsitory.Update<Layers.Data.Sqls.PvdCrm.Enterprises>(new
                {
                    Name = enterprise.Name,
                    //IsDraft = this.IsDraft,
                    Status = (int)enterprise.Status,
                    District = enterprise.District,
                    Place = enterprise.Place,
                    Grade = enterprise.Grade,
                    Summary = enterprise.Summary,
                    DyjCode = enterprise.DyjCode,
                    ModifyDate = enterprise.ModifyDate
                }, item => item.ID == enterprise.ID);
            }
            else
            {

                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Enterprise);
                }
                reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.Enterprises()
                {
                    ID = enterprise.ID,
                    Name = enterprise.Name,
                    IsDraft = enterprise.IsDraft,
                    Status = (int)enterprise.Status,
                    District = enterprise.District,
                    Place = enterprise.Place,
                    Grade = enterprise.Grade,
                    Summary = enterprise.Summary,
                    CreateDate = enterprise.CreateDate,
                    DyjCode = enterprise.DyjCode,
                    ModifyDate = enterprise.ModifyDate
                });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="register"></param>
        /// <param name="reponsitory"></param>
        internal protected void Enter(Layers.Data.Sqls.PvdCrm.EnterpriseRegisters register, PvdCrmReponsitory reponsitory)
        {
            if (reponsitory.GetTable<Layers.Data.Sqls.PvdCrm.EnterpriseRegisters>().Any(x => x.ID == this.ID))
            {
                reponsitory.Update<Layers.Data.Sqls.PvdCrm.EnterpriseRegisters>(new
                {
                    IsSecret = register.IsSecret,
                    IsInternational = register.IsInternational,
                    Corperation = register.Corperation,
                    RegAddress = register.RegAddress,
                    Uscc = register.Uscc,
                    Currency = (int?)register.Currency,
                    RegistFund = register.RegistFund,
                    RegistCurrency = (int)register.RegistCurrency,
                    Industry = register.Industry,
                    RegistDate = register.RegistDate,
                    Summary = register.Summary,
                    BusinessState = register.BusinessState,
                    Employees = register.Employees,
                    WebSite = register.WebSite,
                    Nature = register.Nature
                }, item => item.ID == this.ID);
            }
            else
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.EnterpriseRegisters()
                {
                    ID = register.ID,
                    IsSecret = register.IsSecret,
                    IsInternational = register.IsInternational,
                    Corperation = register.Corperation,
                    RegAddress = register.RegAddress,
                    Uscc = register.Uscc,
                    Currency = (int?)register.Currency,
                    RegistFund = register.RegistFund,
                    RegistCurrency = (int)register.RegistCurrency,
                    Industry = register.Industry,
                    RegistDate = register.RegistDate,
                    Summary = register.Summary,
                    BusinessState = register.BusinessState,
                    Employees = register.Employees,
                    WebSite = register.WebSite,
                    Nature = register.Nature,
                });
            }


        }
        #region Clone
        virtual public object Clone()
        {
            return new Layers.Data.Sqls.PvdCrm.Enterprises
            {
                ID = this.ID,
                Name = this.Name,
                IsDraft = this.IsDraft,
                Status = (int)this.Status,
                District = this.District,
                Grade = this.Grade,
                Summary = this.Summary,
                CreateDate = this.CreateDate
            } as object;
        }

        virtual public object Clone(bool isCloneDb)
        {
            if (isCloneDb)
            {
                return new Layers.Data.Sqls.PvdCrm.Enterprises
                {
                    ID = this.ID,
                    Name = this.Name,
                    IsDraft = this.IsDraft,
                    Status = (int)this.Status,
                    District = this.District,
                    Grade = this.Grade,
                    Summary = this.Summary,
                    CreateDate = this.CreateDate,
                    ModifyDate = this.ModifyDate,
                    DyjCode = this.DyjCode
                } as object;
            }
            else
            {
                return this.Json().JsonTo<Enterprise>();
            }
        }
        #endregion

    }
    #endregion


}






