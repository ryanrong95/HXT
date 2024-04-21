using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Crm.Carriers.Drivers
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Carrier = Erp.Current.Crm.Carriers[Request.QueryString["id"]].Enterprise;
            }
        }
        protected object data()
        {
            var id = Request.QueryString["id"];
            var query = Erp.Current.Crm.Carriers[id].Drivers.Where(Predicate());

            return this.Paging(query.OrderBy(item => item.Enterprise.Name), item => new
            {
                item.ID,
                item.IDCard,
                item.Name,
                item.Mobile,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Creator = item.Creator == null ? null : item.Creator.RealName,
                Status = item.Status,
                StatusName = item.Status.GetDescription(),
                item.Mobile2,
                item.CustomsCode,
                item.PortCode,
                item.LBPassword,
                item.CardCode,

            });
        }

        Expression<Func<Driver, bool>> Predicate()
        {
            Expression<Func<Driver, bool>> predicate = item => true;
            var name = Request["s_name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name) || item.IDCard.Contains(name));
            }

            return predicate;
        }
        protected void del()
        {
            var id = Request["driverid"];
            string carrierid = Request["carrierid"];
            var Driver = Erp.Current.Crm.Carriers[carrierid].Drivers[id];
            Driver.AbandonSuccess += Driver_AbandonSuccess;
            Driver.Abandon();

        }

        private void Driver_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Driver;
            var model = new CarrierModel();
            var carrier = Erp.Current.Crm.Carriers[entity.Enterprise.ID];
            model.Carrier = new apiCarrier
            {
                Name = carrier.Enterprise.Name,
                Code = carrier.Code,
                Summary = carrier.Summary,
                Status = 200,
                CarrierType = Commons.ConvertType(carrier.Type, carrier.Enterprise.Place),
                Creator = entity.CreatorID
            };
            model.Driver = new apiDriver
            {
                EnterpriseName = entity.Enterprise.Name,
                Name = entity.Name,
                CustomsCode = entity.CustomsCode,//海关编码
                CardCode = entity.CardCode,//司机卡号
                Mobile2 = entity.Mobile2,//临时手机号
                Mobile = entity.Mobile,//大陆手机号，必填，
                LBPassword = entity.LBPassword,
                IDCard = entity.IDCard,
                PortCode = entity.PortCode,
                Status = 400,
                CreatorID=entity.CreatorID
            };
            model.Unify();
        }
    }
}