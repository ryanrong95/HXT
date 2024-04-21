using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Api
{
    /// <summary>
    /// 审批流
    /// </summary>
    public partial class VoteFlows : ClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 初始化审批流基础数据
        /// </summary>
        /// <param name="id">审批流id</param>
        protected new string Init()
        {
            string id = Request.Form["id"];
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                Services.VoteFlowManager.Current.Init();
            });

            return "ok";
        }
    }
}