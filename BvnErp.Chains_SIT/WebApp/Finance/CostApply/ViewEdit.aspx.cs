using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.CostApply
{
    public partial class ViewEdit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            string From = Request.QueryString["From"];
            string CostApplyID = Request.QueryString["CostApplyID"];

            var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

            this.Model.From = From;

            if (From == "Add")
            {
                this.Model.CostApplyDetail = new
                {
                    CostApplyID = string.Empty,
                    PayeeName = string.Empty,
                    PayeeAccount = string.Empty,
                    PayeeBank = string.Empty,
                    CostTypeInt = string.Empty,
                    CostTypeStr = string.Empty,
                    FeeTypeInt = string.Empty,
                    FeeTypeStr = string.Empty,
                    FeeDesc = string.Empty,
                    Amount = string.Empty,
                    Currency = Needs.Ccs.Services.Enums.Currency.CNY.ToString(),
                    ApplicantID = admin.ID,
                    ApplicantName = admin.ByName,
                    Summary = string.Empty,
                    //CostStatusInt = string.Empty,
                }.Json();
            }
            else if (From == "Edit")
            {
                var costApplyDetail = new Needs.Ccs.Services.Views.CostApplyDetailView().GetResult(CostApplyID);

                this.Model.CostApplyDetail = new
                {
                    CostApplyID = costApplyDetail.CostApplyID,
                    PayeeName = costApplyDetail.PayeeName,
                    PayeeAccount = costApplyDetail.PayeeAccount,
                    PayeeBank = costApplyDetail.PayeeBank,
                    CostTypeInt = costApplyDetail.CostType.GetHashCode(),
                    CostTypeStr = costApplyDetail.CostType.ToString(),
                    FeeTypeInt = costApplyDetail.FeeType.GetHashCode(),
                    FeeTypeStr = costApplyDetail.FeeType.ToString(),
                    FeeDesc = costApplyDetail.FeeDesc,
                    Amount = costApplyDetail.Amount,
                    Currency = costApplyDetail.Currency,
                    ApplicantID = costApplyDetail.ApplicantID,
                    ApplicantName = costApplyDetail.ApplicantName,
                    Summary = costApplyDetail.Summary,
                    //CostStatusInt = (int)costApplyDetail.CostStatus,
                    MoneyTypeName = costApplyDetail.MoneyType.GetDescription(),

                    CashTypeInt = (int)costApplyDetail.CashType,
                    CashTypeName = costApplyDetail.CashType.GetDescription(),
                }.Json();
            }

            Dictionary<string, string> dicCostType = new Dictionary<string, string>();
            foreach (int item in Enum.GetValues(typeof(Needs.Ccs.Services.Enums.CostTypeEnum)))
            {
                string strName = Enum.GetName(typeof(Needs.Ccs.Services.Enums.CostTypeEnum), item);
                string strVaule = item.ToString();
                dicCostType.Add(strVaule, strName);
            }
            this.Model.CostType = dicCostType.Select(item => new { item.Key, item.Value }).Json();

            Dictionary<string, string> dicFeeType = new Dictionary<string, string>();
            foreach (int item in Enum.GetValues(typeof(Needs.Ccs.Services.Enums.FeeTypeEnum)))
            {
                string strName = Enum.GetName(typeof(Needs.Ccs.Services.Enums.FeeTypeEnum), item);
                string strVaule = item.ToString();
                if (item < 20000)
                {
                    dicFeeType.Add(strVaule, strName);
                }
            }
            dicFeeType.Remove("0");
            this.Model.FeeType = dicFeeType.Select(item => new { item.Key, item.Value }).Json();

            //查询当前操作人是否是财务人员
            var oneAdmin = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles
                            .Where(t => t.Role.Name.Contains("财务") && t.Admin.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID).FirstOrDefault();
            this.Model.IsFinanceStaff = (oneAdmin != null) ? "true" : "false";
        }

        /// <summary>
        /// 费用申请文件
        /// </summary>
        protected void CostApplyFiles()
        {
            string CostApplyID = Request.QueryString["CostApplyID"];

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

        /// <summary>
        /// 撤销申请
        /// </summary>
        protected void Cancel()
        {
            try
            {
                string CostApplyID = Request.Form["CostApplyID"];

                var costApply = new Needs.Ccs.Services.Views.CostApplyDetailView().GetResult(CostApplyID);

                var approval = new Needs.Ccs.Services.Models.CostApplyApproval(CostApplyID);

                string rtnMsg = string.Empty;
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                bool result = approval.Cancel(admin, costApply.CostStatus, out rtnMsg);

                if (result)
                {
                    Response.Write((new { success = true, message = "操作成功" }).Json());
                }
                else
                {
                    Response.Write((new { success = false, message = "错误：" + rtnMsg }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "发生错误：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 首次提交/重新提交
        /// </summary>
        protected void Submit()
        {
            try
            {
                string From = Request.Form["From"];

                string CostApplyID = string.Empty;
                if (From.ToLower() == "Add".ToLower())
                {
                    CostApplyID = Needs.Overall.PKeySigner.Pick(PKeyType.PayApplicant);//修改ID的存值 2020-09-15 by yeshuangshuang //Guid.NewGuid().ToString("N");
                }
                else if (From.ToLower() == "Edit".ToLower())
                {
                    CostApplyID = Request.Form["CostApplyID"];
                }

                string CostType = Request.Form["CostType"];
                string FeeTypeInt = Request.Form["FeeType"];
                string FeeDesc = Request.Form["FeeDesc"];
                string PayeeName = Request.Form["PayeeName"];
                string PayeeAccount = Request.Form["PayeeAccount"];
                string PayeeBank = Request.Form["PayeeBank"];
                string Amount = Request.Form["Amount"];
                string Currency = Request.Form["Currency"];
                string Files = Request.Form["Files"].Replace("&quot;", "'");
                string Summary = Request.Form["Summary"];
                string MoneyType = Request.Form["MoneyType"];
                string strIsCash = Request.Form["IsCash"];
                bool isCash = false;
                if (!string.IsNullOrEmpty(strIsCash))
                {
                    bool.TryParse(strIsCash, out isCash);
                }

                if (string.IsNullOrEmpty(FeeTypeInt))
                {
                    FeeTypeInt = "0";
                }

                var FileList = Files.JsonTo<List<dynamic>>();

                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                List<Needs.Ccs.Services.Models.CostApplyFile> costApplyFiles = new List<Needs.Ccs.Services.Models.CostApplyFile>();
                foreach (var item in FileList)
                {
                    costApplyFiles.Add(new Needs.Ccs.Services.Models.CostApplyFile
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        CostApplyID = CostApplyID,
                        AdminID = admin.ID,
                        Name = item.Name,
                        FileType = CostApplyFileTypeEnum.Inovice,
                        FileFormat = item.FileFormat,
                        URL = Convert.ToString(item.VirtualPath).Replace(@"/", @"\"),
                        Status = Status.Normal,
                        CreateDate = DateTime.Now,
                    });
                }

                Needs.Ccs.Services.Models.CostApply costApply = new Needs.Ccs.Services.Models.CostApply
                {
                    ID = CostApplyID,
                    PayeeName = PayeeName,
                    PayeeAccount = PayeeAccount,
                    PayeeBank = PayeeBank,
                    Amount = Convert.ToDecimal(Amount),
                    Currency = Currency,
                    CostStatus = Needs.Ccs.Services.Enums.CostStatusEnum.FinanceStaffUnApprove,
                    AdminID = admin.ID,
                    Status = Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = Summary,
                    MoneyType = !string.IsNullOrEmpty(MoneyType) ? (Needs.Ccs.Services.Enums.MoneyTypeEnum)int.Parse(MoneyType) : MoneyTypeEnum.IndividualApply,
                    CashType = isCash ? CashTypeEnum.Cash : CashTypeEnum.Common,
                };

                if (From.ToLower() == "Edit".ToLower())
                {
                    var oldCostApply = new Needs.Ccs.Services.Views.CostApplyDetailView().GetResult(CostApplyID);
                    costApply.CreateDate = oldCostApply.CreateDate;
                    costApply.MoneyType = oldCostApply.MoneyType;
                    costApply.CashType = oldCostApply.CashType;
                }

                string costApplyLogSummary = string.Empty;
                if (From.ToLower() == "Add".ToLower())
                {
                    costApplyLogSummary = admin.ByName + "提交了费用申请";
                }
                else if (From.ToLower() == "Edit".ToLower())
                {
                    costApplyLogSummary = admin.ByName + "重新提交了费用申请";
                }

                Needs.Ccs.Services.Models.CostApplyLog costApplyLog = new Needs.Ccs.Services.Models.CostApplyLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    CostApplyID = CostApplyID,
                    AdminID = admin.ID,
                    CurrentCostStatus = CostStatusEnum.UnSubmit,
                    NextCostStatus = CostStatusEnum.FinanceStaffUnApprove,
                    CreateDate = DateTime.Now,
                    Summary = costApplyLogSummary,
                };

                //保存 Begin

                costApply.AbandonFileFiles();
                foreach (var costApplyFile in costApplyFiles)
                {
                    costApplyFile.Enter();
                }
                costApply.Enter();
                costApplyLog.Enter();

                //保存 End

                Response.Write((new { success = true, message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "提交失败：" + ex.Message }).Json());
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
                                FileType = FileType.OriginalInvoice.GetDescription(),
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


    }
}
