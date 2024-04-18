using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class ExcelList   : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Load_Data();
        }
        protected void Load_Data()
        {
            this.Model.CusReceiptCodeData = Needs.Ccs.Services.MultiEnumUtils.ToDictionary<Needs.Ccs.Services.Enums.CusDecStatus>()
                .Select(item => new { Value = item.Key, Text = item.Value }).Json();

            //this.Model.VoyageType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.VoyageType>().Select(item => new { TypeValue = item.Key, TypeText = item.Value }).Json();
            Dictionary<string, string> dicVoyageType = new Dictionary<string, string>();
            dicVoyageType.Add(VoyageType.Normal.GetHashCode().ToString(), VoyageType.Normal.GetDescription());
            dicVoyageType.Add(VoyageType.CharterBus.GetHashCode().ToString(), VoyageType.CharterBus.GetDescription());
            this.Model.VoyageType = dicVoyageType.Select(item => new { TypeValue = item.Key, TypeText = item.Value }).Json();

            this.Model.DecHeadSpecialType = EnumUtils.ToDictionary<DecHeadSpecialTypeEnum>().Select(item => new { TypeValue = item.Key, TypeText = item.Value }).Json();

            var thisAdminOriginID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
            this.Model.ThisAdminOriginID = thisAdminOriginID;
        }

        /// <summary>
        /// 旧的
        /// </summary>
        protected void data1()
        {
            string ContrNO = Request.QueryString["ContrNo"];
            string OrderID = Request.QueryString["OrderID"];
            string SeqNo = Request.QueryString["SeqNo"];
            string CusReceiptCode = Request.QueryString["BaseCusReceiptCode"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string VoyageID = Request.QueryString["VoyageID"];
            string VoyageType = Request.QueryString["VoyageType"];
            var decHeadSpecialTypeForm = Request.QueryString["DecHeadSpecialType"] != null ? Request.QueryString["DecHeadSpecialType"].Replace("&quot;", "\'").Replace("amp;", "") : null;

            var DecHead = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.ExcelUploadDecHeadsList.AsQueryable();


            if (!string.IsNullOrEmpty(ContrNO))
            {
                ContrNO = ContrNO.Trim();
                DecHead = DecHead.Where(t => t.ContrNo == ContrNO);
            }
            if (!string.IsNullOrEmpty(OrderID))
            {
                OrderID = OrderID.Trim();
                DecHead = DecHead.Where(t => t.OrderID == OrderID);
            }
            if (!string.IsNullOrEmpty(SeqNo))
            {
                SeqNo = SeqNo.Trim();
                DecHead = DecHead.Where(t => t.SeqNo == SeqNo);
            }
            if (!string.IsNullOrEmpty(CusReceiptCode))
            {
                DecHead = DecHead.Where(t => t.CusDecStatus == CusReceiptCode);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                StartDate = StartDate.Trim();
                var from = DateTime.Parse(StartDate);
                DecHead = DecHead.Where(t => t.CreateTime >= from);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                EndDate = EndDate.Trim();
                var to = DateTime.Parse(EndDate).AddDays(1);
                DecHead = DecHead.Where(t => t.CreateTime <= to);
            }
            if (!string.IsNullOrEmpty(VoyageID))
            {
                VoyageID = VoyageID.Trim();
                DecHead = DecHead.Where(t => t.VoyageID.Contains(VoyageID));
            }
            if (!string.IsNullOrEmpty(VoyageType) && VoyageType != "0" && VoyageType != "全部")
            {
                int intVoyageType = 0;
                if (int.TryParse(VoyageType, out intVoyageType))
                {
                    DecHead = DecHead.Where(t => t.VoyageType == (Needs.Ccs.Services.Enums.VoyageType)intVoyageType);
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
                            DecHead = DecHead.Where(t => t.IsCharterBus == true);
                            break;
                        case DecHeadSpecialTypeEnum.HighValue:
                            DecHead = DecHead.Where(t => t.IsHighValue == true);
                            break;
                        case DecHeadSpecialTypeEnum.Inspection:
                            DecHead = DecHead.Where(t => t.IsInspection == true);
                            break;
                        case DecHeadSpecialTypeEnum.Quarantine:
                            DecHead = DecHead.Where(t => t.IsQuarantine == true);
                            break;
                        case DecHeadSpecialTypeEnum.CCC:
                            DecHead = DecHead.Where(t => t.IsCCC == true);
                            break;
                        default:
                            break;
                    }
                }
            }

            Func<Needs.Ccs.Services.Models.UploadDecHead, object> convert = head => new
            {
                ID = head.ID,
                ContrNO = head.ContrNo,
                OrderID = head.OrderID,
                BillNo = head.BillNo,
                EntryId = head.EntryId,
                SeqNo = head.SeqNo,
                //AgentName = head.AgentName,
                //IsInspection = head.InspQuarName,
                CreateDate = head.CreateTime.ToString("yyyy-MM-dd HH:mm"),
                InputerID = head.InputerID,
                Status = head.CusDecStatus,
                StatusName = head.StatusName,
                IsDecHeadFile = head.IsDecHeadFile,
                URL = Needs.Utils.FileDirectory.Current.FileServerUrl + @"/" + head.decheadFile?.Url.ToUrl(),
                ClientCode = head.ClientCode,
                ClientName = head.ClientName,
                IsCharterBus = head.IsCharterBus,
                IsHighValue = head.IsHighValue,
                IsInspection = head.IsInspection,
                IsQuarantine = head.IsQuarantine,
                IsCCC = head.IsCCC,
                VoyageID = head.VoyageID,
                VoyageType = head.VoyageType.GetDescription(),

                PackNo = head.PackNo,
                GrossWt = head.GrossWt,
                TotalQty = head.TotalQty,
                ModelAmount = head.ModelAmount,
                TotalAmount = head.TotalAmount,
            };
            this.Paging(DecHead, convert);


            
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ContrNO = Request.QueryString["ContrNo"];
            string OrderID = Request.QueryString["OrderID"];
            string SeqNo = Request.QueryString["SeqNo"];
            string CusReceiptCode = Request.QueryString["BaseCusReceiptCode"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string VoyageID = Request.QueryString["VoyageID"];
            string VoyageType = Request.QueryString["VoyageType"];
            var decHeadSpecialTypeForm = Request.QueryString["DecHeadSpecialType"] != null ? Request.QueryString["DecHeadSpecialType"].Replace("&quot;", "\'").Replace("amp;", "") : null;

            using (var query = new Needs.Ccs.Services.Views.DecHeadsListViewRJ())
            {
                var view = query;

                view = view.SearchByE0();//过滤表格申报

                if (!string.IsNullOrWhiteSpace(ContrNO))
                {
                    ContrNO = ContrNO.Trim();
                    view = view.SearchByContractID(ContrNO);
                }

                if (!string.IsNullOrWhiteSpace(OrderID))
                {
                    OrderID = OrderID.Trim();
                    view = view.SearchByOrderID(OrderID);
                }

                if (!string.IsNullOrWhiteSpace(SeqNo))
                {
                    SeqNo = SeqNo.Trim();
                    view = view.SearchBySeqNo(SeqNo);
                }

                if (!string.IsNullOrWhiteSpace(CusReceiptCode))
                {
                    CusReceiptCode = CusReceiptCode.Trim();
                    view = view.SearchByCusReceiptCode(CusReceiptCode);
                }

                if (!string.IsNullOrWhiteSpace(VoyageID))
                {
                    VoyageID = VoyageID.Trim();
                    view = view.SearchByVoyageID(VoyageID);
                }

                if (!string.IsNullOrEmpty(VoyageType) && VoyageType != "0" && VoyageType != "全部")
                {
                    int intVoyageType = 0;
                    if (int.TryParse(VoyageType, out intVoyageType))
                    {
                        view = view.SearchByVoyageType(intVoyageType);
                    }
                }

                if (!string.IsNullOrEmpty(StartDate))
                {
                    StartDate = StartDate.Trim();
                    var from = DateTime.Parse(StartDate);
                    view = view.SearchByFrom(from);
                }

                if (!string.IsNullOrEmpty(EndDate))
                {
                    EndDate = EndDate.Trim();
                    var to = DateTime.Parse(EndDate).AddDays(1);
                    view = view.SearchByTo(to);
                }

                if (decHeadSpecialTypeForm != null)
                {
                    var specials = decHeadSpecialTypeForm.JsonTo<dynamic[]>().Select(item => (DecHeadSpecialTypeEnum)(item.DecHeadSpecialTypeValue)).ToArray();
                    if (specials != null && specials.Any())
                    {
                        view = view.SearchBySpecialType(specials);
                    }
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }

        }

        /// <summary>
        /// 批量转换舱单
        /// </summary>
        protected void Transform()
        {
            try
            {
                string ids = Request.Form["ID"];
                var headlist = ids.Split(',').ToList();

                headlist.ForEach(t => {
                    var head = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DecHeads[t];
                    head.ToManifest();
                });

                Response.Write((new { success = true, message = "转换成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "转换失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 批量制单
        /// </summary>
        protected void Make()
        {
            try
            {
                string ids = Request.Form["ID"];
                bool split = bool.Parse(Request.Form["Split"]);
                var headlist = ids.Split(',').ToList();

                string UnknownCountryModels = "";

                headlist.ForEach(t => {
                    var head = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DecHeads[t];

                    //是否两步申报
                    if (split)
                    {
                        //两步申报只附加一次 3001 尾缀
                        if (head.Type.Length < 3)
                        {
                            head.Type = head.Type + "30" + ((head.IsInspection || head.IsQuarantine.Value) ? "1" : "0") + "1";
                        }
                    }

                    foreach (var p in head.Lists)
                    {

                        if (p.OriginCountry == Needs.Ccs.Services.Icgoo.UnknownCountry)
                        {
                            UnknownCountryModels += head.ID + ":" + p.GoodsModel + ";";
                        }
                        if (UnknownCountryModels != "")
                        {
                            return;
                        }
                    }
                    head.Make();
                });

                if (UnknownCountryModels != "")
                {
                    Response.Write((new { success = false, message = UnknownCountryModels.Substring(0, UnknownCountryModels.Length - 1) + "原产地未识别，请先更改！" }).Json());
                }
                else
                {
                    Response.Write((new { success = true, message = "已申报！" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "申报失败：" + ex.Message }).Json());
            }
        }

        protected void DownloadFiles()
        {
            string ID = Request.Form["DeclarationID"];

            var edoc = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.EdocRealations.Where(t => t.DeclarationID == ID);            
            string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];

            List<string> urls = new List<string>();
            foreach(var item in edoc)
            {
                urls.Add(FileServerUrl + @"/" + item.FileUrl.ToUrl());
            }

            Response.Write(new { success = true, data = urls }.Json());
        }
    }
}