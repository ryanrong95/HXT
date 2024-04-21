using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Crm.Carriers
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected object data()
        {
            try {
                var query = Erp.Current.Crm.Carriers.Where(Predicate());

                return this.Paging(query.OrderBy(item => item.Enterprise.Name), item => new
                {
                    item.ID,
                    item.Enterprise.Name,
                    item.Enterprise.District,
                    Creator = item.Creator == null ? null : item.Creator.RealName,
                    item.Enterprise.Uscc,
                    item.Enterprise.Corporation,
                    item.Enterprise.RegAddress,
                    item.Icon,
                    item.Code,
                    item.Summary,
                    item.Status,
                    Type = item.Type.GetDescription(),
                    Place = Enum.GetValues(typeof(Origin)).Cast<Origin>().Any(i => i.GetOrigin().Code == item.Enterprise.Place) ? Enum.GetValues(typeof(Origin)).Cast<Origin>().SingleOrDefault(i => i.GetOrigin().Code == item.Enterprise.Place).GetOrigin()?.ChineseName : null,
                    StatusName = item.Status.GetDescription(),
                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    item.IsInternational
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
        Expression<Func<Carrier, bool>> Predicate()
        {
            Expression<Func<Carrier, bool>> predicate = item => true;
            var name = Request["s_name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Enterprise.Name.Contains(name));
            }

            return predicate;
        }
        protected void del()
        {
            var id = Request.Form["id"];
            var entity = Erp.Current.Crm.Carriers[id];
            entity.AbandonSuccess += Entity_AbandonSuccess;
            entity.Abandon();
        }
        protected void enable()
        {
            var id = Request.Form["id"];
            var entity = Erp.Current.Crm.Carriers[id];
            if (entity.Status == GeneralStatus.Deleted)
            {
                entity.Enable();
                //向芯达通同步
                var model = new CarrierModel();
                model.Carrier = new apiCarrier
                {
                    Name = entity.Enterprise.Name,
                    Code = entity.Code,
                    Summary = entity.Summary,
                    Status = 200,
                    CarrierType = Commons.ConvertType(entity.Type, entity.Enterprise.Place),
                    Creator = entity.CreatorID
                };
                model.Unify();
            }
        }
        private void Entity_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Carrier;
            var model = new CarrierModel();
            model.Carrier = new apiCarrier
            {
                Name = entity.Enterprise.Name,
                Code = entity.Code,
                Summary = entity.Summary,
                Status = 400,
                CarrierType = Commons.ConvertType(entity.Type, entity.Enterprise.Place),
                Creator = entity.CreatorID
            };
            model.Unify();
        }
    }
}