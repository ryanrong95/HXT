using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.Admins
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.ClientID = Request.QueryString["id"];
            //var admin = new AdminsAllRoll()[Request.QueryString["adminid"]];
            //this.Model.Admin = new { ID = admin?.ID, Name = admin?.RealName };
            //var company = new CompaniesRoll()[Request.QueryString["companyid"]]?.Enterprise;
            //this.Model.Company = new { ID = company?.ID, Name = company?.Name };
        }
        protected object Sales()
        {
            return new YaHv.Csrm.Services.Views.Rolls.AdminsAllRoll().Where(Query()).OrderBy(item => item.ID).ToArray();
        }
        Expression<Func<YaHv.Csrm.Services.Models.Origins.Admin, bool>> Query()
        {
            string clientid = Request["id"];
            var selectedSales = Erp.Current.Crm.Clients[clientid].Sales.Select(item => item.ID).ToArray();
            Expression<Func<YaHv.Csrm.Services.Models.Origins.Admin, bool>> predicate = item => item.StaffID != null;
            //销售经理、销售
            predicate = predicate.And(item => (item.RoleID == FixedRole.SaleManager.GetFixedID() || item.RoleID == FixedRole.Sale.GetFixedID()));
            if (selectedSales.Count() > 0)
            {
                predicate = predicate.And(item => !selectedSales.Contains(item.ID));
            }
            string keyword = Request["keyword"];
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                predicate = predicate.And(item => item.ID.Contains(keyword) || item.UserName.Contains(keyword) || item.RealName.Contains(keyword));
            }
            return predicate;
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                var adminid = Request["Sale"];
                var clientid = Request.QueryString["id"];
                bool isdefault = Request["IsDefault"] == null ? false : true;
                string companyid = Request["txt_InternalCompany"];
                var client = new TradingClientsRoll()[clientid];
                if (client.Sales.Any(item => item.Company?.ID == companyid && item.ID != adminid))
                {
                    Easyui.Alert("提示", "销售公司已有销售在管理", Web.Controls.Easyui.Sign.Info);
                    return;
                }
                client.CompanyID = companyid;
                //暂不设置合作公司
               // client.AdminBinding(adminid, isdefault, Business.Trading_Sale);
                if (!string.IsNullOrEmpty(adminid) && !string.IsNullOrEmpty(clientid))
                {
                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                             nameof(Yahv.Systematic.Crm),
                                            "AdminBinding", "客户" + clientid + "，分配管理员" + adminid + ",合作过公司ID:" + companyid, "");
                }
                Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);

            }
            catch (Exception ex)
            {
                Easyui.Alert("提示", ex.Message, Web.Controls.Easyui.Sign.Info);
            }

        }
    }
}