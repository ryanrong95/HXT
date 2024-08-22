using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class DecListEdit : Uc.PageBase
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
            this.Model.BaseUnit = Needs.Wl.Admin.Plat.AdminPlat.Units.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json();
            this.Model.BaseCurrency = Needs.Wl.Admin.Plat.AdminPlat.Currencies.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json();
            this.Model.BaseCountry = Needs.Wl.Admin.Plat.AdminPlat.Countries.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json();
            this.Model.BaseDistrictCode = Needs.Wl.Admin.Plat.AdminPlat.BaseDistrictCode.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json();
            this.Model.BaseDutyMode = Needs.Wl.Admin.Plat.AdminPlat.BaseDutyMode.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json();
            this.Model.BaseGoodsAttr = Needs.Wl.Admin.Plat.AdminPlat.BaseGoodsAttr.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json();
            this.Model.BasePurpose = Needs.Wl.Admin.Plat.AdminPlat.BasePurpose.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json();
            this.Model.BaseOriginArea = Needs.Wl.Admin.Plat.AdminPlat.BaseOriginArea.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json();
            this.Model.BaseDestCode = Needs.Wl.Admin.Plat.AdminPlat.BaseDestCode.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json();
            string ReplaceQuotes = "这里是一个双引号";
            this.Model.ReplaceQuotes = ReplaceQuotes;

            string ID = Request.QueryString["ID"];

            var DecList = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DecOriginList[ID];

            if (DecList != null)
            {
                this.Model.DecList = new
                {
                    GNo = DecList.GNo,
                    ContrItem = DecList.ContrItem,
                    CodeTS = DecList.CodeTS,
                    CiqCode = DecList.CiqCode,
                    CiqName = DecList.CiqName,
                    GName = DecList.GName,
                    GModel = DecList.GModel.Replace("\"", ReplaceQuotes).Replace("\'", "%27"),
                    GQty = DecList.GQty,
                    GUnit = DecList.GUnit,
                    DeclPrice = DecList.DeclPrice,
                    DeclTotal = DecList.DeclTotal,
                    TradeCurr = DecList.TradeCurr,
                    FirstQty = DecList.FirstQty,
                    FirstUnit = DecList.FirstUnit,
                    OriginCountry = DecList.OriginCountry,
                    DestinationCountry = DecList.DestinationCountry,
                    SecondQty = DecList.SecondQty,
                    SecondUnit = DecList.SecondUnit,
                    DistrictCode = DecList.DistrictCode,
                    DestCode = DecList.DestCode,
                    DutyMode = DecList.DutyMode,
                    OrigPlaceCode = DecList.OrigPlaceCode,
                    GoodsSpec = DecList.GoodsSpec == null ? "" : DecList.GoodsSpec.Replace("\'", "%27").Replace("\"", ReplaceQuotes),
                    GoodsAttr = DecList.GoodsAttr,
                    Purpose = DecList.Purpose,
                    GoodsBrand = DecList.GoodsBrand,
                    GoodsModel = DecList.GoodsModel.Replace("\"", ReplaceQuotes).Replace("\'", "%27"),
                    GoodsBatch = DecList.GoodsBatch
                }.Json();
            }
            else
            {
                this.Model.DecList = new { }.Json();
            }


        }

        protected void Save()
        {
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            dynamic model = Model.JsonTo<dynamic>();
            string changeMessage = "";
            string ID = model.ID;
            var DecList = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DecOriginList[ID];

            string decHeadID = DecList.DeclarationID;


            var oldDecList = (Needs.Ccs.Services.Models.DecList)DecList.Copy();
            var admin = Needs.Wl.Admin.Plat.AdminPlat.Current;
            var byName = new Needs.Ccs.Services.Views.AdminsTopView().FirstOrDefault(item => item.ID == admin.ID).ByName;//获取别名

            if (DecList != null)
            {
                #region 记录修改
                if (DecList.CodeTS != model.CodeTS.Value)
                {
                    changeMessage += "报关员:" + byName + "把商品项" + ID + "[" + DecList.GoodsModel + "]商品编码:" + DecList.CodeTS + "变更为:" + model.CodeTS + ";";
                }
                if (DecList.OriginCountry != model.OriginCountry.Value)
                {
                    changeMessage += "报关员:" + byName + "把商品项" + ID + "[" + DecList.GoodsModel + "]原产国:" + DecList.OriginCountry + "变更为:" + model.OriginCountry + ";";
                }
                if (DecList.GoodsBrand != model.GoodsBrand.Value.Replace("%26", "&").Replace("%2C", ",").Replace("%27", "\'"))
                {
                    changeMessage += "报关员:" + byName + "把商品项" + ID + "[" + DecList.GoodsModel + "]品牌:" + DecList.GoodsBrand + "变更为:" + model.GoodsBrand.Value.Replace("%26", "&").Replace("%2C", ",").Replace("%27", "\'") + ";";
                }
                if (DecList.GoodsModel != model.GoodsModel.Value)
                {
                    changeMessage += "报关员:" + byName + "把商品项" + ID + "[" + DecList.GoodsModel + "]型号:" + DecList.GoodsModel + "变更为:" + model.GoodsModel + ";";
                }
                if (DecList.GName != model.GName.Value)
                {
                    changeMessage += "报关员:" + byName + "把商品项" + ID + "[" + DecList.GoodsModel + "]商品名称:" + DecList.GName + "变更为:" + model.GName + ";";
                }
                if (DecList.GQty != Convert.ToDecimal(model.GQty.Value))
                {
                    changeMessage += "报关员:" + byName + "把商品项" + ID + "[" + DecList.GoodsModel + "]商品数量:" + DecList.GQty + "变更为:" + model.GQty + ";";
                }
                if (DecList.CiqName != model.CiqName.Value)
                {
                    changeMessage += "报关员:" + byName + "把商品项" + ID + "[" + DecList.GoodsModel + "]监管类别名称:" + DecList.CiqName + "变更为:" + model.CiqName + ";";
                }
                if (DecList.GoodsBatch != model.GoodsBatch.Value)
                {
                    changeMessage += "报关员:" + byName + "把商品项" + ID + "[" + DecList.GoodsModel + "]批次号:" + DecList.GoodsBatch + "变更为:" + model.GoodsBatch + ";";
                }
                if (DecList.FirstUnit != model.FirstUnit.Value)
                {
                    changeMessage += "报关员:" + byName + "把商品项" + ID + "[" + DecList.GoodsModel + "]法一单位:" + DecList.FirstUnit + "变更为:" + model.FirstUnit + ";";
                }
                if (DecList.SecondUnit != model.SecondUnit.Value)
                {
                    changeMessage += "报关员:" + byName + "把商品项" + ID + "[" + DecList.GoodsModel + "]法二单位:" + DecList.SecondUnit + "变更为:" + model.SecondUnit + ";";
                }
                string elements = Convert.ToString(model.GModel).Replace("%26", "&").Replace("%2C", ",").Replace("%27", "\'").Replace("%999", "±").Replace("%22", "\"");
                if (DecList.GModel != elements)
                {
                    changeMessage += "报关员:" + byName + "把商品项" + ID + "[" + DecList.GoodsModel + "]申报要素:" + DecList.GModel + "变更为:" + elements + ";";
                }
                #endregion


                DecList.GNo = model.GNo;
                DecList.ContrItem = model.ContrItem == "" ? null : model.ContrItem;
                DecList.CodeTS = model.CodeTS;
                DecList.CiqCode = model.CiqCode;
                DecList.CiqName = model.CiqName;
                DecList.GName = model.GName;
                DecList.GModel = Convert.ToString(model.GModel).Replace("%26", "&").Replace("%2C", ",").Replace("%27", "\'").Replace("%999", "±").Replace("%22", "\"");
                DecList.GQty = model.GQty;
                DecList.GUnit = model.GUnit;
                DecList.DeclPrice = model.DeclPrice;
                DecList.DeclTotal = model.DeclTotal;
                DecList.TradeCurr = model.TradeCurr;
                DecList.FirstQty = model.FirstQty;
                DecList.FirstUnit = model.FirstUnit;
                DecList.OriginCountry = model.OriginCountry;
                DecList.SecondQty = model.SecondQty == "" ? null : model.SecondQty;
                DecList.SecondUnit = model.SecondUnit == "" ? null : model.SecondUnit;
                DecList.OrigPlaceCode = model.OrigPlaceCode == "" ? null : model.OrigPlaceCode;
                DecList.GoodsSpec = Convert.ToString(model.GoodsSpec).Replace("%26", "&").Replace("%2C", ",").Replace("%27", "\'").Replace("%22", "\"");
                if (Convert.ToString(model.GoodsAttr) != "")
                {
                    DecList.GoodsAttr = model.GoodsAttr;
                }
                if (Convert.ToString(model.Purpose) != "")
                {
                    DecList.Purpose = model.Purpose;
                }
                DecList.GoodsBrand = Convert.ToString(model.GoodsBrand).Replace("%26", "&").Replace("%2C", ",").Replace("%27", "\'").Replace("%22", "\"");
                DecList.GoodsModel = Convert.ToString(model.GoodsModel).Replace("%26", "&").Replace("%2C", ",").Replace("%27", "\'").Replace("%22", "\"");
                DecList.GoodsBatch = model.GoodsBatch;

                DecList.OldDecList = oldDecList;

                DecList.EnterError += DecHead_EnterError;
                DecList.EnterSuccess += DecHead_EnterSuccess;
                DecList.Enter();

                if (changeMessage != "")
                {
                    var head = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[DecList.DeclarationID];
                    var vendor = new Needs.Ccs.Services.VendorContext(Needs.Ccs.Services.VendorContextInitParam.Pointed, head.OwnerName).Current1;
                    //重新生成合同，装箱单，发票
                    head.PaymentInstructionSaveAs();
                    head.ContractSaveAs();
                    head.PackingListSaveAs(vendor);
                    //记录Trace
                    Needs.Ccs.Services.Models.DecTrace trace = new Needs.Ccs.Services.Models.DecTrace();
                    trace.DeclarationID = DecList.DeclarationID;
                    trace.Channel = head.CusDecStatus;
                    trace.Message = changeMessage.Replace("%26", "&").Replace("%2C", ",").Replace("%27", "\'").Replace("%999", "±").Replace("%22", "\""); ;
                    trace.NoticeDate = DateTime.Now;
                    trace.Enter();

                    DecHisUpdate his = new DecHisUpdate(DecList);
                    his.UpdataHistory();
                }
            }
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecHead_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = e.Message }).Json());
        }

        /// <summary>
        /// 保存成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecHead_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var newDecList = (Needs.Ccs.Services.Models.DecList)e.Object;

            #region 触发产品变更通知

            var oldDecList = newDecList.OldDecList;

            new Needs.Ccs.Services.Models.DecListTriggerProductChange(
                Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID),
                newDecList.OrderID,
                newDecList.OrderItemID,
                oldDecList,
                newDecList).DoTrigger();

            #endregion

            Response.Write((new { success = true, message = "保存成功", ID = newDecList.ID }).Json());
        }
    }
}
