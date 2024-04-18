using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Notice
{
    public partial class SearchList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
        }

        protected void LoadComboBoxData()
        {
            var noticeStatus = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.DeclareNoticeStatus>().Select(item => new { item.Key, item.Value });
            this.Model.NoticeStatus = noticeStatus.Json();
            //this.Model.VoyageType = EnumUtils.ToDictionary<VoyageType>().Select(item => new { TypeValue = item.Key, TypeText = item.Value }).Json();

            Dictionary<string, string> dicVoyageType = new Dictionary<string, string>();
            dicVoyageType.Add(VoyageType.Normal.GetHashCode().ToString(), VoyageType.Normal.GetDescription());
            dicVoyageType.Add(VoyageType.CharterBus.GetHashCode().ToString(), VoyageType.CharterBus.GetDescription());
            this.Model.VoyageType = dicVoyageType.Select(item => new { TypeValue = item.Key, TypeText = item.Value }).Json();

            this.Model.DecHeadSpecialType = EnumUtils.ToDictionary<DecHeadSpecialTypeEnum>().Select(item => new { TypeValue = item.Key, TypeText = item.Value }).Json();
        }

        /*
        /// <summary>
        /// 初始化订单数据
        /// </summary>
        protected void data()
        {
            string NoticeID = Request.QueryString["NoticeID"];
            string OrderID = Request.QueryString["OrderID"];
            string ClientName = Request.QueryString["ClientName"];
            string NoticeStatus = Request.QueryString["NoticeStatus"];
            string VoyageID = Request.QueryString["VoyageID"];
            string VoyageType = Request.QueryString["VoyageType"];
            var decHeadSpecialTypeForm = Request.QueryString["DecHeadSpecialType"] != null ? Request.QueryString["DecHeadSpecialType"].Replace("&quot;", "\'").Replace("amp;", "") : null;

            var DeclarationNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DelareNoticeMaked.AsQueryable();

            if (!string.IsNullOrEmpty(OrderID))
            {
                OrderID = OrderID.Trim();
                DeclarationNotice = DeclarationNotice.Where(t => t.order.ID == OrderID);
            }
            if (!string.IsNullOrEmpty(NoticeID))
            {
                NoticeID = NoticeID.Trim();
                DeclarationNotice = DeclarationNotice.Where(t => t.ID == NoticeID);
            }
            if (!string.IsNullOrEmpty(ClientName))
            {
                ClientName = ClientName.Trim();
                DeclarationNotice = DeclarationNotice.Where(t => t.order.Client.Company.Name.Contains(ClientName));
            }
            if (!string.IsNullOrEmpty(NoticeStatus))
            {
                int noticeStatus = Int32.Parse(NoticeStatus.Trim());
                DeclarationNotice = DeclarationNotice.Where(t => t.Status == (Needs.Ccs.Services.Enums.DeclareNoticeStatus)noticeStatus);
            }
            if (!string.IsNullOrEmpty(VoyageID))
            {
                VoyageID = VoyageID.Trim();
                DeclarationNotice = DeclarationNotice.Where(t => t.VoyageID.Contains(VoyageID));
            }
            if (!string.IsNullOrEmpty(VoyageType) && VoyageType != "0" && VoyageType != "全部")
            {
                int intVoyageType = 0;
                if (int.TryParse(VoyageType, out intVoyageType))
                {
                    DeclarationNotice = DeclarationNotice.Where(t => t.VoyageType == (Needs.Ccs.Services.Enums.VoyageType)intVoyageType);
                }
            }
            if (decHeadSpecialTypeForm != null)
            {
                var decHeadSpecialTypeModel = decHeadSpecialTypeForm.JsonTo<dynamic>();
                foreach (var decHeadSpecialType in decHeadSpecialTypeModel)
                {
                    string typeValue = decHeadSpecialType.DecHeadSpecialTypeValue;
                    Needs.Ccs.Services.Enums.DecHeadSpecialTypeEnum enumType = (Needs.Ccs.Services.Enums.DecHeadSpecialTypeEnum)int.Parse(typeValue);
                    switch (enumType)
                    {
                        case DecHeadSpecialTypeEnum.CharterBus:
                            DeclarationNotice = DeclarationNotice.Where(t => t.IsCharterBus == true);
                            break;
                        case DecHeadSpecialTypeEnum.HighValue:
                            DeclarationNotice = DeclarationNotice.Where(t => t.IsHighValue == true);
                            break;
                        case DecHeadSpecialTypeEnum.Inspection:
                            DeclarationNotice = DeclarationNotice.Where(t => t.IsInspection == true);
                            break;
                        case DecHeadSpecialTypeEnum.Quarantine:
                            DeclarationNotice = DeclarationNotice.Where(t => t.IsQuarantine == true);
                            break;
                        default:
                            break;
                    }
                }
            }

            DeclarationNotice = DeclarationNotice.OrderByDescending(t => t.CreateDate);

            Func<Needs.Ccs.Services.Models.DeclarationNotice, object> convert = declareNotice => new
            {
                NoticeID = declareNotice.ID,
                OrderID = declareNotice.order.ID,
                ClientName = declareNotice.order.Client.Company.Name,
                SupplierName = declareNotice.order.OrderConsignee.ClientSupplier.ChineseName,
                Currency = declareNotice.order.Currency,
                Status = declareNotice.Status.GetDescription(),
                CreateDate = declareNotice.CreateDate.ToShortDateString(),
                //InspQuarName = declareNotice.InspQuarName,
                DeclarantName = declareNotice.order.Client.Merchandiser.RealName,
                ContrNo = declareNotice.DecHead.ContrNo,
                PackNo = declareNotice.DecHead.PackNo,
                TotalDeclarePrice = declareNotice.order.Items.Sum(t => t.TotalPrice),
                TotalQty = declareNotice.DecHead.Lists.Sum(t => t.GQty),
                TotalListQty = declareNotice.DecHead.Lists.Count(),
                IsCharterBus = declareNotice.IsCharterBus,
                IsHighValue = declareNotice.IsHighValue,
                IsInspection = declareNotice.IsInspection,
                IsQuarantine = declareNotice.IsQuarantine,
                VoyageID = declareNotice.VoyageID,
                VoyageType = declareNotice.VoyageType.GetDescription(),
            };
            this.Paging(DeclarationNotice, convert);
        }
        */

        /// <summary>
        /// 初始化已制单的报关通知数据
        /// </summary>
        protected void data()
        {
            string NoticeID = Request.QueryString["NoticeID"];
            string OrderID = Request.QueryString["OrderID"];
            string ClientName = Request.QueryString["ClientName"];
            string NoticeStatus = Request.QueryString["NoticeStatus"];
            string VoyageID = Request.QueryString["VoyageID"];
            string VoyageType = Request.QueryString["VoyageType"];
            var decHeadSpecialTypeForm = Request.QueryString["DecHeadSpecialType"] != null ? Request.QueryString["DecHeadSpecialType"].Replace("&quot;", "\'").Replace("amp;", "") : null;
            string contrNo = Request.QueryString["ContrNo"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            var noticesView = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DelareNoticeMaked;
            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.DeclarationNotice, bool>> expression = item => true;

            #region 页面查询条件
            if (!string.IsNullOrWhiteSpace(OrderID))
            {
                Expression<Func<Needs.Ccs.Services.Models.DeclarationNotice, bool>> lambda1 = item => item.OrderID == OrderID;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrWhiteSpace(NoticeID))
            {
                Expression<Func<Needs.Ccs.Services.Models.DeclarationNotice, bool>> lambda1 = item => item.ID == NoticeID;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(ClientName))
            {
                var orderIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders.Where(item => item.Client.Company.Name.Contains(ClientName)).Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.DeclarationNotice, bool>> lambda1 = item => orderIds.Contains(item.OrderID);
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(NoticeStatus))
            {
                int noticeStatus = Int32.Parse(NoticeStatus.Trim());
                Expression<Func<Needs.Ccs.Services.Models.DeclarationNotice, bool>> lambda1 = item => (int)item.Status == noticeStatus;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(VoyageID))
            {
                Expression<Func<Needs.Ccs.Services.Models.DeclarationNotice, bool>> lambda1 = item => item.VoyageID == VoyageID;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(VoyageType) && VoyageType != "0" && VoyageType != "全部")
            {
                int intVoyageType = 0;
                if (int.TryParse(VoyageType, out intVoyageType))
                {
                    var voyageIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.ManifestSure.Where(v => (int)v.Type == intVoyageType).Select(v => v.ID).ToArray();
                    Expression<Func<Needs.Ccs.Services.Models.DeclarationNotice, bool>> lambda1 = item => voyageIds.Contains(item.VoyageID);
                    lamdas.Add(lambda1);
                }
            }
            if (decHeadSpecialTypeForm != null)
            {
                var decHeadSpecialTypeModel = decHeadSpecialTypeForm.JsonTo<dynamic>();
                foreach (var decHeadSpecialType in decHeadSpecialTypeModel)
                {
                    string typeValue = decHeadSpecialType.DecHeadSpecialTypeValue;
                    DecHeadSpecialTypeEnum specialType = (DecHeadSpecialTypeEnum)int.Parse(typeValue);
                    var decHeadIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DecHeadSpecialTypes.Where(st => st.Type == specialType).Select(st => st.DecHeadID).ToArray();
                    Expression<Func<Needs.Ccs.Services.Models.DeclarationNotice, bool>> lambda1 = item => decHeadIds.Contains(item.DecHeadID);
                    lamdas.Add(lambda1);
                }
            }
            if (!string.IsNullOrWhiteSpace(contrNo))
            {
                Expression<Func<Needs.Ccs.Services.Models.DeclarationNotice, bool>> lambda1 = item => item.DecHead.ContrNo == contrNo.Trim();
                lamdas.Add(lambda1);
            }

            if (!string.IsNullOrWhiteSpace(StartDate))
            {
                StartDate = StartDate.Trim();

                DateTime dt;
                if (DateTime.TryParse(StartDate, out dt))
                {
                    Expression<Func<Needs.Ccs.Services.Models.DeclarationNotice, bool>> lambda1 = item => item.CreateDate >=dt;
                    lamdas.Add(lambda1);
                }
            }
            if (!string.IsNullOrWhiteSpace(EndDate))
            {
                EndDate = EndDate.Trim();

                DateTime dt;
                if (DateTime.TryParse(EndDate, out dt))
                {
                    var to = dt.AddDays(1);
                    Expression<Func<Needs.Ccs.Services.Models.DeclarationNotice, bool>> lambda1 = item => item.CreateDate <= to;
                    lamdas.Add(lambda1);
                }
            }
            #endregion

            #region 页面需要数据
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var notices = noticesView.GetPageList(page, rows, expression, lamdas.ToArray());

            Response.Write(new
            {
                rows = notices.Select(
                        notice => new
                        {
                            NoticeID = notice.ID,
                            OrderID = notice.OrderID,
                            ClientName = notice.order.Client.Company.Name,
                            SupplierName = notice.order.OrderConsignee.ClientSupplier.ChineseName,
                            Currency = notice.order.Currency,
                            Status = notice.Status.GetDescription(),
                            CreateDate = notice.CreateDate.ToString("yyyy-MM-dd"),
                            DeclarantName = notice.order.Client.Merchandiser.RealName,
                            ContrNo = notice.DecHead.ContrNo,
                            PackNo = notice.DecHead.PackNo,
                            TotalDeclarePrice = notice.order.DeclarePrice * Needs.Ccs.Services.ConstConfig.TransPremiumInsurance,
                            TotalQty = notice.TotalQty,
                            TotalListQty = notice.TotalModelQty,
                            IsCharterBus = notice.IsCharterBus,
                            IsHighValue = notice.IsHighValue,
                            IsInspection = notice.IsInspection,
                            IsQuarantine = notice.IsQuarantine,
                            IsCCC = notice.IsCCC,
                            VoyageID = notice.VoyageID,
                            VoyageType = notice.VoyageType.GetDescription(),
                        }
                     ).ToArray(),
                total = notices.Total,
            }.Json());
            #endregion
        }
    }
}