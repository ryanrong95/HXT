using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Adress
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.ID = Request.QueryString["id"];

            }
        }


        protected object data()
        {

            string id = Request.QueryString["id"];
            var query = Erp.Current.CrmPlus.Addresses[id, RelationType.Trade].Where(Predicate());
            return this.Paging(query.ToArray().Select(item => new {
                item.ID,
                item.Contact,
                item.Phone,
                District = item.PlaceDescription,
                item.Context,
                item.PostZip,
                AddressType = item.AddressType.GetDescription(),
                item.Title,
                RelationType = item.RelationType.GetDescription(),
                Creator = item.Admin.RealName,
                StatusDes = item.Status.GetDescription(),
            }));

        }


        Expression<Func<Service.Models.Origins.Address, bool>> Predicate()
        {
            Expression<Func<Yahv.CrmPlus.Service.Models.Origins.Address, bool>> predicate = item => true;
            var key = Request["txtKey"];
            var addressType = Request["AddressType"];
            if (!string.IsNullOrWhiteSpace(key))
            {
                predicate = predicate.And(item => item.Contact.Contains(key)||item.Context.Contains(key));
            }
            //手机号或者电话
            if (!string.IsNullOrWhiteSpace(key))
            {
                predicate = predicate.And(item => item.Context.Contains(key));
            }

            AddressType type;
            if (Enum.TryParse(addressType, out type))
            {
                predicate = predicate.And(item => item.AddressType == type);
            }
            return predicate;
        }


        /// <summary>
        /// 停用
        /// </summary>
        protected void Closed()
        {
            var id = Request.Form["ID"];
            try
            {

                var entity = Erp.Current.CrmPlus.Addresses[id];
                entity.Closed();
                LogsOperating.LogOperating(Erp.Current, entity.EnterpriseID, $"停用地址:{ entity.ID}");

            }
            catch (Exception ex)
            {
                LogsOperating.LogOperating(Erp.Current, id, $"启用地址信息 操作失败" + ex);
            }
        }


        /// <summary>
        /// 启用
        /// </summary>
        protected void Enable()
        {
            var id = Request.Form["ID"];
            try
            {
                var entity = Erp.Current.CrmPlus.Addresses[id];
                entity.Enable();
                LogsOperating.LogOperating(Erp.Current, entity.EnterpriseID, $"启用地址:{ entity.ID}");
            }
            catch (Exception ex)
            {
                LogsOperating.LogOperating(Erp.Current, id, $"启用地址信息 操作失败" + ex);
            }
        }


    }
}