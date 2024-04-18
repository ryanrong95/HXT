using Needs.Utils.Descriptions;
using Needs.Utils.Http;
using Needs.Utils.Serializers;
using Newtonsoft.Json.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Needs.Utils.Http.HttpHelper;

namespace WebApp.Crm.MyClients
{
    /// <summary>
    /// 客户编辑页面
    /// </summary>
    public partial class Edit : Needs.Web.Sso.Forms.ErpPage
    {
        #region 页面初始化加载数据
        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.PageInit();
            }
        }

        /// <summary>
        /// 页面数据初始化
        /// </summary>
        private void PageInit()
        {
            SetDropDownList();
            string id = Request.QueryString["ID"];
            if (!string.IsNullOrEmpty(id))
            {
                var client = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClientsBase[id];
                string ntext = client.NTextString;
                this.Model.CustomerData = ntext;
                this.Model.ClientStaus = (int)client.Status;
            }
            else
            {
                this.Model.CustomerData = "".Json();
                this.Model.ClientStaus = 0;
            }
            var files = new NtErp.Crm.Services.Views.FileAlls().Where(item => item.ClientID == id && item.Status == Status.Normal);
            this.Model.Files = files.Select(item => new { item.ID, item.Name, item.Url }).Json();
        }

        /// <summary>
        /// 下拉框数据加载
        /// </summary>
        private void SetDropDownList()
        {
            this.Model.CustomerTypeData = EnumUtils.ToDictionary<CustomerType>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.CustomerNatureData = EnumUtils.ToDictionary<CustomerNature>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.CustomerAreaData = EnumUtils.ToDictionary<CustomerArea>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.BusinessTypeData = EnumUtils.ToDictionary<BusinessType>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.CustomerLevelData = EnumUtils.ToDictionary<CustomerLevel>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.ImportantLevelData = EnumUtils.ToDictionary<ImportantLevel>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.CurrencyData = EnumUtils.ToDictionary<CurrencyType>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.ProtectLevel = EnumUtils.ToDictionary<ProtectLevel>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.CustomerStatus = EnumUtils.ToDictionary<CustomerStatus>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.CustomerCompanyData = Needs.Erp.ErpPlot.Current.ClientSolutions.Companys.Where(item => item.Type == CompanyType.plot).OrderBy(item => item.Name).Json();
            this.Model.Manufacture = Needs.Erp.ErpPlot.Current.ClientSolutions.MyManufactures.
                Select(item => new { item.ID, item.Name }).OrderBy(item => item.Name).Json();
            this.Model.AdminTop = Needs.Underly.FkoFactory<AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID).Json();

            this.Model.ReIndustry = Needs.Erp.ErpPlot.Current.ClientSolutions.MyIndustries.Where(item => item.FatherID == null).
                Select(item => new { item.ID, item.Name }).Json();

            this.Model.DrpCategory = new NtErp.Crm.Services.Views.IndustryTree().tree;
            this.Model.AreaData = new NtErp.Crm.Services.Views.AreaTree().tree;
        }

        #endregion


        #region 保存
        /// <summary>
        /// 数据保存
        /// </summary>
        protected void Save()
        {
            string id = Request.QueryString["id"];
            var client = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClientsBase[id] ?? new Client();
            foreach (var key in Request.Form.AllKeys)
            {
                if (key != "__VIEWSTATE" && key != "__VIEWSTATEGENERATOR" && key != "__EVENTVALIDATION" && key != "btnSumit")
                {
                    if (key == "Name")
                    {
                        client[key] = Request.Form[key].Replace("\\", "").Replace("[", "").Replace("]", "").Trim();
                    }
                    else
                    {
                        client[key] = Request.Form[key].Replace("\\", "").Replace("[", "").Replace("]", "");
                    }
                }

            }
            if (!Request.Form.AllKeys.Contains("IndustryInvolved"))
            {
                client.IndustryInvolved = "";
            }
            //审批不通过数据,修改后重新申请
            if (client.Status == ActionStatus.Reject)
            {
                client.Status = ActionStatus.Auditing;
            }
            client.IsSafe = IsProtected.Yes;
            client.EnterSuccess += client_EnterSuccess;
            client.Enter();
        }

        /// <summary>
        /// 持久化成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void client_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var client = sender as Client;
            if (client.Status == ActionStatus.Auditing)
            {
                //申请
                Apply(e.Object);
            }
            if (string.IsNullOrWhiteSpace(Request.QueryString["id"]))
            {
                //新增绑定销售
                AdminTop Admin = Needs.Underly.FkoFactory<AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
                Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.Binding(e.Object, Admin, null, null);
            }
            SaveFile(e.Object);
            ClientBinding(e.Object);
            Alert("保存成功", Request.Url, true);
        }

        /// <summary>
        /// 保存营业执照
        /// </summary>
        /// <param name="clientid">clientid</param>
        protected void SaveFile(string clientid)
        {
            //文件保存到服务器
            HttpFileCollection files = Request.Files;
            if (files.Count == 0 || files[0].ContentLength == 0)
            {
                return;
            }
            HttpPostedFile file = files[0];
            string path = DateTime.Now.ToString("yyyyMMddhhmmssfff") + System.IO.Path.GetExtension(file.FileName).ToLower();
            file.SaveAs(AppDomain.CurrentDomain.BaseDirectory + "/UploadFiles/" + path);

            var File = new NtErp.Crm.Services.Views.FileAlls().Where(item => item.ClientID == clientid).SingleOrDefault() ?? new File();
            File.Client = Needs.Underly.FkoFactory<Client>.Create(clientid);
            File.Admin = Needs.Underly.FkoFactory<AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
            File.Url = Request.ApplicationPath + "/UploadFiles/" + path;
            File.Name = file.FileName;
            File.Enter();
        }

        /// <summary>
        /// 客户绑定品牌和行业
        /// </summary>
        /// <param name="clientid"></param>
        private void ClientBinding(string clientid)
        {
            string[] Manufactures = Request.Form["AgentBrand"]?.Split(',') ?? new string[0];
            string[] Industries = Request.Form["ReIndustry"]?.Split(',') ?? new string[0];

            Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.DeleteBinding(clientid);

            //品牌绑定
            foreach (var item in Manufactures)
            {
                var manu = Needs.Underly.FkoFactory<Company>.Create(item);
                Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.Binding(clientid, null, null, manu);
            }

            //行业绑定
            foreach (var item in Industries)
            {
                var industry = Needs.Underly.FkoFactory<Industry>.Create(item);
                Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.Binding(clientid, null, industry, null);
            }
        }
        #endregion

        /// <summary>
        /// 申请
        /// </summary>
        protected void Apply(string id)
        {
            var apply = new Apply();
            apply.MainID = id;
            apply.Type = ApplyType.CreatedClient;
            apply.Admin = Needs.Underly.FkoFactory<AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
            apply.Summary = "客户申请";
            apply.Status = ApplyStatus.Audting;
            apply.Enter();

        }

        #region 重名校验
        /// <summary>
        /// 实时校验客户是否重名
        /// </summary>
        /// <returns></returns>
        protected void ValidName()
        {
            string name = Request.Form["Name"];
            string Url = Request.Form["Url"];

            var clients = Needs.Erp.ErpPlot.Current.ClientSolutions.Clients.Where(item => item.Name == name &&
            item.Status != ActionStatus.Delete && item.Status != ActionStatus.Reject);
            if (clients.Count() > 0)
            {
                Response.Write("true");
                return;
            }

            CookieCollection responseCookies = new CookieCollection();
            var result = HttpHelper.Get(Url.Replace("&amp;", "&"), responseCookies, Accept.json);
            var data = result.Data.JsonTo()["data"];
            var admin = Needs.Underly.FkoFactory<AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
            int count = data.Count(item => item["ShortName"].ToString().Split('.')[0] == admin.DyjID);
            if (data.Count() > 0 && count == 0)
            {
                Response.Write("false");
            }
        }
        #endregion
    }
}