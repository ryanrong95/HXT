using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.Erm.Services.Extends;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm.Advantages.PartNumbers
{
    public partial class List : ErpParticlePage
    {
        protected string AdminID
        {
            get
            {
                return Request.QueryString["id"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var entity = Alls.Current.Admins[AdminID];

                var emp = new[] {
                        new
                        {
                            Name="",
                            Manufacturer=""
                        }
                    };
                if (entity == null || entity.Advantage == null)
                {
                    this.Model = emp;
                }
                else if (entity.Advantage.PartNumbers == null)
                {
                    this.Model = emp;
                }
                else
                {
                    this.Model = entity.Advantage.PartNumbers.ToPartnnumberView().OrderBy(item => item.Name).Select(item => new
                    {
                        item.Name,
                        item.Manufacturer
                    });
                }
            }
        }

        protected object submit()
        {
            try
            {
                var arry = Request["source"].Replace("&quot;", "\"").JsonTo<PartNumber[]>();
                List<int> errorlines = new List<int>();
                List<PartNumber> list = new List<PartNumber>();

                for (int i = 0; i < arry.Length; i++)
                {
                    var model = arry[i];
                    if (string.IsNullOrWhiteSpace(model.Name) || list.Any(item => item.Name == model.Name && item.Manufacturer == model.Manufacturer))
                    {
                        errorlines.Add(i + 1);
                    }
                    list.Add(new PartNumber
                    {
                        Name = model.Name,
                        Manufacturer = model.Manufacturer
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
                    var admin = Alls.Current.Admins[Request["adminid"]];
                    var advantage = admin.Advantage ?? new Advantage();
                    advantage.Admin = admin;
                    advantage.PartNumbers = list.ToArray().Json();
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