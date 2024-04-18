using Layer.Data.Sqls.BvScsm;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Manifest
{
    public partial class DoubleCheck : Uc.PageBase
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
            SetDropDownList();
            LoadData();
        }
        protected void LoadData()
        {       
            var ID = Request.QueryString["ID"];          
            var manifestHeads = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ManifestsNew[ID];
            //舱单编辑
            if (manifestHeads != null)
            {
                this.Model.Manifest = manifestHeads.Json();
            }
            else
            {
                this.Model.Manifest = "".Json();
            }
            this.Model.DateTimeNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        }
        private void SetDropDownList()
        {
            this.Model.CustomsCodeData = Needs.Wl.Admin.Plat.AdminPlat.BaseCustomMaster.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json();
            this.Model.ConditionCodeData = getConditionCode().Select(item => new { Value = item.Key, Text = item.Value }).OrderBy(item => item.Value).Json();
            this.Model.PaymentTypeData = getPaymentType().Select(item => new { Value = item.Key, Text = item.Value }).OrderBy(item => item.Value).Json();
            this.Model.GovProcedureData = Needs.Wl.Admin.Plat.AdminPlat.BaseGovProcedure.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json();
            this.Model.PackTypeData = Needs.Wl.Admin.Plat.AdminPlat.BaseWrapTypesView.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json();
            this.Model.CurrencyData = Needs.Wl.Admin.Plat.AdminPlat.Currencies.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json();
        }
        private Dictionary<string, string> getConditionCode()
        {
            Dictionary<string, string> mark = new Dictionary<string, string>();
            mark.Add("10", "10: port to port港到港");
            mark.Add("27", "27: door to door门到门");
            mark.Add("28", "28: door to pier门到点");
            mark.Add("29", "29: pier to door点到门");
            mark.Add("30", "30: pier to pier点到点");
            return mark;
        }
        private Dictionary<string, string> getPaymentType()
        {
            Dictionary<string, string> mark = new Dictionary<string, string>();
            mark.Add("1", "1-Direct payment");
            return mark;
        }
        protected void Save()
        {
            try
            {
                #region 前台数据

                var VoyageNo = Request.Form["VoyageNo"];
                //var TrafMode = Request.Form["TrafMode"];
                var CustomsCode = Request.Form["CustomsCode"];
                var CarrierCode = Request.Form["CarrierCode"];
                var TransAgentCode = Request.Form["TransAgentCode"];
                //var LoadingDate = Request.Form["LoadingDate"];
                var LoadingLocationCode = Request.Form["LoadingLocationCode"];
                //var ArrivalDate = Request.Form["ArrivalDate"];
                var CustomMaster = Request.Form["CustomMaster"];
                //var UnitCode = Request.Form["UnitCode"];
                var MsgRepName = Request.Form["MsgRepName"];
                var AdditionalInformation = Request.Form["AdditionalInformation"];
                var InfCount = int.Parse(Request.Form["InfCount"]);
                string Items = Request.Form["items"].Replace("&quot;", "\'");
                dynamic models = Items.JsonTo<dynamic>();
                var ID = Request.Form["ID"];
                #endregion
                var manifestHeads = new Needs.Ccs.Services.Models.Manifest();
                manifestHeads.ID = VoyageNo;
                //manifestHeads.TrafMode = int.Parse(TrafMode);
                manifestHeads.CustomsCode = CustomsCode;
                manifestHeads.CarrierCode = CarrierCode;
                manifestHeads.TransAgentCode = TransAgentCode;
                //manifestHeads.LoadingDate = DateTime.Parse(LoadingDate);
                manifestHeads.LoadingLocationCode = LoadingLocationCode;
                //manifestHeads.ArrivalDate = DateTime.Parse(ArrivalDate);
                manifestHeads.CustomMaster = CustomMaster;
                //manifestHeads.UnitCode = UnitCode;
                manifestHeads.MsgRepName = MsgRepName;
                manifestHeads.AdditionalInformation = AdditionalInformation;
                ///manifestHeads.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                for (int n = 0; n < InfCount; n++)
                {
                    var bill = new ManifestConsignment();
                    bill.ID = models[n].BillNo;
                    bill.ConditionCode = models[n].ConditionCode;
                    bill.PaymentType = models[n].PaymentType;
                    bill.GovProcedureCode = models[n].GovProcedureCode;
                    bill.TransitDestination = models[n].TransitDestination;
                    bill.PackNum = models[n].PackNum;
                    bill.PackType = models[n].PackType;
                    bill.Cube = models[n].Cube;
                    bill.GrossWt = models[n].GrossWt;
                    bill.GoodsValue = models[n].GoodsValue;
                    bill.Currency = models[n].Currency;
                    bill.Consolidator = models[n].Consolidator;
                    bill.ConsignorName = models[n].ConsignorName;
                    var contains = models[n].Container;
                    foreach (var item in contains)
                    {
                        bill.Containers.Add(new ManifestConsignmentContainer
                        {
                            ContainerNo = item
                        });
                    }
                    foreach (var item in models[n].Products)
                    {
                        bill.Items.Add(new ManifestConsignmentItem
                        {
                            GoodsSeqNo = item.GoodsSeqNo,
                            GoodsPackNum = item.GoodsPackNum,
                            GoodsPackType = item.GoodsPackType,
                            GoodsGrossWt = item.GoodsGrossWt,
                            GoodsBriefDesc = item.GoodsBriefDesc,
                            UndgNo = item.UndgNo,
                            HsCode = item.HsCode,
                            GoodsDetailDesc = item.GoodsDetailDesc,
                        });
                    }
                    //manifestHeads.Consignment = bill;
                }
                manifestHeads.EnterSuccess += ManifestHeads_EnterSuccess;
                //manifestHeads.EnterManifest();
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

        private void ManifestHeads_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功！" }).Json());
        }

        protected void data()
        {
           
            var ID = Request.QueryString["ID"];            
            var manifestHeads = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ManifestsNew[ID];
            var items = manifestHeads.Items;
            Func<Needs.Ccs.Services.Models.ManifestConsignmentItem, object> convert = head => new
            {
                GoodsSeqNo = head.GoodsSeqNo,
                GoodsPackNum = head.GoodsPackNum,
                GoodsPackType = head.GoodsPackType,
                GoodsGrossWt = head.GoodsGrossWt,
                GoodsBriefDesc = head.GoodsBriefDesc,
                UndgNo = head.UndgNo,
                HsCode = head.HsCode,
                GoodsDetailDesc = head.GoodsDetailDesc,
            };
            Response.Write(new
            {
                rows = items.Select(convert).ToArray(),
                total = items.Count()
            }.Json());

        }

        protected void Submit()
        {
            try
            {
                string ManifestConsignmentID = Request.Form["ManifestID"];
                string summary = Request.Form["Reason"];
                string openTime = Request.Form["StartTime"];
               
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);               

                var mftConsignment = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ManifestConsignments.First(c => c.ID == ManifestConsignmentID);
                mftConsignment.DoubleCheck();

                var traceMsg = admin.ByName + "在 " + openTime + " 打开了页面,开始复核,在 " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 结束复核。";
                if (!string.IsNullOrEmpty(summary))
                {
                    traceMsg += "备注:" + summary;
                }
                mftConsignment.Trace(traceMsg);

                Response.Write((new { success = true, message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new
                {
                    success = false,
                    message = "提交失败：" + ex.Message
                }).Json());
            }
        }

        protected void Refuse()
        {
            try
            {
                string DeclarationID = Request.Form["DeclarationID"];
                string summary = Request.Form["Reason"];
                string openTime = Request.Form["StartTime"];
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var decHead = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[DeclarationID];

                var traceMsg = admin.ByName + "在 " + openTime + " 打开了页面,开始复核,在 " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 退回该报关单。拒绝理由：" + summary;

                decHead.Trace(traceMsg);
                decHead.CheckerDoubleRefuse();

                Response.Write((new { success = true, message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new
                {
                    success = false,
                    message = "提交失败：" + ex.Message
                }).Json());
            }
        }
    }
}
