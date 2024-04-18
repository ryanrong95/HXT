using Needs.Ccs.Services.Enums;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Notice
{
    public partial class SearchListDetail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                load();
            }
        }

        private void load()
        {
            string DeclarationNoticeID = Request.QueryString["NoticeID"];
            var headinfo = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareSearchView.Where(item => item.DeclarationNoticeID == DeclarationNoticeID).AsQueryable();
            
            this.Model.DeclareDetail = headinfo.Json();


            var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DelareNoticeMaked[DeclarationNoticeID];
            string OrderID = notice.OrderID;

            var orderitemList = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderItems.Where(t => t.OrderID == OrderID).AsQueryable();

            this.Model.OrderDetail = orderitemList.Json();

      
        }


        /// <summary>
        /// 装箱单列表
        /// </summary>
        protected void data()
        {
            string NoticeID = Request.QueryString["NoticeID"];
            var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DelareNoticeMaked[NoticeID];
            string OrderID = notice.OrderID;

            var orderitemList = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderItems.Where(t => t.OrderID == OrderID).AsQueryable();

            Func<Needs.Ccs.Services.Models.OrderItem, object> convert = item => new
            {
                ProductName = item.Category.Name,
                Model = item.Model,
                Quantity = item.Quantity,
                TotalPrice = item.TotalPrice,
                Origin = item.Origin
            };

            Response.Write(new
            {
                rows = orderitemList.Select(convert).ToArray(),
            }.Json());
        }



        /// <summary>
        /// 初始化订单附件
        /// </summary>
        protected void dataFiles()
        {

            string NoticeID = Request.QueryString["NoticeID"];
            var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DelareNoticeMaked[NoticeID];
            if (!string.IsNullOrEmpty(notice.OrderID))
            {
                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[notice.OrderID];
                var files = order.MainOrderFiles.Where(file => file.FileType == FileType.OriginalInvoice);
                Func<Needs.Ccs.Services.Models.MainOrderFile, object> convert = orderFile => new
                {
                    orderFile.ID,
                    orderFile.Name,
                    FileType = orderFile.FileType.GetDescription(),
                    orderFile.FileFormat,
                    Url = FileDirectory.Current.FileServerUrl + "/" + orderFile.Url.ToUrl()
                };

                Response.Write(new
                {
                    rows = files.Select(convert).ToList(),
                    total = files.Count()
                }.Json());
            }
            else
            {
                Response.Write(new { }.Json());
            }
        }
    }
}