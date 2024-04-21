using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class InvoicesOrigin : Yahv.Linq.UniqueView<Models.Origins.Invoice, PvdCrmReponsitory>
    {
        internal InvoicesOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        InvoicesOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Invoice> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            var registerView = new EnterpriseRegistersOrigin(this.Reponsitory);
            var invoiceView = Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Invoices>();
            return from entity in invoiceView
                   join enterprise in enterprisesView on entity.EnterpriseID equals enterprise.ID
                   //join register in registerView on entity.ID equals register.ID
                   join admin in adminsView on entity.CreatorID equals admin.ID into g
                   from admin in g.DefaultIfEmpty()
                   select new Yahv.CrmPlus.Service.Models.Origins.Invoice
                   {

                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       Enterprise = enterprise,
                       RelationType =(RelationType)entity.RelationType,
                       Bank = entity.Bank,
                       Address = entity.Address,
                       Account = entity.Account,
                       //Uscc = register.Uscc,
                       CreateDate = entity.CreateDate,
                       Status = (DataStatus)entity.Status,
                       Admin = admin,
                       Tel = entity.Tel,
                   };
        }
    }


}

