using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using YaHv.Csrm.Services.Models.Origins;
using Yahv.Underly;

namespace Yahv.Csrm.WebApp.Crm.Carriers.Transports
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
            var query = Erp.Current.Crm.Carriers[id].Transports.Where(Predicate());

            return this.Paging(query.OrderBy(item => item.Enterprise.Name), item => new
            {
                item.ID,
                item.Enterprise.Name,
                item.Enterprise.District,
                Creator = item.Creator == null ? null : item.Creator.RealName,
                Type = item.Type.GetDescription(),
                item.CarNumber1,
                item.CarNumber2,
                item.Weight,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.Status,
                StatusName = item.Status.GetDescription()
            });
        }

        Expression<Func<Transport, bool>> Predicate()
        {
            Expression<Func<Transport, bool>> predicate = item => true;
            var name = Request["s_name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.CarNumber1.Contains(name) || item.CarNumber2.Contains(name));
            }

            return predicate;
        }

        protected void del()
        {
            var carrier = Erp.Current.Crm.Carriers[Request["carrierid"]];
            var transport = carrier.Transports[Request["transportid"]];
            if (transport != null)
            {
                transport.AbandonSuccess += Transport_AbandonSuccess;
                transport.Abandon();
            }
        }

        private void Transport_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Transport;
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
            model.Transport = new apiTransport
            {
                EnterpriseName = entity.Enterprise.Name,
                CarNumber2 = entity.CarNumber2,
                CarNumber1 = entity.CarNumber1,
                Weight = entity.Weight,
                Type = entity.Type,
                Status = 400,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Creator = entity.CreatorID
            };
            model.Unify();
        }
    }
}