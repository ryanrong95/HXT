using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.CrmPlus;
using Yahv.Usually;
using Yahv.Utils.Serializers;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    /// <summary>
    /// 待审批客户
    /// </summary>
    public class WatingClient : Enterprise, IUnique, IMyCloneable
    {
        #region 属性
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

        public Yahv.Underly.CrmPlus.ClientType ClientType { get; set; }

        /// <summary>
        /// 客户等级 
        /// </summary>
        public ClientGrade ClientGrade { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AuditStatus ClientStatus { get; set; }


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
        /// 
        /// </summary>
        override public DateTime ModifyDate { get; set; }
        /// <summary>
        /// 利润率
        /// </summary>
        public decimal? ProfitRate { get; set; }

        /// <summary>
        /// 是否是供应商
        /// </summary>
        public bool IsSupplier { get; set; }
        /// <summary>
        /// 业务
        /// </summary>
        public Conduct Conduct { set; get; }
        /// <summary>
        /// 合作公司
        /// </summary>
        public Relation Relation { set; get; }

        public string Owner { get; set; }
        #endregion

        #region 事件
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;
        #endregion

        #region 持久化
        /// <summary>
        /// 只做审批处理
        /// </summary>
        public void Approve()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                #region 审批通过
                var newClient = this.Clone(false) as WatingClient;
                //查询库里是否存在正式数据
                var enterprise = new Views.Origins.EnterprisesOrigin().FirstOrDefault(item => item.Name == this.Name && item.IsDraft == false);
                if (enterprise == null)
                {
                    newClient.ID = "";
                }
                else
                {
                    newClient.ID = enterprise.ID;

                    //是否同为供应商
                    newClient.IsSupplier = reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Suppliers>().Any(item => item.ID == enterprise.ID);
                    if (newClient.IsSupplier)
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.Suppliers>(new
                        {
                            IsClient = true
                        }, item => item.ID == newClient.ID);

                    }
                }
                newClient.ClientEnter(reponsitory, newClient);
                #endregion

                #region 修改草稿数据（现在是审批通过）
                reponsitory.Update<Layers.Data.Sqls.PvdCrm.Clients>(new
                {
                    Status = (int)AuditStatus.Normal
                }, item => item.ID == this.ID);
                reponsitory.Update<Layers.Data.Sqls.PvdCrm.Enterprises>(new
                {
                    Status = (int)AuditStatus.Normal
                }, item => item.ID == this.ID);
                reponsitory.Update<Layers.Data.Sqls.PvdCrm.Relations>(new
                {
                    Status = (int)AuditStatus.Normal
                }, item => item.ID == this.Relation.ID);
                #endregion

                tran.Commit();
            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
        }

        /// <summary>
        /// 审批拒绝
        /// </summary>
        public void Reject()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.Enterprises>(new
                {
                    Status = (int)AuditStatus.Voted,
                    Summary = this.Summary
                }, item => item.ID == this.ID);

                repository.Update<Layers.Data.Sqls.PvdCrm.Clients>(new
                {
                    Status = (int)AuditStatus.Voted
                }, item => item.ID == this.ID);
                repository.Update<Layers.Data.Sqls.PvdCrm.ApplyTasks>(new
                {
                    Status = ApplyStatus.Voted
                }, item => item.MainID == this.ID && item.MainType == (int)MainType.Clients && item.ApplyTaskType == (int)ApplyTaskType.ClientRegist);

                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        public void ClientEnter(PvdCrmReponsitory reponsitory, WatingClient watingClient)
        {
            #region Client

            watingClient.Status = AuditStatus.Normal;
            watingClient.IsDraft = false;
            watingClient.Enter();
           // watingClient.Conduct.EnterpriseID = watingClient.ID;
            watingClient.Conduct.Enter();
            watingClient.Relation.Status = AuditStatus.Normal;
         //   watingClient.Relation.ClientID = watingClient.ID;
            watingClient.Relation.Enter();
            #endregion

            if (!reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Clients>().Any(item => item.ID == watingClient.ID))
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.Clients()
                {
                    ID = watingClient.ID,
                    Grade = (int)this.ClientGrade,
                    Type = (int)this.ClientType,
                    Vip = (int)Underly.VIPLevel.NonVIP,
                    Source = this.Source,
                    IsMajor = this.IsMajor,
                    IsSpecial = this.IsSpecial,
                    IsSupplier = this.IsSupplier,
                    Industry = this.Industry,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    ModifyDate = this.ModifyDate,
                    ProfitRate = this.ProfitRate,
                    Owner = this.Owner,
                });

            }
            else
            {
                reponsitory.Update<Layers.Data.Sqls.PvdCrm.Clients>(new
                {
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
                    Owner = this.Owner,
                }, item => item.ID == watingClient.ID);


            }


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
            var newer = json.JsonTo<WatingClient>();
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
