using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
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

namespace WebApp.AdvanceMoney
{
    public partial class View : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }
        protected void LoadData()
        {
            this.Model.AdvanceMoneyApply = "".Json();

            string id = Request.QueryString["ID"];
            string From = Request.QueryString["From"];
            var advanceMoneyApply = new Needs.Ccs.Services.Views.AdvanceMoneyApplyView().FirstOrDefault(t => t.ID == id);

            if (advanceMoneyApply != null)
            {
                string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["PvDataFileUrl"];
                var serviceFile = new CenterFilesTopView().FirstOrDefault(x => x.ClientID == advanceMoneyApply.ClientID && x.Type == (int)Needs.Ccs.Services.Enums.FileType.AdvanceMoneyApplyAgreement && x.Status != FileDescriptionStatus.Delete && x.ApplicationID == id);
                if (serviceFile != null)
                {
                    this.Model.ServiceFile = (new { Name = serviceFile.CustomName, Url = FileServerUrl + @"/" + serviceFile.Url.ToUrl() }).Json();
                }
                else
                {
                    this.Model.ServiceFile = null;
                }

                //申请信息
                this.Model.AdvanceMoneyApply = new
                {
                    From = From,
                    ID = advanceMoneyApply.ID,
                    ClientCode = advanceMoneyApply.ClientCode,
                    ClientName = advanceMoneyApply.ClientName,
                    ClientID = advanceMoneyApply.ClientID,
                    Amount = advanceMoneyApply.Amount,
                    AmountUsed = advanceMoneyApply.AmountUsed,
                    LimitDays = advanceMoneyApply.LimitDays,
                    InterestRate = advanceMoneyApply.InterestRate,
                    OverdueInterestRate = advanceMoneyApply.OverdueInterestRate,
                    Summary = advanceMoneyApply.Summary,
                    IntStatus = advanceMoneyApply.IntStatus
                }.Json();
            }

        }

        /// <summary>
        /// 日志记录
        /// </summary>
        protected void LoadLogs()
        {
            string ApplyID = Request.Form["ApplyID"];
            var applyLogs = new Needs.Ccs.Services.Views.AdvanceMoneyApplyLogView().Where(t => t.ApplyID == ApplyID).ToArray();
            Func<AdvanceMoneyApplyLogModel, object> convert = item => new
            {
                Summary = item.Summary,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            };
            Response.Write(new { rows = applyLogs.OrderByDescending(t => t.CreateDate).Select(convert).ToArray() }.Json());
        }

        /// <summary>
        /// 加载附件
        /// </summary>
        protected void filedata()
        {
            string id = Request.QueryString["ID"];
            if (string.IsNullOrEmpty(ID))
            {
                return;
            }
            var data = new Needs.Ccs.Services.Views.CenterFilesView().Where(t => t.ApplicationID == id).ToList();

            Func<CenterFileModel, object> convert = item => new
            {
                ID = item.ID,
                FileName = item.CustomName,
                FileFormat = item.FileFormat,
                Url = item.Url,    //数据库相对路径
                                   //WebUrl = FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),//查看路径
                                   // WebUrl = (DateTime.Compare(item.CreateDate, Convert.ToDateTime(FileDirectory.Current.IsChainsDate)) > 0)
                                   //? FileDirectory.Current.PvDataFileUrl + "/" + item.Url.ToUrl()
                                   //: FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),
                WebUrl = FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),
            };
            Response.Write(new
            {
                rows = data.Select(convert).ToArray(),
                total = data.Count(),
            }.Json());
        }
        /// <summary>
        /// 导出协议word
        /// </summary>
        protected void ExportModel()
        {
            try
            {
                var clientID = Request.Form["ClientID"];
                var StrIsdraft = Request.Form["IsDraft"];

                var clientAgreement = new Needs.Ccs.Services.Views.ClientAgreementsView().FirstOrDefault(t => t.ClientID == clientID && t.Status == Status.Normal);
                if (clientAgreement != null)
                {
                    var agreement = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientAgreements[clientAgreement.ID];
                    //创建文件夹
                    var fileName = DateTime.Now.Ticks + "芯达通垫款保证协议.docx";
                    FileDirectory file = new FileDirectory(fileName);
                    file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                    file.CreateDataDirectory();
                    //保存文件
                    agreement.XDTSaveAs(file.FilePath);
                    bool IsDraft;
                    bool.TryParse(StrIsdraft, out IsDraft);
                    if (IsDraft)
                    {
                        Aspose.Words.Document document = new Aspose.Words.Document(file.FilePath);
                        InsertWatermarkText(document, true, "仅供预览", -45);
                        document.Save(file.FilePath);
                    }


                    Response.Write((new { success = true, message = "导出成功", url = file.FileUrl }).Json());
                }

            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败" + ex.Message }).Json());
            }
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        protected void UploadFiles()
        {
            try
            {
                List<dynamic> fileList = new List<dynamic>();
                var TemporaryID = Request.Form["ID"];
                HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            //文件保存
                            string fileName = file.FileName.ReName();

                            //创建文件目录
                            FileDirectory fileDic = new FileDirectory(fileName);
                            fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.AdvanceMoney);
                            fileDic.CreateDataDirectory();
                            file.SaveAs(fileDic.FilePath);
                            fileList.Add(new
                            {
                                FileName = file.FileName,
                                FileFormat = file.ContentType,
                                WebUrl = fileDic.FileUrl,
                                Url = fileDic.VirtualPath,
                            });
                        }
                    }
                }
                Response.Write((new { success = true, data = fileList }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, data = ex.Message }).Json());
            }
        }
        protected void Submit()
        {
            try
            {
                string applyID = Request.Form["ApplyID"];
                string advanceAmount = Request.Form["AdvanceAmount"];
                string limitDay = Request.Form["LimitDay"];
                string summary = Request.Form["Summary"];
                string from = Request.Form["From"];
                string clientID = Request.Form["ClientID"];
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                if (from == "Audit")
                {
                    string files = Request.Form["Files"].Replace("&quot;", "'");
                    //var FileList = files.JsonTo<List<dynamic>>();
                    IEnumerable<CenterFileModel> FileList = files.JsonTo<IEnumerable<CenterFileModel>>();
                    var ids = new Needs.Ccs.Services.Views.CenterFilesView().Where(t => t.ApplicationID == applyID).Select(t => t.ID);
                    var fileids = FileList.Select(t => t.ID);
                    foreach (var ID in ids)
                    {
                        if (!fileids.Contains(ID))
                        {
                            new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Delete }, ID);
                        }
                    }
                    foreach (var filelist in FileList)
                    {
                        #region 关联付汇委托书上传中心

                        if (string.IsNullOrEmpty(filelist.ID))
                        {
                            var entity = new CenterFileDescription();
                            entity.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                            entity.ApplicationID = applyID;
                            entity.Type = 1;
                            entity.Url = Convert.ToString(filelist.Url).Replace(@"/", @"\");
                            entity.Status = FileDescriptionStatus.Normal;
                            entity.CreateDate = DateTime.Now;
                            entity.CustomName = filelist.FileName;

                            DateTime liunxStart = new DateTime(1970, 1, 1);
                            var linuxtime = (DateTime.Now - liunxStart).Ticks;
                            string topID = "F" + linuxtime;

                            new CenterFilesTopView().Insert(entity, topID);

                        }

                        #endregion
                    }
                    if (!string.IsNullOrEmpty(applyID))
                    {
                        //风控修改垫资申请状态
                        Needs.Ccs.Services.Models.AdvanceMoneyApplyModel advanceMoneyApply = new Needs.Ccs.Services.Models.AdvanceMoneyApplyModel
                        {
                            ApplyID = applyID,
                            Amount = Convert.ToDecimal(advanceAmount),
                            LimitDays = Convert.ToInt32(limitDay),
                            Status = Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Auditing,
                            UpdateDate = DateTime.Now,
                            Summary = summary,
                        };

                        var RealName = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;
                        var RiskName = System.Configuration.ConfigurationManager.AppSettings["RiskManagementName"];

                        if (Needs.Wl.Admin.Plat.AdminPlat.Current.RealName == "张令金" || Needs.Wl.Admin.Plat.AdminPlat.Current.RealName == "张庆永")
                        {
                            RealName = RiskName;
                        }
                        Needs.Ccs.Services.Models.AdvanceMoneyApplyLogModel advanceMoneyApplyLogs = new Needs.Ccs.Services.Models.AdvanceMoneyApplyLogModel()
                        {
                            ID = Guid.NewGuid().ToString("N"),
                            ApplyID = applyID,
                            Status = Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Auditing,
                            AdminID = admin.ID,
                            CreateDate = DateTime.Now,
                            Summary = "风控【" + RealName + "】审核通过了垫资申请,等待总经理审批；备注：" + summary,
                        };

                        //保存 Begin

                        advanceMoneyApply.Update();

                        advanceMoneyApplyLogs.Enter();

                        Response.Write((new { success = true, message = "提交成功" }).Json());
                    }
                    else
                    {
                        Response.Write((new { success = false, message = "提交失败" }).Json());
                    }
                }
                else
                {
                    // var advanceMoneyApply = new Needs.Ccs.Services.Views.AdvanceMoneyApplyView1().FirstOrDefault(t => t.ClientID == ClientID && t.Status == Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Auditing);
                    var agreement = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientAgreements.Where(t => t.ClientID == clientID && t.Status == Needs.Ccs.Services.Enums.Status.Normal).FirstOrDefault();
                    if (agreement != null)
                    {
                        var clientAgreement = new Needs.Ccs.Services.Models.ClientAgreement();

                        clientAgreement.ProductFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
                        clientAgreement.TaxFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
                        clientAgreement.AgencyFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
                        clientAgreement.IncidentalFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();

                        //clientAgreement.ID = agreement.ID;
                        clientAgreement.ClientID = clientID;
                        clientAgreement.AdminID = agreement.AdminID;

                        clientAgreement.StartDate = Convert.ToDateTime(agreement.StartDate);
                        clientAgreement.EndDate = Convert.ToDateTime(agreement.EndDate);
                        clientAgreement.AgencyRate = agreement.AgencyRate;
                        clientAgreement.MinAgencyFee = agreement.MinAgencyFee;
                        clientAgreement.IsPrePayExchange = agreement.IsPrePayExchange;
                        clientAgreement.IsLimitNinetyDays = agreement.IsLimitNinetyDays;
                        clientAgreement.InvoiceType = agreement.InvoiceType;
                        clientAgreement.InvoiceTaxRate = agreement.InvoiceTaxRate;
                        clientAgreement.Summary = agreement.Summary;

                        //货款
                        clientAgreement.ProductFeeClause.AgreementID = clientAgreement.ID;//agreement.ID;
                        clientAgreement.ProductFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Product;
                        clientAgreement.ProductFeeClause.PeriodType = Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod;
                        clientAgreement.ProductFeeClause.DaysLimit = Convert.ToInt32(limitDay);//advanceMoneyApply.LimitDays;
                        clientAgreement.ProductFeeClause.MonthlyDay = agreement.ProductFeeClause.MonthlyDay.ToString() == "" ? (int?)null : agreement.ProductFeeClause.MonthlyDay;
                        clientAgreement.ProductFeeClause.UpperLimit = Convert.ToDecimal(advanceAmount); //advanceMoneyApply.Amount;
                        clientAgreement.ProductFeeClause.ExchangeRateType = agreement.ProductFeeClause.ExchangeRateType;
                        clientAgreement.ProductFeeClause.ExchangeRateValue = agreement.ProductFeeClause.ExchangeRateValue;
                        clientAgreement.ProductFeeClause.AdminID = agreement.ProductFeeClause.AdminID;

                        //税款
                        clientAgreement.TaxFeeClause.AgreementID = clientAgreement.ID;// agreement.ID;
                        clientAgreement.TaxFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Tax;
                        clientAgreement.TaxFeeClause.PeriodType = agreement.TaxFeeClause.PeriodType;
                        clientAgreement.TaxFeeClause.DaysLimit = agreement.TaxFeeClause.DaysLimit.ToString() == "" ? (int?)null : agreement.TaxFeeClause.DaysLimit;
                        clientAgreement.TaxFeeClause.MonthlyDay = agreement.TaxFeeClause.MonthlyDay.ToString() == "" ? (int?)null : agreement.TaxFeeClause.MonthlyDay;
                        clientAgreement.TaxFeeClause.UpperLimit = agreement.TaxFeeClause.UpperLimit.ToString() == "" ? (int?)null : agreement.TaxFeeClause.UpperLimit;
                        clientAgreement.TaxFeeClause.ExchangeRateType = agreement.TaxFeeClause.ExchangeRateType;
                        clientAgreement.TaxFeeClause.ExchangeRateValue = agreement.TaxFeeClause.ExchangeRateValue;

                        clientAgreement.TaxFeeClause.AdminID = agreement.TaxFeeClause.AdminID;

                        //代理费
                        clientAgreement.AgencyFeeClause.AgreementID = clientAgreement.ID;// agreement.ID;
                        clientAgreement.AgencyFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.AgencyFee;
                        clientAgreement.AgencyFeeClause.PeriodType = agreement.AgencyFeeClause.PeriodType;
                        clientAgreement.AgencyFeeClause.DaysLimit = agreement.AgencyFeeClause.DaysLimit.ToString() == "" ? (int?)null : agreement.AgencyFeeClause.DaysLimit;
                        clientAgreement.AgencyFeeClause.MonthlyDay = agreement.AgencyFeeClause.MonthlyDay.ToString() == "" ? (int?)null : agreement.AgencyFeeClause.MonthlyDay;
                        clientAgreement.AgencyFeeClause.UpperLimit = agreement.AgencyFeeClause.UpperLimit.ToString() == "" ? (int?)null : agreement.AgencyFeeClause.UpperLimit;
                        clientAgreement.AgencyFeeClause.ExchangeRateType = agreement.AgencyFeeClause.ExchangeRateType;
                        clientAgreement.AgencyFeeClause.ExchangeRateValue = agreement.AgencyFeeClause.ExchangeRateValue;
                        clientAgreement.AgencyFeeClause.AdminID = agreement.AgencyFeeClause.AdminID;

                        //杂费
                        clientAgreement.IncidentalFeeClause.AgreementID = clientAgreement.ID;// agreement.ID;
                        clientAgreement.IncidentalFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Incidental;
                        clientAgreement.IncidentalFeeClause.PeriodType = agreement.IncidentalFeeClause.PeriodType;
                        clientAgreement.IncidentalFeeClause.DaysLimit = agreement.IncidentalFeeClause.DaysLimit.ToString() == "" ? (int?)null : agreement.IncidentalFeeClause.DaysLimit;
                        clientAgreement.IncidentalFeeClause.MonthlyDay = agreement.IncidentalFeeClause.MonthlyDay.ToString() == "" ? (int?)null : agreement.IncidentalFeeClause.MonthlyDay;
                        clientAgreement.IncidentalFeeClause.UpperLimit = agreement.IncidentalFeeClause.UpperLimit.ToString() == "" ? (int?)null : agreement.IncidentalFeeClause.UpperLimit;
                        clientAgreement.IncidentalFeeClause.ExchangeRateType = agreement.IncidentalFeeClause.ExchangeRateType;
                        clientAgreement.IncidentalFeeClause.ExchangeRateValue = agreement.IncidentalFeeClause.ExchangeRateValue;

                        clientAgreement.IncidentalFeeClause.AdminID = agreement.IncidentalFeeClause.AdminID;

                        clientAgreement.EnterError += ClientAgreement_EnterError;
                        clientAgreement.EnterSuccess += ClientAgreement_EnterSuccess;
                        #region  调用之后
                        try
                        {
                            string requestUrl = URL + "/CrmUnify/Contract";
                            HttpResponseMessage response = new HttpResponseMessage();
                            var client = Needs.Wl.Admin.Plat.AdminPlat.Clients[clientAgreement.ClientID];
                            var entity = new Needs.Ccs.Services.Models.ApiModel.ClientAgreement()
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
                                Agreement = new ApiModel.Agreement
                                {
                                    StartDate = clientAgreement.StartDate,
                                    EndDate = clientAgreement.EndDate,
                                    AgencyRate = clientAgreement.AgencyRate,
                                    MinAgencyFee = clientAgreement.MinAgencyFee,
                                    ExchangeMode = clientAgreement.IsPrePayExchange ? 1 : 2,
                                    InvoiceType = (int)clientAgreement.InvoiceType,
                                    InvoiceTaxRate = clientAgreement.InvoiceTaxRate,
                                    Summary = clientAgreement.Summary,
                                    CreateDate = DateTime.Now.ToString(),
                                    UpdateDate = DateTime.Now.ToString(),
                                    AgencyFeeClause = clientAgreement.AgencyFeeClause,
                                    ProductFeeClause = clientAgreement.ProductFeeClause,
                                    TaxFeeClause = clientAgreement.TaxFeeClause,
                                    IncidentalFeeClause = clientAgreement.IncidentalFeeClause


                                },
                                Creator = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName,
                            };

                            string apiAgreement = JsonConvert.SerializeObject(entity);
                            response = new HttpClientHelp().HttpClient("POST", requestUrl, apiAgreement);
                            if (response == null || response.StatusCode != HttpStatusCode.OK)
                            {
                                Response.Write((new { success = false, message = "请求会员接口失败：" }).Json());
                                return;
                            }
                            clientAgreement.Enter();

                            //经理修改垫资申请状态
                            Needs.Ccs.Services.Models.AdvanceMoneyApplyModel advanceMoneyApplym = new Needs.Ccs.Services.Models.AdvanceMoneyApplyModel
                            {
                                ApplyID = applyID,
                                Status = Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Effective,
                                UpdateDate = DateTime.Now,
                                Summary = summary,
                            };

                            var RealName = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;
                            if (Needs.Wl.Admin.Plat.AdminPlat.Current.RealName == "张令金")
                            {
                                RealName = "张庆永";
                            }
                            Needs.Ccs.Services.Models.AdvanceMoneyApplyLogModel advanceMoneyApplyLogs = new Needs.Ccs.Services.Models.AdvanceMoneyApplyLogModel()
                            {
                                ID = Guid.NewGuid().ToString("N"),
                                ApplyID = applyID,
                                Status = Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Effective,
                                AdminID = admin.ID,
                                CreateDate = DateTime.Now,
                                Summary = "经理【" + RealName + "】审批通过了垫资申请；备注：" + summary,
                            };

                            //保存 Begin

                            advanceMoneyApplym.Audit();

                            advanceMoneyApplyLogs.Enter();
                        }
                        catch (Exception ex)
                        {
                            Response.Write(new { success = false, message = ex.Message });
                        }

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write((new
                {
                    success = false,
                    message = "提交失败：" + ex.Message
                }).Json());
            }
        }
        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientAgreement_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientAgreement_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }

        /// <summary>
        /// 垫资申请拒绝
        /// </summary>
        protected void Refuse()
        {
            try
            {
                string applyID = Request.Form["AdvanceMoneyApplyID"];
                string reason = Request.Form["ApproveSummary"];
                string From = Request.Form["From"];
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var summary = "";
                if (From == "Audit")
                {
                    var RealName = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;
                    var RiskName = System.Configuration.ConfigurationManager.AppSettings["RiskManagementName"];
                    if (Needs.Wl.Admin.Plat.AdminPlat.Current.RealName == "张令金" || Needs.Wl.Admin.Plat.AdminPlat.Current.RealName == "张庆永")
                    {
                        RealName = RiskName;
                    }
                    summary = "风控【" + RealName + "】拒绝了垫资申请；备注：" + reason;
                }
                else
                {
                    var RealName = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;
                    if (Needs.Wl.Admin.Plat.AdminPlat.Current.RealName == "张令金")
                    {
                        RealName = "张庆永";
                    }
                    summary = "经理【" + RealName + "】拒绝了垫资申请；备注：" + reason;
                }
                if (!string.IsNullOrEmpty(applyID))
                {
                    //修改垫资申请状态
                    Needs.Ccs.Services.Models.AdvanceMoneyApplyModel advanceMoneyApply = new Needs.Ccs.Services.Models.AdvanceMoneyApplyModel
                    {
                        ApplyID = applyID,
                        Status = Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Delete,
                        UpdateDate = DateTime.Now,
                        Summary = reason,
                    };
                    Needs.Ccs.Services.Models.AdvanceMoneyApplyLogModel advanceMoneyApplyLogs = new Needs.Ccs.Services.Models.AdvanceMoneyApplyLogModel()
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        ApplyID = applyID,
                        Status = Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Delete,
                        AdminID = admin.ID,
                        CreateDate = DateTime.Now,
                        Summary = summary
                    };

                    //保存 Begin

                    advanceMoneyApply.Delete();

                    advanceMoneyApplyLogs.Enter();

                    //保存 End
                    Response.Write((new { success = true, message = "保存成功" }).Json());
                }
                else
                {
                    Response.Write((new { success = false, message = "保存失败" }).Json());
                }

            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 插入水印
        /// </summary>
        /// <param name="doc">要插入水印的文档对象</param>
        /// <param name="isTextWatermaker">是否文字水印（否的话需要传入图片的绝对地址）</param>
        /// <param name="watermarkText">水印内容（文字或图片的绝对地址）</param>
        /// <param name="rotation"></param>
        private static void InsertWatermarkText(Aspose.Words.Document doc, bool isTextWatermaker, string watermarkText, int rotation)
        {
            // Create a watermark shape. This will be a WordArt shape.
            // You are free to try other shape types as watermarks.
            Aspose.Words.Drawing.Shape watermark = null;

            if (isTextWatermaker)
            {
                watermark = new Aspose.Words.Drawing.Shape(doc, Aspose.Words.Drawing.ShapeType.TextPlainText);

                // Set up the text of the watermark.
                watermark.TextPath.Text = watermarkText;
                //watermark.TextPath.FontFamily = "微软雅黑";
                watermark.TextPath.FontFamily = "Arial";
                watermark.Width = 500;
                watermark.Height = 100;

            }
            else
            {
                watermark = new Aspose.Words.Drawing.Shape(doc, Aspose.Words.Drawing.ShapeType.Image);
                watermark.ImageData.SetImage(watermarkText);
                watermark.Width = watermark.ImageData.ImageSize.WidthPixels;
                watermark.Height = watermark.ImageData.ImageSize.HeightPixels;
                watermark.HorizontalAlignment = Aspose.Words.Drawing.HorizontalAlignment.Right; //靠右对齐
                //watermark.BehindText = true;
            }
            // Text will be directed from the bottom-left to the top-right corner.
            watermark.Rotation = rotation;
            // Remove the following two lines if you need a solid black text.
            watermark.Fill.Color = System.Drawing.Color.LightGray; // Try LightGray to get more Word-style watermark
            watermark.StrokeColor = System.Drawing.Color.LightGray; // Try LightGray to get more Word-style watermark

            // Place the watermark in the page center.
            watermark.RelativeHorizontalPosition = Aspose.Words.Drawing.RelativeHorizontalPosition.Page;
            watermark.RelativeVerticalPosition = Aspose.Words.Drawing.RelativeVerticalPosition.Page;
            watermark.WrapType = Aspose.Words.Drawing.WrapType.None;
            watermark.VerticalAlignment = Aspose.Words.Drawing.VerticalAlignment.Center;
            watermark.HorizontalAlignment = Aspose.Words.Drawing.HorizontalAlignment.Center;

            // Create a new paragraph and append the watermark to this paragraph.
            Aspose.Words.Paragraph watermarkPara = new Aspose.Words.Paragraph(doc);
            watermarkPara.AppendChild(watermark);

            // Insert the watermark into all headers of each document section.
            foreach (Aspose.Words.Section sect in doc.Sections)
            {
                // There could be up to three different headers in each section, since we want
                // the watermark to appear on all pages, insert into all headers.
                InsertWatermarkIntoHeader(watermarkPara, sect, Aspose.Words.HeaderFooterType.HeaderPrimary);
                InsertWatermarkIntoHeader(watermarkPara, sect, Aspose.Words.HeaderFooterType.HeaderFirst);
                InsertWatermarkIntoHeader(watermarkPara, sect, Aspose.Words.HeaderFooterType.HeaderEven);
            }
        }

        private static void InsertWatermarkIntoHeader(Aspose.Words.Paragraph watermarkPara, Aspose.Words.Section sect, Aspose.Words.HeaderFooterType headerType)
        {
            Aspose.Words.HeaderFooter header = sect.HeadersFooters[headerType];

            if (header == null)
            {
                // There is no header of the specified type in the current section, create it.
                header = new Aspose.Words.HeaderFooter(sect.Document, headerType);
                sect.HeadersFooters.Add(header);
            }

            // Insert a clone of the watermark into the header.
            header.AppendChild(watermarkPara.Clone(true));
        }
    }
}
