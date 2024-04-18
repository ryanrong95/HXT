using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
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
    public partial class List : Uc.PageBase
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
            this.Model.Status = Needs.Ccs.Services.MultiEnumUtils.ToDictionary<Needs.Ccs.Services.Enums.CusDecStatus>()
                .Select(item => new { Value = item.Key, Text = item.Value }).Json();

            //this.Model.VoyageType = EnumUtils.ToDictionary<VoyageType>().Select(item => new { TypeValue = item.Key, TypeText = item.Value }).Json();
            Dictionary<string, string> dicVoyageType = new Dictionary<string, string>();
            dicVoyageType.Add(VoyageType.Normal.GetHashCode().ToString(), VoyageType.Normal.GetDescription());
            dicVoyageType.Add(VoyageType.CharterBus.GetHashCode().ToString(), VoyageType.CharterBus.GetDescription());
            this.Model.VoyageType = dicVoyageType.Select(item => new { TypeValue = item.Key, TypeText = item.Value }).Json();

            this.Model.DecHeadSpecialType = EnumUtils.ToDictionary<DecHeadSpecialTypeEnum>().Select(item => new { TypeValue = item.Key, TypeText = item.Value }).Json();

            this.Model.CandidateData = new Needs.Ccs.Services.Views.SelectableCandidatesView().GetUseCandidates(Needs.Ccs.Services.Enums.DeclarantCandidateType.CustomSubmiter)
                    .Select(item => new { value = item.AdminID, text = item.AdminName }).Json();

            var thisAdminOriginID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
            this.Model.ThisAdminOriginID = thisAdminOriginID;
            this.Model.RealName = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;
        }

        /// <summary>
        /// 旧的
        /// </summary>
        protected void data1()
        {
            string ContrNo = Request.QueryString["ContrNo"];
            string OrderID = Request.QueryString["OrderID"];
            string PreEntryId = Request.QueryString["PreEntryId"];
            string Status = Request.QueryString["Status"];
            string VoyageID = Request.QueryString["VoyageID"];
            string VoyageType = Request.QueryString["VoyageType"];
            var decHeadSpecialTypeForm = Request.QueryString["DecHeadSpecialType"] != null ? Request.QueryString["DecHeadSpecialType"].Replace("&quot;", "\'").Replace("amp;", "") : null;

            var DecHead = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHeadDraftList.OrderByDescending(item => item.CreateTime).AsQueryable();


            if (!string.IsNullOrEmpty(ContrNo))
            {
                ContrNo = ContrNo.Trim();
                DecHead = DecHead.Where(t => t.ContrNo == ContrNo);
            }
            if (!string.IsNullOrEmpty(OrderID))
            {
                OrderID = OrderID.Trim();
                DecHead = DecHead.Where(t => t.OrderID == OrderID);
            }
            if (!string.IsNullOrEmpty(PreEntryId))
            {
                PreEntryId = PreEntryId.Trim();
                DecHead = DecHead.Where(t => t.PreEntryId == PreEntryId);
            }
            if (!string.IsNullOrEmpty(Status))
            {
                DecHead = DecHead.Where(t => t.Status == Status);
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

            Func<Needs.Ccs.Services.Models.DecHeadList, object> convert = head => new
            {
                ID = head.ID,
                ContrNo = head.ContrNo,
                OrderID = head.OrderID,
                BillNo = head.BillNo,
                EntryId = head.EntryId,
                PreEntryId = head.PreEntryId,
                //AgentName = head.AgentName,
                //ConsignorName = head.ConsignorName,
                ConsigneeName = head.ConsigneeName,
                //IsInspection = head.InspQuarName,
                CreateDate = head.CreateTime.ToString("yyyy-MM-dd HH:mm"),
                InputerID = head.InputerID,
                Status = head.Status,
                StatusName = head.StatusName,
                Transformed = head.Transformed,
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
            };
            this.Paging(DecHead, convert);
        }

        /// <summary>
        /// 新的
        /// </summary>
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ContrNO = Request.QueryString["ContrNo"];
            string OrderID = Request.QueryString["OrderID"];
            //string SeqNo = Request.QueryString["SeqNo"];
            //string CusReceiptCode = Request.QueryString["BaseCusReceiptCode"];
            //string StartDate = Request.QueryString["StartDate"];
            //string EndDate = Request.QueryString["EndDate"];
            string VoyageID = Request.QueryString["VoyageID"];
            string VoyageType = Request.QueryString["VoyageType"];
            var decHeadSpecialTypeForm = Request.QueryString["DecHeadSpecialType"] != null ? Request.QueryString["DecHeadSpecialType"].Replace("&quot;", "\'").Replace("amp;", "") : null;

            string MyDecHead = Request.QueryString["MyDecHead"];

            using (var query = new Needs.Ccs.Services.Views.DecHeadsListViewRJ())
            {
                var view = query;

                view = view.SearchByDraft();//过滤草稿

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

                //if (!string.IsNullOrWhiteSpace(SeqNo))
                //{
                //    SeqNo = SeqNo.Trim();
                //    view = view.SearchBySeqNo(SeqNo);
                //}

                //if (!string.IsNullOrWhiteSpace(CusReceiptCode))
                //{
                //    CusReceiptCode = CusReceiptCode.Trim();
                //    view = view.SearchByCusReceiptCode(CusReceiptCode);
                //}

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

                //if (!string.IsNullOrEmpty(StartDate))
                //{
                //    StartDate = StartDate.Trim();
                //    var from = DateTime.Parse(StartDate);
                //    view = view.SearchByFrom(from);
                //}

                //if (!string.IsNullOrEmpty(EndDate))
                //{
                //    EndDate = EndDate.Trim();
                //    var to = DateTime.Parse(EndDate).AddDays(1);
                //    view = view.SearchByTo(to);
                //}

                if (decHeadSpecialTypeForm != null)
                {
                    var specials = decHeadSpecialTypeForm.JsonTo<dynamic[]>().Select(item => (DecHeadSpecialTypeEnum)(item.DecHeadSpecialTypeValue)).ToArray();
                    if (specials != null && specials.Any())
                    {
                        view = view.SearchBySpecialType(specials);
                    }
                }

                bool MyDecHeadBoolean = false;
                if (bool.TryParse(MyDecHead, out MyDecHeadBoolean))
                {
                    if (MyDecHeadBoolean)
                    {
                        var adminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                        view = view.SearchByDeclareCreatorID(adminID);
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

                headlist.ForEach(t =>
                {
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

                headlist.ForEach(t =>
                {
                    var head = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DecHeads[t];

                    //是否两步申报
                    if (split)
                    {
                        head.Type = head.Type + "30" + ((head.IsInspection || head.IsQuarantine.Value) ? "1" : "0") + "1";
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


        /// <summary>
        /// 导出Excel走泰州物流报关
        /// </summary>
        protected void DownloadExcel()
        {
            try
            {
                //1.创建文件夹(文件压缩后存放的地址)
                FileDirectory file = new FileDirectory();
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Export);
                file.CreateDataDirectory();

                string ID = Request.Form["ID"];
                //string CustomSubmiterAdminID = Request.Form["CustomSubmiterAdminID"];

                //var vendor = new VendorContext(VendorContextInitParam.DecHeadID, ID).Current1;

                Needs.Ccs.Services.Models.DecHead headinfo = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[ID];
                var vendor = new VendorContext(VendorContextInitParam.Pointed, headinfo.OwnerName).Current1;
                foreach (var p in headinfo.Lists)
                {
                    string UnknownCountryModels = "";
                    if (p.OriginCountry == Needs.Ccs.Services.Icgoo.UnknownCountry)
                    {
                        UnknownCountryModels += p.GNo + ";";
                    }
                    if (UnknownCountryModels != "")
                    {
                        //Response.Write((new { success = false, info = UnknownCountryModels.Substring(0, UnknownCountryModels.Length - 1) + "原产地未识别，请先更改！" }).Json());
                        Response.Write((new { success = false, info = "有原产地未识别的型号，请先更改！" }).Json());
                        return;
                    }

                }

                ExcelDeclareDocument excel = new ExcelDeclareDocument(headinfo);
                string FileName = headinfo.ContrNo + ".xlsx";
                string DomainUrl = System.Configuration.ConfigurationManager.AppSettings["DomainUrl"];
                string FileAddress = file.FileUrl.Replace(DomainUrl, "");
                excel.setFilePath(FileAddress.Substring(1, FileAddress.Length - 1));
                excel.SaveAs(FileName, vendor);
                Response.Write(new { result = true, info = "导出成功", url = file.FileUrl + FileName }.Json());
            }
            catch (Exception ex)
            {
                Response.Write(new { result = false, info = "保存错误" + ex.ToString() }.Json());
            }

        }

        /// <summary>
        /// 验证 未知产地
        /// </summary>
        protected void CheckOrigin()
        {
            try
            {
                string ID = Request.Form["ID"];

                Needs.Ccs.Services.Models.DecHead headinfo = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[ID];
                var HOrigin = new Needs.Ccs.Services.Views.BaseCountriesView().Where(t => t.Preferential != "L").ToList();
                foreach (var p in headinfo.Lists)
                {
                    string UnknownCountryModels = "";
                    if (p.OriginCountry == Needs.Ccs.Services.Icgoo.UnknownCountry)
                    {
                        UnknownCountryModels += p.GNo + ";";
                    }
                    if (UnknownCountryModels != "")
                    {
                        //Response.Write((new { success = false, info = UnknownCountryModels.Substring(0, UnknownCountryModels.Length - 1) + " 原产地未识别，请先更改！" }).Json());
                        Response.Write((new { success = false, info = "有原产地未识别的型号，请先更改！" }).Json());
                        return;
                    }

                    if (HOrigin.Any(t => t.Code == p.OriginCountry))
                    {
                        Response.Write((new { success = true, info = "第" + p.GNo + "项 产地为非最惠国，请确认！" }).Json());
                        return;
                    }
                    
                }
                Response.Write((new { success = true, info = "" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write(new { success = false, info = "保存错误" + ex.ToString() }.Json());
            }

        }

        protected void ActualBoxNumbers()
        {
            string[] innerCompanies = DyjInnerCompanies.Current.Companies.Split(',');
            List<string> innerBoxes = new List<string>();
            List<string> outBoxes = new List<string>();
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            List<actualBoxModel> selectedModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<actualBoxModel>>(Model);
            foreach (var item in selectedModel)
            {
                string clientcode = item.OrderID.Substring(0, 5);
                string[] boxes = item.PackBoxes.Split(';');
                if (innerCompanies.Contains(clientcode))
                {
                    foreach (var box in boxes)
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

            Response.Write(new { success = true, totalPack = innerCount + outCount }.Json());
        }

        public class actualBoxModel
        {
            public string OrderID { get; set; }
            public string PackBoxes { get; set; }
        }
    }
}
