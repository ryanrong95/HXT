using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Approvals.PublicSeas
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsAction && !IsPostBack)
            {
                loadData();
            }
        }

        protected void loadData()
        {
            Dictionary<string, string> typelst = new Dictionary<string, string>() { { "0", "全部" } };
            this.Model.ClientType = typelst.Concat(ExtendsEnum.ToDictionary<Yahv.Underly.CrmPlus.ClientType>()).Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value
            });

            ////认领人
            //this.Model.Sales = new YaHv.CrmPlus.Services.Views.Rolls.AdminsAllRoll().Where(item => item.RoleID == FixedRole.Sale.GetFixedID() || item.RoleID == FixedRole.SaleA.GetFixedID() || item.RoleID == FixedRole.SaleManager.GetFixedID()).Select(item => new
            //{
            //    value = item.ID,
            //    text = $"{item.RealName}-{item.RoleName}"
            //});
        }

        protected object data()
        {


            var query = new PublicClientsExtendRoll().Where(Predicate());
            var result = this.Paging(query.ToArray().Select(item => new
            {
                item.ID,
                item.Name,
                ClientType = item.ClientType.GetDescription(),
                //item.EnterpriseRegister.Nature,
                IsInternational = item.EnterpriseRegister.IsInternational,
                item.IsMajor,
                item.IsSpecial,
                District = item.DistrictDes,
                item.Status,
                item.Grade,
                Vip = item.Vip,
                StatusDes = item.Status.GetDescription(),
                ConductType = item.Conduct?.ConductType,
                ConductTypeDes = item.Conduct?.ConductType.GetDescription(),
                Company = item.Relation.Company.Name,
                //Creator = item.Admin?.RealName,
                Claimant = item.Relation?.Admin.RealName,
                ClaimTime = item.Relation?.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.EnterpriseRegister.Industry,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
            }));

            return result;
        }
        Expression<Func<Yahv.CrmPlus.Service.Models.Origins.PublicClient, bool>> Predicate()
        {
            Expression<Func<Yahv.CrmPlus.Service.Models.Origins.PublicClient, bool>> predicate = item => true;
            var name = Request["s_name"];
            var clientType = Request["clientType"];
            var startDate = Request["startDate"];
            var endDate = Request["endDate"];

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                var from = DateTime.Parse(startDate);
                predicate = predicate.And(item => item.CreateDate >= from);

            }
            if (!string.IsNullOrEmpty(endDate))
            {
                var to = DateTime.Parse(endDate);
                predicate.And(item => item.CreateDate < to.AddDays(1));
            }

            Yahv.Underly.CrmPlus.ClientType clientTypeData;
            if (Enum.TryParse(clientType, out clientTypeData))
            {
                predicate = predicate.And(item => item.ClientType == clientTypeData);
            }
            return predicate;

        }




        protected void Approve()
        {
            try
            {
                bool result = bool.Parse(Request["result"]);
                string id = Request["id"];
                string conductType = Request["conductType"];

                var entity = new PublicClientsExtendRoll().FirstOrDefault(x => x.ID == id && x.Conduct.ConductType == (ConductType)int.Parse(conductType));
                if (entity != null)
                {
                    var relationsCount = new RelationsRoll().Count(item => item.ClientID == entity.ID && item.CompanyID == entity.Relation.CompanyID && item.Status == AuditStatus.Normal);
                    if (relationsCount > 0)
                    {
                        Response.Write(new { success = false, code = 100, message = "审批失败,该客户已被认领" }.Json());
                        return;
                    }


                }

                entity.ClientStatus = result == true ? entity.ClientStatus = AuditStatus.Normal : AuditStatus.Voted;
                //申请任务
                #region  业务类型
                //添加业务类型
                entity.Conduct.EnterpriseID = id;
                entity.Conduct.IsPublic = false;
                #endregion

                #region   我方合作公司
                entity.Relation.Status = AuditStatus.Normal;
                #endregion
                entity.EnterSuccess += Entity_EnterSuccess;

                //理论上通过了，就等待下一步或是角色操作了
                //否决了一般会有其他操作因此建议增加 Allowed 与 Voted 事件
                if (result)
                {
                    entity.Allowed(Erp.Current.ID);
                }
                else
                {
                    entity.Voted(Erp.Current.ID);

                }
            }
            catch (Exception ex)
            {
                Response.Write(new { success = false, code = 100, message = "操作失败" + ex }.Json());

            }

        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var client = sender as PublicClient;
            Service.LogsOperating.LogOperating(Erp.Current, client.ID, $"审批了公海客户,{client.Name}");

            (new ApplyTask()
            {
                MainID = client.ID,
                MainType = MainType.Clients,
                ApplyTaskType = ApplyTaskType.ClientPublic,
                ApplierID = client.Relation.OwnerID,
                ApproverID = Erp.Current.ID,
                Status = ApplyStatus.Allowed
            }).EnterExtend();
            Response.Write(new { success = true, code = 200, message = "" }.Json());
        }

    }
}