using Needs.Erp;
using Needs.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Bom
{
    public partial class List : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            Expression<Func<NtErp.Wss.Services.Models.Boms, bool>> expression = null;

            using (var context = new Needs.Linq.LinqContext())
            {
                IQueryable<NtErp.Wss.Services.Models.Boms> data = ErpPlot.Current.Publishs.MyBoms;

                if (expression != null)
                {
                    data = data.Where(expression);
                }
                Response.Paging(data, item => new
                {
                    item.ID,
                    item.CreateDate,                                         
                    item.Contact,
                    item.Email,
                    item.Uri
                });

                Needs.Linq.LinqContext.Current.Dispose();
            }

        }
        protected void del()
        {
            string id = Request.Form["id"];
            var entity = ErpPlot.Current.Publishs.MyBoms[id];
            if (entity == null)
            {
                throw new Exception("data is not exist!");
            }
            entity.AbandonSuccess += EnterSuccess;
            entity.Abandon();
        }
        private void EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            base.Alert("删除成功", this.Request.Url);
        }
    
    }
}
