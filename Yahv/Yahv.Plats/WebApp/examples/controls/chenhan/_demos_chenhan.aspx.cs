using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Forms;

namespace WebApp.examples.controls
{
    public partial class _demos_chenhan : ClientPage
    {

        public _demos_chenhan()
        {
            this.FileUploaded += _demos_chenhan_FileUploaded;
        }

        private void _demos_chenhan_FileUploaded(object sender, FileUploaderEventArgs e)
        {
            //自定义：保存地址
            DirectoryInfo di = new DirectoryInfo(@"d:\_uploader_tester\");
            if (!di.Exists)
            {
                di.Create();
            }

            foreach (var file in e.Files)
            {
                //可以自已定义保存名称，这里没有修改名称
                FileInfo fi = new FileInfo(Path.Combine(di.FullName, file.FileName));

                file.SaveAs(fi.FullName);
            }

            // 可以增加数据库操作
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            //for (int index = 0; index < Request.Files.Count; index++)
            //{
            //    HttpPostedFile file = Request.Files[index];
            //    //file.SaveAs("c:\\inetpub\\test\\UploadedFiles\\" + file.FileName);
            //    Console.WriteLine(file.FileName);
            //}


        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            foreach (string key in Request.Form)
            {
                Console.WriteLine($"{key}:{Request.Form[key]}");
            }
        }
    }
}