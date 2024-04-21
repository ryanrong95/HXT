using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Approvals.Address
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.AddressType = ExtendsEnum.ToArray<AddressType>().Select(item => new
            {
                value = item,
                text = item.GetDescription()
            });
        }

        protected object data()
        {
            var query = Erp.Current.CrmPlus.Addresses.Where(Predicate()).Where(x=>x.RelationType==RelationType.Trade &&x.Status==AuditStatus.Waiting);
            var result = this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                ClientName = item.Enterprise.Name,
                AddressType = item.AddressType.GetDescription(),
                item.District,
                item.Context,
                item.Contact,
                item.Phone,
                item.PostZip,
                item.Status,
                creator = item.Admin.RealName,
                StatusDes = item.Status.GetDescription(),
                CreteDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            }));

            return result;

        }


        Expression<Func<Service.Models.Origins.Address, bool>> Predicate()
        {
            Expression<Func<Yahv.CrmPlus.Service.Models.Origins.Address, bool>> predicate = item => true;
            var name = Request["Name"];
            var endDate = Request["EndDate"];
            var startDate = Request["StartDate"];
            var type = Request["AddressType"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Enterprise.Name.Contains(name));
            }

            AddressType addressType;
            if (Enum.TryParse(type, out addressType) && addressType != 0)
            {
                predicate = predicate.And(item => item.AddressType == addressType);
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
            return predicate;
        }


        protected void Approval()
        {
            var id = Request.Form["id"];
            var entity = Erp.Current.CrmPlus.Addresses[id];
            entity.Status = AuditStatus.Normal;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enable();
        }

        protected void ApproveRefuse()
        {
            var id = Request.Form["id"];
            var summary = Request.Form["Summary"];
            var entity = Erp.Current.CrmPlus.Addresses[id];
            entity.Summary = summary;
            entity.Status = AuditStatus.Voted;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Refuse();
        }



        //private void Entity_ErrorSuccess(object sender, Usually.ErrorEventArgs e)
        //{
        //    Response.Write((new { success = false, message = e.Message }).Json());
        //}
        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Yahv.CrmPlus.Service.Models.Origins.Address;

            var applyTask = new Service.Models.Origins.ApplyTask();
            applyTask.MainID = entity.ID;
            applyTask.MainType = MainType.Clients;
            applyTask.ApplyTaskType = ApplyTaskType.ClientAddress;
            applyTask.ApplierID = Erp.Current.ID;
            if (entity.Status == AuditStatus.Voted)
            {
            applyTask.Status = ApplyStatus.Voted;
            }
            if (entity.Status == AuditStatus.Normal)
            {
                applyTask.Status = ApplyStatus.Allowed;
            }
            applyTask.Approve();

            LogsOperating.LogOperating(Erp.Current, entity.ID, $"审批了客户地址:{ entity.ID}");
            var data = new { success = true, message = "操作成功" };
            Response.Write(data.Json());
            // Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }



    }
}