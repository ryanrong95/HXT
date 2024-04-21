using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.CrmPlus.Service.Extends;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Serializers;
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    public class Supplier : Enterprise, IMyCloneable, IDataEntity
    {
        public Supplier()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
            this.Status=this.SupplierStatus = AuditStatus.Waiting;
        }
        public override string ID
        {
            get
            {
                return base.ID;
            }
            set
            {
                string id = base.ID = value;
                if (this.EnterpriseRegister != null)
                    this.EnterpriseRegister.ID = id;
            }
        }

        #region 属性

        /// <summary>
        /// 等级
        /// </summary>
        public int? SupplierGrade { set; get; }
        /// <summary>
        /// 产品
        /// </summary>
        public string Products { set; get; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { set; get; }
        /// <summary>
        /// 供应商类型
        /// </summary>
        public Underly.CrmPlus.SupplierType Type { set; get; }
        /// <summary>
        /// 结算类型：账期，现款，预付款
        /// </summary>
        public Underly.CrmPlus.SettlementType SettlementType { set; get; }
        /// <summary>
        /// 下单方式：网站下单，PO下单，API下单
        /// </summary>
        public Underly.CrmPlus.OrderType OrderType { set; get; }
        /// <summary>
        /// 发票类型：普通发票，增值税普通发票，专用发票
        /// </summary>
        public InvoiceType InvoiceType { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        override public DateTime CreateDate { set; get; }
        /// <summary>
        /// 是否特色
        /// </summary>
        public bool IsSpecial { set; get; }
        /// <summary>
        /// 是否同为客户
        /// </summary>
        public bool IsClient { set; get; }
        /// <summary>
        /// 是否保护
        /// </summary>
        public bool IsProtected { set; get; }
        /// <summary>
        /// 是否代理
        /// </summary>
        public bool IsAgent { set; get; }
        /// <summary>
        /// 是否有账期
        /// </summary>
        public bool IsAccount { set; get; }
        /// <summary>
        /// 上班时间
        /// </summary>
        public string WorkTime { set; get; }
        /// <summary>
        /// 是否固定渠道
        /// </summary>
        public bool IsFixed { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public AuditStatus SupplierStatus { set; get; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { set; get; }
        public Admin CreatorAdmin { internal set; get; }
        /// <summary>
        /// 保护人
        /// </summary>
        public string OwnerID { set; get; }
        public Admin OwnerAdmin { internal set; get; }
        /// <summary>
        /// 固定渠道信息
        /// </summary>

        public FixedSupplier FiexedSupplier { set; get; }
        /// <summary>
        /// 下单公司ID
        /// </summary>
        public string OrderCompanyID { set; get; }
        /// <summary>
        /// 下单公司
        /// </summary>
        public Enterprise OrderCompany { internal set; get; }
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
        /// <summary>
        /// 取消保护事件
        /// </summary>
        public event SuccessHanlder CancelProtectSuccess;
        /// <summary>
        /// 审批通过成功事件
        /// </summary>
        public event SuccessHanlder AllowedSuccess;
        /// <summary>
        /// 否决成功事件
        /// </summary>
        public event SuccessHanlder VotedSuccess;
        #endregion
        /// <summary>
        /// 资质证件
        /// </summary>
        public List<CallFile> Lisences { set; get; }
        public CallFile Logo { set; get; }
        //public List<CallFile> QualificationCertificates { set; get; }
        #region 持久化
        /// <summary>
        ///
        /// </summary>
        override public void Enter()
        {
            using (var reponsitory = new PvdCrmReponsitory())
            using (var tran = reponsitory.OpenTransaction())
            {
                base.Enter();
                this.Enter(this.ToLinq(), reponsitory);
                //文件
                if (!string.IsNullOrWhiteSpace(this.Logo?.CallUrl))
                {
                    var logolist = new Views.Rolls.FilesDescriptionRoll()[this.ID, CrmFileType.Logo].ToArray();
                    logolist.Abandon();//废弃Logo
                }

                if (this.Lisences?.Count() > 0)
                {
                    var licenselist = new Views.Rolls.FilesDescriptionRoll()[this.ID, CrmFileType.License].ToArray();
                    licenselist.Abandon();//废弃Lisences
                }

                this.FilesToList().Enter();//新增新的上传文件
                tran.Commit();

                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
                this.EnterError?.Invoke(this, new ErrorEventArgs());
            }
        }
        /// <summary>
        /// 取消保护
        /// </summary>
        public void CancelProtect()
        {
            using (var reponsitory = new PvdCrmReponsitory())
            {
                reponsitory.Update<Layers.Data.Sqls.PvdCrm.Suppliers>(new
                {
                    IsProtected = false,
                    OwnerID = ""
                }, item => item.ID == this.ID);

                this.CancelProtectSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 审批通过
        /// </summary>
        public void Allowed(string approverid)
        {
            //企业黑名单
            if (base.Status == AuditStatus.Black)
            {
                return;
            }
            this.SupplierStatus = base.Status = AuditStatus.Normal;
            using (var reponsitory = new PvdCrmReponsitory())
            //using (var tran = reponsitory.OpenTransaction())
            {
                var enterprise = new Views.Origins.EnterprisesOrigin(reponsitory).FirstOrDefault(item => item.Name == this.Name && item.IsDraft == false);
                var files = new Views.Rolls.FilesDescriptionRoll()[this.ID, CrmFileType.License, CrmFileType.Logo].Where(item => item.Status == DataStatus.Normal).ToArray();
                base.Enter(reponsitory);//保存草稿Enterprise
                this.Enter(this.ToLinq(), reponsitory);//保存草稿供应商


                #region Enterprises
                if (enterprise == null)
                {
                    this.ID = this.EnterpriseRegister.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Enterprise);
                    this.IsDraft = false;
                }
                else
                {
                    this.ID = this.EnterpriseRegister.ID = enterprise.ID;
                    this.IsClient = reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Clients>().Any(item => item.ID == enterprise.ID);
                }
                this.IsDraft = false;
                #endregion
                var clonesupllier = this.Clone(true) as Layers.Data.Sqls.PvdCrm.Suppliers; ;
                var cloneenterprise = base.Clone(true) as Layers.Data.Sqls.PvdCrm.Enterprises;
                var register = this.EnterpriseRegister.Clone(true) as Layers.Data.Sqls.PvdCrm.EnterpriseRegisters;
                bool isSupplier = false;//客户是否同为供应商

                base.Enter(cloneenterprise, reponsitory);
                base.Enter(clonesupllier, reponsitory);
                base.Enter(register, reponsitory);

                #region 是否同为客户
                if (isSupplier && enterprise != null)
                {
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.Clients>(new
                    {
                        IsSupplier = true
                    }, item => item.ID == enterprise.ID);
                }
                #endregion

                #region 文件处理

                List<FilesDescription> list = new List<FilesDescription>();
                foreach (var item in files)
                {
                    item.EnterpriseID = this.ID;
                    list.Add(item);
                }
                list.Enter();
                #endregion

                //tran.Commit();
            }

            this.AllowedSuccess?.Invoke(this, new SuccessEventArgs(this));
            this.EnterError?.Invoke(this, new ErrorEventArgs());

        }
        /// <summary>
        /// 审批否决
        /// </summary>
        public void Voted(string approverid)
        {
            using (var reponsitory = new PvdCrmReponsitory())
            using (var tran = reponsitory.OpenTransaction())
            {
                this.SupplierStatus = AuditStatus.Voted;
                this.Enter(this.ToLinq(), reponsitory);

                reponsitory.Update<Layers.Data.Sqls.PvdCrm.EnterpriseRegisters>(new
                {
                    Summary = this.EnterpriseRegister.Summary
                }, item => item.ID == this.ID);

                tran.Commit();
            }
            this.VotedSuccess?.Invoke(this, new SuccessEventArgs(this));
        }

        #endregion
        /// <summary>
        /// 上传的 文件
        /// </summary>
        /// <returns></returns>
        private List<FilesDescription> FilesToList()
        {
            List<FilesDescription> listFile = new List<FilesDescription>();
            if (this.Lisences?.Count() > 0)
            {
                foreach (var item in this.Lisences)
                {
                    var file = new FilesDescription
                    {
                        CustomName = item.FileName,
                        Url = item.CallUrl,
                        Type = CrmFileType.License,
                        EnterpriseID = this.ID,
                        CreatorID = this.CreatorID
                    };
                    listFile.Add(file);
                }
            }

            if (!string.IsNullOrWhiteSpace(this.Logo?.CallUrl))
            {
                listFile.Add(new FilesDescription
                {
                    CustomName = this.Logo.FileName,
                    Url = this.Logo.CallUrl,
                    Type = CrmFileType.Logo,
                    EnterpriseID = this.ID,
                    CreatorID = this.CreatorID
                });
            }
            return listFile;
        }
        #region Clone
        /// <summary>
        /// 深度复制
        /// </summary>
        /// <remarks>
        /// 包涵数据本身的复制(JSON)
        /// 与实际数据的复制（DB）
        /// </remarks>
        override public object Clone()
        {
            return this.ToLinq() as object;
        }

        override public object Clone(bool isCloneDb)
        {
            var json = this.Json();
            if (isCloneDb)
            {
                return this.Clone();
            }
            else
            {
                return json.JsonTo<Supplier>();
            }
        }
        #endregion
    }

}


