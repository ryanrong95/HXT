using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.ProjectManagement
{
    public partial class ImportList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region Excel导入

        protected void Import()
        {
            HttpFileCollection files = Request.Files;
            var file = files[0];

            #region 文件保存到本地
            string ext = Path.GetExtension(file.FileName);
            var fileName = file.FileName.Replace(ext, DateTime.Now.ToString("-yyyyMMddHHmmssfff") + ext);
            string path = Path.Combine(Server.MapPath("~/UploadFiles/"), fileName);
            file.SaveAs(path);
            #endregion
            Response.Write(fileName);
        }
        #endregion
    }
}