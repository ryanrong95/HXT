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
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;

namespace WebApp.HKWarehouse.Fee.UnApproved
{
    public partial class Approval : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PageInit();
        }

        private void PageInit()
        {
            this.Model.OrderWhesPremiumData = "".Json();

            string FeeID = Request.QueryString["FeeID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OrderWhesPremium[FeeID];
            if (order != null)
            {
                this.Model.OrderWhesPremiumData = new
                {
                    ID = order.ID,
                    OrderID = order.OrderID,
                    ClientName = order.Client.Company.Name,
                    Type = order.WarehousePremiumType.GetDescription(),
                    Creater = order.Creater.RealName,
                    Price = order.UnitPrice * order.Count,
                    CreateDate = order.CreateDate.ToString(),
                    ExchangeRate = order.ExchangeRate,
                    ApprovalPrice = (order.UnitPrice * order.Count) * order.ExchangeRate,
                    Summary = order.Summary,
                }.Json();
            }
        }

        /// <summary>
        /// 费用附件
        /// </summary>
        protected void filedata()
        {
            string FeeID = Request.QueryString["FeeID"];

            List<object> allFiles = new List<object>();

            //芯达通库中的文件
            var files = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OrderWhesPremiumFile.Where(t => t.OrderWhesPremiumID == FeeID);

            Func<OrderWhesPremiumFile, object> convert = item => new
            {
                ID = item.ID,
                Name = item.Name,
                FileFormat = item.FileFormat,
                URL = item.URL,
                Summary = item.Summary,
                CreateDate = item.CreateDate.ToString(),
                WebUrl = FileDirectory.Current.FileServerUrl + "/" + item.URL.ToUrl(),//查看路径
            };

            allFiles.AddRange(files.Select(convert));

            //中心库中的文件
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Views.FilesDescriptionTopViewModel, bool>> lambda = item => item.PayID == FeeID;
            lambdas.Add(lambda);
            var centerFiles = new Needs.Ccs.Services.Views.FilesDescriptionTopView().GetResults(lambdas.ToArray());

            Func<Needs.Ccs.Services.Views.FilesDescriptionTopViewModel, object> convert2 = item => new
            {
                ID = item.ID,
                Name = item.CustomName,
                FileFormat = "",
                URL = item.Url,
                WebUrl = FileDirectory.Current.PvDataFileUrl + "/" + item.Url.ToUrl(),//查看路径
                CreateDate = item.CreateDate.ToString(),
            };

            allFiles.AddRange(centerFiles.Select(convert2));


            Response.Write(new
            {
                rows = allFiles.ToArray(),  //files.Select(convert).ToArray(),
                total = allFiles.Count()  //files.Count()
            }.Json());
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

        /// <summary>
        /// 审批费用
        /// </summary>
        protected void ApprovalFee()
        {
            try
            {
                string ID = Request.Form["ID"];
                string ApprovalPrice = Request.Form["ApprovalPrice"];
                string Summary = Request.Form["Summary"];

                string fileData = Request.Form["FileData"].Replace("&quot;", "'");
                IEnumerable<OrderWhesPremiumFile> files = fileData.JsonTo<IEnumerable<OrderWhesPremiumFile>>();
                
                //查询费用
                var Fees = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OrderWhesPremium.AsQueryable();
                var fee = Fees.Where(t => t.ID == ID).FirstOrDefault();
                if (fee.WarehousePremiumsStatus == WarehousePremiumsStatus.Payed)
                {
                    Response.Write((new { success = false, message = "审批失败：已经付款" }).Json());
                    return;
                }

                fee.ApprovalPrice = decimal.Parse(ApprovalPrice);
                fee.Summary = Summary;
                fee.Files = new OrderWhesPremiumFiles(files);
                fee.SetOperator(Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID));
                //审批费用
                fee.Approval();
                Response.Write((new { success = true, message = "审批通过" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "审批失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 取消费用
        /// </summary>
        protected void CancelFee()
        {
            try
            {
                string ID = Request.Form["ID"];
                var Fees = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OrderWhesPremium.AsQueryable();
                var fee = Fees.Where(t => t.ID == ID).FirstOrDefault();
                if (fee.WarehousePremiumsStatus == WarehousePremiumsStatus.Payed)
                {
                    Response.Write((new { success = false, message = "取消失败：已经付款" }).Json());
                    return;
                }
                fee.SetOperator(Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID));
                fee.Cancel();
                Response.Write((new { success = true, message = "取消成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "取消失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        protected void LoadLogs()
        {
            string FeeID = Request.Form["FeeID"];
            var logs = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OrderWhesPremiumLogs.Where(t => t.OrderWhesPremiumID == FeeID);
            logs = logs.OrderByDescending(t => t.CreateDate);
            Func<OrderWhesPremiumLog, object> convert = item => new
            {
                ID = item.ID,
                Summary = item.Summary,
                Type = item.Type.GetDescription(),
                Operator = item.Admin.RealName,
                CreateDate = item.CreateDate.ToString(),
            };
            Response.Write(new
            {
                rows = logs.Select(convert).ToArray()
            }.Json());
        }
    }
}