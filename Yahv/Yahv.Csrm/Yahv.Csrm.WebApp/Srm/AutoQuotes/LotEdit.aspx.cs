using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Utils.Serializers;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Srm.AutoQuotes
{
    public partial class LotEdit : BasePage
    {
        protected string SupplierID
        {
            get
            {
                return Request.QueryString["supplierid"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected object submit()
        {
            try
            {
                var supplierid = Request["supplierid"];
                var arry = Request["source"].Replace("&quot;", "\"").JsonTo<AutoQuote[]>();
                List<int> errorlines = new List<int>();
                List<AutoQuote> list = new List<AutoQuote>();

                for (int i = 0; i < arry.Length; i++)
                {
                    var model = arry[i];
                    if (string.IsNullOrWhiteSpace(model.Name) || list.Any(item => item.Name == model.Name))
                    {
                        errorlines.Add(i + 1);
                    }
                    list.Add(new AutoQuote
                    {
                        Name = model.Name,
                        SupplierID = supplierid,
                        Supplier = model.Supplier,
                        Manufacturer = model.Manufacturer,
                        DateCode = model.DateCode,
                        PackageCase = model.PackageCase,
                        Packaging = model.Packaging,
                        Prices = model.Prices,
                        UnitPrice = model.UnitPrice,
                        Quantity = model.Quantity,
                        ReporterID = Yahv.Erp.Current.ID,
                        Deadline = model.Deadline
                    });

                }
                if (errorlines.Count > 0)
                {
                    return new
                    {
                        codes = 600,
                        message = string.Join(",", errorlines)
                    };
                }
                if (list.Count > 0)
                {
                    list.Enter();
                }
                return new
                {
                    codes = 200,
                    message = "success"
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    codes = 400,
                    message = "异常错误：" + ex.Message
                };
            }
        }
    }
}