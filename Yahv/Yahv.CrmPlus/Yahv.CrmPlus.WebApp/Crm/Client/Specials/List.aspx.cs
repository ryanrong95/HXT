using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Specials
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.ID = Request.QueryString["id"];
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        protected object data()
        {
            string enterpriseid = Request.QueryString["id"];
            var query = new RequiremensRoll().Where(x => x.EnterpriseID == enterpriseid);
            var filelist = new Yahv.CrmPlus.Service.Views.Rolls.FilesDescriptionRoll()[enterpriseid, CrmFileType.Requirements].ToArray();
            return this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.Status,
                StatusDes = item.Status.GetDescription(),
                item.EnterpriseID,
                SpecialType = item.SpecialType.GetDescription(),
                item.Content,
                CreteDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.ModifyDate,
                Creator = item.Admin.RealName,
                FileName = filelist.FirstOrDefault(x => x.SubID == item.ID)?.CustomName,
                Url = filelist.FirstOrDefault(x => x.SubID == item.ID)?.Url
            }));
        }



        /// <summary>
        /// 停用
        /// </summary>
        protected void Closed()
        {
            var id = Request.Form["ID"];
            try
            {

                var entity =new Yahv.CrmPlus.Service.Views.Rolls.RequiremensRoll()[id];
                entity.Closed();
                LogsOperating.LogOperating(Erp.Current, entity.EnterpriseID, $"特殊要求:{ entity.ID}");

            }
            catch (Exception ex)
            {
                LogsOperating.LogOperating(Erp.Current, id, $"启用特殊要求 操作失败" + ex);
            }
        }


        /// <summary>
        /// 启用
        /// </summary>
        protected void Enable()
        {
            var id = Request.Form["ID"];
            try
            {
                var entity = new Yahv.CrmPlus.Service.Views.Rolls.RequiremensRoll()[id];
                entity.Enable();
                LogsOperating.LogOperating(Erp.Current, entity.EnterpriseID, $"特殊要求:{ entity.ID}");
            }
            catch (Exception ex)
            {
                LogsOperating.LogOperating(Erp.Current, id, $"启用特殊要求 操作失败" + ex);
            }
        }


    }
}