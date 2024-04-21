using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm_KQ.Application_Resignation
{
    public partial class Details : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitData();
            }
        }

        #region 加载数据
        /// <summary>
        /// 加载数据
        /// </summary>
        private void InitData()
        {
            string id = Request.QueryString["id"];
            var entity = JsonConvert.DeserializeObject<Resignation>(Erp.Current.Erm.Applications[id].Context);
            entity.ID = id;
            this.Model.Data = entity;
        }

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <returns></returns>
        protected object LoadFile()
        {
            string ApplicationID = Request.QueryString["id"];
            var files = new Services.Views.ApplicationFileAlls(ApplicationID).AsEnumerable();
            var linq = files.Select(t => new
            {
                ID = t.ID,
                CustomName = t.CustomName,
                FileTypeDec = ((FileType)t.Type).GetDescription(),
                CreateDate = t.CreateDate?.ToString("yyyy-MM-dd"),
                Creater = t.AdminID,
                Url = FileDirectory.ServiceRoot + t.Url,
            });
            return linq;
        }

        /// <summary>
        /// 审批日志
        /// </summary>
        /// <returns></returns>
        protected object LoadLogs()
        {
            var id = Request.QueryString["ID"];
            var logs = Erp.Current.Erm.Logs_ApplyVoteSteps.Where(item => item.ApplicationID == id);

            var data = logs.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                AdminID = item.Admin.RealName,
                Status = item.Status.GetDescription(),
                Summary = item.Summary,
                item.VoteStepName,
            });
            return new
            {
                rows = data,
                total = data.Count(),
            };
        }
        #endregion
    }
}