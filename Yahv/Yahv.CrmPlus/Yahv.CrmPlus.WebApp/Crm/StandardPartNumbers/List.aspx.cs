using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Extends;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.StandardPartNumbers
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Dictionary<string, string> list = new Dictionary<string, string>() { { "0", "全部" } };

                this.Model.DataStatus = list.Concat(ExtendsEnum.ToDictionary<DataStatus>()).Select(item => new
                {
                    value = int.Parse(item.Key),
                    text = item.Value.ToString()
                });
            }
        }
        protected object data()
        {
            var query = new CrmPlus.Service.Views.Rolls.StandardPartNumbersRoll();
            return this.Paging(query.OrderBy(item => item.CreateDate).Where(Predicate()), item => new
            {
                item.ID,
                item.PartNumber,
                item.Brand,
                item.ProductName,
                item.PackageCase,
                item.Packaging,
                item.Moq,
                item.Mpq,
                item.TaxCode,
                item.Eccn,
                item.TariffRate,
                item.Ccc,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                ModifyDate = item.ModifyDate.ToString("yyyy-MM-dd"),
                item.Status,
                StatusDes = item.Status.GetDescription(),
                Summary = item.Summary
            });
        }
        Expression<Func<StandardPartNumber, bool>> Predicate()
        {
            Expression<Func<StandardPartNumber, bool>> predicate = item => true;
            var status = Request["cboStatus"];
            var name = Request["s_name"];

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.PartNumber.Contains(name) || item.Brand.Contains(name) || item.ProductName.Contains(name));
            }

            if (status != "0")
            {
                predicate = predicate.And(item => item.Status == (DataStatus)int.Parse(status));
            }
            return predicate;
        }
        bool disableSuccess=false;
        protected bool disable()
        {
            var id = Request.Form["id"];
            var entity = new Service.Views.Rolls.StandardPartNumbersRoll()[id];
            if (entity == null)
            {
                return false;
            }
            entity.AbandonSuccess += Entity_AbandonSuccess; ;
            entity.Abandon();
            return disableSuccess;
        }


        private void Entity_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var id = Request.Form["id"];
            LogsOperating.LogOperating(Erp.Current, id, $"停用标准型号:{ id}");
            this.disableSuccess = true;
        }
        protected bool enable()
        {
            try {
                var id = Request.Form["id"];
                var entity = new Service.Views.Rolls.StandardPartNumbersRoll()[id];
                entity.Enable();
                LogsOperating.LogOperating(Erp.Current, id, $"启用标准型号:{ id}");
                return true;
            } catch(Exception ex) {
                return false;
            }
           
        }
    }
}