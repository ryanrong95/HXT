using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Srm.Granule
{
    public partial class Suppliers : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Admin = new AdminsAllRoll()[Request.QueryString["id"]];
                //级别
                this.Model.Grade = ExtendsEnum.ToArray<SupplierGrade>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
            }
        }
        protected object data()
        {
            var suppliersids = Erp.Current.Srm.Admins[Request.QueryString["id"]].Suppliers.Select(item => item.ID).ToArray();
            var query = Yahv.Erp.Current.Srm.Suppliers.Where(Predicate());
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
                // AreaType = item.AreaType.GetDescription(),
                IsFactory = item.IsFactory ? "是" : "否",
                item.AgentCompany,
                InvoiceType = item.InvoiceType.GetDescription(),
                TaxType = item.InvoiceType.GetDescription(),
                item.RepayCycle,
                Currency = item.Currency.GetDescription(),
                Price = item.Currency.GetCurrency() + " " + item.Price,
                Admin = item.Creator.RealName,
                IsChecked = suppliersids.Contains(item.ID)
            });
        }
        Expression<Func<TradingSupplier, bool>> Predicate()
        {
            Expression<Func<TradingSupplier, bool>> predicate = item => true;
            var name = Request["s_name"];
            var grade = Request["selGrade"];

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Enterprise.Name.Contains(name));
            }
            if (!string.IsNullOrWhiteSpace(grade))
            {
                predicate = predicate.And(item => item.Grade == (SupplierGrade)int.Parse(grade));
            }
            if (bool.Parse(Request["factory"]))
            {
                predicate = predicate.And(item => item.IsFactory == true);
            }
            return predicate;
        }



        /// <summary>
        /// 绑定
        /// </summary>
        protected JMessage Bind()
        {
            var supplierid = Request["supplierid"];
            var adminid = Request["adminid"];
            if (string.IsNullOrWhiteSpace(supplierid) || string.IsNullOrWhiteSpace(adminid))
            {
                return new JMessage
                {
                    success = false,
                    code = 100,
                    data = "绑定失败"
                };
            }
            else
            {
                Erp.Current.Crm.Admins[adminid].Binding(supplierid, adminid, MapsType.Supplier);
                return new JMessage
                {
                    success = true,
                    code = 200,
                    data = "绑定成功"
                };
            }

        }
        /// <summary>
        /// 解绑
        /// </summary>
        protected JMessage UnBind()
        {
            var supplierid = Request["supplierid"];
            var adminid = Request["adminid"];
            if (string.IsNullOrWhiteSpace(supplierid) || string.IsNullOrWhiteSpace(adminid))
            {
                return new JMessage
                {
                    success = false,
                    code = 100,
                    data = "绑定失败"
                };
            }
            else
            {
                Erp.Current.Crm.Admins[adminid].Unbind(supplierid, MapsType.Supplier);
                return new JMessage
                {
                    success = true,
                    code = 200,
                    data = "操作成功"
                };
            }

        }
    }
}