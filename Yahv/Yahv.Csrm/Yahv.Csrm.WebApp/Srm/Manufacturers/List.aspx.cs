using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Srm.Manufacturers
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var entity = new ManufacturersRoll();

                if (entity.Count() > 0)
                {
                    this.Model = entity.OrderBy(item => item.Name).Select(item => new
                    {
                        item.ID,
                        item.Name,
                        Agent = item.Agent
                    });
                }
                else
                {
                    this.Model = new[] {
                        new
                        {
                            Name="",
                            Agent=false
                        }
                    };
                }
            }
        }


        protected object submit()
        {
            try
            {
                var arry = Request["source"].Replace("&quot;", "\"").JsonTo<ViewManufacturer[]>();
                List<int> errorlines = new List<int>();
                List<Manufacturer> list = new List<Manufacturer>();

                for (int i = 0; i < arry.Length; i++)
                {
                    var model = arry[i];
                    if (string.IsNullOrWhiteSpace(model.Name) || list.Any(item => item.Name == model.Name))
                    {
                        errorlines.Add(i + 1);
                    }
                    list.Add(new Manufacturer
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