using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.DollarEquityApply
{
    public partial class Payment : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string DollarEquityApplyID = Request.QueryString["DollarEquityApplyID"];

            var dollarEquityApply = new Needs.Ccs.Services.Views.Origins.DollarEquityAppliesOrigin().Where(t => t.ID == DollarEquityApplyID).FirstOrDefault();

            this.Model.DollarEquityApply = new
            {
                DollarEquityApplyID = dollarEquityApply.ID,

                DollarEquityApplyCreateDate = dollarEquityApply.CreateDate.ToString("yyyy-MM-dd"),
                DollarEquityApplyExpectDate = dollarEquityApply.ExpectDate.ToString("yyyy-MM-dd"),
                DollarEquityApplyAmount = dollarEquityApply.Amount,
                DollarEquityApplyCurrency = dollarEquityApply.Currency,

                SupplierChnName = dollarEquityApply.SupplierChnName,
                BankName = dollarEquityApply.BankName,
                BankAddress = dollarEquityApply.BankAddress,
                BankAccount = dollarEquityApply.BankAccount,
                SwiftCode = dollarEquityApply.SwiftCode,

                //FileURL = dollarEquityApply.FileURL,
            }.Json();

            this.Model.PaymentType = Needs.Utils.Descriptions.EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.PaymentType>()
                .Select(item => new { Value = item.Key, Text = item.Value }).Json();
            this.Model.FinanceVaultData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault.Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal)
                .Select(item => new { Value = item.ID, Text = item.Name }).Json();

            //获取当前 AdminID 和姓名
            string CurrentAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
            string CurrentAdminName = "";
            var adminModel = new Needs.Ccs.Services.Views.AdminsTopView2().Where(t => t.OriginID == CurrentAdminID).FirstOrDefault();
            if (adminModel != null)
            {
                CurrentAdminName = adminModel.RealName;
            }

            //this.Model.CurrentAdminID = CurrentAdminID;
            this.Model.CurrentAdminName = CurrentAdminName;
        }

        protected void getAccounts()
        {
            string VaultID = Request.Form["VaultID"];
            string Currency = Request.Form["Currency"];
            string IsCash = Request.Form["IsCash"];

            var result = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal)
                .Where(t => t.FinanceVaultID == VaultID && t.Currency == Currency);

            if (!string.IsNullOrEmpty(IsCash) && IsCash == "true")
            {
                result = result.Where(t => t.IsCash == true);
            }
            else
            {
                result = result.Where(t => t.IsCash == false);
            }

            if (result != null)
            {
                Response.Write(result.Select(item => new { Value = item.ID, Text = item.AccountName }).Json());
            }
        }

        /// <summary>
        /// 上传付款凭证
        /// </summary>
        protected void UploadFile()
        {
            try
            {
                var file = Request.Files["uploadFile"];

                //处理附件
                if (file.ContentLength != 0)
                {
                    //文件保存
                    string fileName = file.FileName.ReName();

                    //创建文件目录
                    FileDirectory fileDic = new FileDirectory(fileName);
                    fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.PayExchange);
                    fileDic.CreateDataDirectory();
                    file.SaveAs(fileDic.FilePath);
                    var data = new
                    {
                        FileName = file.FileName,
                        FileFormat = file.ContentType,
                        WebUrl = fileDic.FileUrl,
                        Url = fileDic.VirtualPath,
                    };
                    Response.Write((new { success = true, data = data }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, data = ex.Message }).Json());
            }
        }

        /// <summary>
        /// 确认收款
        /// </summary>
        protected void Confirm()
        {
            try
            {
                string CurrentAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;

                var DollarEquityApplyID = Request.Form["DollarEquityApplyID"];
                var SeqNo = Request.Form["SeqNo"];
                var PayType = Request.Form["PayType"];
                var FinanceVaultID = Request.Form["FinanceVaultID"];
                var FinanceAccountID = Request.Form["FinanceAccountID"];

                string FileID = Request.Form["FileID"];
                string FileName = Request.Form["FileName"];
                string FileFormat = Request.Form["FileFormat"];
                string FileUrl = Request.Form["FileUrl"];

                SeqNo = SeqNo.Trim();
                int PayTypeInt = int.Parse(PayType);

                var dollarEquityApply = new Needs.Ccs.Services.Views.DollarEquityAppliesViewOrigin().Where(t => t.ID == DollarEquityApplyID).FirstOrDefault();
                dollarEquityApply.SeqNo = SeqNo;
                dollarEquityApply.PayType = (Needs.Ccs.Services.Enums.PaymentType)PayTypeInt;
                dollarEquityApply.FinanceVaultID = FinanceVaultID;
                dollarEquityApply.FinanceAccountID = FinanceAccountID;
                dollarEquityApply.PayerID = CurrentAdminID;

                //查询金额是否足够
                var icgooBalance = new Needs.Ccs.Services.Models.IcgooBalance();
                icgooBalance.ClientID = dollarEquityApply.ClientID;
                icgooBalance.Currency = dollarEquityApply.Currency;
                decimal balance = icgooBalance.ReadBalance();
                if (balance < dollarEquityApply.Amount)
                {
                    Response.Write((new { success = false, message = "保存失敗：剩余金额不足！" }).Json());
                    return;
                }

                var dollarEHandle = new Needs.Ccs.Services.Models.DollarEHandle();
                dollarEHandle.Apply = dollarEquityApply;
                bool handleResult = dollarEHandle.Handle();
                if (handleResult)
                {
                    //保存付款凭证文件
                    Needs.Ccs.Services.Models.DollarEquityApplyFile NoticeFile = new Needs.Ccs.Services.Models.DollarEquityApplyFile()
                    {
                        ID = FileID,
                        DollarEquityApplyID = DollarEquityApplyID,
                        FileName = FileName,
                        FileFormat = FileFormat,
                        Url = FileUrl,
                        FileType = Needs.Ccs.Services.Enums.FileType.PaymentVoucher,
                        AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                        CreateDate = DateTime.Now,
                        Status = Needs.Ccs.Services.Enums.Status.Normal,
                    };
                    NoticeFile.Enter();
                }

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失敗：" + ex.Message }).Json());
            }
        }


    }
}