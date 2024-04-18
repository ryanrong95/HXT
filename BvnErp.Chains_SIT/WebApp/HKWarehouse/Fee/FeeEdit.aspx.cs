using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using WebApp.App_Utils;
using Needs.Ccs.Services;
using Needs.Utils.Descriptions;

namespace WebApp.HKWarehouse.Fee
{
    public partial class FeeEdit : Uc.PageBase
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
            //付款方式
            this.Model.PaymentType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.WhsePaymentType>()
                .Select(item => new { Value = item.Key, Text = item.Value }).Json();
            //库房费用类型
            this.Model.WarehousePremiumType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.WarehousePremiumType>()
                .Select(item => new { Value = item.Key, Text = item.Value }).Json();

            this.Model.FeeData = "".Json();
            string FeeID = Request.QueryString["FeeID"];
            var Fee = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OrderWhesPremium.Where(t => t.ID == FeeID).FirstOrDefault();

            if (Fee != null)
            {
                this.Model.FeeData = new
                {
                    PaymentType = Fee.WhsePaymentType,
                    Currency = Fee.Currency,
                    FeeType = Fee.WarehousePremiumType,
                    Count = Fee.Count,
                    UnitPrice = Fee.UnitPrice,
                    UnitName = Fee.UnitName,
                    Summary = Fee.Summary,
                }.Json();
            }
        }

        /// <summary>
        /// 费用附件
        /// </summary>
        protected void filedata()
        {
            string FeeID = Request.QueryString["FeeID"];
            var files = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OrderWhesPremiumFile.Where(t => t.OrderWhesPremiumID == FeeID);

            Func<OrderWhesPremiumFile, object> convert = item => new
            {
                ID = item.ID,
                Name = item.Name,
                FileFormat = item.FileFormat,
                URL = item.URL,
                WebUrl = FileDirectory.Current.FileServerUrl + "/" + item.URL.ToUrl(),//查看路径
                CreateDate = item.CreateDate.ToString(),
            };
            Response.Write(new
            {
                rows = files.Select(convert).ToArray(),
                total = files.Count()
            }.Json());
        }

        /// <summary>
        /// 保存费用
        /// </summary>
        protected void Save()
        {
            try
            {
                string FeeID = Request.Form["FeeID"];
                string PaymentType = Request.Form["PaymentType"];
                string Currency = Request.Form["Currency"];
                string FeeType = Request.Form["FeeType"];
                string Count = Request.Form["Count"];
                string UnitPrice = Request.Form["UnitPrice"];
                string UnitName = Request.Form["UnitName"];
                string Summary = Request.Form["Summary"];

                string fileData = Request.Form["FileData"].Replace("&quot;", "'");
                IEnumerable<OrderWhesPremiumFile> files = fileData.JsonTo<IEnumerable<OrderWhesPremiumFile>>();
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                ////保存费用
                var fee = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OrderWhesPremium.Where(t => t.ID == FeeID).FirstOrDefault();
                if (fee == null)
                {
                    Response.Write((new { success = false, message = "保存失败：该费用不存在" }).Json());
                    return;
                }
                fee.WarehousePremiumType = (Needs.Ccs.Services.Enums.WarehousePremiumType)int.Parse(FeeType);
                fee.WhsePaymentType = (Needs.Ccs.Services.Enums.WhsePaymentType)int.Parse(PaymentType);
                //是否付款
                if (fee.WhsePaymentType == Needs.Ccs.Services.Enums.WhsePaymentType.Cash)
                {
                    fee.WarehousePremiumsStatus = Needs.Ccs.Services.Enums.WarehousePremiumsStatus.Payed;
                }
                fee.Currency = Currency;
                //使用实时汇率
                var customRate = Needs.Wl.Admin.Plat.AdminPlat.RealTimeRates.Where(item => item.Code == Currency).SingleOrDefault();
                fee.ExchangeRate = customRate.Rate;
                fee.Count = int.Parse(Count);
                fee.UnitPrice = decimal.Parse(UnitPrice);
                fee.ApprovalPrice = int.Parse(Count) * decimal.Parse(UnitPrice);
                fee.UnitName = UnitName;
                fee.Summary = Summary;
                fee.Files = new OrderWhesPremiumFiles(files);
                fee.SetOperator(admin);
                fee.Enter();

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 上传费用附件
        /// </summary>
        protected void UploadFile()
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
                            fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Warehouse);
                            fileDic.CreateDataDirectory();
                            file.SaveAs(fileDic.FilePath);
                            fileList.Add(new
                            {
                                Name = file.FileName,
                                FileFormat = file.ContentType,
                                WebUrl = fileDic.FileUrl,
                                Url = fileDic.VirtualPath,
                                CreateDate = DateTime.Now.ToString()
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
    }
}