using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
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
    public partial class CheckingList : Uc.PageBase
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
            //string VoyageType = Request.QueryString["VoyageType"];
            var decHeadSpecialTypeForm = Request.QueryString["DecHeadSpecialType"] != null ? Request.QueryString["DecHeadSpecialType"].Replace("&quot;", "\'").Replace("amp;", "") : null;

            string MyDecHead = Request.QueryString["MyDecHead"];

            using (var query = new Needs.Ccs.Services.Views.DecHeadsListViewRJ())
            {
                var view = query;

                view = view.SearchBy05();//过滤草稿

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

                //if (!string.IsNullOrEmpty(VoyageType) && VoyageType != "0" && VoyageType != "全部")
                //{
                //    int intVoyageType = 0;
                //    if (int.TryParse(VoyageType, out intVoyageType))
                //    {
                //        view = view.SearchByVoyageType(intVoyageType);
                //    }
                //}

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
    }
}