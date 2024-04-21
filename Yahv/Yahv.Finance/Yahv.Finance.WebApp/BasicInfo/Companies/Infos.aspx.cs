using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Layers.Data.Sqls.PvcCrm;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using WsClientsTopView = Yahv.Finance.Services.Views.WsClientsTopView;

namespace Yahv.Finance.WebApp.BasicInfo.Companies
{
    public partial class Infos : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Index()
        {
            string callback = Request.QueryString["callback"];

            using (var query1 = new EnterprisesRoll())
            {
                var view = query1;
                var ets = view.Select(item => new
                {
                    item.ID,
                    item.Name
                }).Take(20);

                Response.Write(callback + "(" + new { Code = "200", Data = ets }.Json() + ")");
            }
        }

        protected void Search()
        {
            string name = Request.QueryString["name"];
            string callback = Request.QueryString["callback"];

            try
            {
                using (var query1 = new EnterprisesRoll())
                {
                    var view = query1;
                    var all = view.Where(t => t.Name.Contains(name))
                        .OrderBy(item => item.Name)
                        .Take(20)
                        .ToArray()
                        .Select(item => new
                        {
                            ID = item.ID,
                            Name = item.Name,
                        });

                    Response.Write(callback + "(" + new { Code = "200", Data = all }.Json() + ")");
                }
            }
            catch (Exception ex)
            {
                Response.Write(callback + "(" + new { Code = "300", Data = ex.Message }.Json() + ")");
            }
        }

        protected void Getbyid()
        {
            string id = Request.QueryString["id"];
            string callback = Request.QueryString["callback"];

            try
            {
                using (var query1 = new EnterprisesRoll())
                {
                    var view = query1;
                    var enterprise = view.Where(t => t.ID == id).FirstOrDefault();

                    Response.Write(callback + "(" + new { Code = "200", Data = new { ID = enterprise.ID, Name = enterprise.Name } }.Json() + ")");
                }
            }
            catch (Exception ex)
            {
                Response.Write(callback + "(" + new { Code = "300", Data = ex.Message }.Json() + ")");
            }
        }

        protected void Index_Company()
        {
            string callback = Request.QueryString["callback"];

            using (var query1 = new EnterprisesRoll())
            {
                var view = query1.Where(item => (item.Type & EnterpriseAccountType.Company) != 0);
                var ets = view.Select(item => new
                {
                    item.ID,
                    item.Name
                }).Take(20);

                Response.Write(callback + "(" + new { Code = "200", Data = ets }.Json() + ")");
            }
        }

        protected void Search_Company()
        {
            string name = Request.QueryString["name"];
            string callback = Request.QueryString["callback"];

            try
            {
                using (var query1 = new EnterprisesRoll())
                {
                    var view = query1.Where(item => (item.Type & EnterpriseAccountType.Company) != 0);
                    var all = view.Where(t => t.Name.Contains(name))
                        .OrderBy(item => item.Name)
                        .Take(20)
                        .ToArray()
                        .Select(item => new
                        {
                            ID = item.ID,
                            Name = item.Name,
                        });

                    Response.Write(callback + "(" + new { Code = "200", Data = all }.Json() + ")");
                }
            }
            catch (Exception ex)
            {
                Response.Write(callback + "(" + new { Code = "300", Data = ex.Message }.Json() + ")");
            }
        }

        protected void Getbyid_Company()
        {
            string id = Request.QueryString["id"];
            string callback = Request.QueryString["callback"];

            try
            {
                using (var query1 = new EnterprisesRoll())
                {
                    var view = query1.Where(item => (item.Type & EnterpriseAccountType.Company) != 0);
                    var enterprise = view.Where(t => t.ID == id).FirstOrDefault();

                    Response.Write(callback + "(" + new { Code = "200", Data = new { ID = enterprise.ID, Name = enterprise.Name } }.Json() + ")");
                }
            }
            catch (Exception ex)
            {
                Response.Write(callback + "(" + new { Code = "300", Data = ex.Message }.Json() + ")");
            }
        }


        protected void Index_WsClient()
        {
            string callback = Request.QueryString["callback"];

            using (var query1 = new WsClientsTopView())
            {
                var view = query1;
                var ets = view.Select(item => new
                {
                    item.ID,
                    item.Name
                }).Take(20);

                Response.Write(callback + "(" + new { Code = "200", Data = ets }.Json() + ")");
            }
        }

        protected void Search_WsClient()
        {
            string name = Request.QueryString["name"];
            string callback = Request.QueryString["callback"];

            try
            {
                using (var query1 = new WsClientsTopView())
                using (var query2 = new EnterprisesRoll())
                {
                    var wsClients = query1.Select(item => new { item.ID, item.Name }).ToArray();
                    var enterprises = query2.Where(item => (item.Type & EnterpriseAccountType.Client) != 0)
                        .Select(item => new { item.ID, item.Name }).ToArray();

                    var view = wsClients.Concat(enterprises).Distinct();

                    var all = view.Where(t => t.Name.Contains(name))
                        .OrderBy(item => item.Name)
                        .Take(20)
                        .ToArray()
                        .Select(item => new
                        {
                            ID = item.ID,
                            Name = item.Name,
                        });

                    Response.Write(callback + "(" + new { Code = "200", Data = all }.Json() + ")");
                }
            }
            catch (Exception ex)
            {
                Response.Write(callback + "(" + new { Code = "300", Data = ex.Message }.Json() + ")");
            }
        }

        protected void Getbyid_WsClient()
        {
            string id = Request.QueryString["id"];
            string callback = Request.QueryString["callback"];

            try
            {
                using (var query1 = new WsClientsTopView())
                using (var query2 = new EnterprisesRoll())
                {
                    var wsClients = query1.Select(item => new { item.ID, item.Name }).ToArray();
                    var enterprises = query2.Where(item => (item.Type & EnterpriseAccountType.Client) != 0)
                        .Select(item => new { item.ID, item.Name }).ToArray();

                    var view = wsClients.Concat(enterprises).Distinct();

                    var enterprise = view.Where(t => t.ID == id).FirstOrDefault();

                    Response.Write(callback + "(" + new { Code = "200", Data = new { ID = enterprise.ID, Name = enterprise.Name } }.Json() + ")");
                }
            }
            catch (Exception ex)
            {
                Response.Write(callback + "(" + new { Code = "300", Data = ex.Message }.Json() + ")");
            }
        }
    }
}