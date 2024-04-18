using Needs.Ccs.Services.Models;
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

namespace WebApp.Client.Consignee
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
            var consignees = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientConsignees.AsQueryable();
            consignees = consignees.Where(t => t.ClientID == id && t.Status != Needs.Ccs.Services.Enums.Status.Delete);
            this.Model.Count = consignees.Count();
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string id = Request.QueryString["ID"];

            var consignees = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientConsignees.AsQueryable();
            consignees = consignees.Where(t => t.ClientID == id && t.Status != Needs.Ccs.Services.Enums.Status.Delete);

            Func<Needs.Ccs.Services.Models.ClientConsignee, object> convert = consignee => new
            {
                consignee.ID,
                consignee.Name,
                ContactName = consignee.Contact.Name,
                consignee.Contact.Mobile,
                consignee.Address,
                IsDefault = consignee.IsDefault ? "是" : "否",
                CreateDate = consignee.CreateDate.ToShortDateString(),
                Status = consignee.Status.GetDescription(),
            };

            this.Paging(consignees, convert);
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected void DeleteClientConsignee()
        {
            //string ids = Request.Form["ID"];
            //ids.Split(',').ToList().ForEach(t =>
            //{
            //    var consignee = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientConsignees[t];
            //    consignee.Status = Needs.Ccs.Services.Enums.Status.Delete;
            //    consignee.Enter();
            //});

            string id = Request.Form["ID"];
            var consignee = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientConsignees[id];
            consignee.Status = Needs.Ccs.Services.Enums.Status.Delete;
            if (string.IsNullOrEmpty(URL))
            {
                #region   调用前
                consignee.Enter();
                #endregion
            }
            else
            {
                #region 调用后
                try
                {
                    string requestUrl = URL + "/CrmUnify/DelConsignee";
                    HttpResponseMessage response = new HttpResponseMessage();
                    string requestClientUrl = requestUrl;//请求地址
                    var client = Needs.Wl.Admin.Plat.AdminPlat.Clients[consignee.ClientID];
                    //var addressdata = new ApiAddressHelp().GetReceiver(consignee.Address);
                    var entity = new ApiModel.ClientConsigin()
                    {
                        Enterprise = new EnterpriseObj
                        {
                            AdminCode = "",
                            District = "",
                            Corporation = client.Company.Corporate,
                            Name = client.Company.Name,
                            RegAddress = client.Company.Address,
                            Uscc = client.Company.Code,
                            Status = 200
                        },
                        Title = consignee.Name,
                        DyjCode = "",
                        District = 1,
                        Province ="",
                        City ="",
                        Land = "",
                        Name = consignee.Contact.Name,
                        Mobile = consignee.Contact.Mobile,
                        Address = consignee.Address,
                        Tel = consignee.Contact.Mobile,
                        Email = consignee.Contact.Email,
                        IsDefault = consignee.IsDefault,
                        Status = 200,
                        Creator = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName,
                        Postzip = "",
                        CreateDate = DateTime.Now.ToString(),
                        UpdateDate = DateTime.Now.ToString(),
                    };
                    string apiclient = JsonConvert.SerializeObject(entity);
                    response = new HttpClientHelp().HttpClient("POST", requestClientUrl, apiclient);
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                    {
                        Response.Write((new { success = false, message = "请求会员接口失败：" }).Json());
                        return;
                    }
                    consignee.Enter();
                }
                catch (Exception)
                {

                    throw;
                }
                #endregion
            }

        }
    }
}