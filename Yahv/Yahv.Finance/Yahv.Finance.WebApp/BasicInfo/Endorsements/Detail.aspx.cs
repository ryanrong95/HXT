using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.BasicInfo.Endorsements
{
    public partial class Detail : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitData();
            }
        }

        #region 加载数据
        public void InitData()
        {
            string id = Request.QueryString["id"];
            if (!string.IsNullOrEmpty(id))
            {
                this.Model.Data = new EndorsementsRoll()[id];
            }
        }

        #region 加载附件
        /// <summary>
        /// 加载附件
        /// </summary>
        protected void filedata()
        {
            string id = Request.QueryString["id"];

            using (var query1 = new Yahv.Finance.Services.Views.Rolls.FilesDescriptionRoll())
            {
                var view = query1;

                view = view.SearchByFilesMapValue(FilesMapName.EndorsementID.ToString(), id);

                var files = view.ToArray();

                string fileWebUrlPrefix = ConfigurationManager.AppSettings["FileWebUrlPrefix"];

                Func<FilesDescription, object> convert = item => new
                {
                    FileID = item.ID,
                    CustomName = item.CustomName,
                    FileFormat = "",
                    Url = item.Url,    //数据库相对路径
                    WebUrl = string.Concat(fileWebUrlPrefix, "/" + item.Url),
                };
                Response.Write(new
                {
                    rows = files.Select(convert).ToArray(),
                    total = files.Count(),
                }.Json());
            }
        }

        #endregion
        #endregion

    }
}