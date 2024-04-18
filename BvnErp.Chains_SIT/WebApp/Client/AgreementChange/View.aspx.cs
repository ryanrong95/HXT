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

namespace WebApp.Client.AgreementChange
{
    public partial class View : Uc.PageBase
    {
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
            var AgreementApply = new Needs.Ccs.Services.Views.AgreementChangeApplyView().Where(t => t.ID == id).ToList();

            if (AgreementApply != null)
            {
                foreach (var item in AgreementApply)
                {
                    //if (item.AgreementChangeType)
                    //{

                    //}
                }
                //申请信息
                //this.Model.AgreementApply = new
                //{
                //    From = From,
                //    ID = AgreementApply.ID,
                //    ClientCode = AgreementApply.ClientCode,
                //    ClientName = AgreementApply.ClientName,
                //    ClientID = AgreementApply.ClientID,
                //    ClientRank = AgreementApply.ClientRank,
                //    ChangeType = AgreementApply.AgreementChangeType 
                //}.Json();
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
                        Needs.Ccs.Services.Models.AdvanceMoneyApplyLogModel advanceMoneyApplyLogs = new Needs.Ccs.Services.Models.AdvanceMoneyApplyLogModel()
                        {
                            ID = Guid.NewGuid().ToString("N"),
                            ApplyID = applyID,
                            Status = Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Auditing,
                            AdminID = admin.ID,
                            CreateDate = DateTime.Now,
                            Summary = "风控【" + admin.RealName + "】审核通过了垫资申请,等待总经理审批；备注：" + summary,
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

                        clientAgreement.ID = agreement.ID;
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

                        clientAgreement.IsTen = agreement.IsTen;

                        //货款
                        clientAgreement.ProductFeeClause.AgreementID = agreement.ID;
                        clientAgreement.ProductFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Product;
                        clientAgreement.ProductFeeClause.PeriodType = Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod;
                        clientAgreement.ProductFeeClause.DaysLimit = Convert.ToInt32(limitDay);//advanceMoneyApply.LimitDays;
                        clientAgreement.ProductFeeClause.MonthlyDay = agreement.ProductFeeClause.MonthlyDay.ToString() == "" ? (int?)null : agreement.ProductFeeClause.MonthlyDay;
                        clientAgreement.ProductFeeClause.UpperLimit = Convert.ToDecimal(advanceAmount); //advanceMoneyApply.Amount;
                        clientAgreement.ProductFeeClause.ExchangeRateType = agreement.ProductFeeClause.ExchangeRateType;
                        clientAgreement.ProductFeeClause.ExchangeRateValue = agreement.ProductFeeClause.ExchangeRateValue;
                        clientAgreement.ProductFeeClause.AdminID = agreement.ProductFeeClause.AdminID;

                        //税款
                        clientAgreement.TaxFeeClause.AgreementID = agreement.ID;
                        clientAgreement.TaxFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Tax;
                        clientAgreement.TaxFeeClause.PeriodType = agreement.TaxFeeClause.PeriodType;
                        clientAgreement.TaxFeeClause.DaysLimit = agreement.TaxFeeClause.DaysLimit.ToString() == "" ? (int?)null : agreement.TaxFeeClause.DaysLimit;
                        clientAgreement.TaxFeeClause.MonthlyDay = agreement.TaxFeeClause.MonthlyDay.ToString() == "" ? (int?)null : agreement.TaxFeeClause.MonthlyDay;
                        clientAgreement.TaxFeeClause.UpperLimit = agreement.TaxFeeClause.UpperLimit.ToString() == "" ? (int?)null : agreement.TaxFeeClause.UpperLimit;
                        clientAgreement.TaxFeeClause.ExchangeRateType = agreement.TaxFeeClause.ExchangeRateType;
                        clientAgreement.TaxFeeClause.ExchangeRateValue = agreement.TaxFeeClause.ExchangeRateValue;

                        clientAgreement.TaxFeeClause.AdminID = agreement.TaxFeeClause.AdminID;

                        //代理费
                        clientAgreement.AgencyFeeClause.AgreementID = agreement.ID;
                        clientAgreement.AgencyFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.AgencyFee;
                        clientAgreement.AgencyFeeClause.PeriodType = agreement.AgencyFeeClause.PeriodType;
                        clientAgreement.AgencyFeeClause.DaysLimit = agreement.AgencyFeeClause.DaysLimit.ToString() == "" ? (int?)null : agreement.AgencyFeeClause.DaysLimit;
                        clientAgreement.AgencyFeeClause.MonthlyDay = agreement.AgencyFeeClause.MonthlyDay.ToString() == "" ? (int?)null : agreement.AgencyFeeClause.MonthlyDay;
                        clientAgreement.AgencyFeeClause.UpperLimit = agreement.AgencyFeeClause.UpperLimit.ToString() == "" ? (int?)null : agreement.AgencyFeeClause.UpperLimit;
                        clientAgreement.AgencyFeeClause.ExchangeRateType = agreement.AgencyFeeClause.ExchangeRateType;
                        clientAgreement.AgencyFeeClause.ExchangeRateValue = agreement.AgencyFeeClause.ExchangeRateValue;
                        clientAgreement.AgencyFeeClause.AdminID = agreement.AgencyFeeClause.AdminID;

                        //杂费
                        clientAgreement.IncidentalFeeClause.AgreementID = agreement.ID;
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
                            //string requestUrl = URL + "/CrmUnify/Contract";
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
                           // response = new HttpClientHelp().HttpClient("POST", requestUrl, apiAgreement);
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
                            Needs.Ccs.Services.Models.AdvanceMoneyApplyLogModel advanceMoneyApplyLogs = new Needs.Ccs.Services.Models.AdvanceMoneyApplyLogModel()
                            {
                                ID = Guid.NewGuid().ToString("N"),
                                ApplyID = applyID,
                                Status = Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Effective,
                                AdminID = admin.ID,
                                CreateDate = DateTime.Now,
                                Summary = "经理【" + admin.RealName + "】审批通过了垫资申请；备注：" + summary,
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
                    summary = "风控【" + admin.RealName + "】拒绝了垫资申请；备注：" + reason;
                }
                else
                {
                    summary = "经理【" + admin.RealName + "】拒绝了垫资申请；备注：" + reason;
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
    }
}