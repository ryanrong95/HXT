using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Utils.Serializers;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Srm.Advantages.Manufacturers
{
    public partial class Edit : BasePage
    {
        protected string SupplierID
        {
            get
            {
                return Request.QueryString["id"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected object submit()
        {
            try
            {
                var arry = Request["source"].Replace("&quot;", "\"").JsonTo<ViewManufacturer[]>();
                List<int> errorlines = new List<int>();
                List<ViewManufacturer> list = new List<ViewManufacturer>();

                for (int i = 0; i < arry.Length; i++)
                {
                    var model = arry[i];
                    if (string.IsNullOrWhiteSpace(model.Name) || list.Any(item => item.Name == model.Name))
                    {
                        errorlines.Add(i + 1);
                    }
                    list.Add(new ViewManufacturer
                    {
                        Name = model.Name,
                        Agent = model.Agent
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
                    var supplier = new SuppliersRoll()[Request["supplierid"]];
                    var advantage = supplier.Advantage ?? new Advantage();
                    advantage.Enterprise = supplier.Enterprise;
                    advantage.Manufacturers = list.ToArray().Json();
                    advantage.Enter();
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