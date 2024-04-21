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
    public class Client : Enterprise, IUnique, IMyCloneable
    {
        #region 属性

        public Underly.CrmPlus.ClientType ClientType { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public Conduct Conduct { get; set; }


        /// <summary>
        /// 客户等级 
        /// </summary>
        public ClientGrade ClientGrade { get; set; }

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
        /// 客户保护人
        /// </summary>

        public Admin Admin { get; set; }

        public string Owner { get; set; }

        /// <summary>
        /// 当前创建人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 营业执照(是否允许上传多个)
        /// </summary>
        public List<CallFile> Lisences { set; get; }
        public CallFile Logo { set; get; }
        #endregion
        #region 拓展属性
        public string SourceDes
        {
            get
            {
                return EnumsDictionary<FixedSource>.Current[this.Source];
            }
        }
        #endregion

        #region 事件
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder ErrorSuccess;
        #endregion

        #region 其他
        /// <summary>
        /// 我方合作公司
        /// </summary>
        public Relation Relation { get; set; }

        /// <summary>
        /// 关联关系
        /// </summary>
        public List<MapsEnterprise> MapsEnterprises { get; set; }
        /// <summary>
        /// Top10
        /// </summary>
        public MapsTopN MapsTopN { get; set; }
        #endregion

        public Client()
        {
            this.Status = AuditStatus.Waiting;
            this.ModifyDate = this.CreateDate = DateTime.Now;
        }


        #region   持久化

        public override void Enter()
        {
            if (this.Status == AuditStatus.Black)
                return;
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                {

                    //草稿待审批
                    if (this.IsDraft && this.Status == AuditStatus.Waiting)
                    {
                        //待审批编辑
                        //var enterprise = new Views.Origins.EnterprisesOrigin(reponsitory).FirstOrDefault(item => item.Name == this.Name && item.IsDraft==true);
                        //if (enterprise != null)
                        //{
                        //    this.ID = enterprise.ID;
                        //}
                        base.Enter();



                        this.Conduct.EnterpriseID = base.ID;
                        this.Conduct.Enter();

                        if (this.Relation != null)
                        {
                            this.Relation.ClientID = base.ID;
                            this.Relation.Enter();
                        }
                        ClientEnter(reponsitory);

                        throw new Exception("test");
                    }
                    //已审批的
                    else if (this.IsDraft == false && this.Status == AuditStatus.Normal)
                    {
                        //只有管理员可以修改 ，要控制
                        base.Enter();

                        ClientEnter(reponsitory);
                    }
                    #region  附件处理
                    if (this.Lisences?.Count() > 0)
                    {
                        reponsitory.Delete<Layers.Data.Sqls.PvdCrm.FilesDescription>(item => item.EnterpriseID == this.ID && item.Type == (int)CrmFileType.License);
                        reponsitory.Insert(this.Lisences.Select(item => new Layers.Data.Sqls.PvdCrm.FilesDescription
                        {

                            ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.File),
                            EnterpriseID = this.ID,
                            CustomName = item.FileName,
                            Url = item.CallUrl,
                            Summary = this.Summary,
                            Type = (int)CrmFileType.License,
                            CreateDate = DateTime.Now,
                            CreatorID = Creator,
                            Status = (int)AuditStatus.Normal,
                        }).ToArray());

                    }
                    if (this.Logo != null)
                    {
                        reponsitory.Delete<Layers.Data.Sqls.PvdCrm.FilesDescription>(item => item.EnterpriseID == this.ID && item.Type == (int)CrmFileType.Logo);
                        reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.FilesDescription()
                        {
                            ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.File),
                            EnterpriseID = this.ID,
                            CustomName = this.Logo.FileName,
                            Url = this.Logo.CallUrl,
                            Summary = this.Summary,
                            Type = (int)CrmFileType.Logo,
                            CreateDate = this.CreateDate,
                            CreatorID = Creator,
                            Status = (int)AuditStatus.Normal,
                        });

                    }
                    #endregion
                }
                tran.Commit();
            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this.ID));
        }


        public void ClientEnter(PvdCrmReponsitory reponsitory)
        {
            //添加
            if (!reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Clients>().Any(item => item.ID == this.ID))
            {
                reponsitory.Insert(this.ToLinq());
            }
            //修改
            else
            {
                reponsitory.Update<Layers.Data.Sqls.PvdCrm.Clients>(new
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
                    Owner = this.Owner,
                }, item => item.ID == this.ID);
            }
        }

        #endregion

        /// <summary>
        /// 加入公海
        /// </summary>
        public void JoinSea()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                {
                    this.Conduct.Enter();
                    var relations = new RelationsOrigin().Where(x => x.Type == this.Relation.Type && x.ClientID == this.Relation.ClientID && x.Status != AuditStatus.Closed);

                    if (relations != null)
                    {
                        //批量插入
                        foreach (var item in relations)
                        {
                            reponsitory.Update<Layers.Data.Sqls.PvdCrm.Relations>(new
                            {
                                Status = (int)AuditStatus.Closed,

                            }, x => x.ID == item.ID);

                        }


                    }

                }
                tran.Commit();
            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this.ID));
        }

        /// <summary>
        /// 审批否决修改 再次提交时调用
        /// </summary>
        public void UpdateStatus()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.Enterprises>(new
                {
                    Status = (int)AuditStatus.Waiting,
                }, item => item.ID == this.ID);

                repository.Update<Layers.Data.Sqls.PvdCrm.Clients>(new
                {
                    Status = (int)AuditStatus.Waiting
                }, item => item.ID == this.ID);
                repository.Update<Layers.Data.Sqls.PvdCrm.ApplyTasks>(new
                {
                    Status = ApplyStatus.Waiting
                }, item => item.MainID == this.ID && item.MainType == (int)MainType.Clients && item.ApplyTaskType == (int)ApplyTaskType.ClientRegist);

                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }

        }
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
            var newer = json.JsonTo<Client>();
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
