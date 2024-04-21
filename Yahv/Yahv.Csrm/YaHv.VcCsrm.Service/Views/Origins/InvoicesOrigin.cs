using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.VcCsrm.Service.Models;

namespace YaHv.VcCsrm.Service.Views.Origins
{
    public class InvoicesOrigin : Yahv.Linq.UniqueView<Models.Invoice, PvcCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        public InvoicesOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal InvoicesOrigin(PvcCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Invoice> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
           // var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvcCrm.Invoices>()
                   join enterprises in enterpriseView on entity.EnterpriseID equals enterprises.ID
                   //join admin in adminsView on entity.AdminID equals admin.ID into _admin
                   //from admin in _admin.DefaultIfEmpty()
                   select new Invoice()
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       Type = (InvoiceType)entity.Type,
                       Bank = entity.Bank,
                       BankAddress = entity.BankAddress,
                       Account = entity.Account,
                       TaxperNumber = entity.TaxperNumber,
                       Name = entity.Name,
                       Tel = entity.Tel,
                       Mobile = entity.Mobile,
                       Email = entity.Email,
                       District = (District)entity.District,
                       Address = entity.Address,
                       Postzip = entity.Postzip,
                       Status = (ApprovalStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       CreatorID = entity.AdminID,
                       Province = entity.Province,
                       City = entity.City,
                       Land = entity.Land,
                       DeliveryType = (InvoiceDeliveryType)entity.DeliveryType,
                       CompanyTel = entity.CompanyTel
                   };
        }
    }
}
