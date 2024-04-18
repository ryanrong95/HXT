using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;

namespace WebApp.Client.Supplier
{
    public partial class Edit : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadCombobox();
            LoadData();
        }

        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            this.Model.ID = id;

            string supplierid = Request.QueryString["SupplierID"];
            this.Model.ClientSupplierID = supplierid ?? "";

            if (!string.IsNullOrEmpty(supplierid))
            {
                var supplier = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSuppliers[supplierid];
                this.Model.ClientSupplierData = new
                {
                    ID = supplier.ID,
                    ChineseName = supplier.ChineseName,
                    Name = supplier.Name.Replace("'", "#39;"),
                    Summary = supplier.Summary?.Replace("'", "#39;"),
                    Rank = supplier.SupplierGrade,
                    supplier.Place
                }.Json();
            }
            else
            {
                this.Model.ClientSupplierData = null;
            }
        }

        protected void LoadCombobox()
        {
            //供应商等级
            this.Model.SuplierRanks = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.SupplierGrade>().Select(item => new { item.Key, item.Value }).Json();
            this.Model.Places = Needs.Wl.Admin.Plat.AdminPlat.Countries.Select(x => new { x.Code, x.Name }).Json();
        }

        protected void SaveClientSupplier()
        {
            string Model = Request.Form["Model"].Replace("&quot;", "\'").Replace("&amp;", "&");
            dynamic model = Model.JsonTo<dynamic>();
            string suplierID = model.SuplierID;
            // int rankID = model.RankID;
            string place = model.PlaceID;
            string englishname = model.Name;
            string chineseName = model.ChineseName;


            Regex regex = new Regex(@"\s+", RegexOptions.Singleline);
            string name = regex.Replace(englishname, " ").Trim();
            name = Needs.Utils.InputTextExtends.SBCToDBC(name);
            name = name.Replace("#39;", "\'");

            var supplier = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSuppliers[suplierID] ?? new Needs.Ccs.Services.Models.ClientSupplier();
            supplier.Name = name;
            supplier.ClientID = model.ClientID;
            supplier.ChineseName = string.IsNullOrEmpty(chineseName) ? name : chineseName;

            //supplier.SupplierGrade = SupplierGrade.Second;
            if (string.IsNullOrEmpty(suplierID)) //如果是新增，根据供应商英文名称找出一个最新设置的等级
            {
                supplier.SupplierGrade = GetGradeForNewSupplier(name);
            }

            supplier.Place = place;
            supplier.Summary = model.Summary;
            supplier.IsShowClient = model.IsShowClient;
            supplier.EnterError += Supplier_EnterError;
            supplier.EnterSuccess += Supplier_EnterSuccess;
            if (string.IsNullOrEmpty(URL))
            {
                #region   调用前
                supplier.Enter();
                #endregion
            }
            else
            {
                #region 调用后
                try
                {
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
                            Grade = (int)supplier.SupplierGrade,
                            Status = supplier.IsShowClient ? 200 : 400,
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
                                Place = place
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
                }
                catch (Exception)
                {

                    throw;
                }

                #endregion
            }
        }

        /// <summary>
        /// 给新增供应商获取一个等级
        /// </summary>
        /// <returns></returns>
        private SupplierGrade GetGradeForNewSupplier(string supplierEngName)
        {
            SupplierGrade grade = SupplierGrade.Second;

            var gradeInfo = new SupplierGradeByEngNameView(supplierEngName).GetGrade();
            if (gradeInfo != null && gradeInfo.SupplierGrade != null)
            {
                grade = (SupplierGrade)gradeInfo.SupplierGrade;
            }

            return grade;
        }


        /// <summary>
        ///判断名称是否重复
        /// </summary>
        /// <returns></returns>
        protected bool IsExitName()
        {
            var result = false;
            var name = Request.Form["Name"];
            string id = Request.Form["ID"];
            Regex regex = new Regex(@"\s+", RegexOptions.Singleline);
            name = regex.Replace(name, " ").Trim();
            string ClientID = Request.Form["ClientID"];
            if (!string.IsNullOrWhiteSpace(id))
            {
                result = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSuppliers.Where(x => x.ID != id && x.ClientID == ClientID && x.Name == name && x.Status == Status.Normal).Count() < 1;
            }
            else
            {
                int count = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSuppliers.Where(x => x.ClientID == ClientID && x.Name == name && x.Status == Status.Normal).Count();
                result = count < 1;
            }
            return result;
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Supplier_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Supplier_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }


    }
}