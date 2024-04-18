using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;

namespace WebApp.Client.Control
{
    public partial class Edit : Uc.PageBase
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
                    Rank = client.ClientRank,
                    ClientNature = client.ClientNature,
                    ClientAgreementData = client.Agreement,
                    InvoiceType = client.Agreement.InvoiceType.GetDescription(),
                    StartDate = client.Agreement.StartDate.ToShortDateString(),
                    EndDate = client.Agreement.EndDate.ToShortDateString(),
                    GoodsPeriodTypeDec = client.Agreement.ProductFeeClause.PeriodType.GetDescription(),
                    GoodsExchangeRateTypeDec = client.Agreement.ProductFeeClause.ExchangeRateType.GetDescription(),
                    TaxExchangeRateTypeDec = client.Agreement.TaxFeeClause.ExchangeRateType.GetDescription(),
                    TaxPeriodTypeDec = client.Agreement.TaxFeeClause.PeriodType.GetDescription(),
                    AgencyExchangeRateTypeDec = client.Agreement.AgencyFeeClause.ExchangeRateType.GetDescription(),
                    AgencyPeriodType = client.Agreement.AgencyFeeClause.PeriodType.GetDescription(),
                    IncidentalExchangeRateTypeDec = client.Agreement.IncidentalFeeClause.ExchangeRateType.GetDescription(),
                    IncidentalPeriodTypeDec = client.Agreement.IncidentalFeeClause.PeriodType.GetDescription(),
                    ServiceType = client.ServiceType,
                    ServiceTypeDes = client.ServiceType.GetDescription(),
                    StorageType = ((StorageType)client.StorageType).GetDescription(),
                }.Json();
            }
            var serviceFile = new Needs.Ccs.Services.Models.CenterFilesTopView().OrderByDescending(x => x.CreateDate).FirstOrDefault(t => t.ClientID == id && t.Type == (int)Needs.Ccs.Services.Enums.FileType.ServiceAgreement && t.Status != FileDescriptionStatus.Delete);
            var businessLicenceFile = new Needs.Ccs.Services.Models.CenterFilesTopView().OrderByDescending(x => x.CreateDate).FirstOrDefault(x => x.ClientID == id && x.Type == (int)Needs.Ccs.Services.Enums.FileType.BusinessLicense && x.Status != FileDescriptionStatus.Delete);
            var hkbusinessLicenceFile = new Needs.Ccs.Services.Models.CenterFilesTopView().FirstOrDefault(x => x.ClientID == id && x.Type == (int)Needs.Ccs.Services.Enums.FileType.HKBusinessLicense);
            var storageAgreementFile = new Needs.Ccs.Services.Models.CenterFilesTopView().FirstOrDefault(x => x.ClientID == id && x.Type == (int)Needs.Ccs.Services.Enums.FileType.StorageAgreement);

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
            else
            {
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
            else
            {

                this.Model.businessLicenceFile = "".Json();
            }

            if (hkbusinessLicenceFile != null)
            {
                this.CenterbusiniseUrl = FileDirectory.Current.PvDataFileUrl + "/" + hkbusinessLicenceFile?.Url.ToUrl();
                this.Model.HKbusinessLicenceFile = new
                {
                    ID = hkbusinessLicenceFile?.ID,
                    FileName = hkbusinessLicenceFile?.CustomName,
                    // FileFormat = serviceFile.FileFormat,
                    Url = hkbusinessLicenceFile.Url.Replace(@"\", @"\\"),
                    WebUrl = this.CenterbusiniseUrl,
                    FileType = FileType.HKBusinessLicense
                }.Json();

            }
            else
            {

                this.Model.HKbusinessLicenceFile = "".Json();
            }

            if (storageAgreementFile != null)
            {
                this.CenterbusiniseUrl = FileDirectory.Current.PvDataFileUrl + "/" + storageAgreementFile?.Url.ToUrl();
                this.Model.StorageAgreementFile = new
                {
                    ID = storageAgreementFile?.ID,
                    FileName = storageAgreementFile?.CustomName,
                    // FileFormat = serviceFile.FileFormat,
                    Url = storageAgreementFile.Url.Replace(@"\", @"\\"),
                    WebUrl = this.CenterbusiniseUrl,
                    FileType = FileType.StorageAgreement
                }.Json();

            }
            else
            {
                this.Model.StorageAgreementFile = "".Json();
            }

            //客户等级
            this.Model.ClientRanks = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.ClientRank>().Select(item => new { item.Key, item.Value }).Json();
            this.Model.ClientNature = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.ClientNature>().Where(t => t.Key == "1" || t.Key == "2").Select(item => new { item.Key, item.Value }).Json();

        }



        protected void LoadLogs()
        {
            string ID = Request.Form["ID"];
            var list = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientLogs.Where(item => item.ClientID == ID).OrderByDescending(item => item.CreateDate).AsQueryable();
            Func<Needs.Ccs.Services.Models.ClientLog, object> convert = item => new
            {
                ID = item.ID,
                Client = item.ClientID,
                Admin = string.IsNullOrEmpty(item.Admin.ByName) ?  item.Admin.RealName : item.Admin.ByName,
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
                string Reason = Request.Form["Reason"];
                string id = Request.Form["ID"];
                string Rank = Request.Form["Rank"];
                string Nature = Request.Form["Nature"];
                decimal GoodsUpperLimit = string.IsNullOrEmpty(Request.Form["GoodsUpperLimit"]) ? 0 : Convert.ToDecimal(Request.Form["GoodsUpperLimit"]);
                decimal TaxUpperLimit = string.IsNullOrEmpty(Request.Form["TaxUpperLimit"]) ? 0 : Convert.ToDecimal(Request.Form["TaxUpperLimit"]);
                decimal AgencyFeeUpperLimit = string.IsNullOrEmpty(Request.Form["AgencyFeeUpperLimit"]) ? 0 : Convert.ToDecimal(Request.Form["AgencyFeeUpperLimit"]);
                decimal IncidentalUpperLimit = string.IsNullOrEmpty(Request.Form["IncidentalUpperLimit"]) ? 0 : Convert.ToDecimal(Request.Form["IncidentalUpperLimit"]);
                var entity = Needs.Wl.Admin.Plat.AdminPlat.Clients[id];

                //领导的奇怪要求 显示风控操作
                var adminid = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                //var RealName = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;

                //var RiskName = System.Configuration.ConfigurationManager.AppSettings["RiskManagementName"];
                //var RiskID = System.Configuration.ConfigurationManager.AppSettings["RiskManagementID"];

                //if (RealName == "张令金" || RealName == "张庆永")
                //{
                //    adminid = RiskID;//风控的ID
                //    RealName = RiskName;
                //}

                var admin = new Needs.Ccs.Services.Views.AdminsTopView2().FirstOrDefault(t => t.ID == adminid);

                var RiskName = string.IsNullOrEmpty(admin.ByName) ? admin.RealName : admin.ByName;

                var summary = string.Empty;
                if (entity != null)
                {
                    entity.Admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create((string)adminid);
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        List<RiskChanges> changes = new List<RiskChanges>();

                        if ((int)entity.ClientRank != Convert.ToInt16(Rank))
                        {
                            RiskChanges rankChange = new RiskChanges();
                            rankChange.ChangeType = RiskControlChangeType.RankChange;
                            rankChange.NewValue = Convert.ToInt16(Rank);
                            rankChange.OldValue = (int)entity.ClientRank;
                            changes.Add(rankChange);
                        }
                        if (entity.ClientNature == null || (int)entity.ClientNature != Convert.ToInt16(Nature))
                        {
                            entity.ClientNature = (int)ClientNature.Trade;
                            RiskChanges natureChange = new RiskChanges();
                            natureChange.ChangeType = RiskControlChangeType.NatureChange;
                            natureChange.NewValue = Convert.ToInt16(Nature);
                            natureChange.OldValue = (int)entity.ClientNature;
                            changes.Add(natureChange);
                        }
                        EditRankOrNature(entity, changes);
                        decimal productFee = (entity.Agreement.ProductFeeClause == null || entity.Agreement.ProductFeeClause.PeriodType == PeriodType.PrePaid) ? 0 : entity.Agreement.ProductFeeClause.UpperLimit.Value;
                        if (productFee != GoodsUpperLimit)
                        {
                            RiskChanges rankChange = new RiskChanges();
                            rankChange.ChangeType = RiskControlChangeType.GoodsUpperLimitChange;
                            rankChange.NewValue = GoodsUpperLimit;
                            rankChange.OldValue = productFee;
                            changes.Add(rankChange);
                        }
                        decimal taxFee = (entity.Agreement.TaxFeeClause.UpperLimit == null || entity.Agreement.TaxFeeClause.PeriodType == PeriodType.PrePaid) ? 0 : entity.Agreement.TaxFeeClause.UpperLimit.Value;
                        if (taxFee != TaxUpperLimit)
                        {
                            RiskChanges rankChange = new RiskChanges();
                            rankChange.ChangeType = RiskControlChangeType.TaxUpperLimitChange;
                            rankChange.NewValue = TaxUpperLimit;
                            rankChange.OldValue = taxFee;
                            changes.Add(rankChange);
                        }
                        decimal agencyFeeClause = (entity.Agreement.AgencyFeeClause.UpperLimit == null || entity.Agreement.AgencyFeeClause.PeriodType == PeriodType.PrePaid) ? 0 : entity.Agreement.AgencyFeeClause.UpperLimit.Value;
                        if (agencyFeeClause != AgencyFeeUpperLimit)
                        {
                            RiskChanges rankChange = new RiskChanges();
                            rankChange.ChangeType = RiskControlChangeType.AgencyFeeUpperLimitChange;
                            rankChange.NewValue = AgencyFeeUpperLimit;
                            rankChange.OldValue = agencyFeeClause;
                            changes.Add(rankChange);
                        }
                        decimal incidentalFee = (entity.Agreement.IncidentalFeeClause.UpperLimit == null || entity.Agreement.IncidentalFeeClause.PeriodType == PeriodType.PrePaid) ? 0 : entity.Agreement.IncidentalFeeClause.UpperLimit.Value;
                        if (incidentalFee != IncidentalUpperLimit)
                        {
                            RiskChanges rankChange = new RiskChanges();
                            rankChange.ChangeType = RiskControlChangeType.IncidentalUpperLimitChange;
                            rankChange.NewValue = IncidentalUpperLimit;
                            rankChange.OldValue = incidentalFee;
                            changes.Add(rankChange);
                        }



                        ChangeHandler h1 = new RankChangeHandler(entity, changes, entity.Admin);
                        ChangeHandler h2 = new NatureChangeHandler(entity, changes, entity.Admin);
                        ChangeHandler h3 = new GoodsChangeHandler(entity, changes, entity.Admin);
                        ChangeHandler h4 = new TaxChangeHandler(entity, changes, entity.Admin);
                        ChangeHandler h5 = new AgencyFeeChangeHandler(entity, changes, entity.Admin);
                        ChangeHandler h6 = new IncidentalFeeChangeHandler(entity, changes, entity.Admin);
                        ChangeHandler h7 = new Agreement2Crm(entity, changes, entity.Admin);

                        h1.setNextHandler(h2);
                        h2.setNextHandler(h3);
                        h3.setNextHandler(h4);
                        h4.setNextHandler(h5);
                        h5.setNextHandler(h6);
                        h6.setNextHandler(h7);
                        string additionMsg = "";
                        if (!string.IsNullOrEmpty(Reason))
                        {
                            additionMsg += "备注:" + Reason;
                        }

                        h1.HandleReqeust("风控人员[" + RiskName + "]审核通过：" + entity.Company.Name + ",", additionMsg);
                    });

                    Thread.Sleep(300);
                    entity.EnterSuccess += ClientStatus_EnterError_EnterSuccess;
                    entity.EnterError += ClientStatus_EnterError;
                    entity.ClientRank = (Needs.Ccs.Services.Enums.ClientRank)Convert.ToInt16(Rank);
                    entity.Audit();
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 风控通过会员列表修改等级，垫款上限
        /// </summary>
        protected void ToModify()
        {
            try
            {
                string Reason = Request.Form["Reason"];
                string id = Request.Form["ID"];
                string Rank = Request.Form["Rank"];
                string Nature = Request.Form["Nature"];
                decimal GoodsUpperLimit = string.IsNullOrEmpty(Request.Form["GoodsUpperLimit"]) ? 0 : Convert.ToDecimal(Request.Form["GoodsUpperLimit"]);
                decimal TaxUpperLimit = string.IsNullOrEmpty(Request.Form["TaxUpperLimit"]) ? 0 : Convert.ToDecimal(Request.Form["TaxUpperLimit"]);
                decimal AgencyFeeUpperLimit = string.IsNullOrEmpty(Request.Form["AgencyFeeUpperLimit"]) ? 0 : Convert.ToDecimal(Request.Form["AgencyFeeUpperLimit"]);
                decimal IncidentalUpperLimit = string.IsNullOrEmpty(Request.Form["IncidentalUpperLimit"]) ? 0 : Convert.ToDecimal(Request.Form["IncidentalUpperLimit"]);
                var entity = Needs.Wl.Admin.Plat.AdminPlat.Clients[id];

                //领导的奇怪要求 显示风控操作
                var adminid = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                var RealName = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;

                var RiskName = System.Configuration.ConfigurationManager.AppSettings["RiskManagementName"];
                var RiskID = System.Configuration.ConfigurationManager.AppSettings["RiskManagementID"];

                if (RealName == "张令金" || RealName == "张庆永")
                {
                    adminid = RiskID;//风控的ID
                    RealName = RiskName;
                }

                var summary = string.Empty;
                if (entity != null)
                {
                    entity.Admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create((string)adminid);

                    System.Threading.Tasks.Task.Run(() =>
                    {
                        List<RiskChanges> changes = new List<RiskChanges>();

                        if ((int)entity.ClientRank != Convert.ToInt16(Rank))
                        {
                            RiskChanges rankChange = new RiskChanges();
                            rankChange.ChangeType = RiskControlChangeType.RankChange;
                            rankChange.NewValue = Convert.ToInt16(Rank);
                            rankChange.OldValue = (int)entity.ClientRank;
                            changes.Add(rankChange);
                        }
                        if (entity.ClientNature == null || (int)entity.ClientNature != Convert.ToInt16(Nature))
                        {
                            entity.ClientNature = (int)ClientNature.Trade;
                            RiskChanges natureChange = new RiskChanges();
                            natureChange.ChangeType = RiskControlChangeType.NatureChange;
                            natureChange.NewValue = Convert.ToInt16(Nature);
                            natureChange.OldValue = (int)entity.ClientNature;
                            changes.Add(natureChange);
                        }
                        EditRankOrNature(entity, changes);
                        decimal productFee = (entity.Agreement.ProductFeeClause == null || entity.Agreement.ProductFeeClause.PeriodType == PeriodType.PrePaid) ? 0 : entity.Agreement.ProductFeeClause.UpperLimit.Value;
                        if (productFee != GoodsUpperLimit)
                        {
                            RiskChanges rankChange = new RiskChanges();
                            rankChange.ChangeType = RiskControlChangeType.GoodsUpperLimitChange;
                            rankChange.NewValue = GoodsUpperLimit;
                            rankChange.OldValue = productFee;
                            changes.Add(rankChange);
                        }
                        decimal taxFee = (entity.Agreement.TaxFeeClause.UpperLimit == null || entity.Agreement.TaxFeeClause.PeriodType == PeriodType.PrePaid) ? 0 : entity.Agreement.TaxFeeClause.UpperLimit.Value;
                        if (taxFee != TaxUpperLimit)
                        {
                            RiskChanges rankChange = new RiskChanges();
                            rankChange.ChangeType = RiskControlChangeType.TaxUpperLimitChange;
                            rankChange.NewValue = TaxUpperLimit;
                            rankChange.OldValue = taxFee;
                            changes.Add(rankChange);
                        }
                        decimal agencyFeeClause = (entity.Agreement.AgencyFeeClause.UpperLimit == null || entity.Agreement.AgencyFeeClause.PeriodType == PeriodType.PrePaid) ? 0 : entity.Agreement.AgencyFeeClause.UpperLimit.Value;
                        if (agencyFeeClause != AgencyFeeUpperLimit)
                        {
                            RiskChanges rankChange = new RiskChanges();
                            rankChange.ChangeType = RiskControlChangeType.AgencyFeeUpperLimitChange;
                            rankChange.NewValue = AgencyFeeUpperLimit;
                            rankChange.OldValue = agencyFeeClause;
                            changes.Add(rankChange);
                        }
                        decimal incidentalFee = (entity.Agreement.IncidentalFeeClause.UpperLimit == null || entity.Agreement.IncidentalFeeClause.PeriodType == PeriodType.PrePaid) ? 0 : entity.Agreement.IncidentalFeeClause.UpperLimit.Value;
                        if (incidentalFee != IncidentalUpperLimit)
                        {
                            RiskChanges rankChange = new RiskChanges();
                            rankChange.ChangeType = RiskControlChangeType.IncidentalUpperLimitChange;
                            rankChange.NewValue = IncidentalUpperLimit;
                            rankChange.OldValue = incidentalFee;
                            changes.Add(rankChange);
                        }



                        ChangeHandler h1 = new RankChangeHandler(entity, changes, entity.Admin);
                        ChangeHandler h2 = new NatureChangeHandler(entity, changes, entity.Admin);
                        ChangeHandler h3 = new GoodsChangeHandler(entity, changes, entity.Admin);
                        ChangeHandler h4 = new TaxChangeHandler(entity, changes, entity.Admin);
                        ChangeHandler h5 = new AgencyFeeChangeHandler(entity, changes, entity.Admin);
                        ChangeHandler h6 = new IncidentalFeeChangeHandler(entity, changes, entity.Admin);
                        ChangeHandler h7 = new Agreement2Crm(entity, changes, entity.Admin);

                        h1.setNextHandler(h2);
                        h2.setNextHandler(h3);
                        h3.setNextHandler(h4);
                        h4.setNextHandler(h5);
                        h5.setNextHandler(h6);
                        h6.setNextHandler(h7);
                        string additionMsg = "";
                        if (!string.IsNullOrEmpty(Reason))
                        {
                            additionMsg += "备注:" + Reason;
                        }

                        h1.HandleReqeust("风控人员[" + RealName + "]编辑会员,", additionMsg);
                    });
                }

                Response.Write((new { success = true, message = "修改成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = ex.Message }).Json());
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

                //领导的奇怪要求 显示风控操作
                var adminid = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                var RealName = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;

                var RiskName = System.Configuration.ConfigurationManager.AppSettings["RiskManagementName"];
                var RiskID = System.Configuration.ConfigurationManager.AppSettings["RiskManagementID"];

                if (RealName == "张令金" || RealName == "张庆永")
                {
                    adminid = RiskID;//风控的ID
                }

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


        /// <summary>
        /// 获取客户风险等级
        /// </summary>
        /// <returns></returns>
        protected void CheckCompanyRisk()
        {
            try
            {
                //var ss = new Needs.Ccs.Services.Views.NoticeBoardView().FirstOrDefault(t => t.ID == "222222222").NoticeContent;

                //Response.Write(new { success = true, message = "获取企业风险信息成功", riskinfo = ss }.Json());

                //return;

                string companyName = Request.Form["CompanyName"];
                if (!string.IsNullOrEmpty(companyName))
                {
                    companyName = companyName.Trim();
                }

                //天眼查调用接口 start
                string TycUrl = ConfigurationManager.AppSettings["TycUrlRisk"];
                string Tyctoken = ConfigurationManager.AppSettings["Tyctoken"];

                string url = $"{TycUrl}?keyword={companyName}";
                var response = HttpGet.CommonHttpRequest(url, "GET", authorization: Tyctoken);
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                #region 解析工商信息
                try
                {
                    TycRisk baseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<TycRisk>(response);
                    if (baseInfo.error_code == "0" && baseInfo.result != null)
                    {
                        Response.Write(new { success = true, message = "获取企业风险信息成功", riskinfo = baseInfo.result }.Json());
                    }
                    else
                    {
                        Response.Write(new { success = false, message = $"获取企业风险信息。异常：" + baseInfo.reason }.Json());
                    }
                }
                catch (Exception ex)
                {
                    Response.Write(new { success = false, message = $"获取企业风险信息。结果解析异常：{companyName}" + ex.Message }.Json());
                }
                #endregion

                //天眼查调用接口 end
            }
            catch (Exception ex)
            {
                Response.Write(new { success = false, message = ex.Message }.Json());
            }
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

        /// <summary>
        /// 风控修改会员等级
        /// </summary>
        protected bool EditRank(Needs.Ccs.Services.Models.Client client, int rank)
        {
            bool success = false;
            try
            {
                string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
                client.ChangeRank(client.ClientRank, (ClientRank)rank);
                //调用CRM接口
                string requestUrl = URL + "/CrmUnify/SetGrade?EnterpriseName=" + client.Company.Name + "&Grade=" + rank;
                HttpResponseMessage response = new HttpResponseMessage();
                response = new HttpClientHelp().HttpClient("POST", requestUrl);
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {

                }
                else
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {


            }

            return success;
        }

        protected bool EditRankOrNature(Needs.Ccs.Services.Models.Client client, List<RiskChanges> changes)
        {
            bool bSuccess = true;
            var RankChange = changes.Where(t => t.ChangeType == RiskControlChangeType.RankChange).FirstOrDefault();
            var NatureChange = changes.Where(t => t.ChangeType == RiskControlChangeType.NatureChange).FirstOrDefault();

            int Rank = (int)client.ClientRank;
            int clientNature = (int)client.ClientNature;

            if (RankChange != null)
            {
                Rank = Convert.ToInt16(RankChange.NewValue);
                client.ChangeRank((ClientRank)RankChange.OldValue, (ClientRank)Convert.ToInt16(RankChange.NewValue));
            }

            if (NatureChange != null)
            {
                clientNature = Convert.ToInt16(NatureChange.NewValue);
                client.ChangeNature((ClientNature)NatureChange.OldValue, (ClientNature)Convert.ToInt16(NatureChange.NewValue));
            }


            if (RankChange != null || NatureChange != null)
            {
                string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
                string requestUrl = URL + "/CrmUnify/WsClientEnter";

                string Corporate = client.Company.Corporate;
                string CompanyName = client.Company.Name;
                string Address = client.Company.Address;
                string CompanyCode = client.Company.Code;
                string ContactName = client.Company.Contact.Name;
                string Mobile = client.Company.Contact.Mobile;
                string Tel = client.Company.Contact.Tel;
                string Fax = client.Company.Contact.Fax;
                string Email = client.Company.Contact.Email;
                string CustomsCode = client.Company.CustomsCode;
                string ClientCode = client.ClientCode;
                string Summary = client.Summary;

                HttpResponseMessage response = new HttpResponseMessage();
                var entity = new Needs.Ccs.Services.Models.ApiModel.ClientModel()
                {
                    Enterprise = new EnterpriseObj
                    {
                        AdminCode = "",
                        District = "",
                        Corporation = Corporate,
                        Name = CompanyName,
                        RegAddress = Address,
                        Uscc = CompanyCode,
                        Status = 200
                    },
                    Contact = new Needs.Ccs.Services.Models.ApiModel.ApiContact
                    {
                        Type = 1,
                        Name = ContactName,
                        Mobile = Mobile,
                        Tel = Tel,
                        Email = Email,
                        Fax = Fax,
                        CreateDate = DateTime.Now.ToString(),
                        UpdateDate = DateTime.Now.ToString()
                    },
                    Vip = false,
                    Creator = client.ServiceManager.RealName,//传业务员给CRM
                    CustomsCode = CustomsCode,
                    EnterCode = ClientCode,
                    Grade = Rank,
                    Summary = Summary,
                    Status = client.ClientStatus == Needs.Ccs.Services.Enums.ClientStatus.Confirmed ? 200 : 1,
                    CreateDate = DateTime.Now.ToString(),
                    UpdateDate = DateTime.Now.ToString(),
                    ClientNature = clientNature,
                    ServiceType = (int)client.ServiceType,

                };
                string apiclient = JsonConvert.SerializeObject(entity);
                response = new HttpClientHelp().HttpClient("POST", requestUrl, apiclient);
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    Response.Write((new { success = false, message = "请求会员接口失败：" }).Json());
                }
            }

            return bSuccess;
        }

    }
}
