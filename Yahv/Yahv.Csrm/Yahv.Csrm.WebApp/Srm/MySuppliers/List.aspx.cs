using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv;
using Yahv.Csrm.WebApp;
using Yahv.Linq.Extends;
using Yahv.Underly;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace WebApp.Srm.MySuppliers
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                init();
            }
        }
        protected object data()
        {
            var query = Yahv.Erp.Current.Srm.MySuppliers.Where(Predicate());
            return this.Paging(query.OrderBy(item => item.Enterprise.Name), item => new
            {
                item.ID,
                item.Enterprise.Name,
                item.Enterprise.AdminCode,
                item.TaxperNumber,
                item.Grade,
                item.DyjCode,
                Type = item.Type.GetDescription(),
                Nature = item.Nature.GetDescription(),
                item.Enterprise.District,
                item.SupplierStatus,
                StatusName = item.SupplierStatus.GetDescription(),
                //AreaType = item.AreaType.GetDescription(),
                IsFactory = item.IsFactory ? "是" : "否",
                InvoiceType = item.InvoiceType.GetDescription(),
                item.AgentCompany,
                TaxType = item.InvoiceType.GetDescription(),
                Currency = item.Currency.GetDescription(),
                item.RepayCycle,
                Price = item.Currency.GetCurrency() + " " + item.Price,
                Admin = item.Creator == null ? null : item.Creator.RealName,
                Cooper = item.Purchasers.FirstOrDefault()?.Company?.Name,
                Origin = Enum.GetValues(typeof(Origin)).Cast<Origin>().Any(i => i.GetOrigin().Code == item.Place) ? Enum.GetValues(typeof(Origin)).Cast<Origin>().SingleOrDefault(i => i.GetOrigin().Code == item.Place).GetOrigin()?.ChineseName : null,
            });
        }
        void init()
        {
            Dictionary<string, string> list = new Dictionary<string, string>() { { "-100", "全部" } };
            //状态
            this.Model.Status = list.Concat(ExtendsEnum.ToDictionary<ApprovalStatus>()).Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });
            //级别
            this.Model.Grade = list.Concat(ExtendsEnum.ToDictionary<SupplierGrade>()).Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });
            //性质
            this.Model.Nature = list.Concat(ExtendsEnum.ToDictionary<SupplierNature>()).Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });
            //类型
            this.Model.Type = list.Concat(ExtendsEnum.ToDictionary<SupplierType>()).Select(item => new
            {
                value = int.Parse(item.Key),
                text = item.Value.ToString()
            });
        }
        Expression<Func<TradingSupplier, bool>> Predicate()
        {
            Expression<Func<TradingSupplier, bool>> predicate = item => true;
            var status = Request["selStatus"];
            var name = Request["s_name"];
            //var areatype = Request["selAreaType"];
            var nature = Request["selNature"];
            var grade = Request["selGrade"];
            var type = Request["selType"];
            if (status != "-100")
            {
                predicate = predicate.And(item => item.SupplierStatus == (ApprovalStatus)int.Parse(status));
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Enterprise.Name.Contains(name));
            }
            if (nature != "-100")
            {
                predicate = predicate.And(item => item.Nature == (SupplierNature)int.Parse(nature));
            }
            if (type != "-100")
            {
                predicate = predicate.And(item => item.Type == (SupplierType)int.Parse(type));
            }
            if (grade != "-100")
            {
                predicate = predicate.And(item => item.Grade == (SupplierGrade)int.Parse(grade));
            }
            if (bool.Parse(Request["factory"]))
            {
                predicate = predicate.And(item => item.IsFactory == true);
            }
            return predicate;
        }
        protected void del()
        {
            var arry = Request.Form["items"].Split(',');
            //if (Erp.Current.IsSuper)
            //{
            var entity = new SuppliersRoll().Where(t => arry.Contains(t.ID));
            entity.Delete();
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                           nameof(Yahv.Systematic.Srm),
                                          "SupplierUpdate", "修改供应商状态删除：" + it.Enterprise.Name, "");
            }
            //}
            //else
            //{
            //    var suppliers = Erp.Current.Srm.Suppliers.Where(s => arry.Contains(s.ID));
            //    foreach (var supplier in suppliers)
            //    {
            //        supplier.Abandon();
            //        Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
            //                                   nameof(Yahv.Systematic.Srm),
            //                                  "SupplierDelete", "删除供应商：" + supplier.Enterprise.Name, "");
            //    }
            //}
        }
        protected void Enable()
        {
            var arry = Request.Form["items"].Split(',');
            var entity = new SuppliersRoll().Where(t => arry.Contains(t.ID));
            entity.Enable();
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                        nameof(Yahv.Systematic.Srm),
                                       "启用供应商", "启用供应商：" + it.Enterprise.Name, "");
            }
        }
        protected void Unable()
        {
            var arry = Request.Form["items"].Split(',');
            var entity = new SuppliersRoll().Where(t => arry.Contains(t.ID));
            entity.Unable();
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                          nameof(Yahv.Systematic.Srm),
                                         "停用供应商", "停用供应商：" + it.Enterprise.Name, "");
            }
        }
        protected void Black()
        {
            var arry = Request.Form["items"].Split(',');
            var entity = new SuppliersRoll().Where(t => arry.Contains(t.ID));
            entity.Blacked();
            foreach (var it in entity)
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Srm),
                                        "供应商加入黑名单", "供应商加入黑名单：" + it.Enterprise.Name, "");
            }
        }
    }
}