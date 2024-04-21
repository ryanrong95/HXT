using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Data.Models;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.Linq;

namespace Yahv.CrmPlus.Service.Views
{
    public class SuppliersView : vDepthView<Models.Origins.Supplier, Entity.Supplier, PvdCrmReponsitory>
        , ISeach<SuppliersView, Models.Origins.Supplier>
    {
        public SuppliersView()
        {
        }

        public SuppliersView(IQueryable<Models.Origins.Supplier> iQueryable) : base(iQueryable)
        {
        }

        protected SuppliersView(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected SuppliersView(PvdCrmReponsitory reponsitory, IQueryable<Models.Origins.Supplier> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        public SuppliersView Searh(Expression<Func<Models.Origins.Supplier, bool>> predicate)
        {
            var query = this.IQueryable.Where(predicate);
            return new SuppliersView(this.Reponsitory, query);
        }

        protected override IQueryable<Models.Origins.Supplier> GetIQueryable()
        {
            var enterpriseView = new EnterprisesOrigin(this.Reponsitory);
            return from supplier in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Suppliers>()
                   join enterprise in enterpriseView on  supplier.ID equals enterprise.ID
                   select new Models.Origins.Supplier
                   {
                       ID = supplier.ID,
                       //Name = enterprise.Name,
                       //IsDraft = enterprise.IsDraft,
                       //Status = enterprise.Status,
                       //District = enterprise.District,
                       //Place = enterprise.Place,
                       //Grade = supplier.Grade,
                       //Summary = enterprise.Summary,
                       //CreateDate = enterprise.CreateDate,
                       //ModifyDate = enterprise.ModifyDate,
                       //DyjCode = enterprise.DyjCode,
                       //SupplierGrade = supplier.Grade,
                       //Products = supplier.Products,
                       //Source = supplier.Source,
                       //EnterpriseRegister = enterprise.EnterpriseRegister,
                       //Type = (Underly.CrmPlus.SupplierType)supplier.Type,
                       //SettlementType = (Underly.CrmPlus.SettlementType)supplier.SettlementType,
                       //OrderType = (Underly.CrmPlus.OrderType)supplier.OrderType,
                       //InvoiceType = (Underly.InvoiceType)supplier.InvoiceType,
                       //IsSpecial = supplier.IsSpecial,
                       //IsClient = supplier.IsClient,
                       //IsProtected = supplier.IsProtected,
                       //IsAgent = supplier.IsAgent,
                       //IsAccount = supplier.IsAccount,
                       //WorkTime = supplier.WorkTime,
                       //IsFixed = supplier.IsFixed,
                       //CreatorID = supplier.ID,
                       //SupplierStatus = (Underly.AuditStatus)supplier.Status,
                       //OrderCompanyID = supplier.OrderCompanyID,
                   };
        }

        protected override IEnumerable<Entity.Supplier> OnMyPage(IQueryable<Models.Origins.Supplier> iquery)
        {
            var data = iquery.ToArray();
            var linq_suppliers = data.Select(item => new Entity.Supplier
            {
                
            });
            return linq_suppliers;
        }
    }
}
