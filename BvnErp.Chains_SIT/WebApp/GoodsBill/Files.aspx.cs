using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.GoodsBill
{
    public partial class Files : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            string ID = Request.QueryString["ID"];
            string OrderItemID = Request.QueryString["OrderItemID"];
            string WaybillID = Request.QueryString["WaybillID"];
            string InvoiceNoticeID = Request.QueryString["InvoiceNoticeID"];

            List<Needs.Ccs.Services.Models.EdocRealation> files = new List<Needs.Ccs.Services.Models.EdocRealation>();

            if (!string.IsNullOrEmpty(ID))
            {
                //合同相关
                var edoc = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.EdocRealations.Where(t => t.DeclarationID == ID);

                string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];
                foreach (var item in edoc)
                {
                    Needs.Ccs.Services.Models.EdocRealation efile = new Needs.Ccs.Services.Models.EdocRealation();
                    efile.ID = item.ID;
                    efile.Edoc = item.Edoc;
                    efile.EdocCopId = item.EdocCopId;
                    efile.FileUrl = FileServerUrl + @"/" + item.FileUrl.ToUrl();
                    files.Add(efile);
                }
            }

            if (!string.IsNullOrEmpty(OrderItemID))
            {
                var InputInfo = new Needs.Ccs.Services.Views.Sz_Cfb_InViewOrigin().Where(t => t.OrderItemID == OrderItemID).FirstOrDefault();

                //入库相关文件
                List<LambdaExpression> lambdas = new List<LambdaExpression>();
                Expression<Func<Needs.Ccs.Services.Views.FilesDescriptionTopViewModel, bool>> lambda = item => item.ShipID == InputInfo.LotNumber;
                lambdas.Add(lambda);
                var centerFiles = new Needs.Ccs.Services.Views.FilesDescriptionTopView().GetResults(lambdas.ToArray());

                string CenterServerUrl = FileDirectory.Current.PvDataFileUrl;
                foreach (var item in centerFiles)
                {
                    Needs.Ccs.Services.Models.EdocRealation efile = new Needs.Ccs.Services.Models.EdocRealation();
                    efile.ID = item.ID;
                    Needs.Ccs.Services.Models.BaseEdocCode edocCode = new Needs.Ccs.Services.Models.BaseEdocCode();
                    edocCode.Name = "库房文件";
                    efile.Edoc = edocCode;
                    efile.EdocCopId = item.CustomName;
                    efile.FileUrl = CenterServerUrl + @"/" + item.Url.ToUrl();
                    files.Add(efile);
                }
            }

            if (!string.IsNullOrEmpty(WaybillID))
            {
                //出库相关文件
                List<LambdaExpression> lambdas = new List<LambdaExpression>();
                Expression<Func<Needs.Ccs.Services.Views.FilesDescriptionTopViewModel, bool>> lambda = item => item.WaybillID == WaybillID;
                lambdas.Add(lambda);
                var centerFiles = new Needs.Ccs.Services.Views.FilesDescriptionTopView().GetResults(lambdas.ToArray());

                string CenterServerUrl = FileDirectory.Current.PvDataFileUrl;
                foreach (var item in centerFiles)
                {
                    Needs.Ccs.Services.Models.EdocRealation efile = new Needs.Ccs.Services.Models.EdocRealation();
                    efile.ID = item.ID;
                    Needs.Ccs.Services.Models.BaseEdocCode edocCode = new Needs.Ccs.Services.Models.BaseEdocCode();
                    edocCode.Name = "库房文件";
                    efile.Edoc = edocCode;
                    efile.EdocCopId = item.CustomName;
                    efile.FileUrl = CenterServerUrl + @"/" + item.Url.ToUrl();
                    files.Add(efile);
                }
            }


            if (!string.IsNullOrEmpty(InvoiceNoticeID))
            {
                var invoiceWaybill = new Needs.Ccs.Services.Views.InvoiceWaybillOriginView().Where(t => t.ID == InvoiceNoticeID).FirstOrDefault();
                if (invoiceWaybill != null)
                {
                    Needs.Ccs.Services.Models.EdocRealation efile = new Needs.Ccs.Services.Models.EdocRealation();
                    efile.ID = "";
                    Needs.Ccs.Services.Models.BaseEdocCode edocCode = new Needs.Ccs.Services.Models.BaseEdocCode();
                    edocCode.Name = "发票运单号";
                    efile.Edoc = edocCode;
                    efile.EdocCopId = invoiceWaybill.WaybillCode;
                    efile.FileUrl = "";
                    files.Add(efile);
                }
            }


            Func<Needs.Ccs.Services.Models.EdocRealation, object> convert = item => new
            {
                ID = item.ID,
                EdocName = item.Edoc.Name,
                EdocCopId = item.EdocCopId,
                FileUrl = item.FileUrl,
            };

            Response.Write(new
            {
                rows = files.Select(convert).ToArray(),
                total = files.Count()
            }.Json());

        }
    }
}