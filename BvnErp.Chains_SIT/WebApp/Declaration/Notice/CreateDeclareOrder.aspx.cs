
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Notice
{
    public partial class CreateDeclareOrder : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        public void LoadData()
        {
            string ID = Request.QueryString["ID"];
            //var DeclarationNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DelareNotice.Where(item => item.ID == ID).AsQueryable().FirstOrDefault();
            var DeclarationNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DelareNotice[ID];

            if (DeclarationNotice != null)
            {
                this.Model.DeclareNotice = new
                {
                    NoticeID = ID,
                    OrderID = DeclarationNotice.order.ID,
                    ClientName = DeclarationNotice.order.Client.Company.Name,
                    ClientID = DeclarationNotice.order.Client.Company.ID,
                    AdminName = DeclarationNotice.order.Client.Merchandiser.RealName,
                }.Json();
            }
            else
            {
                this.Model.DeclareNotice = new
                {
                    NoticeID = ID,
                    OrderID = "",
                    ClientName = "",
                    ClientID = "",
                    AdminName = "",
                }.Json();
            }

        }

        protected void data()
        {
            string ID = Request.QueryString["ID"];
            string Currency = Request.QueryString["Currency"];
            var DeclarationNoticeItem = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareNotictItem.
                                       Where(item => item.DeclarationNoticeID == ID).AsQueryable();
            //var Packs = DeclarationNoticeItem.Select(item => item.Packing).Distinct();

            List<DeclarationNoticeItemView> noticeItems = new List<DeclarationNoticeItemView>();

            foreach (var data in DeclarationNoticeItem)
            {
                var Admin = Needs.Underly.FkoFactory<Admin>.Create(data.Sorting.AdminID);

                DeclarationNoticeItemView temp = new DeclarationNoticeItemView();              
                temp.SortingID = data.Sorting.ID;
                temp.CaseNumber = data.Sorting.BoxIndex;
                temp.Batch = data.Sorting.OrderItem.Batch;
                temp.Name = data.Sorting.OrderItem.Name;
                temp.Manufacturer = data.Sorting.OrderItem.Manufacturer;
                temp.Model = data.Sorting.OrderItem.Model;
                temp.Origin = data.Sorting.OrderItem.Origin;
                temp.Quantity = data.Sorting.Quantity;
                temp.GrossWeight = data.Sorting.GrossWeight;
                temp.AdminName = Admin.RealName;
                temp.PackingDate = data.Sorting.CreateDate;
                temp.Status = data.Status;
                temp.Currency = Currency;
                OrderItem orderitem = data.Sorting.OrderItem;
                if (orderitem != null)
                {
                    temp.Unit = orderitem.Unit;
                    temp.UnitPrice = orderitem.UnitPrice;
                    temp.TotalPrice = orderitem.TotalPrice;

                }
                noticeItems.Add(temp);
            }

            Func<DeclarationNoticeItemView, object> convert = declareNoticeItem => new
            {
                CaseNumber = declareNoticeItem.CaseNumber,
                Batch = declareNoticeItem.Batch,
                Name = declareNoticeItem.Name,
                Manufacturer = declareNoticeItem.Manufacturer,
                Model = declareNoticeItem.Model,
                Origin = declareNoticeItem.Origin,
                Quantity = declareNoticeItem.Quantity,
                Unit = declareNoticeItem.Unit,
                UnitPrice = declareNoticeItem.UnitPrice,
                TotalPrice = declareNoticeItem.TotalPrice,
                Currency = declareNoticeItem.Currency,
                GrossWeight = declareNoticeItem.GrossWeight,
                AdminName = declareNoticeItem.AdminName,
                PackingDate = declareNoticeItem.PackingDate.ToShortDateString(),
                Status = declareNoticeItem.Status.GetDescription(),              
                SortingID = declareNoticeItem.SortingID
            };

            Response.Write(new
            {
                rows = noticeItems.Select(convert).ToArray(),
                total = noticeItems.Count()
            }.Json());
        }

        protected void AllDeclare()
        {
            string ids = Request.Form["ID"];
            var casenolist = ids.Split(',').ToList().Distinct();

        }
    }
}