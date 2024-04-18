using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Needs.Wl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Notice
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
        }

        protected void LoadComboBoxData()
        {
            var noticeStatus = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.DeclareNoticeStatus>().Select(item => new { item.Key, item.Value });
            this.Model.NoticeStatus = noticeStatus.Json();
            this.Model.OrderSpecialType = EnumUtils.ToDictionary<OrderSpecialType>().Select(item => new { TypeValue = item.Key, TypeText = item.Value }).Json();
            //this.Model.VoyageType = EnumUtils.ToDictionary<VoyageType>().Select(item => new { TypeValue = item.Key, TypeText = item.Value }).Json();

            Dictionary<string, string> dicVoyageType = new Dictionary<string, string>();
            dicVoyageType.Add(VoyageType.Normal.GetHashCode().ToString(), VoyageType.Normal.GetDescription());
            dicVoyageType.Add(VoyageType.CharterBus.GetHashCode().ToString(), VoyageType.CharterBus.GetDescription());
            this.Model.VoyageType = dicVoyageType.Select(item => new { TypeValue = item.Key, TypeText = item.Value }).Json();

            var thisAdminOriginID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;

            string isChecker = "false";
            var theDeclarantCandidate = new Needs.Ccs.Services.Views.Origins.DeclarantCandidatesOrigin()
                                            .Where(t => t.Status == Status.Normal 
                                                && t.AdminID == thisAdminOriginID
                                                && t.Type == DeclarantCandidateType.Checker).FirstOrDefault();
            if (theDeclarantCandidate != null)
            {
                isChecker = "true";
            }

            this.Model.ThisAdminOriginID = thisAdminOriginID;
            this.Model.IsChecker = isChecker;
        }

        /// <summary>
        /// 初始化订单数据
        /// </summary>
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);
            string NoticeID = Request.QueryString["NoticeID"];
            string OrderID = Request.QueryString["OrderID"]?.Trim();
            string ClientName = Request.QueryString["ClientName"];
            string NoticeStatus = Request.QueryString["NoticeStatus"];
            string VoyageID = Request.QueryString["VoyageID"];
            string VoyageType = Request.QueryString["VoyageType"];
            var orderSpecialTypeForm = Request.QueryString["OrderSpecialType"] != null ? Request.QueryString["OrderSpecialType"].Replace("&quot;", "\'").Replace("amp;", "") : null;
            string MyDecNotice = Request.QueryString["MyDecNotice"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            using (var query = new Needs.Ccs.Services.Views.DeclarationNotice.DecNoticeListViewOpmz())
            {
                var view = query;

                if (!string.IsNullOrWhiteSpace(OrderID))
                {
                    OrderID = OrderID.Trim();
                    view = view.SearchByOrderID(OrderID);
                }

                if (!string.IsNullOrEmpty(ClientName))
                {
                    ClientName = ClientName.Trim();
                    view = view.SearchByClientName(ClientName);
                }


                if (!string.IsNullOrEmpty(VoyageID))
                {
                    VoyageID = VoyageID.Trim();
                    view = view.SearchByVoyageID(VoyageID);
                }

                if (!string.IsNullOrEmpty(VoyageType) && VoyageType != "0" && VoyageType != "全部")
                {
                    int intVoyageType = 0;
                    if (int.TryParse(VoyageType, out intVoyageType))
                    {
                        view = view.SearchByVoyageType((Needs.Ccs.Services.Enums.VoyageType)intVoyageType);
                    }
                }

                if (orderSpecialTypeForm != null)
                {
                    var specials = orderSpecialTypeForm.JsonTo<dynamic[]>().Select(item => (OrderSpecialType)(item.OrderSpecialTypeValue)).ToArray();
                    if (specials != null && specials.Any())
                    {
                        view = view.SearchBySpecialType(specials);
                    }                    
                }

                bool MyDecNoticeBoolean = false;
                if (bool.TryParse(MyDecNotice, out MyDecNoticeBoolean))
                {
                    if (MyDecNoticeBoolean)
                    {
                        var adminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                        view = view.SearchByDeclareCreatorID(adminID);
                    }
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

        protected void VoyageData()
        {
            var manifest = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.ManifestSure.Where(t => t.CutStatus == CutStatus.UnCutting).AsQueryable();
            manifest = manifest.OrderByDescending(t => t.CreateTime);

            Func<Needs.Ccs.Services.Models.Voyage, object> convert = item => new
            {
                VoyageID = item.ID,
                VoyageNo = item.ID,
                //Carrier = item.Carrier?.Name,
                //item.DriverCode,
                //item.DriverName,
                //item.HKLicense,
                //item.CustomsCode,
                //Type = item.Type.GetDescription(),
                //CreateTime = item.CreateTime.ToString("yyyy-MM-dd"),
                TransportTime = (item.TransportTime != null) ? item.TransportTime?.ToString("yyyy-MM-dd") : string.Empty,
                Summary = item.Summary,
                VoyageTypeValue = (int)item.Type,
                VoyageTypeName = item.Type.GetDescription(),
            };

            //this.Paging(manifest, convert);
            Response.Write(new
            {
                rows = manifest.Select(convert).ToList(),
            }.Json());
        }

        /// <summary>
        /// 提交设置 OrderVoyage 中的 VoyageID
        /// </summary>
        protected void SubmitVoyageID()
        {
            try
            {
                var voyageId = Request.Form["VoyageID"];
                var decNoticesForm = Request.Form["DecNotices"].Replace("&quot;", "\'").Replace("amp;", "");
                var decNoticesModel = decNoticesForm.JsonTo<dynamic>();

                //判断该 Voyage 是否已截单
                var voyage = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.ManifestSure.Where(t => t.ID == voyageId && t.CutStatus == CutStatus.UnCutting).FirstOrDefault();
                if (voyage == null)
                {
                    Response.Write((new { success = "false", message = "提交失败：该运输批次已截单" }).Json());
                    return;
                }

                var admin = new Needs.Ccs.Services.Views.AdminsTopView2().Where(t => t.OriginID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID).FirstOrDefault();

                foreach (var decNotice in decNoticesModel)
                {
                    string decNoticeId = decNotice.DecNoticeID;

                    var decNoticeVoyage = new Needs.Ccs.Services.Models.DecNoticeVoyage()
                    {
                        DeclarationNotice = new Needs.Ccs.Services.Models.DeclarationNotice { ID = decNoticeId },
                        Voyage = new Needs.Ccs.Services.Models.Voyage { ID = voyageId },
                        Admin = admin,
                        Summary = string.Empty,
                    };

                    decNoticeVoyage.Enter();
                }

                Response.Write((new { success = "true", message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = "false", message = "提交失败：" + ex.Message }).Json());
            }
        }

        protected void SetVoyageType()
        {
            try
            {
                var voyageId = Request.Form["VoyageID"];
                var voyageType = Request.Form["VoyageType"];

                var voyage = new Needs.Ccs.Services.Models.Voyage();
                voyage.ID = voyageId;
                voyage.Type = (Needs.Ccs.Services.Enums.VoyageType)int.Parse(voyageType);
                voyage.UpdateDate = DateTime.Now;
                voyage.SetVoyageType();

                Response.Write((new { success = "true", message = "修改运输类型成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = "false", message = "修改运输类型失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 装箱单列表
        /// </summary>
        protected void PackingList()
        {
            string OrderID = Request.QueryString["OrderID"];

            var packingList = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.PackingListView.GetResult(OrderID);

            Func<Needs.Ccs.Services.Models.PackingListModel, object> convert = packingInfo => new
            {
                BoxIndex = packingInfo.BoxIndex,
                ProductName = packingInfo.ProductName,
                Model = packingInfo.Model,
                OrderItemCategoryType = packingInfo.OrderItemCategoryType,
                OrderItemCategoryTypeDisplay = ((Needs.Ccs.Services.Enums.ItemCategoryType)packingInfo.OrderItemCategoryType).GetFlagsDescriptions("|"),
                Quantity = packingInfo.Quantity,
                TotalPrice = packingInfo.TotalPrice,
                NetWeight = packingInfo.NetWeight,
                GrossWeight = packingInfo.GrossWeight,
                Origin = packingInfo.Origin,
                HSCode = packingInfo.HSCode
            };

            Response.Write(new
            {
                rows = packingList.Select(convert).ToArray(),
            }.Json());
        }


        /// <summary>
        /// 装箱单列表
        /// </summary>
        protected void OrderList()
        {
            string OrderID = Request.QueryString["OrderID"];

            var orderitemList = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderItems.Where(t=>t.OrderID == OrderID).AsQueryable();

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
        /// 验证是否有型号价格异常 和其他（是否有型号变更没有处理）
        /// </summary>
        protected void CheckPriceAndOthers()
        {
            try
            {
                string OrderID = Request.Form["OrderID"];
                string NoticeID = Request.Form["NoticeID"];
                var orderitems = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderItems.GetOrderItemsOrigin(OrderID);
                var message1 = string.Empty;

                // ======================== 判断是否有价格异常 Begin ======================== 
                foreach (var item in orderitems)
                {
                    if (item.TotalPrice < 0.01M)
                    {
                        message1 += "型号：" + item.Model + "，总价：" + item.TotalPrice + "；";
                    }
                }

                if (!string.IsNullOrEmpty(message1))
                {
                    Response.Write(new { result = false, info = message1 + "价格异常！" }.Json());
                    return;
                }

                // ======================== 判断是否有价格异常 End ======================== 

                // ======================== 判断是否有型号变更没有处理 Begin ======================== 
                StringBuilder sbMessage2 = new StringBuilder();

                var unProcessedOrderItemChangeNoticeInfos = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.UnProcessedOrderItemChangeNoticeView.GetQueryable(OrderID).ToList();
                if (unProcessedOrderItemChangeNoticeInfos != null && unProcessedOrderItemChangeNoticeInfos.Any())
                {
                    var types = unProcessedOrderItemChangeNoticeInfos.Select(t => t.OrderItemChangeType).Distinct().ToArray();
                    if (types != null && types.Any())
                    {
                        foreach (var type in types)
                        {
                            var thisTypeModelChanges = unProcessedOrderItemChangeNoticeInfos.Where(t => t.OrderItemChangeType == type).ToList();
                            if (thisTypeModelChanges == null || !thisTypeModelChanges.Any())
                            {
                                continue;
                            }

                            string[] models = thisTypeModelChanges.Select(t => t.Model).ToArray();
                            string strModels = string.Join(" , ", models);

                            sbMessage2.Append("型号 <span style='color: green;'>" + strModels + "</span> 存在 <span style='color: red;'>" + type.GetDescription() + "</span>。<br/>");
                        }
                    }
                }

                string unProcessedMessage = sbMessage2.ToString();
                if (!string.IsNullOrEmpty(unProcessedMessage))
                {
                    unProcessedMessage = "订单 " + OrderID + " 存在型号信息变更没有处理：<br/>" + unProcessedMessage;

                    Response.Write(new { result = false, info = unProcessedMessage, }.Json());
                    return;
                }

                // ======================== 判断是否有型号变更没有处理 End ======================== 

                // ======================== 判断是否有未处理的订单变更 Begin ======================== 

                StringBuilder sbMessageUnProcessOrderChange = new StringBuilder();
                int countUnProcessOrderChange = new Needs.Ccs.Services.Views.Origins.OrderChangeNoticesOrigin()
                                                        .Where(t => t.OrderID == OrderID
                                                            && t.Status == Status.Normal
                                                            && (t.processState == ProcessState.UnProcess || t.processState == ProcessState.Processing)).Count();
                if (countUnProcessOrderChange > 0)
                {
                    Response.Write(new { result = false, info = "订单 " + OrderID + " 存在未处理的订单变更", }.Json());
                    return;
                }

                // ======================== 判断是否有未处理的订单变更 End ======================== 

                // ======================== 判断订单是否挂起，如果挂起当前未处理的管控 Begin ======================== 

                StringBuilder sbMessage3 = new StringBuilder();
                var unAuditControlView = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.UnAuditControlViewForDecNotice;
                bool isHangUp = unAuditControlView.GetOrderIsHangUp(OrderID);
                if (isHangUp)
                {
                    sbMessage3.Append("订单 <span style='color: green;'>" + OrderID + "</span> 已被挂起：<br/>");

                    //var unAuditControlInfos = unAuditControlView.GetUnAuditControlInfos(OrderID);
                    var unAuditControlInfos = unAuditControlView.GetUnAuditControlInfos(OrderID).Where(t=>t.OrderControlType!=OrderControlType.ExceedLimit).ToList();
                    if (unAuditControlInfos != null && unAuditControlInfos.Any())
                    {
                        for (int i = 0; i < unAuditControlInfos.Count; i++)
                        {
                            sbMessage3.Append((i + 1) + ". <span style='color: green;'>" + unAuditControlInfos[i].OrderControlStep.GetDescription()
                                + "</span> 还未审批 <span style='color: red;'>" + unAuditControlInfos[i].OrderControlType.GetDescription() + "</span>");

                            if (i != unAuditControlInfos.Count - 1)
                            {
                                sbMessage3.Append("<br/>");
                            }
                        }

                        Response.Write(new { result = false, info = sbMessage3.ToString(), }.Json());
                        return;
                    }
                    else
                    {
                        unAuditControlView.CancelOrderHangUp(OrderID);
                    }
                }

                // ======================== 判断订单是否挂起，如果挂起当前未处理的管控 End ======================== 

                // ======================== 判断是否含有朝鲜产地 Begin ======================== 

                StringBuilder sbMessage4 = new StringBuilder();
                var decListAdapters = new Needs.Ccs.Services.Views.DeclaresTopView().Where(t=>t.TinyOrderID == OrderID && t.Origin == "PRK");
                foreach (var item in decListAdapters)
                {
                    sbMessage4.Append("型号:" + item.PartNumber + "  产地为 <span style='color: red;'>朝鲜</span> 不能报关 <br/>");
                }

                if (!string.IsNullOrEmpty(sbMessage4.ToString()))
                {
                    Response.Write(new { result = false, info = sbMessage4.ToString(), }.Json());
                    return;
                }

                // ======================== 判断是否含有朝鲜产地 End ======================== 

                Needs.Ccs.Services.Models.NoticeLog noticeLog = new Needs.Ccs.Services.Models.NoticeLog();
                noticeLog.MainID = OrderID;
                noticeLog.NoticeType = SendNoticeType.DecNoticePending;
                noticeLog.SendNotice();

                Response.Write(new { result = true, info = "" }.Json());
            }
            catch (Exception ex)
            {
                Response.Write(new { result = false, info = "错误" + ex.ToString() }.Json());
            }
        }
        
        protected void ActualBoxNumbers()
        {
            string[] innerCompanies = DyjInnerCompanies.Current.Companies.Split(',');
            List<string> innerBoxes = new List<string>();
            List<string> outBoxes = new List<string>();
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            List<actualBoxModel> selectedModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<actualBoxModel>>(Model);
            foreach(var item in selectedModel)
            {
                string clientcode = item.OrderID.Substring(0, 5);
                string[] boxes = item.PackBoxes.Split(';');
                if (innerCompanies.Contains(clientcode))
                {
                    foreach(var box in boxes)
                    {
                        innerBoxes.Add(box);
                    }
                }
                else
                {
                    foreach (var box in boxes)
                    {
                        outBoxes.Add(box);
                    }
                }
            }

            int innerCount = new CalculateContext(CompanyTypeEnums.Inside, innerBoxes).CalculatePacks();
            int outCount = new CalculateContext(CompanyTypeEnums.Icgoo, outBoxes).CalculatePacks();

            Response.Write(new { success = true, totalPack = innerCount+ outCount }.Json());
        }

        public class actualBoxModel
        {
            public string OrderID { get; set; }
            public string PackBoxes { get; set; }
        }
    }
}