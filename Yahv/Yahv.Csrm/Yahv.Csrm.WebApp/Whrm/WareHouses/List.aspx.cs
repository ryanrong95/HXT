using System;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Whrm.WareHouses
{
    public partial class List : ClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected object data()
        {
            Expression<Func<WareHouse, bool>> predicate = item => true;
            var name = Request["s_name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Enterprise.Name.Contains(name)|| item.DyjCode.Contains(name));
            }
            var query = Erp.Current.Crm.WareHouses.Where(predicate);
            return new
            {
                rows = query.OrderBy(item => item.CreateDate).ToArray().Select(item => new
                {
                    item.ID,
                    item.Enterprise.Name,
                    item.DyjCode,
                    District = item.District.GetDescription(),
                    Grade = item.Grade,
                    //item.Address,
                    item.Enterprise.Corporation,
                    item.Enterprise.Uscc,
                    item.Enterprise.RegAddress,
                    item.Status,
                    StatusName=item.Status.GetDescription()
                })
            };
        }

        protected void del()
        {
            var id = Request.Form["id"];
            var entity = new WareHousesRoll()[id];
            entity.Abandon();
        }
    }
}