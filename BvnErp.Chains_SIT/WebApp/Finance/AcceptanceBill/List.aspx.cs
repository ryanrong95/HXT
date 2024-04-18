using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Models.HttpUtility;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.AcceptanceBill
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                load();
            }
        }

        private void load()
        {
            this.Model.BillStatus = Needs.Utils.Descriptions.EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.MoneyOrderStatus>().Select(item => new { Value = item.Key, Text = item.Value }).Json();
        }

        protected void data()
        {
            string Code = Request.QueryString["Code"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string BillStatus = Request.QueryString["BillStatus"];
            string CreateStartDate = Request.QueryString["CreateStartDate"];
            string CreateEndDate = Request.QueryString["CreateEndDate"];

            var financeAccounts = new Needs.Ccs.Services.Views.AcceptanceBillView().Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal);

            if (!string.IsNullOrEmpty(Code))
            {
                Code = Code.Trim();
                financeAccounts = financeAccounts.Where(t => t.Code == Code);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                StartDate = StartDate.Trim();
                DateTime dtStart = Convert.ToDateTime(StartDate);
                financeAccounts = financeAccounts.Where(t => t.EndDate> dtStart);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                EndDate = EndDate.Trim();
                DateTime dtEnd = Convert.ToDateTime(EndDate).AddDays(1);
                financeAccounts = financeAccounts.Where(t => t.EndDate < dtEnd);
            }
            if (!string.IsNullOrEmpty(BillStatus))
            {
                BillStatus = BillStatus.Trim();
                financeAccounts = financeAccounts.Where(t => t.BillStatus == (Needs.Ccs.Services.Enums.MoneyOrderStatus)Convert.ToInt32(BillStatus));
            }
            if (!string.IsNullOrEmpty(CreateStartDate))
            {
                CreateStartDate = CreateStartDate.Trim();
                DateTime dtCreateStart = Convert.ToDateTime(CreateStartDate);
                financeAccounts = financeAccounts.Where(t => t.CreateDate > dtCreateStart);
            }
            if (!string.IsNullOrEmpty(CreateEndDate))
            {
                CreateEndDate = CreateEndDate.Trim();
                DateTime dtCreateEnd = Convert.ToDateTime(CreateEndDate).AddDays(1);
                financeAccounts = financeAccounts.Where(t => t.CreateDate < dtCreateEnd);
            }

            Func<Needs.Ccs.Services.Models.AcceptanceBill, object> convert = item => new
            {
                ID = item.ID,
                InAccountName = item.PayeeAccount.AccountName,
                Code = item.Code,
                Price = item.Price,
                OutAccountName = item.PayerAccount.AccountName,
                StartDate = item.StartDate.ToString("yyyy-MM-dd"),
                EndDate = item.EndDate.ToString("yyyy-MM-dd"),
                //AdminName = item.Creator.RealName,
                AcceptedDate = item.AcceptedDate == null ? "" : item.AcceptedDate.Value.ToString("yyyy-MM-dd"),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Status = item.BillStatus.GetDescription(),
                ExchangeDate = item.ExchangeDate,
            };
            this.Paging(financeAccounts, convert);
        }

        protected void UploadFile()
        {
            try
            {
                List<dynamic> fileList = new List<dynamic>();
                IList<HttpPostedFile> files = System.Web.HttpContext.Current.Request.Files.GetMultiple("uploadFile");
                if (files.Count > 0)
                {
                    var validTypes = new List<string>() { ".pdf" };
                    for (int i = 0; i < files.Count; i++)
                    {
                        string ext = Path.GetExtension(files[i].FileName);
                        if (!validTypes.Contains(ext.ToLower()))
                        {
                            Response.Write((new { success = false, message = "上传的文件只能是pdf格式！" }).Json());
                            return;
                        }

                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            //文件保存
                            string fileName = files[i].FileName.ReName();

                            //创建文件目录
                            FileDirectory fileDic = new FileDirectory(fileName);
                            fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Cost);
                            fileDic.CreateDataDirectory();
                            file.SaveAs(fileDic.FilePath);

                            //文件提交给pdf处理程序处理
                            string URL = System.Configuration.ConfigurationManager.AppSettings[AcceptanceApiSetting.ApiName];
                            string requestUrl = URL + AcceptanceApiSetting.AcceptanceUrl+ "?url="+ fileDic.VirtualPath+ "&fileName="+ fileName;
                            requestUrl = requestUrl.Replace("\\", "/");
                            HttpResponseMessage response = new HttpResponseMessage();
                            response = new HttpClientHelp().HttpClient("GET", requestUrl);
                            if (response == null || response.StatusCode != HttpStatusCode.OK)
                            {
                                Response.Write((new { success = false, message = "自动识别失败" }).Json());
                            }
                        }
                    }
                }

              
                Response.Write((new { success = true, data = fileList }).Json());
                
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "上传失败：" + ex.Message }).Json());
            }
        }

        protected void GenFinanceReceipt() 
        {
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            var model = Model.JsonTo<List<AccID>>();

            foreach(var item in model) 
            {               
                var oldBill = new Needs.Ccs.Services.Views.AcceptanceBillView().Where(t => t.ID == item.ID).FirstOrDefault();
                var receipt = new Needs.Ccs.Services.Views.FinanceReceiptsView().Where(t => t.SeqNo == oldBill.Code).FirstOrDefault();
                if (receipt != null) 
                {
                    continue;
                }
                FinanceReceipt financeReceipt = new FinanceReceipt(oldBill);
                var currentAdmin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                financeReceipt.Admin = currentAdmin;

                financeReceipt.Enter();
                financeReceipt.Post2Center("");
            }

            Response.Write((new { success = true }).Json());
        }

        public class AccID
        {
            public string ID { get; set; }
         
        }
    }
}