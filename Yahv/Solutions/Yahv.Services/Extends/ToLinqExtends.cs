using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services
{
    public static class ToLinqExtends
    {
        #region 客户付款人
        public static Layers.Data.Sqls.PvbCrm.Payers ToLinq(this Models.wsPayer entity)
        {
            return new Layers.Data.Sqls.PvbCrm.Payers
            {
                ID = entity.ID,
                EnterpriseID = entity.EnterpriseID,
                RealID = entity.RealEnterpriseID,
                Bank = entity.Bank,
                BankAddress = entity.BankAddress,
                Account = entity.Account,
                SwiftCode = entity.SwiftCode,
                Methord = (int)entity.Methord,
                Currency = (int)entity.Currency,
                Contact = entity.Contact,
                Tel = entity.Tel,
                Mobile = entity.Mobile,
                Email = entity.Email,
                Status = (int)entity.Status,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Creator = entity.CreatorID ?? "",
                Place = entity.Place
            };
        }
        #endregion

        #region 企业收款人
        public static Layers.Data.Sqls.PvbCrm.Payees ToLinq(this Models.wsPayee entity)
        {
            return new Layers.Data.Sqls.PvbCrm.Payees
            {
                ID = entity.ID,
                EnterpriseID = entity.EnterpriseID,
                RealID = entity.RealEnterpriseID,
                Bank = entity.Bank,
                BankAddress = entity.BankAddress,
                Account = entity.Account,
                SwiftCode = entity.SwiftCode,
                Methord = (int)entity.Methord,
                Currency = (int)entity.Currency,
                Contact = entity.Contact,
                Tel = entity.Tel,
                Mobile = entity.Mobile,
                Email = entity.Email,
                Status = (int)entity.Status,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Creator = entity.CreatorID,
                Place = entity.Place
            };
        }
        #endregion

        #region 客户供应商的收款人
        public static Layers.Data.Sqls.PvbCrm.nPayees ToLinq(this Models.wsnSupplierPayee entity)
        {
            return new Layers.Data.Sqls.PvbCrm.nPayees
            {
                ID = entity.ID,
                nSupplierID = entity.nSupplierID,
                EnterpriseID = entity.OwnID,//所属企业，客户ID
                RealID = entity.RealEnterpriseID,//供应商的企业ID
                Bank = entity.Bank,
                BankAddress = entity.BankAddress,
                Account = entity.Account,
                SwiftCode = entity.SwiftCode,
                Methord = (int)entity.Methord,
                Currency = (int)entity.Currency,
                Contact = entity.Contact,
                Tel = entity.Tel,
                Mobile = entity.Mobile,
                Email = entity.Email,
                Status = (int)entity.Status,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Creator = entity.Creator,
                IsDefault = entity.IsDefault,
                Place = entity.Place
            };
        }
        #endregion

        #region 客户供应商的联系人=私有联系人
        public static Layers.Data.Sqls.PvbCrm.nContacts ToLinq(this Models.wsnContact entity)
        {
            return new Layers.Data.Sqls.PvbCrm.nContacts
            {
                ID = entity.ID,
                nSupplierID = entity.nSupplierID,//供应商ID
                EnterpriseID = entity.OwnID,//所属企业，客户ID
                RealID = entity.RealID,//供应商的企业ID
                Name = entity.Name,
                Tel = entity.Tel,
                Mobile = entity.Mobile,
                Email = entity.Email,
                Fax = entity.Fax,
                QQ = entity.QQ,
                Status = (int)entity.Status,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Creator = entity.CreaterID
            };
        }
        #endregion

        #region 客户供应商的交货地址=私有交货地址
        /// <summary>
        /// Consignor To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvbCrm.nConsignors ToLinq(this Models.wsnSupplierConsignor entity)
        {
            return new Layers.Data.Sqls.PvbCrm.nConsignors
            {
                ID = entity.ID,
                nSupplierID = entity.nSupplierID,
                Title = entity.Title,
                EnterpriseID = entity.OwnID,//客户ID
                RealID = entity.RealEnterpriseID,//供应商的企业ID
                Province = null,
                City = null,
                Land = null,
                Address = entity.Address,
                Postzip = null,
                Contact = entity.Contact,
                Tel = entity.Tel,
                Mobile = entity.Mobile,
                Email = entity.Email,
                Status = (int)entity.Status,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                AdminID = entity.CreatorID,
                IsDefault = entity.IsDefault,
                Place = entity.Place
            };
        }
        #endregion
    }
}
