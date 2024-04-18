using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.PvCustom_ConsultClassify
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Save()
        {
            try
            {

                Response.Write((new { success = false, message = "保存失败，此功能已停用，请至新比一比后台新增咨询" }).Json());


                //var Model = Request.Form["Model"].Trim();
                //var Brand = Request.Form["Brand"].Trim();
                //var Qty = Request.Form["Qty"].Trim();
                //var UnitPrice = Request.Form["UnitPrice"].Trim();
                //var Currency = Request.Form["Currency"].Trim();
                //var ProductUniqueCode = Request.Form["ProductUniqueCode"].Trim();
                //if (string.IsNullOrEmpty(ProductUniqueCode))
                //{
                //    TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                //    ProductUniqueCode = Convert.ToInt64(ts.TotalSeconds).ToString();
                //}

                //PreProduct pre = new PreProduct();
                //pre.UseType = Needs.Ccs.Services.Enums.PreProductUserType.Consult;
                //pre.Model = Model;
                //pre.Manufacturer = Brand;
                //pre.Qty = Convert.ToDecimal(Qty);
                //pre.Price = Convert.ToDecimal(UnitPrice);
                //pre.Currency = Currency;
                //pre.ProductUnionCode = ProductUniqueCode;
                //pre.ClientID = System.Configuration.ConfigurationManager.AppSettings["XL037ID"];
                //pre.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                //pre.CompanyType = Needs.Ccs.Services.Enums.CompanyTypeEnums.Inside;
                //pre.Enter();

                //Response.Write((new { success = true, message = "保存成功"}).Json());
            }
            catch(Exception ex)
            {
                Response.Write((new { success = false, message = ex.ToString() }).Json());
            }
            

        }
    }
}