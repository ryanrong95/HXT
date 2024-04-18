using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class DecOtherPack : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        protected void data()
        {
            var PackType = Needs.Wl.Admin.Plat.AdminPlat.BasePackType.OrderBy(item => item.Code);
            Func<Needs.Ccs.Services.Models.BasePackType, object> convert = item => new
            {
                PackType = item.Code,
                Name = item.Name,
            };

            Response.Write(new
            {
                rows = PackType.Select(convert).ToArray(),
                total = PackType.Count()
            }.Json());
        }
    }
}