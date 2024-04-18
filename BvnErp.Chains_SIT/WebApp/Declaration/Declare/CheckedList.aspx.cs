using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Services;
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
    public partial class CheckedList : Uc.PageBase
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
            //Dictionary<string, string> dicVoyageType = new Dictionary<string, string>();
            //dicVoyageType.Add(VoyageType.Normal.GetHashCode().ToString(), VoyageType.Normal.GetDescription());
            //dicVoyageType.Add(VoyageType.CharterBus.GetHashCode().ToString(), VoyageType.CharterBus.GetDescription());
            //this.Model.VoyageType = dicVoyageType.Select(item => new { TypeValue = item.Key, TypeText = item.Value }).Json();

            this.Model.DecHeadSpecialType = EnumUtils.ToDictionary<DecHeadSpecialTypeEnum>().Select(item => new { TypeValue = item.Key, TypeText = item.Value }).Json();

            var thisAdminOriginID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
            this.Model.ThisAdminOriginID = thisAdminOriginID;
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
            //string VoyageType = Request.QueryString["VoyageType"];
            var decHeadSpecialTypeForm = Request.QueryString["DecHeadSpecialType"] != null ? Request.QueryString["DecHeadSpecialType"].Replace("&quot;", "\'").Replace("amp;", "") : null;

            using (var query = new Needs.Ccs.Services.Views.DecHeadsListViewRJ())
            {
                var view = query;

                view = view.SearchBy06();//过滤待复核

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

                //if (!string.IsNullOrEmpty(VoyageType) && VoyageType != "0" && VoyageType != "全部")
                //{
                //    int intVoyageType = 0;
                //    if (int.TryParse(VoyageType, out intVoyageType))
                //    {
                //        view = view.SearchByVoyageType(intVoyageType);
                //    }
                //}

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

        protected void DownloadFiles()
        {
            string ID = Request.Form["DeclarationID"];

            var edoc = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.EdocRealations.Where(t => t.DeclarationID == ID);
            string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];

            List<string> urls = new List<string>();
            foreach (var item in edoc)
            {
                urls.Add(FileServerUrl + @"/" + item.FileUrl.ToUrl());
            }

            Response.Write(new { success = true, data = urls }.Json());
        }

        /// <summary>
        /// 获取保函信息
        /// </summary>
        protected void GetGuarantees()
        {
            List<Needs.Ccs.Services.Models.DecTaxGuarantee> result = new Needs.Ccs.Services.Views.DecTaxGuaranteeView().ToList();

            if (result != null)
            {
                Response.Write(result.Json());
            }
        }

        /// <summary>
        /// 保函信息
        /// </summary>
        protected void GuaranteeData()
        {
            var guarantee = new Needs.Ccs.Services.Views.DecTaxGuaranteeView().AsQueryable();

            Func<Needs.Ccs.Services.Models.DecTaxGuarantee, object> convert = gua => new
            {
                ID = gua.ID,
                GuaranteeNo = gua.GuaranteeNo,
                PutOnCustoms = gua.PutOnCustoms,
                GuaranteeAmount = gua.GuaranteeAmount,
                RemainAmount = gua.RemainAmount,
                BankName = gua.BankName,
                ApproveDate = gua.ApproveDate.ToString("yyyy-MM-dd"),
                ValidDate = gua.ValidDate.ToString("yyyy-MM-dd"),
                Summary = gua.Summary
            };

            Response.Write(new
            {
                rows = guarantee.Select(convert).ToList(),
            }.Json());
        }

        protected void SubmitGuaranteeNo()
        {
            try
            {
                var guaranteeNo = Request.Form["GuaranteeNo"];
                var decHeadID = Request.Form["DecHeads"].Replace("&quot;", "\'").Replace("amp;", "");
                var decHeadsModel = decHeadID.JsonTo<dynamic>();

                var guarantee = new Needs.Ccs.Services.Views.DecTaxGuaranteeView().FirstOrDefault(t => t.ID == guaranteeNo);
                var msg = string.Empty;
                foreach (var dechead in decHeadsModel)
                {
                    
                    ///获取当前需要报关的报关单的税费
                    var entity = new DecTaxQuota();
                    var currentHeadTaxFee = new CustomsFeeService().GetCustomsFeeSum((string)dechead.OrderID);

                    //额度够用
                    if (guarantee.RemainAmount > currentHeadTaxFee.TaxFeeTotal)
                    {
                        //记录报关税费
                        entity.DeclarationID = dechead.DecHeadID;
                        entity.AddedValueTax = currentHeadTaxFee.AddedValue.Value;
                        entity.Tariff = currentHeadTaxFee.TariffValue.Value;
                        entity.PayStatus = TaxStatus.Unpaid;
                        entity.Enter();

                        //设置保函编号
                        var decheadid = (string)dechead.DecHeadID;
                        var decheadentity = new Needs.Ccs.Services.Views.DecHeadsView().FirstOrDefault(t => t.ID == decheadid);
                        decheadentity.GuarateeNo = guaranteeNo;
                        decheadentity.SetGuarateeNo();
                        guarantee.RemainAmount = guarantee.RemainAmount - currentHeadTaxFee.TaxFeeTotal;

                    }
                    else
                    {
                        msg = "剩余额度不足，请先进行缴税";
                        break;
                    }
                }

                //更新保函余额
                guarantee.Enter();


                Response.Write((new { success = "true", message = string.IsNullOrEmpty(msg) ? "提交成功," : msg }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = "false", message = "提交失败：" + ex.Message }).Json());
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
                        //两步申报只附加一次 3001 尾缀
                        if (head.Type.Length < 3)
                        {
                            head.Type = head.Type + "30" + ((head.IsInspection || head.IsQuarantine.Value) ? "1" : "0") + "1";
                        }
                    }
                    else
                    {
                        head.Type = "ZC";
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
    }
}