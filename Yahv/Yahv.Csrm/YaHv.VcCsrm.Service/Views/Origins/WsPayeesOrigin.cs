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
    public class WsPayeesOrigin : Yahv.Linq.UniqueView<WsPayee, PvcCrmReponsitory>
    {
        internal WsPayeesOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal WsPayeesOrigin(PvcCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<WsPayee> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Views.Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvcCrm.WsPayees>()
                   join enterprise in enterpriseView on entity.WsSupplierID equals enterprise.ID
                   //join admin in adminsView on entity.CreatorID equals admin.ID
                   select new WsPayee
                   {
                       ID = entity.ID,
                       InvoiceType = (InvoiceType)entity.InvoiceType,
                       WsSupplierID = entity.WsSupplierID,
                       Bank = entity.Bank,
                       BankAddress = entity.BankAddress,
                       Account = entity.Account,
                       SwiftCode = entity.SwiftCode,
                       Methord = (Methord)entity.Methord,
                       Currency = (Currency)entity.Currency,
                       District = (District)entity.District,
                       Name = entity.Name,
                       Tel = entity.Tel,
                       Mobile = entity.Mobile,
                       Email = entity.Email,
                       Status = (GeneralStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       CreatorID = entity.CreatorID,
                       //IsDefault=entity.IsDefault
                   };

        }
    }
}
