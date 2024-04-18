using Needs.Ccs.Services.Models;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;

namespace WebApp.Client.Supplier
{
    public partial class List : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            this.Model.ID = id;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string id = Request.QueryString["ID"];
            string ChineseName = Request.QueryString["ChineseName"];
            string EnglishName = Request.QueryString["Name"];

            var suppliers = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSuppliers.AsQueryable();
            suppliers = suppliers.Where(t => t.ClientID == id && t.Status != Needs.Ccs.Services.Enums.Status.Delete);
            if (!string.IsNullOrEmpty(ChineseName))
            {
                suppliers = suppliers.Where(t => t.ChineseName.Contains(ChineseName));
            }

            Func<Needs.Ccs.Services.Models.ClientSupplier, object> convert = supplier => new
            {
                supplier.ID,
                ChineseName = supplier.ChineseName,
                EnglishName = supplier.Name.Replace("#39;", "'"),
                CreateDate = supplier.CreateDate.ToString("yyyy-MM-dd"),
                UpdateDate = supplier.UpdateDate.ToString(),
                Status = supplier.Status.GetDescription(),
                SupplierGrade=supplier.SupplierGrade.GetDescription(),
                supplier.Place,
                Summary = supplier.Summary
            };
            this.Paging(suppliers, convert);
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected void DeleteSupplier()
        {
            string id = Request.Form["ID"];
            var supplier = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientSuppliers[id];
            supplier.AbandonError += Supplier_AbandonError;
            supplier.AbandonSuccess += Supplier_AbandonSuccess;
            supplier.Abandon();
        }

        private void Supplier_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            string requestUrl = URL + "/CrmUnify/DelWsSuppliers";
            HttpResponseMessage response = new HttpResponseMessage();
            string requestClientUrl = requestUrl;//请求地址
            var supplier = sender as ClientSupplier;
            var apiclient = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[supplier.ClientID];
            var entity = new ApiModel.ClientSupplier()
            {
                client = new EnterpriseObj
                {
                    AdminCode = "",
                    District = "",
                    Corporation = apiclient.Company.Corporate,
                    Name = apiclient.Company.Name,
                    RegAddress = apiclient.Company.Address,
                    Uscc = apiclient.Company.Code,
                    Status = 200
                },
                supplier = new ApiModel.Supplier
                {
                    ChineseName = supplier.ChineseName,
                    EnglishName = supplier.Name,
                    Grade = 2,
                    Status = 200,
                    Summary = supplier.Summary,
                    Enterprise = new EnterpriseObj
                    {

                        Name = supplier.Name,
                    }
                }
            };
           
            string ApiSupplier = JsonConvert.SerializeObject(entity);
            response = new HttpClientHelp().HttpClient("POST", requestClientUrl, ApiSupplier);
            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                Response.Write((new { success = false, message = "请求会员接口失败：" }).Json());
                return;
            }
            Response.Write((new { success = true, message = "删除成功", ID = e.Object }).Json());
        }

        private void Supplier_AbandonError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = e.Message }).Json());
        }
    }
}