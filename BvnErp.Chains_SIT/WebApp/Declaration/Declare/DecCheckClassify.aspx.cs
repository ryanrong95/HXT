using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
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
    public partial class DecCheckClassify : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadFiles();
            }
        }

        protected void LoadFiles()
        {
            string OrderID = Request.QueryString["OrderID"];
            this.Model.OrderID = OrderID;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string OrderID = Request.QueryString["OrderID"];

            var classitems = new Needs.Ccs.Services.Views.DecListsClassifyView().Where(x => x.OrderID == OrderID).OrderBy(t=>t.GNo);
           

            Func<DecListsClassify, object> convert = item => new
            {
                GNo = item.GNo,
                OrderID = item.OrderID,
                HsCode = item.HsCode,
                Name = item.Name,
                Elements = item.Elements,
                StandardElements = item.StandardElements,
                Operate1 = item.Operate1,
                Operate2 = item.Operate2

            };

            this.Paging(classitems, convert);
        }

    }
}
