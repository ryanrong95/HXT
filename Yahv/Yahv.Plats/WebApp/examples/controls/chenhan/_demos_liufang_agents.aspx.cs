using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Forms;

namespace WebApp.examples.controls
{
    public partial class _demos_liufang_agents : ClientPage
    {

        public _demos_liufang_agents()
        {
            this.FileUploaded += _demos_chenhan_FileUploaded;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void _demos_chenhan_FileUploaded(object sender, FileUploaderEventArgs e)
        {
            ////自定义：保存地址
            //DirectoryInfo di = new DirectoryInfo(@"d:\_uploader_tester\");
            //if (!di.Exists)
            //{
            //    di.Create();
            //}

            //foreach (var file in e.Files)
            //{
            //    //可以自已定义保存名称，这里没有修改名称
            //    FileInfo fi = new FileInfo(Path.Combine(di.FullName, file.FileName));

            //    file.SaveAs(fi.FullName);
            //}

            //// 可以增加数据库操作
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Easyui.Alert("操作提示", "提交成功!<br>" + Request.Form["fileUploaderForJson"]);
        }
    }
}