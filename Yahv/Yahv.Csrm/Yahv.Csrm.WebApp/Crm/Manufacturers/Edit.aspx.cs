using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Crm.Manufacturers
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 列名
        /// </summary>
        /// <returns></returns>
        protected string GetColNames()
        {
            List<string> list = new List<string>();

            Type type = typeof(YaHv.Csrm.Services.Models.Origins.ViewManufacturer);
            foreach (var property in type.GetProperties())
            {
                string str = property.GetDescription();
                if (string.IsNullOrWhiteSpace(str))
                {
                    str = property.Name;
                }

                if (property.Name == "Name")
                {
                    str = str + "<font style='color: red; '>*</font>";
                }

                list.Add(str);
            }

            return list.Json();
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