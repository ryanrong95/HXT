using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models.PveCrm;

namespace Yahv.Services.Views.PveCrm
{
    public class InvoicesTopView<TReponsitory> : UniqueView<Invoice, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public InvoicesTopView()
        {

        }
        public InvoicesTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Invoice> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PveCrm.InvoicesTopView>()
                   join district in this.Reponsitory.ReadTable<Layers.Data.Sqls.PveCrm.EnumsDictionariesTopView>() on entity.District equals district.ID
                   select new Invoice
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       Bank = entity.Bank,
                       BankAddress = entity.BankAddress,
                       Account = entity.Account,
                       Name = entity.Name,
                       Tel = entity.Tel,
                       Mobile = entity.Mobile,
                       //Type = (InvoiceType)entity.Type,//发票类型
                       Email = entity.Email,
                       District = entity.District,
                       DistrictDesc = district.Description,
                       Postzip = entity.Postzip,
                       Address = entity.Address,
                       //Status = (ApprovalStatus)entity.Status,
                       EnterpriseName = entity.EnterpriseName,
                       //DeliveryType = (InvoiceDeliveryType)entity.DeliveryType,//开票类型
                       //Uscc = entity.Uscc,
                       TaxperNumber = entity.TaxperNumber,
                       //RegAddress = entity.RegAddress
                   };
        }
    }
}
