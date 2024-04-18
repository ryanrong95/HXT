using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Flow.Process;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.NoticeBoard
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        protected void LoadData()
        {
            this.Model.NoticeBoard = "".Json();
            var adminRole = new Needs.Ccs.Services.Views.NoticeBoardAdminRolesView().Select(item => new { item.RoleID, item.RoleName }).ToArray();
            var adminRole2 = adminRole.Select(t2 => new{ RoleID = "ALL", RoleName = "全部" }).ToArray();
            this.Model.AdminRole = adminRole2.Union(adminRole).Json();
            var id = Request.QueryString["ID"];
            var form = Request.QueryString["Form"];
            if (!string.IsNullOrEmpty(id))
            {
                var noticeBoard = new Needs.Ccs.Services.Views.NoticeBoardView().FirstOrDefault(item => item.ID == id);
                this.Model.NoticeBoard = new
                {
                    ID = id,
                    NoticeTitle = noticeBoard.NoticeTitle,
                    NoticeContent = noticeBoard.NoticeContent,
                    RoleID = noticeBoard.RoleID,
                    RoleName = noticeBoard.RoleName,
                    Form = form

                }.Json();

            }
        }
        protected void uploadImages()
        {
            try
            {
                List<dynamic> fileList = new List<dynamic>();
                HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                if (files.Count > 0)
                {
                    //处理附件
                    HttpPostedFile file = files[0];
                    if (file.ContentLength != 0)
                    {
                        //文件保存
                        string fileName = file.FileName.ReName();

                        //创建文件目录
                        FileDirectory fileDic = new FileDirectory(fileName);
                        fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.NoticeBoard);
                        fileDic.CreateDataDirectory();
                        file.SaveAs(fileDic.FilePath);
                        //{ "originalName", "name", "url", "size", "state", "type" };
                        var result = new
                        {
                            originalName = file.FileName,
                            name = fileName,
                            url = fileDic.VirtualPath,
                            size = file.ContentLength,
                            state = "SUCCESS",
                            type = Path.GetExtension(file.FileName)
                        }.Json();
                        string callback = Context.Request["callback"] ?? "jsonpCallback";
                        //Response.Write(String.Format("<script>{0}(JSON.parse(\'{1}\'));</script>", callback, result));
                        Response.Write(result);
                    }
                }

            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, data = ex.Message }).Json());
            }
        }
        protected void Save()
        {
            try
            {
                string noticeTitle = Request.Form["NoticeTitle"].Replace("&amp;", "&");
                string noticeContent = Request.Form["NoticeContent"];
                string roleID = Request.Form["RoleID"];
                string roleName = Request.Form["RoleName"];
                NoticeBoardModel noticeBoard = new NoticeBoardModel();
                noticeBoard.ID = Guid.NewGuid().ToString("N");
                noticeBoard.NoticeTitle = noticeTitle;
                noticeBoard.NoticeContent = noticeContent;
                noticeBoard.CreateDate = DateTime.Now;
                noticeBoard.RoleID = roleID;
                noticeBoard.RoleName = roleName;
                noticeBoard.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                noticeBoard.Status = Needs.Ccs.Services.Enums.Status.Normal;
                noticeBoard.Enter();
                Response.Write((new { success = true, message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new
                {
                    success = false,
                    message = "提交失败：" + ex.Message
                }).Json());
            }
        }
    }
}