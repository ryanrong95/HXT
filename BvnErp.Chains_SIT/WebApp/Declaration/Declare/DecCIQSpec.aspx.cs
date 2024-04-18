using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class DecCIQSpec : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
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
                    GModel = DecList.GModel,
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
                    GoodsSpec = DecList.GoodsSpec == null ? "" : DecList.GoodsSpec.Replace("\'", "%27"),
                    GoodsAttr = DecList.GoodsAttr,
                    Purpose = DecList.Purpose,
                    GoodsBrand = DecList.GoodsBrand,
                    GoodsModel = DecList.GoodsModel,
                    GoodsBatch = DecList.GoodsBatch
                }.Json();
            }
            else
            {
                this.Model.DecList = new { }.Json();
            }
        }
    }
}