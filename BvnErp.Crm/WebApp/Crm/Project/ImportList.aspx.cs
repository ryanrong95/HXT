using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Utils.Descriptions;
using NtErp.Crm.Services;
using NtErp.Crm.Services.Enums;
using System.IO;

namespace WebApp.Crm.Project
{
    public partial class ImportList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }


        #region Excel导入
        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Import()
        {
            //获取当前导入的文件
            HttpFileCollection files = Request.Files;
            var file = files[0];

            #region 文件保存到本地
            string ext = Path.GetExtension(file.FileName); //获取拓展名
            var filename = file.FileName.Replace(ext, DateTime.Now.ToString("-yyyyMMddHHmmssfff") + ext); //重命名
            //拼凑出文件保存路径
            string path = Path.Combine(Server.MapPath("~/UploadFiles/"), filename);

            file.SaveAs(path);
            #endregion

            Response.Write(filename);
        }
        #endregion
    }
}