using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.CrmPlus.Service.Extends;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Linq;
using Yahv.Linq.Persistence;
using Yahv.Underly;
using Yahv.Underly.CrmPlus;
using Yahv.Underly.Erps;
using Yahv.Usually;
using Yahv.Utils.Serializers;
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    /// <summary>
    /// 公海客户
    /// </summary>
    public class PublicClient : Enterprise, IUnique, IMyCloneable
    {
        public override string ID
        {
            get
            {
                return base.ID;
            }
            set
            {
                string id = base.ID = value;
                if (this.Conduct != null)
                {
                    this.Conduct.EnterpriseID = id;

                }
                if (this.Relation != null)
                {
                    this.Relation.ClientID = id;
                }
                if (this.EnterpriseRegister != null)
                    this.EnterpriseRegister.ID = id;
            }
        }

        public PublicClient()
        {
            this.Status = AuditStatus.Waiting;
            this.ModifyDate = this.CreateDate = DateTime.Now;
        }

        #region 属性

        //  public string ID { get; set; }
        /// <summary>
        /// 企业
        /// </summary>
        // public Enterprise Enterprise { set; get; }
        /// <summary>
        /// 企业基础信息
        /// </summary>
        //  public EnterpriseRegister EnterpriseRegister { set; get; }

        [Obsolete("一定没有用")]
        public Conduct Conduct { set; get; }



        [Obsolete("一定没有用")]
        public Relation Relation { set; get; }

        public Yahv.Underly.CrmPlus.ClientType ClientType { get; set; }

        /// <summary>
        /// 客户等级 
        /// </summary>
        public ClientGrade ClientGrade { get; set; }


        /// <summary>
        /// VIp等级
        /// </summary>
        public VIPLevel Vip { get; set; }
        /// <summary>
        /// 来源：线上代购、线上期货、线上BOM、线下贸易、代理推广、报关服务、检测服务、香港本地服务
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// District的描述
        /// </summary>
        public string SourceName
        {
            get
            {
                return EnumsDictionary<FixedArea>.Current[this.Source];
            }
        }

        /// <summary>
        /// 是否重点客户
        /// </summary>
        public bool IsMajor { get; set; }

        /// <summary>
        /// 是否特殊
        /// </summary>
        public bool IsSpecial { get; set; }

        /// <summary>
        /// 所属行业
        /// </summary>
        public string Industry { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public AuditStatus ClientStatus { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        //public DateTime CreateDate { get; set; }

        public override DateTime CreateDate
        {
            get
            {
                return base.CreateDate;
            }

            set
            {
                base.CreateDate = value;
            }
        }

        /// <summary>
        /// 修改时间
        /// </summary>
        override public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 利润率
        /// </summary>
        //[Obsolete("不知道这个有什么作用？")]
        public decimal? ProfitRate { get; set; }

        /// <summary>
        /// 是否是供应商
        /// </summary>
        public bool IsSupplier { get; set; }

        #endregion

        #region 事件
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;
        #endregion

        #region 持久化



        //注册认领，Clone为草稿 客户
        public override void Enter()
        {
            using (var reponsitory = new PvdCrmReponsitory())
            //using (var tran = reponsitory.OpenTransaction())
            //{
                { 
                this.IsDraft = this.Status == Yahv.Underly.AuditStatus.Waiting;
                base.Enter(reponsitory);
                this.Conduct.Enter();
                this.Binding();
                //if (!this.IsDraft)
                //{
                //    this.Binding();
                //}
                //else {
                //this.Relation.Enter();
                //}

                var apply = new ApplyTask
                {
                    MainID = this.ID,
                    MainType = MainType.Clients,
                    ApplierID = this.Relation.OwnerID,
                    ApplyTaskType = this.Status == AuditStatus.Normal ? ApplyTaskType.ClientPublic : ApplyTaskType.ClientRegist,
                };
                 apply.EnterExtend();
                this.Enter(new Layers.Data.Sqls.PvdCrm.Clients
                {
                    ID = this.ID,
                    Grade = (int)this.ClientGrade,
                    Type = (int)this.ClientType,
                    Vip = (int)this.Vip,
                    Source = this.Source,
                    IsMajor = this.IsMajor,
                    IsSpecial = this.IsSpecial,
                    IsSupplier = this.IsSupplier,
                    Industry = this.Industry,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    ModifyDate = this.ModifyDate,
                    ProfitRate = this.ProfitRate,
                }, reponsitory);
            //}
            //    tran.Commit();
           
            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this.ID));
        }


        public void Binding()
        {
            using (var reponsitory = new PvdCrmReponsitory())
            {
                var exist = reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Relations>().
                    Where(x => x.ClientID == this.Relation.ClientID && x.CompanyID == this.Relation.CompanyID && x.Type == (int)this.Relation.Type &&x.OwnerID==this.Relation.OwnerID&& x.Status==(int)Yahv.Underly.AuditStatus.Waiting);
                if (exist.Any())
                {
                    return;
                }
                string id = PKeySigner.Pick(PKeyType.Relations);
                reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.Relations()
                {
                    ID=id,
                    Type = (int)this.Relation.Type,
                    OwnerID = this.Relation.OwnerID,
                    CompanyID = this.Relation.CompanyID,
                    ClientID = this.Relation.ClientID,
                    Status = (int)this.Relation.Status,
                    Summary= this.Relation.Summary,
                    CreateDate = DateTime.Now,
                });
            }
        }

        /// <summary>
        /// 认领审批通过
        /// </summary>
        public void Allowed(string approverid)
        {
            using (var reponsitory = new PvdCrmReponsitory())
            using (var tran = reponsitory.OpenTransaction())
            {
                {
                   // var applyStatus = ApplyStatus.Allowed;
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.Conducts>(new
                    {
                        IsPublic = false
                    }, item => item.ID == this.Conduct.ID);
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.Relations>(new
                    {

                        OfferDate = DateTime.Now,
                        Status = (int) AuditStatus.Normal
                    }, item => item.ID == this.Relation.ID);

                   
                }
                tran.Commit();
            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));

        }

        /// <summary>
        /// 公海认领审批拒绝
        /// </summary>
        /// <param name="approverid"></param>
        public void Voted(string approverid)
        {

            using (var reponsitory = new PvdCrmReponsitory())
            using (var tran = reponsitory.OpenTransaction())
            {
                {
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.Relations>(new
                    {

                        OfferDate = DateTime.Now,
                        Status = (int)AuditStatus.Voted
                    }, item => item.ID == this.Relation.ID);
                    (new ApplyTask
                    {
                        MainID = this.ID,
                        MainType = MainType.Clients,
                        ApproverID = approverid,
                        Status = ApplyStatus.Voted,
                        ApplyTaskType = ApplyTaskType.ClientPublic,
                    }).Approve();
                }
                tran.Commit();
            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
        }
        /// <summary>
        /// 新增公海客户
        /// </summary>
        public void Add()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                this.EnterpriseRegister = new EnterpriseRegister
                {
                    IsInternational = false,
                    IsSecret = false,
                };

                base.Enter();

                this.Enter(new Layers.Data.Sqls.PvdCrm.Clients
                {
                    ID = this.ID,
                    Grade = (int)this.ClientGrade,
                    Type = (int)this.ClientType,
                    Vip = (int)this.Vip,
                    Source = this.Source,
                    IsMajor = this.IsMajor,
                    IsSpecial = this.IsSpecial,
                    IsSupplier = this.IsSupplier,
                    Industry = this.Industry,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    ModifyDate = this.ModifyDate,
                    ProfitRate = this.ProfitRate,
                }, reponsitory);
                tran.Commit();
            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this.ID));
        }

        #endregion

        #region Clone
        /// <summary>
        /// 深度复制
        /// </summary>
        /// <remarks>
        /// 包涵数据本身的复制(JSON)
        /// 与实际数据的复制（DB）
        /// </remarks>
        public object Clone()
        {
            return this as object;
        }

        public object Clone(bool isCloneDb)
        {
            var json = this.Json();
            var newer = json.JsonTo<PublicClient>();
            if (isCloneDb)
            {
                return this.Clone();
            }
            else
            {
                return newer;
            }
        }
        #endregion
    }
}
