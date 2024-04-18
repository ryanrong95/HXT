using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;

namespace WebApp.Client.Control
{
    public partial class Detail : Uc.PageBase
    {
        public string CenterServiceUrl { get; set; }
        public string CenterbusiniseUrl { get; set; }
        string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
            GetServiceFileHttpUrl();
            GetBusinessFileHttpUrl();
        }

        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            if (!string.IsNullOrEmpty(id))
            {
                var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id];
                this.Model.ClientInfoData = new
                {
                    ID = client.ID,
                    ClientStatus = client.ClientStatus,
                    CompanyName = client.Company.Name,
                    Rank = client.ClientRank.GetDescription(),
                    ClientAgreementData = client.Agreement,
                    InvoiceType=client.Agreement.InvoiceType.GetDescription(),
                    StartDate= client.Agreement.StartDate.ToShortDateString(),
                    EndDate = client.Agreement.EndDate.ToShortDateString(),
                    GoodsPeriodTypeDec = client.Agreement.ProductFeeClause.PeriodType.GetDescription(),
                    GoodsExchangeRateTypeDec =client.Agreement.ProductFeeClause.ExchangeRateType .GetDescription(),
                    TaxExchangeRateTypeDec = client.Agreement.TaxFeeClause.ExchangeRateType.GetDescription(),
                    TaxPeriodTypeDec = client.Agreement.TaxFeeClause.PeriodType.GetDescription(),
                    AgencyExchangeRateTypeDec = client.Agreement.AgencyFeeClause.ExchangeRateType.GetDescription(),
                    AgencyPeriodType = client.Agreement.AgencyFeeClause.PeriodType.GetDescription(),
                    IncidentalExchangeRateTypeDec = client.Agreement.IncidentalFeeClause.ExchangeRateType.GetDescription(),
                    IncidentalPeriodTypeDec=client.Agreement.IncidentalFeeClause.PeriodType.GetDescription()
                }.Json();
            }
            var serviceFile = new Needs.Ccs.Services.Models.CenterFilesTopView().OrderByDescending(x => x.CreateDate).FirstOrDefault(t => t.ClientID == id && t.Type == (int)Needs.Ccs.Services.Enums.FileType.ServiceAgreement && t.Status != FileDescriptionStatus.Delete);
            var businessLicenceFile = new Needs.Ccs.Services.Models.CenterFilesTopView().OrderByDescending(x=>x.CreateDate).FirstOrDefault(x => x.ClientID == id && x.Type == (int)Needs.Ccs.Services.Enums.FileType.BusinessLicense);
              
            if (serviceFile != null)
            {
                this.CenterServiceUrl = FileDirectory.Current.PvDataFileUrl + "/" + serviceFile?.Url.ToUrl();
                this.Model.serviceFile = new
                {
                    ID = serviceFile?.ID,
                    FileName = serviceFile.CustomName,
                    Url = serviceFile.Url.Replace(@"\", @"\\"),
                    WebUrl = this.CenterServiceUrl,
                }.Json();
            }
            else {
                this.Model.serviceFile = "".Json();

            }
            if (businessLicenceFile != null)
            {
                this.CenterbusiniseUrl = FileDirectory.Current.PvDataFileUrl + "/" + businessLicenceFile?.Url.ToUrl();
                this.Model.businessLicenceFile = new
                {
                    ID = businessLicenceFile?.ID,
                    FileName = businessLicenceFile?.CustomName,
                   // FileFormat = serviceFile.FileFormat,
                    Url = businessLicenceFile.Url.Replace(@"\", @"\\"),
                    WebUrl = this.CenterbusiniseUrl
                }.Json();

            }
            else {

                this.Model.businessLicenceFile = "".Json();
            }

        }



        protected void LoadLogs()
        {
            string ID = Request.Form["ID"];
            var list = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientLogs.Where(item => item.ClientID == ID).OrderByDescending(item => item.CreateDate).AsQueryable();
            Func<Needs.Ccs.Services.Models.ClientLog, object> convert = item => new
            {
                ID = item.ID,
                Client = item.ClientID,
                Admin = item.Admin.RealName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Summary = item.Summary
            };
            Response.Write(new { rows = list.Select(convert).ToArray(), }.Json());
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        protected void ToAudit()
        {
            try
            {
                string id = Request.Form["ID"];
                var entity = Needs.Wl.Admin.Plat.AdminPlat.Clients[id];
                var adminid = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                var summary = string.Empty;
                if (entity != null)
                {
                    entity.Admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create((string)adminid);
                    entity.EnterSuccess += ClientStatus_EnterError_EnterSuccess;
                    entity.EnterError += ClientStatus_EnterError;
                    entity.Audit();
                }
              
            }
            catch (Exception ex)
            {
              
            }


        }


        /// <summary>
        /// 审核不通过
        /// </summary>
        protected void Refuse()
        {
            try
            {
                string id = Request.Form["ID"];
                string summary = Request.Form["ApproveSummary"];
                var entity = Needs.Wl.Admin.Plat.AdminPlat.Clients[id];
                var adminid = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
               // var summary = string.Empty;
                if (entity != null)
                {
                    entity.Admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create((string)adminid);
                    entity.Summary = summary;
                    entity.EnterSuccess += ClientStatus_EnterError_EnterSuccess;
                    entity.EnterError += ClientStatus_EnterError;
                    entity.Refuse();
                }

            }
            catch (Exception ex)
            {

            }


        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientStatus_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = e.Message }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientStatus_EnterError_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "提交成功", ID = e.Object }).Json());
        }

        protected void GetServiceFileHttpUrl()
        {
            var url = this.CenterServiceUrl;
            string fileName = Path.GetFileName(url);
            HttpFile httpFile = new HttpFile(fileName);
            httpFile.SetChildFolder(Needs.Ccs.Services.SysConfig.Client);
            //httpFile.CreateDataDirectory();
            if (!string.IsNullOrEmpty(url))
            { 
                string filePath = Path.Combine(FileDirectory.Current.FilePath, httpFile.VirtualPath, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    WebResponse response = request.GetResponse();
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                        {
                            //创建本地文件写入流
                            byte[] bArr = new byte[1024];
                            int iTotalSize = 0;
                            int size = stream.Read(bArr, 0, (int)bArr.Length);
                            while (size > 0)
                            {
                                iTotalSize += size;
                                fs.Write(bArr, 0, size);
                                size = stream.Read(bArr, 0, (int)bArr.Length);
                            }
                        }
                        stream.Close();
                    }

                    this.FileDoc2.HRef = Path.Combine(FileServerUrl, httpFile.VirtualPath, fileName);
                }
                catch (Exception)
                {
                }
            }
           
        }

        protected void GetBusinessFileHttpUrl()
        {
            var url = this.CenterbusiniseUrl;
            string fileName = Path.GetFileName(url);
            HttpFile httpFile = new HttpFile(fileName);
            httpFile.SetChildFolder(Needs.Ccs.Services.SysConfig.Client);
            //httpFile.CreateDataDirectory();
            if (!string.IsNullOrEmpty(url))
            {
            string filePath = Path.Combine(FileDirectory.Current.FilePath, httpFile.VirtualPath, fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    {
                        //创建本地文件写入流
                        byte[] bArr = new byte[1024];
                        int iTotalSize = 0;
                        int size = stream.Read(bArr, 0, (int)bArr.Length);
                        while (size > 0)
                        {
                            iTotalSize += size;
                            fs.Write(bArr, 0, size);
                            size = stream.Read(bArr, 0, (int)bArr.Length);
                        }
                    }
                    stream.Close();
                }

                this.FileDoc1.HRef = Path.Combine(FileServerUrl, httpFile.VirtualPath, fileName);
            }
            catch (Exception)
            {
            }

            }
        }
        
    }
}