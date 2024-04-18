using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{

    /// <summary>
    /// 个人客户发票通用视图
    /// </summary>
    public class vInvoicesTopView<TReponsitory> : QueryView<vInvoice, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public vInvoicesTopView()
        {

        }
        public vInvoicesTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<vInvoice> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.vInvoicesTopView>()
                   select new vInvoice
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       IsPersonal = entity.IsPersonal,
                       Type = (InvoiceType)entity.Type,
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
                       DeliveryType = (InvoiceDeliveryType)entity.DeliveryType,
                       Status = (GeneralStatus)entity.Status,
                       IsDefault = entity.IsDefault
                   };
        }
    }
}
