using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class UnUploadList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                load();
            }
        }

        private void load()
        {
            this.Model.Status = Needs.Ccs.Services.MultiEnumUtils.ToDictionary<Needs.Ccs.Services.Enums.CusDecStatus>()
                .Select(item => new { Value = item.Key, Text = item.Value }).Json();
        }

        protected void data()
        {
            string ContrNo = Request.QueryString["ContrNo"];
            string OrderID = Request.QueryString["OrderID"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];


            var UnUploadDecHeads = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.UnUploadDecHeadsList.OrderByDescending(item => item.DDate).AsQueryable();

            if (!string.IsNullOrEmpty(ContrNo))
            {
                ContrNo = ContrNo.Trim();
                UnUploadDecHeads = UnUploadDecHeads.Where(t => t.ContrNo.Contains(ContrNo));
            }
            if (!string.IsNullOrEmpty(OrderID))
            {
                OrderID = OrderID.Trim();
                UnUploadDecHeads = UnUploadDecHeads.Where(t => t.OrderID.Contains(OrderID));
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                DateTime start = Convert.ToDateTime(StartDate);
                UnUploadDecHeads = UnUploadDecHeads.Where(t => t.DDate >= start);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                UnUploadDecHeads = UnUploadDecHeads.Where(t => t.DDate < end);
            }

            var UnUploadDecHead = UnUploadDecHeads.ToList();
            UnUploadDecHead = UnUploadDecHead.Where(t => t.IsSuccess && t.IsDecHeadFile == "否").ToList();

            Func<Needs.Ccs.Services.Models.UploadDecHead, object> convert = head => new
            {
                ID = head.ID,
                ContrNo = head.ContrNo,
                OrderID = head.OrderID,
                EntryID = head.EntryId,
                Currency = head.Currency,
                SwapAmount = head.DecAmount,
                DDate = head.DDate?.ToString("yyyy-MM-dd"),
                SwapStatus = head.SwapStatus.GetDescription(),
                Status = head.StatusName,
                IsDecHeadFile = head.IsDecHeadFile,
                URL = Needs.Utils.FileDirectory.Current.FileServerUrl + @"/" + head.decheadFile?.Url.ToUrl(),
            };
            this.Paging(UnUploadDecHead, convert);
        }

        /// <summary>
        /// 上传报关单文件
        /// </summary>
        protected void UploadFile()
        {
            try
            {
                HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                if (files.Count > 0)
                {
                    if (files.Count == 1 && files[0].ContentLength == 0)
                    {
                        Response.Write((new { success = false, message = "上传失败，文件为空" }).Json());
                        return;
                    }

                    for (int i = 0; i < files.Count; i++)
                    {
                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            var FileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                            var decheads = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead.Where(item => item.ContrNo == FileName && item.CusDecStatus != "04");
                            if (decheads.Count() == 0)
                            {
                                throw new Exception("文件：" + FileName + "找不到对应的报关单");
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
                            decHeadFile.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                            decHeadFile.Name = file.FileName;
                            decHeadFile.FileFormat = file.ContentType;
                            decHeadFile.Url = fileDic.VirtualPath;
                            decHeadFile.DecHead = dechead;

                            decHeadFile.FileType = Needs.Ccs.Services.Enums.FileType.DecHeadFile;

                            //持久化
                            decHeadFile.Enter();
                        }
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