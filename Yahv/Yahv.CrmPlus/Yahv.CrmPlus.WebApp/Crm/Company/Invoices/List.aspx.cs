using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Company.Invoices
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {  if (!IsPostBack)
            {
               
                LoadData();
            }
          
        }


        protected void LoadData()
        {
            Dictionary<string, string> list = new Dictionary<string, string>() { { "0", "全部" } };
            //状态
            this.Model.Status = list.Concat(ExtendsEnum.ToDictionary<DataStatus>()).Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });
            this.Model.ID = Request.QueryString["id"];

        }

        protected object data()
        {
            string id = Request.QueryString["id"];
            var query = Erp.Current.CrmPlus.Invoices[id,RelationType.Own].Where(Predicate());

            var result = this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.Enterprise.Name,
                item.Address,
                item.Tel,
                item.Bank,
                item.Account,
                item.Status,
                StatusDes = item.Status.GetDescription(),
                Creator = item.Admin?.RealName,
                CreteDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            }));

            return result;
           
        }


        Expression<Func<Service.Models.Origins.Invoice, bool>> Predicate()
        {
            Expression<Func<Yahv.CrmPlus.Service.Models.Origins.Invoice, bool>> predicate = item => true;
            var name = Request["s_name"];
            var status = Request["selStatus"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Enterprise.Name.Contains(name));
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
            var companyID = Request.Form["CompanyID"];
            try
            {

                var entity = Erp.Current.CrmPlus.Companies[companyID].Invoices[id];
                entity.Closed();
                LogsOperating.LogOperating(Erp.Current, entity.Enterprise.ID, $"停用发票:{ entity.ID}");

            }
            catch (Exception ex)
            {

                LogsOperating.LogOperating(Erp.Current, companyID, $"停用发票 操作失败" + ex);
            }
        }


        /// <summary>
        /// 启用
        /// </summary>
        protected void Enable()
        {
            var id = Request.Form["ID"];
            var companyID = Request.Form["CompanyID"];
            try
            {

                var entity = Erp.Current.CrmPlus.Companies[companyID].Invoices[id];
                entity.Enable();
                LogsOperating.LogOperating(Erp.Current, entity.Enterprise.ID, $"启用发票:{ entity.ID}");

            }
            catch (Exception ex)
            {

                LogsOperating.LogOperating(Erp.Current, companyID, $"启用发票 操作失败" + ex);
            }
        }
    }
}