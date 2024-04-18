using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;

namespace WebApp.HKWarehouse.Temporary
{
    /// <summary>
    /// 暂存新增或编辑操作
    /// </summary>
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        protected void LoadData()
        {
            this.Model.Temporary = "".Json();
            string id = Request.QueryString["ID"];
            if (!string.IsNullOrEmpty(id))
            {
                var temporaryView = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.Temporary;
                var temporary = temporaryView.Where(item => item.ID == id).FirstOrDefault();
                if (temporary != null)
                {
                    this.Model.Temporary = temporary.Json();
                }
            }

            //包装种类
            this.Model.WrapTypeData = Needs.Wl.Admin.Plat.AdminPlat.BasePackType.Select(item => new
            {
                item.ID,
                item.Code,
                item.Name
            }).OrderBy(x => x.Code).Json();

            this.Model.Clients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Select(c => new { Value = c.ClientCode, Text = c.ClientCode+"-"+c.Company.Name }).Json();
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string id = Request.QueryString["ID"];

            var files = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.TemporaryFile.AsQueryable();
            files = files.Where(t => t.TemporaryID == id);

            Func<TemporaryFile, object> convert = file => new
            {
                ID = file.ID,
                Name = file.Name,
                URL = file.URL,
                WebUrl = FileDirectory.Current.FileServerUrl + "/" + file.URL.ToUrl(),//查看路径
                FileFormat = file.FileFormat,
                CreateDate = file.CreateDate.ToString(),
            };

            Response.Write(new
            {
                rows = files.Select(convert).ToArray(),
                total = files.Count()
            }.Json());
        }

        /// <summary>
        /// 上传费用附件
        /// </summary>
        protected void UploadFile()
        {
            try
            {
                List<dynamic> fileList = new List<dynamic>();
                HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            //文件保存
                            string fileName = files[i].FileName.ReName();

                            //创建文件目录
                            FileDirectory fileDic = new FileDirectory(fileName);
                            fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Warehouse);
                            fileDic.CreateDataDirectory();
                            file.SaveAs(fileDic.FilePath);
                            fileList.Add(new
                            {
                                Name = file.FileName,
                                FileFormat = file.ContentType,
                                WebUrl = fileDic.FileUrl,
                                Url = fileDic.VirtualPath,
                                CreateDate = DateTime.Now.ToString()
                            });
                        }
                    }
                }
                Response.Write((new { success = true, data = fileList }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, data = ex.Message }).Json());
            }
        }

        /// <summary>
        /// 保存暂存记录
        /// </summary>
        protected void Save()
        {
            string ID = Request.Form["ID"];
            string fileData = Request.Form["FileData"].Replace("&quot;", "'");
            string EntryNumber = Request.Form["EntryNumber"];
            string CompanyName = Request.Form["CompanyName"];
            string ShelveNumber = Request.Form["ShelveNumber"];
            string EntryDate = Request.Form["EntryDate"];
            string WaybillCode = Request.Form["WaybillCode"];
            string PackNo = Request.Form["PackNo"];
            string WrapType = Request.Form["WrapType"];
            string Summary = Request.Form["Summary"];
            try
            {
                IEnumerable<TemporaryFile> files = fileData.JsonTo<IEnumerable<TemporaryFile>>();
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                Needs.Ccs.Services.Models.Temporary temporary = new Needs.Ccs.Services.Models.Temporary();

                if (string.IsNullOrEmpty(ID) == false)
                {
                    temporary = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.Temporary.Where(item => item.ID == ID).FirstOrDefault();
                }
                else
                {
                    temporary.Admin = admin;
                }
                temporary.EntryNumber = EntryNumber;
                temporary.EntryDate = Convert.ToDateTime(EntryDate);
                temporary.CompanyName = CompanyName;
                temporary.ShelveNumber = ShelveNumber;
                temporary.WaybillCode = WaybillCode;
                temporary.PackNo = int.Parse(PackNo);
                temporary.WrapType = (Needs.Ccs.Services.Enums.BaseWrapType)int.Parse(WrapType);
                temporary.Summary = Summary;
                temporary.Files = new TemporaryFiles(files);
                temporary.SetOperator(admin);
                temporary.Enter();
                Response.Write((new { success = true, message = "保存成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        protected void LoadLogs()
        {
            string ID = Request.Form["ID"];
            var logs = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.TemporaryLog.Where(t => t.TemporaryID == ID);
            logs = logs.OrderByDescending(t => t.CreateDate);

            Func<TemporaryLog, object> convert = item => new
            {
                ID = item.ID,
                Summary = item.Summary,
                Type = item.TemporaryStatus.GetDescription(),
                Operator = item.Admin.RealName,
                CreateDate = item.CreateDate.ToString(),
            };
            Response.Write(new
            {
                rows = logs.Select(convert).ToArray()
            }.Json());
        }
    }
}