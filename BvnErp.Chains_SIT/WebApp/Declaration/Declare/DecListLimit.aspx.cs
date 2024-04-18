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
    public partial class DecListLimit : Uc.PageBase
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

            this.Model.BaseGoodsLimit = Needs.Wl.Admin.Plat.AdminPlat.GoodsLimits.Select(item => new { Value = item.Code, Text = item.Code + "-" + item.Name }).OrderBy(item => item.Value).Json();

            string ID = Request.QueryString["ID"];

            var DecList = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DecOriginList[ID];

            if (DecList != null)
            {
                this.Model.DecListInfo = new
                {
                    ID = DecList.ID,
                    GNo = DecList.GNo,
                    CodeTS = DecList.CodeTS,
                    CiqName = DecList.CiqName,
                    GName = DecList.GName
                }.Json();
            }
            else
            {
                this.Model.DecListInfo = new { }.Json();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void data()
        {
            string ID = Request.QueryString["ID"];

            var DecGoodsLimits = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DecGoodsLimits.AsQueryable().Where(t=>t.DecListID == ID);

            Func<Needs.Ccs.Services.Models.DecGoodsLimit, object> convert = limit => new
            {
                ID = limit.ID,
                DecListID = limit.DecListID,
                GoodsNo = limit.GoodsNo,
                LicTypeCode = limit.LicTypeCode,
                LicTypeName = limit.BaseGoodsLimit.Name,
                LicenceNo = limit.LicenceNo,
                LicWrtofDetailNo = limit.LicWrtofDetailNo,
                LicWrtofQty = limit.LicWrtofQty,
                LicWrtofQtyUnit = limit.LicWrtofQtyUnit,
                FileUrl = limit.FileUrl,

            };

            Response.Write(new
            {
                rows = DecGoodsLimits.Select(convert).ToArray(),
                total = DecGoodsLimits.Count()
            }.Json());
        }


        /// <summary>
        /// 保存许可证
        /// </summary>
        protected void SaveGoodsLimit()
        {
            try
            {
                HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;

                var ID = Request.Form["ID"];
                var GNo = Request.Form["GNo"];
                var BaseGoodsLimit = Request.Form["BaseGoodsLimit"];
                var LicenceNo = Request.Form["LicenceNo"];
                var LicWrtofDetailNo = Request.Form["LicWrtofDetailNo"];
                var LicWrtofQty = Request.Form["LicWrtofQty"];
                var LicWrtofQtyUnit = Request.Form["LicWrtofQtyUnit"];



                if (files.Count > 0)
                {
                    if (files.Count == 1 && files[0].ContentLength == 0)
                    {
                        Response.Write((new { success = false, message = "保存失败，文件为空" }).Json());
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
                        fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.DecHeadCCC);
                        fileDic.CreateDataDirectory();
                        file.SaveAs(fileDic.FilePath);

                        //持久化
                        var limit = new Needs.Ccs.Services.Models.DecGoodsLimit();

                        limit.DecListID = ID;
                        limit.GoodsNo = int.Parse(GNo);
                        limit.LicTypeCode = BaseGoodsLimit;
                        limit.LicenceNo = LicenceNo;
                        limit.LicWrtofDetailNo = LicWrtofDetailNo;
                        limit.LicWrtofQty = LicWrtofQty;
                        limit.LicWrtofQtyUnit = LicWrtofQtyUnit;
                        limit.FileUrl = fileDic.VirtualPath;
                        limit.Enter();
                    }

                    Response.Write((new { success = true, message = "保存成功" }).Json());
                }
                else
                {
                    Response.Write((new { success = false, message = "保存失败，文件为空" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败" + ex.Message }).Json());
            }
        }
    }
}