using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using Needs.Utils.Descriptions;
using Needs.Utils.Npoi;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ClientAgreementChangeApplyModel : IUnique, IPersist
    {
        string id;
        public string ID
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        public string ClientID { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string ApplyID { get; set; }
        public Enums.AgreementChangeApplyStatus Status { get; set; }

        public int IntStatus { get; set; }
        public string AdminID { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }
        public Enums.ClientRank ClientRank { get; set; }
        // public Enums.ClientNature ClientNature { get; set; }
        public int? ClientNature { get; set; }
        public string MerchandiserName { get; set; }

        public string ServiceManager { get; set; }
        public string RealName { get; set; }

        public Enums.AgreementChangeType AgreementChangeType { get; set; }

        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string StartDate { get; set; }
        public string Address { get; set; }
        public string Corporate { get; set; }
        public string From { get; set; }
        public string CustomName { get; set; }

        public string DepartmentCode { get; set; }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public void Enter()
        {

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AgreementChangeApplies>().Where(item => item.Status != (int)Enums.AgreementChangeApplyStatus.Delete
               && item.Status != (int)Enums.AgreementChangeApplyStatus.Effective).Count(item => item.ClientID == this.ClientID);
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.AgreementChangeApplies
                    {
                        ID = this.ID,
                        ClientID = this.ClientID,
                        AdminID = this.AdminID,
                        Status = (int)this.Status,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Summary = this.Summary
                    });
                    if (this.Status == AgreementChangeApplyStatus.RiskAuditing)
                    {
                        this.Summary = "业务员【" + this.RealName + "】提交了协议变更申请，等待风控审核";
                    }
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.Logs
                    {
                        ID = Needs.Overall.PKeySigner.Pick(PKeyType.Logs),
                        Name = "协议变更申请",
                        MainID = this.ID,
                        AdminID = this.AdminID,
                        Summary = Summary,
                        Json = "",
                        CreateDate = DateTime.Now
                    });
                }
            }

            this.OnEnterSuccess();
        }
        public void Update()
        {
            var RiskName = System.Configuration.ConfigurationManager.AppSettings["RiskManagementName"];        

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AgreementChangeApplies>().Count(item => item.ID == this.ApplyID);
                if (count != 0)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.AgreementChangeApplies>(new
                    {
                        Status = (int)this.Status,
                        UpdateDate = DateTime.Now,
                        Summary = this.Summary
                    }, item => item.ID == this.ApplyID);
                }
                if (this.Status == AgreementChangeApplyStatus.Auditing)
                {
                    //var RealName = this.RealName;
                    //if (RealName == "张令金" || RealName == "张庆永")
                    //{
                    //    RealName = RiskName;
                    //}
                    this.Summary = "风控【" + RiskName + "】审核通过了协议变更申请，等待经理审批";
                }
                else if (this.Status == AgreementChangeApplyStatus.Effective)
                {
                    //var RealName = this.RealName;
                    //if (RealName == "张令金")
                    //{
                    //    RealName = "张庆永";
                    //}
                    this.Summary = "经理【张庆永】审批通过了协议变更申请，协议变更申请生效";
                }
                else if (this.Status == AgreementChangeApplyStatus.Delete)
                {
                    if (this.From == "Audit")
                    {
                        //var RealName = this.RealName;
                        //if (RealName == "张令金" || RealName == "张庆永")
                        //{
                        //    RealName = RiskName;
                        //}
                        this.Summary = "风控【" + RiskName + "】拒绝了协议变更申请";
                    }
                    else
                    {
                        //var RealName = this.RealName;
                        //if (RealName == "张令金")
                        //{
                        //    RealName = "张庆永";
                        //}
                        this.Summary = "经理【张庆永】拒绝了协议变更申请";
                    }
                }
                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.Logs
                {
                    ID = Needs.Overall.PKeySigner.Pick(PKeyType.Logs),
                    Name = "协议变更申请",
                    MainID = this.ApplyID,
                    AdminID = this.AdminID,
                    Summary = Summary,
                    Json = "",
                    CreateDate = DateTime.Now
                });
            }

            this.OnEnterSuccess();
        }
        virtual protected void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }

    public class AgreementChangeDateil
    {
        public string StartDate { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
