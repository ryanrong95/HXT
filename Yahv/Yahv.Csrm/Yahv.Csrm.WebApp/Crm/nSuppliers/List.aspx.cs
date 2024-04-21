using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Crm.nSuppliers
{
    public partial class List : Web.Erp.ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.ID = Request.QueryString["id"];
            }
        }
        protected object data()
        {
            string clientid = Request.QueryString["id"];
            var query = Erp.Current.Whs.WsClients[clientid].nSuppliers.Where(Predicate());
            return this.Paging(query.OrderByDescending(item => item.CreateDate), item => new
            {
                item.ID,
                item.Grade,
                Name = item.RealEnterprise.Name,
                RegAddress = item.RealEnterprise.RegAddress,
                Uscc = item.RealEnterprise.Uscc,
                Place = Enum.GetValues(typeof(Origin)).Cast<Origin>().Any(i => i.GetOrigin().Code == item.RealEnterprise.Place) ? Enum.GetValues(typeof(Origin)).Cast<Origin>().SingleOrDefault(i => i.GetOrigin().Code == item.RealEnterprise.Place).GetOrigin()?.ChineseName : null,
                Corporation = item.RealEnterprise.Corporation,
                item.ChineseName,
                item.EnglishName,
                item.Summary,
                item.Status,
                StatusName = item.Status.GetDescription(),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.CHNabbreviation
            });
        }

        Expression<Func<nSupplier, bool>> Predicate()
        {
            Expression<Func<nSupplier, bool>> predicate = item => true;
            var name = Request["s_name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Enterprise.Name.Contains(name)
                || item.ChineseName.Contains(name)
                || item.EnglishName.Contains(name));
            }

            return predicate;
        }
        protected object Del()
        {
            var wsclient = Erp.Current.Whs.WsClients[Request["clientid"]];
            if (wsclient == null)
            {
                return new { msg = "客户不存在", success = false };
            }
            else if (wsclient.nSuppliers[Request["supplierid"]] == null)
            {
                return new { msg = "供应商不存在不存在", success = false };
            }
            else
            {
                var nsupplier = wsclient.nSuppliers[Request["supplierid"]];
                nsupplier.Abandon();
                //调用接口
                var api = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
                if (!string.IsNullOrWhiteSpace(api))
                {
                    Unify(api, wsclient.Enterprise.Name, nsupplier.RealEnterprise.Name);
                }
                return new { msg = "操作成功", success = true };
            }
        }
        object Unify(string api, string clientname, string suppliername)
        {
            var response = HttpClientHelp.CommonHttpRequest(api + "/clients/suppliers?name=" + clientname + "&supplierName=" + suppliername, "DELETE");
            return response;
        }
    }
}
