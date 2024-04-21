using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Yahv.CrmPlus.Service;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Contacts
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.ID = Request.QueryString["id"];
                Dictionary<string, string> list = new Dictionary<string, string>() { { "0", "全部" } };
                //状态
                this.Model.Status = list.Concat(ExtendsEnum.ToDictionary<DataStatus>()).Select(item => new
                {
                    value = int.Parse(item.Key),
                    text = item.Value.ToString()
                });
            }
        }

        protected object data()
        {
            string id = Request.QueryString["id"];
            var query = Erp.Current.CrmPlus.MyContacts[id, RelationType.Suppliers].Where(Predicate());

            var result = this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.Name,
                item.Mobile,
                item.Tel,
                item.Department,
                item.Positon,
                item.Gender,
                item.QQ,
                item.Wx,
                item.Email,
                item.Status,
                StatusDes = item.Status.GetDescription(),
                Creator = item.Admin?.RealName,
                CreteDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            }));

            return result;

        }


        Expression<Func<Service.Models.Origins.Contact, bool>> Predicate()
        {
            Expression<Func<Yahv.CrmPlus.Service.Models.Origins.Contact, bool>> predicate = item => true;
            var name = Request["s_name"];
            var status = Request["selStatus"];
            var mobile = Request["Mobile"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }
            //手机号或者电话
            if (!string.IsNullOrWhiteSpace(mobile))
            {
                predicate = predicate.And(item => item.Mobile.Contains(mobile) || item.Tel.Contains(mobile));
            }

            DataStatus dataStatus;
            if (Enum.TryParse(status, out dataStatus) && dataStatus != 0)
            {
                predicate = predicate.And(item => item.Status == dataStatus);
            }
            return predicate;
        }


        /// <summary>
        /// 停用
        /// </summary>
        protected void Closed()
        {
            var id = Request.Form["ID"];
            try
            {

                var entity = Erp.Current.CrmPlus.MyContacts[id];
                entity.Closed();
                LogsOperating.LogOperating(Erp.Current, entity.EnterpriseID, $"停用联系人:{ entity.ID}");

            }
            catch (Exception ex)
            {
                LogsOperating.LogOperating(Erp.Current, id, $"停用联系人 操作失败" + ex);
            }
        }


        /// <summary>
        /// 启用
        /// </summary>
        protected void Enable()
        {
            var id = Request.Form["ID"];
            try
            {
                var entity = Erp.Current.CrmPlus.MyContacts[id];
                entity.Enable();
                LogsOperating.LogOperating(Erp.Current, entity.EnterpriseID, $"启用联系人:{ entity.ID}");
            }
            catch (Exception ex)
            {
                LogsOperating.LogOperating(Erp.Current, id, $"启用联系人 操作失败" + ex);
            }
        }
    }
}