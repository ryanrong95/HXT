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

namespace WebApp.HKWarehouse.Fee.Approved
{
    public partial class Detail : Uc.PageBase
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
                    ID = Fee.ID,
                    OrderID = Fee.OrderID,
                    ClientName = Fee.Client.Company.Name,
                    Type = Fee.WarehousePremiumType.GetDescription(),
                    Price = (Fee.UnitPrice * Fee.Count),
                    Currency = Fee.Currency,
                    ApprovalPrice = (Fee.UnitPrice * Fee.Count) * Fee.ExchangeRate, 
                    ExchangeRate = Fee.ExchangeRate,
                    WarehousePremiumsStatus = Fee.WarehousePremiumsStatus.GetDescription(),
                    Approver = Fee.Approver == null ? "" : Fee.Approver.RealName,
                    Creater = Fee.Creater.RealName,
                    CreateDate = Fee.CreateDate.ToString("yyyy-MM-dd"),
                    Summary=Fee.Summary,
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

            //华芯通库中的文件
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
                total = allFiles.Count(),  //files.Count()
            }.Json());
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