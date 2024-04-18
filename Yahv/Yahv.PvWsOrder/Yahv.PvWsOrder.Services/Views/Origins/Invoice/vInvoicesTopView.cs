using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views.Origins
{
    /// <summary>
    /// 代仓储客户或个人
    /// 发票通用视图
    /// </summary>
    public class vInvoicesTopView : UniqueView<vInvoice, PvWsOrderReponsitory>
    {
        public  vInvoicesTopView()
        {

        }

        public vInvoicesTopView(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<vInvoice> GetIQueryable()
        {
            var invoices = new Yahv.Services.Views.vInvoicesTopView<PvWsOrderReponsitory>(this.Reponsitory);

            var linq = from entity in invoices
                       select new vInvoice
                       {
                           ID = entity.ID,
                           EnterpriseID = entity.EnterpriseID,
                           IsPersonal = entity.IsPersonal,
                           Type = entity.Type,
                           Title = entity.Title,
                           TaxNumber = entity.TaxNumber,
                           RegAddress = entity.RegAddress,
                           Tel = entity.Tel,
                           BankName = entity.BankName,
                           BankAccount = entity.BankAccount,
                           PostAddress = entity.PostAddress,
                           PostRecipient = entity.PostRecipient,
                           PostTel = entity.PostTel,
                           PostZipCode = entity.PostZipCode,
                           DeliveryType = entity.DeliveryType,
                           Status = entity.Status,
                           IsDefault = entity.IsDefault
                       };
            return linq;
        }
    }
}
