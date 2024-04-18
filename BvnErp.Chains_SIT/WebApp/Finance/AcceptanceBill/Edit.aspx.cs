using Needs.Ccs.Services;
using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Models.HttpUtility;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
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
    public partial class Edit : Uc.PageBase
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
            this.Model.AcceptanceType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.MoneyOrderNature>().Select(item => new { Value = item.Key, Text = item.Value }).Json();
            string ID = Request.QueryString["ID"];
            var acceptBill = new Needs.Ccs.Services.Views.AcceptanceBillView().Where(item => item.ID==ID).FirstOrDefault();
            if (acceptBill != null)
            {
                this.Model.Bill = new
                {
                    ID = acceptBill.ID,
                    StartDate=acceptBill.StartDate,
                    EndDate=acceptBill.EndDate,
                    Code=acceptBill.Code,
                    OutAccountName=acceptBill.PayerAccount.AccountName,
                    OutAccountNo=acceptBill.PayerAccount.BankAccount,
                    OutAccountBankName=acceptBill.PayerAccount.BankName,
                    InAccountName=acceptBill.PayeeAccount.AccountName,
                    InAccountNo=acceptBill.PayeeAccount.BankAccount,
                    InAccountBankName=acceptBill.PayeeAccount.BankName,
                    Name=acceptBill.Name,
                    BankCode=acceptBill.BankCode,
                    BankNo=acceptBill.BankNo,
                    BankName=acceptBill.BankName,
                    Price=acceptBill.Price,
                    Nature=acceptBill.Nature,
                    IsTransfer=acceptBill.IsTransfer,
                    IsMoney=acceptBill.IsMoney,
                    ExchangeDate=acceptBill.ExchangeDate,
                    ExchangePrice=acceptBill.ExchangePrice,
                    OutAccountID = acceptBill.PayerAccount.ID,
                    InAccountID = acceptBill.PayeeAccount.ID,
                    Endorser = acceptBill.Endorser,

                }.Json();
            }
            else
            {
                this.Model.Bill = new { }.Json();
            }

        }

        protected void Save()
        {
            try
            {
                var id = Request.Form["ID"];
                string StartDate = Request.Form["StartDate"];
                string EndDate = Request.Form["EndDate"];
                string Code = Request.Form["Code"];
                string OutAccountID = Request.Form["OutAccountID"];
                string OutAccountNo = Request.Form["OutAccountNo"];
                string InAccountID = Request.Form["InAccountID"];
                string InAccountNo = Request.Form["InAccountNo"];
                string Name = Request.Form["Name"];
                string BankCode = Request.Form["BankCode"];
                string BankNo = Request.Form["BankNo"];
                string BankName = Request.Form["BankName"];
                string Price = Request.Form["Price"];
                string Nature = Request.Form["Nature"];
                string IsTransfer = Request.Form["IsTransfer"];
                string IsMoney = Request.Form["IsMoney"];
                string ExchangeDate = Request.Form["ExchangeDate"];
                string ExchangePrice = Request.Form["ExchangePrice"];
                string Endorser = Request.Form["Endorser"];
                string Files = Request.Form["Files"].Replace("&quot;", "'");

                SendStrcut sendStrcut = new SendStrcut();
                sendStrcut.sender = "FSender001";
                

                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                Needs.Ccs.Services.Models.AcceptanceBill acceptance = new Needs.Ccs.Services.Models.AcceptanceBill();                   
                acceptance.Code = Code;
                acceptance.Name = Name;
                acceptance.BankCode = BankCode;
                acceptance.BankName = BankName;
                acceptance.BankNo = BankNo;
                acceptance.Price = Convert.ToDecimal(Price);
                acceptance.IsMoney = Convert.ToBoolean(Convert.ToInt16(IsMoney));
                acceptance.IsTransfer = Convert.ToBoolean(Convert.ToInt16(IsTransfer));
                acceptance.PayerAccount = new Needs.Ccs.Services.Models.FinanceAccount { ID = OutAccountID,BankAccount=OutAccountNo };
                acceptance.PayeeAccount = new Needs.Ccs.Services.Models.FinanceAccount { ID = InAccountID,BankAccount=InAccountNo };
                acceptance.StartDate = Convert.ToDateTime(StartDate);
                acceptance.EndDate = Convert.ToDateTime(EndDate);
                acceptance.Nature = (Needs.Ccs.Services.Enums.MoneyOrderNature)Convert.ToInt16(Nature);
                acceptance.Creator = admin;
                acceptance.Endorser = Endorser;
                if (!string.IsNullOrEmpty(ExchangeDate))
                {
                    acceptance.ExchangeDate = Convert.ToDateTime(ExchangeDate);
                }
                if (!string.IsNullOrEmpty(ExchangePrice))
                {
                    acceptance.ExchangePrice = Convert.ToDecimal(ExchangePrice);
                }

                var oldBill = new Needs.Ccs.Services.Views.AcceptanceBillView().Where(item => item.ID == id).FirstOrDefault();
                CenterAcceptanceBill centerAcceptanceBill = new CenterAcceptanceBill(acceptance);
                if (oldBill != null)
                {
                    sendStrcut.option = CenterConstant.Update;
                    acceptance.ID = oldBill.ID;
                    acceptance.BillStatus = oldBill.BillStatus;
                    centerAcceptanceBill.OldCode = oldBill.Code;
                }
                else
                {
                    sendStrcut.option = CenterConstant.Enter;
                    acceptance.ID = ChainsGuid.NewGuidUp();
                }

                var FileList = Files.JsonTo<List<dynamic>>();
                List<Needs.Ccs.Services.Models.CostApplyFile> costApplyFiles = new List<Needs.Ccs.Services.Models.CostApplyFile>();
                List<CenterFeeFile> centerFiles = new List<CenterFeeFile>();
                foreach (var item in FileList)
                {
                    costApplyFiles.Add(new Needs.Ccs.Services.Models.CostApplyFile
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        CostApplyID = acceptance.ID,
                        AdminID = admin.ID,
                        Name = item.Name,
                        FileType = CostApplyFileTypeEnum.Inovice,
                        FileFormat = item.FileFormat,
                        URL = Convert.ToString(item.VirtualPath).Replace(@"/", @"\"),
                        Status = Status.Normal,
                        CreateDate = DateTime.Now,
                    });

                    centerFiles.Add(new CenterFeeFile
                    {
                        FileName = item.Name,
                        FileFormat = item.FileFormat,
                        Url = FileDirectory.Current.FileServerUrl + "/" + Convert.ToString(item.VirtualPath).Replace(@"\", @"/"),
                        FileType=1
                    });
                }

                if (centerFiles.Count() > 0)
                {
                    centerAcceptanceBill.Files = centerFiles;
                }


                sendStrcut.model = centerAcceptanceBill; 
                //提交中心
                string URL = System.Configuration.ConfigurationManager.AppSettings[FinanceApiSetting.ApiName];
                string requestUrl = URL + FinanceApiSetting.AcceptanceUrl;
                string apiclient = JsonConvert.SerializeObject(sendStrcut);

                HttpResponseMessage response = new HttpResponseMessage();
                response = new HttpClientHelp().HttpClient("POST", requestUrl, apiclient);


                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    Response.Write((new { success = false, message = "请求中心接口失败：" }).Json());
                }
                else
                {
                    acceptance.Enter();
                   
                    foreach (var costApplyFile in costApplyFiles)
                    {
                        costApplyFile.Enter();
                    }

                    Logs log = new Logs();
                    log.Name = "承兑汇票同步";
                    log.MainID = acceptance.ID;
                    log.AdminID = "";
                    log.Json = apiclient;
                    log.Summary = "";
                    log.Enter();
                }


                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch(Exception ex)
            {
                Response.Write((new { success = false, message = ex.ToString() }).Json());
            }            
        }

        protected void UploadFile()
        {
            try
            {
                List<dynamic> fileList = new List<dynamic>();
                IList<HttpPostedFile> files = System.Web.HttpContext.Current.Request.Files.GetMultiple("uploadFile");
                if (files.Count > 0)
                {
                    var validTypes = new List<string>() { ".jpg", ".bmp", ".jpeg", ".gif", ".png", ".pdf" };
                    for (int i = 0; i < files.Count; i++)
                    {
                        string ext = Path.GetExtension(files[i].FileName);
                        if (!validTypes.Contains(ext.ToLower()))
                        {
                            Response.Write((new { success = false, message = "上传的原始PI只能是图片(jpg、bmp、jpeg、gif、png)或pdf格式！" }).Json());
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

                            fileList.Add(new
                            {
                                Name = file.FileName,
                                FileType = FileType.None.GetDescription(),
                                FileFormat = file.ContentType,
                                VirtualPath = fileDic.VirtualPath,
                                Url = fileDic.FileUrl,
                            });
                        }
                    }
                }

                if (fileList.Count() == 0)
                {
                    Response.Write((new { success = false, data = new { } }).Json());
                }
                else
                {
                    Response.Write((new { success = true, data = fileList }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "上传失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 费用申请文件
        /// </summary>
        protected void CostApplyFiles()
        {
            string CostApplyID = Request.QueryString["ID"];

            var files = new Needs.Ccs.Services.Views.CostApplyFilesView().GetResults(CostApplyID);

            Func<Needs.Ccs.Services.Views.CostApplyFilesViewModel, object> convert = item => new
            {
                CostApplyFileID = item.CostApplyFileID,
                Name = item.FileName,
                FileFormat = item.FileFormat,
                VirtualPath = item.Url,
                WebUrl = FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),
            };

            Response.Write(new
            {
                rows = files.Select(convert).ToArray(),
            }.Json());
        }

    }
}