using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp
{
    public partial class PcFiles : Page
    {
        protected Service.Entity.PcFile[] Files;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var files = new Service.Views.PcFilesView().
                    Search(item => item.MainID == Request.QueryString["id"]).
                    ToMyArray();

                this.Files = files;

            }
        }

        protected bool Delete()
        {
            var id = Request.Form["ID"];
            try
            {
                Service.Views.PcFilesView.Delete(id);
                Service.LogsOperating.LogOperating(Erp.Current, id, $"删除PcFiles:{id}");
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception("内部错误");
            }
        }

    }
}