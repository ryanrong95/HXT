using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views.Rolls.Supplier;
using Needs.Underly;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;

namespace WebApp.Supplier
{
    public partial class List : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {

                this.Model.SuplierGrade = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.SupplierGrade>().Select(item => new { item.Key, item.Value }).Json();
                //客户等级
            }
        }



        protected void data()
        {

            string clientName = Request.QueryString["ClientName"];
            string englishName = Request.QueryString["EnglishName"];
            var supplierGrade = Request.QueryString["SupplierGrade"];
            var suppliers = new SupplierRoll().OrderByDescending(t => t.CreateDate).AsQueryable();
            if (!string.IsNullOrEmpty(englishName))
            {
                suppliers = suppliers.Where(item => item.Name.Contains(englishName.Trim()));
            }
            if (!string.IsNullOrEmpty(clientName))
            {

                suppliers = suppliers.Where(item => item.ClientName.Contains(clientName.Trim()));
            }
            if (!string.IsNullOrEmpty(supplierGrade))
            {
                int grade = Int32.Parse(supplierGrade);
                suppliers = suppliers.Where(item => item.SupplierGrade == (SupplierGrade)grade);
            }

            Func<Needs.Ccs.Services.Models.SupplierModel, object> convert = supplier => new
            {
                supplier.ID,
                EnglishName = supplier.Name,
                supplier.ChineseName,
                supplier.ClientName,
                supplier.ClientCode,
                SupplierGrade = supplier.SupplierGrade.GetDescription(),
                SupplierGradeValue = supplier.SupplierGrade,
                Place = ((Origin)Enum.Parse(typeof(Origin), string.IsNullOrEmpty(supplier.Place) ? nameof(Origin.Unknown) : supplier.Place)).GetDescription(),
                CreateDate = supplier.CreateDate.ToShortDateString(),
                Summary = supplier.Summary,
            };
            this.Paging(suppliers, convert);

        }

        protected void EditRank()
        {
            try
            {
                var id = Request.Form["ID"];
                var rank = Convert.ToInt16(Request.Form["Rank"]);
                var summary = Request.Form["Summary"];
                var supplier = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSuppliers[id] ?? new Needs.Ccs.Services.Models.ClientSupplier();
                supplier.SupplierGrade = (SupplierGrade)rank;
                supplier.Summary = summary;
                string requestUrl = URL + "/CrmUnify/WsSupplierEnter";
                HttpResponseMessage response = new HttpResponseMessage();
                string requestClientUrl = requestUrl;//请求地址
                var apiclient = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[supplier.ClientID];

                var entity = new ApiModel.ClientSupplier()
                {
                    Creator = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName,
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
                        Grade = rank,
                        Status = 200,
                        Summary = supplier.Summary,

                        Enterprise = new EnterpriseObj
                        {
                            AdminCode = "",
                            District = "1",
                            Corporation = "",
                            Name = supplier.Name,
                            RegAddress = "",
                            Uscc = "",
                            Status = 200,
                            Place = supplier.Place
                        }
                    }
                };
                string apiSupplier = JsonConvert.SerializeObject(entity);
                response = new HttpClientHelp().HttpClient("POST", requestClientUrl, apiSupplier);
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    Response.Write((new { success = false, message = "请求会员接口失败：" }).Json());
                    return;
                }
                supplier.Enter();

                Response.Write((new { success = true, message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {

                Response.Write((new { success = false, message = ex.Message }).Json());
            }


        }

    }
}
