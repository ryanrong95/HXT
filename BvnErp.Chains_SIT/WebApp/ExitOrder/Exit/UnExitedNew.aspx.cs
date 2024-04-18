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
using WebApp.App_Utils;

namespace WebApp.ExitOrder.Exit
{
    /// <summary>
    /// 出库通知-出库界面
    /// 深圳库房
    /// </summary>
    public partial class UnExitedNew : Uc.PageBase
    {
        string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["PvDataFileUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            //出库类型
            this.Model.ExitType = EnumUtils.ToDictionary<Needs.Ccs.Services.Models.WaybillType>()
                .Select(item => new { item.Key, item.Value }).Json();
        }

        //未出库通知列表
        protected void data()
        {
            //查询条件
            string OrderID = Request.QueryString["OrderID"];
            string CustomerCode = Request.QueryString["EntryNumber"];
            string ExitType = Request.QueryString["ExitType"];
            //查询通知列表
            var exitNotices = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.MyNewCenterSZExitNotice.AsQueryable();


            if (!string.IsNullOrEmpty(OrderID))
            {
                exitNotices = exitNotices.Where(x => x.OrderID ==OrderID);
            }
            if (!string.IsNullOrEmpty(ExitType))
            {
                exitNotices = exitNotices.Where(x => x.WayBillType == (WaybillType)int.Parse(ExitType));
            }
            if (!string.IsNullOrEmpty(CustomerCode))
            {
                exitNotices = exitNotices.Where(x => x.ClientCode==CustomerCode);
            }

            exitNotices = exitNotices.OrderByDescending(x => x.CreateDate);

            //返回数据
            Func<DeliveryOrder, object> convert = item => new
            {
                ID = item.ID,
                OrderID = item.OrderID,//订单编号
                ClientCode = item.ClientCode,              
                ClientName =item.CompanyName,//客户
                PackNo = item.Quantity,
                AdminName = item.Admin.RealName,//制单人
                ExitType = item.WayBillType.GetDescription(),
                CreateDate = item.CreateDate,
                //NoticeStatus = item.CenterExeStatus.GetDescription(),
                IsModify = item.IsModify,
                Url = FileServerUrl + "/" + item.Url?.ToUrl(),
            };
            this.Paging(exitNotices, convert);
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
                var NO = Request.Form["NO"];
                if (files.Count > 0 && !string.IsNullOrEmpty(NO))
                {
                    if (files.Count == 1 && files[0].ContentLength == 0)
                    {
                        Response.Write((new { success = false, message = "上传失败，文件为空" }).Json());
                        return;
                    }

                    var sss = int.Parse(NO);
                    //处理附件
                    HttpPostedFile file = files[sss];
                    if (file.ContentLength != 0)
                    {
                      
                        string fileName = file.FileName.ReName();
                        HttpFile httpFile = new HttpFile(fileName);
                        httpFile.SetChildFolder(Needs.Ccs.Services.SysConfig.Client);
                        httpFile.CreateDataDirectory();
                        string[] result = httpFile.SaveAs(file);

                        //文件修改为上传中心
                        var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.DeliverGoods;
                        var ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                        var dic = new { CustomName = fileName, WaybillID = ID, AdminID = ErmAdminID };
                        var uploadFile = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(FileDirectory.Current.FilePath + @"\" + httpFile.VirtualPath, centerType, dic);

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