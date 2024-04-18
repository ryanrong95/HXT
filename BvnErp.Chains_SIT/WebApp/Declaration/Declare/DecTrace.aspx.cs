using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class DecTrace : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void data()
        {
            string ID = Request.QueryString["ID"];
            ID = ID.Trim();
            var head = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DecHeads[ID];

            Func<Needs.Ccs.Services.Models.DecTrace, object> convert = trace => new
            {
                ContrNO = head==null?"":head.ContrNo,
                Channel = trace.ChannelName,
                Message = trace.Message,
                NoticeDate = trace.NoticeDate.ToString("yyyy-MM-dd HH:mm:ss")
            };

            this.Paging(head.Traces, convert);
           
            //Response.Write(new
            //{
            //    rows = head.Traces.Select(convert).ToArray(),
            //    total = head.Traces.Count()
            //}.Json());
        }
    }
}