using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.Staffs.Detail
{
    public partial class FileDetails : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }

        protected object data()
        {
            string StaffID = Request.QueryString["ID"];
            var files = new Services.Views.StaffFileAlls(StaffID).AsEnumerable();
            var linq = files.Select(t => new
            {
                ID = t.ID,
                CustomName = t.CustomName,
                FileType = ((FileType)Enum.Parse(typeof(FileType), t.Type.ToString())).GetDescription(),
                CreateDate = t.CreateDate?.ToString("yyyy-MM-dd"),
                Creater = t.AdminID,
                Url = FileDirectory.ServiceRoot + t.Url,
            });
            return linq;
        }
    }
}