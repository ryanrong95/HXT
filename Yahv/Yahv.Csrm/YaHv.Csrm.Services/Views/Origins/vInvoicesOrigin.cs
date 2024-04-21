using Layers.Data.Sqls;
using System.Linq;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;
using System;

namespace YaHv.Csrm.Services.Views.Origins
{
    public class vInvoicesOrigin : Yahv.Linq.UniqueView<Models.Origins.vInvoice, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        public vInvoicesOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal vInvoicesOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<vInvoice> GetIQueryable()
        {
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.vInvoices>()
                   join admin in adminsView on entity.CreatorID equals admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()
                   where entity.Status == (int)GeneralStatus.Normal
                   select new vInvoice()
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
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                       IsDefault = entity.IsDefault,
                       CreatorRealName = admin.RealName
                   };
        }
    }

}
