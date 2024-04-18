using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.WebService
{
    public partial class SaveFileWebForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    //处理附件
                    HttpPostedFile file = Request.Files[0];
                    if (file.ContentLength != 0)
                    {
                        var FileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                        var decheads = new DecHeadsView().Where(item => item.ContrNo == FileName && item.CusDecStatus != "04");
                        if (decheads.Count() == 0)
                        {
                            Response.Write("Error");
                        }
                        //文件保存
                        string fileName = file.FileName;

                        //创建文件目录
                        FileDirectory fileDic = new FileDirectory(fileName);
                        fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.DecHead);
                        fileDic.CreateDataDirectory();
                        file.SaveAs(fileDic.FilePath);

                        var dechead = decheads.FirstOrDefault();
                        DecHeadFile decHeadFile = new DecHeadFile();
                        decHeadFile.DecHeadID = dechead.ID;
                        decHeadFile.AdminID = "Admin0000000282";
                        decHeadFile.Name = file.FileName;
                        decHeadFile.FileFormat = file.ContentType;
                        decHeadFile.Url = fileDic.VirtualPath;
                        decHeadFile.DecHead = dechead;

                        decHeadFile.FileType = Needs.Ccs.Services.Enums.FileType.DecHeadFile;

                        //持久化
                        decHeadFile.Enter();
                        Response.Write("Success");
                    }
                }
                catch(Exception ex)
                {
                    Response.Write("Error");
                }
            }
            else
            {
                Response.Write("Error");
            }
        }
    }
}