using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class DecRequestCert : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        protected void data()
        {
            string DeclarationID = Request.QueryString["ID"];
            var data = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareLicenseDocus.Where(item => item.DeclarationID == DeclarationID).AsQueryable(); ;

            Func<Needs.Ccs.Services.Models.DecLicenseDocu, object> convert = declareLicense => new
            {
                ID = declareLicense.ID,
                DocuCode = declareLicense.DocuCodeCertify==null? declareLicense.DocuCode : declareLicense.DocuCodeCertify.Code + "-"+ declareLicense.DocuCodeCertify.Name,
                CertCode = declareLicense.CertCode,
                FileUrl = string.IsNullOrEmpty(declareLicense.FileUrl) ? "" : (FileDirectory.Current.FileServerUrl + @"/" + declareLicense.FileUrl.Replace(@"\", @"/")),
            };
            this.Paging(data, convert);
        }

        protected void Delete()
        {
            string ID = Request.Form["ID"];
            Needs.Ccs.Services.Models.DecLicenseDocu declicense = new Needs.Ccs.Services.Models.DecLicenseDocu();
            declicense.ID = ID;
            declicense.EnterError += DecHead_EnterError;
            declicense.EnterSuccess += DecHead_EnterSuccess;
            declicense.PhysicalDelete();
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecHead_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecHead_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        protected void UploadFile()
        {
            try
            {
                HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                var ID = Request.Form["ID"];
                if (files.Count > 0)
                {
                    if (files.Count == 1 && files[0].ContentLength == 0)
                    {
                        Response.Write((new { success = false, message = "上传失败，文件为空" }).Json());
                        return;
                    }


                    //处理附件
                    HttpPostedFile file = files[0];
                    if (file.ContentLength != 0)
                    {
                        var FileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                       
                        //文件保存
                        string fileName = file.FileName;

                        //创建文件目录
                        FileDirectory fileDic = new FileDirectory(fileName);
                        fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.DecHead);
                        fileDic.CreateDataDirectory();
                        file.SaveAs(fileDic.FilePath);

                        var docu = new Needs.Ccs.Services.Views.DecLicenseDocusView().FirstOrDefault(t=>t.ID == ID);
                        docu.FileUrl = fileDic.VirtualPath;

                        //持久化
                        docu.Enter();
                    }

                    Response.Write((new { success = true, message = "上传成功" }).Json());
                }
                else
                {
                    Response.Write((new { success = false, message = "上传失败，文件为空" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "上传失败" + ex.Message }).Json());
            }
        }
    }
}