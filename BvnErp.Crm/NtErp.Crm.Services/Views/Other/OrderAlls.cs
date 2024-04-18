using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NtErp.Crm.Services.Views
{
    public class OrderAlls : UniqueView<Order, BvCrmReponsitory>, Needs.Underly.IFkoView<Order>
    {

        protected OrderAlls()
        {

        }

        protected override IQueryable<Order> GetIQueryable()
        {
            return this.GetIQueryable(this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Orders>());
        }
        internal IQueryable<Order> GetIQueryable(IQueryable<Layer.Data.Sqls.BvCrm.Orders> query = null)
        {
            if (query == null)
            {
                query = this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Orders>();
            }

            AdminTopView topview = new AdminTopView(this.Reponsitory);
            BeneficiariesAlls benefitview = new BeneficiariesAlls(this.Reponsitory);
            ContactAlls contactview = new ContactAlls(this.Reponsitory);
            ClientAlls clientview = new ClientAlls(this.Reponsitory);
            return from order in base.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Orders>()
                   join benefit in benefitview on order.BeneficiaryID equals benefit.ID
                   join admin in topview on order.AdminID equals admin.ID
                   join contact in contactview on order.ConsigneeID equals contact.ID
                   join client in clientview on order.ClientID equals client.ID
                   select new Order
                   {
                       ID = order.ID,
                       ClientID = order.ClientID,
                       CatalogueID = order.CatalogueID,
                       AdminID = order.AdminID,
                       Currency = (CurrencyType)order.Currency,
                       BeneficiaryID = order.BeneficiaryID,
                       DeliveryAddress = order.DeliveryAddress,
                       Address = order.Address,
                       ConsigneeID = order.ConsigneeID,
                       Status = (Enums.Status)order.Status,
                       CreateDate = order.CreateDate,
                       UpdateDate = order.UpdateDate,
                       Beneficiaries = benefit,
                       Admin = admin,
                       Client = client,
                       Contact = contact
                   };
        }
    }
}
