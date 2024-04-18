using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.AuxiliaryFunction
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            var consignees = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.ExpressKdds.AsQueryable();
            Func<Needs.Ccs.Services.Models.ExpressKdd, object> convert = consignee => new
            {
                consignee.ID,
                consignee.Receiver,
                consignee.ReveiveAddress,
                consignee.ReveiveMobile,
                ExpressCompany = consignee.ExpressCompany.Name,
                ExpressType = consignee.ExpressType.TypeName,
                PayType = consignee.PayType.GetDescription(),
                CreateDate= consignee.CreateDate.ToShortDateString()
            };

            this.Paging(consignees, convert);
        }

        protected void Delete()
        {
            string id = Request.Form["ID"];
            var entity = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.ExpressKdds[id];
            entity.Status = Needs.Ccs.Services.Enums.Status.Delete;
            if (entity != null)
            {
                entity.AbandonSuccess += Express_AbandonSuccess;
                entity.Abandon();
            }
        }

        /// <summary>
        /// 删除成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Express_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("删除成功!");
        }

    }
}