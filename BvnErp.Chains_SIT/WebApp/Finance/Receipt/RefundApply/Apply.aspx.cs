using Needs.Ccs.Services;
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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Receipt.RefundApply
{
    public partial class Apply : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string CostApplyID = Request.QueryString["ApplyID"];
                var apply = new Needs.Ccs.Services.Views.RefundApplyView().Where(t => t.ID == CostApplyID).FirstOrDefault();
                if (apply != null)
                {
                    this.Model.RefundApply = new
                    {
                        RefundAmount = apply.Amount,
                        PayeeName = apply.PayeeAccount.AccountName,
                        PayeeAccount = apply.PayeeAccount.BankAccount,
                        PayeeBank = apply.PayeeAccount.BankName,
                        PayeeAccountID = apply.PayeeAccount.ID,
                        Remark = apply.Summary
                    }.Json();



                    //显示收款信息
                    this.Model.FinanceReceipt = new Needs.Ccs.Services.Views.ReceiptNoticesView().Where(t => t.ID == apply.FinanceReceiptID).FirstOrDefault().Json().Replace("'","");
                }
                else
                {
                    this.Model.RefundApply = new
                    {
                        
                    }.Json();

                    this.Model.FinanceReceipt = new
                    {

                    }.Json();
                }

                this.Model.Payers = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles
                               .Where(manager => manager.Role.Name.Contains("财务"))
                               .Select(item => new { PayerID = item.Admin.ID, PayerName = item.Admin.RealName }).Json();
            }
        }

        protected void Save()
        {

            try
            {
                string RefundAmount = Request.Form["RefundAmount"];              
                string Remark = Request.Form["Remark"];
                string ReceiptID = Request.Form["ReceiptID"];
                string Files = Request.Form["Files"].Replace("&quot;", "'");

                var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.MyReceiptNotices.Where(x => x.ID == ReceiptID).FirstOrDefault();
                //收款金额
                var ReceiveAmount = notice.Amount;
                //已核销金额
                var ClearAmount = notice.ClearAmount;

                //已申请退款金额
                var RefundApplies = new Needs.Ccs.Services.Views.RefundApplyView().Where(x => x.FinanceReceiptID == ReceiptID&&x.ApplyStatus!=RefundApplyStatus.Rejected&&x.ApplyStatus!= RefundApplyStatus.Canceled).AsQueryable();
                decimal RefundedAmount = 0;
                if (RefundApplies.Count() > 0)
                {
                    RefundedAmount = RefundApplies.Select(t => t.Amount).Sum();
                }
                
                
                //收款金额>=已申请退款金额+已核销金额+此次申请金额
                if(ReceiveAmount< RefundedAmount+ ClearAmount + Convert.ToDecimal(RefundAmount))
                {
                    Response.Write((new { success = false, message = "退款金额不能超过 收款金额-已核销金额-已申请退款金额!" }).Json());
                    return;
                }

                Needs.Ccs.Services.Models.RefundApply apply = new Needs.Ccs.Services.Models.RefundApply();
                apply.ID = Needs.Overall.PKeySigner.Pick(PKeyType.RefundApply);               
                apply.ReceiptInfo = new Needs.Ccs.Services.Models.FinanceReceipt { ID = ReceiptID };
                apply.Client = notice.Client;
                apply.Amount = Convert.ToDecimal(RefundAmount);
                var currentAdmin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                apply.Applicant = currentAdmin;
                apply.Summary = Remark;

                var FileList = Files.JsonTo<List<dynamic>>();
                List<Needs.Ccs.Services.Models.CostApplyFile> costApplyFiles = new List<Needs.Ccs.Services.Models.CostApplyFile>();
                List<CenterFeeFile> centerFiles = new List<CenterFeeFile>();
                foreach (var item in FileList)
                {
                    costApplyFiles.Add(new Needs.Ccs.Services.Models.CostApplyFile
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        CostApplyID = apply.ID,
                        AdminID = currentAdmin.ID,
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
                        FileType = 1
                    });
                }

                apply.Enter();

                foreach (var costApplyFile in costApplyFiles)
                {
                    costApplyFile.Enter();
                }

                Logs log = new Logs();
                log.Name = "退款申请";
                log.MainID = apply.ID;
                log.AdminID = apply.Applicant.ID;
                log.Json = apply.Json();
                log.Summary = "跟单员【"+ currentAdmin.RealName+ "】申请退款";
                log.Enter();
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = ex.ToString() }).Json());
            }
            
        }

        /// <summary>
        /// 费用申请文件
        /// </summary>
        protected void CostApplyFiles()
        {
            string CostApplyID = Request.QueryString["ApplyID"];

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

        protected void Approve()
        {
            try
            {
                string ApplyID = Request.Form["ApplyID"];
                string PayerID = Request.Form["PayerID"];
                string Advice = Request.Form["Advice"];
                var apply = new Needs.Ccs.Services.Views.RefundApplyView().Where(t => t.ID == ApplyID).FirstOrDefault();
                apply.ApplyStatus = RefundApplyStatus.Approved;
                apply.Payer = new Admin { ID = PayerID };

                RefundApprove approve = new RefundApprove();
                approve.HandleRequest(apply);

                string CostApplyFinanceStaffName = System.Configuration.ConfigurationManager.AppSettings["CostApplyFinanceStaffName"];

                var currentAdmin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                Logs log = new Logs();
                log.Name = "退款申请";
                log.MainID = apply.ID;
                log.AdminID = apply.Applicant.ID;
                log.Json = apply.Json();
                log.Summary = "财务负责人【" + CostApplyFinanceStaffName + "】审批通过了退款申请 ";
                if (!string.IsNullOrEmpty(Advice))
                {
                    log.Summary += "意见：" + Advice;
                }
                log.Enter();
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch(Exception ex)
            {
                Response.Write((new { success = false, message = ex.ToString() }).Json());
            }
        }

        protected void Cancel()
        {
            try
            {
                string ApplyID = Request.Form["ApplyID"];
                string Advice = Request.Form["Advice"];
                var apply = new Needs.Ccs.Services.Views.RefundApplyView().Where(t => t.ID == ApplyID).FirstOrDefault();
                apply.ApplyStatus = RefundApplyStatus.Rejected;
                
                RefundDeny approve = new RefundDeny();
                approve.HandleRequest(apply);

                string CostApplyFinanceStaffName = System.Configuration.ConfigurationManager.AppSettings["CostApplyFinanceStaffName"];

                var currentAdmin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                Logs log = new Logs();
                log.Name = "退款申请";
                log.MainID = apply.ID;
                log.AdminID = apply.Applicant.ID;
                log.Json = apply.Json();
                log.Summary = "财务负责人【" + CostApplyFinanceStaffName + "】审批拒绝了退款申请";
                if (!string.IsNullOrEmpty(Advice))
                {
                    log.Summary += "意见：" + Advice;
                }
                log.Enter();
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch(Exception ex)
            {
                Response.Write((new { success = false, message = ex.ToString() }).Json());
            }
            
        }
    }
}