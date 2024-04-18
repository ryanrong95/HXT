using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class ImportedList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Load_Data();
        }
        protected void Load_Data()
        {
            this.Model.CusReceiptCodeData = Needs.Ccs.Services.MultiEnumUtils.ToDictionary<Needs.Ccs.Services.Enums.CusDecStatus>()
                .Select(item => new { Value = item.Key, Text = item.Value }).Json();
        }
        protected void data()
        {
            string ContrNO = Request.QueryString["ContrNo"];
            string OrderID = Request.QueryString["OrderID"];
            string PreEntryId = Request.QueryString["PreEntryId"];
            string CusReceiptCode = Request.QueryString["BaseCusReceiptCode"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            var DecHead = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHeadImportedList.AsQueryable();


            if (!string.IsNullOrEmpty(ContrNO))
            {
                ContrNO = ContrNO.Trim();
                DecHead = DecHead.Where(t => t.ContrNo == ContrNO);
            }
            if (!string.IsNullOrEmpty(OrderID))
            {
                OrderID = OrderID.Trim();
                DecHead = DecHead.Where(t => t.OrderID == OrderID);
            }
            if (!string.IsNullOrEmpty(PreEntryId))
            {
                PreEntryId = PreEntryId.Trim();
                DecHead = DecHead.Where(t => t.PreEntryId == PreEntryId);
            }
            if (!string.IsNullOrEmpty(CusReceiptCode))
            {
                DecHead = DecHead.Where(t => t.CusDecStatus == CusReceiptCode);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                StartDate = StartDate.Trim();
                var from = DateTime.Parse(StartDate);
                DecHead = DecHead.Where(t => t.CreateTime >= from);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                EndDate = EndDate.Trim();
                var to = DateTime.Parse(EndDate).AddDays(1);
                DecHead = DecHead.Where(t => t.CreateTime <= to);
            }

            Func<Needs.Ccs.Services.Models.UploadDecHead, object> convert = head => new
            {
                ID = head.ID,
                ContrNO = head.ContrNo,
                OrderID = head.OrderID,
                BillNo = head.BillNo,
                EntryId = head.EntryId,
                PreEntryId = head.PreEntryId,
                AgentName = head.AgentName,
                IsInspection = head.IsInspection == true ? "是" : "否",
                CreateDate = head.CreateTime.ToShortDateString(),
                InputerID = head.InputerID,
                Status = head.CusDecStatus,
                StatusName = head.StatusName,
                IsDecHeadFile = head.IsDecHeadFile,
                URL = Needs.Utils.FileDirectory.Current.FileServerUrl + @"/" + head.decheadFile?.Url.ToUrl(),
            };
            this.Paging(DecHead, convert);
        }

        /// <summary>
        /// 批量转换舱单
        /// </summary>
        protected void Transform()
        {
            try
            {
                string ids = Request.Form["ID"];
                var headlist = ids.Split(',').ToList();

                headlist.ForEach(t => {
                    var head = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DecHeads[t];
                    head.ToManifest();
                });

                Response.Write((new { success = true, message = "转换成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "转换失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 暂用 不是实际流程
        /// </summary>
        protected void Succeed()
        {
            try
            {
                string ids = Request.Form["ID"];
                var headlist = ids.Split(',').ToList();

                headlist.ForEach(t => {
                    var head = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DecHeads[t];
                    head.DeclareSucceess();
                });

                Response.Write((new { success = true, message = "保存成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

    }
}