using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;

namespace Yahv.CrmPlus.Service.Views.Plugins
{
    public class SupplierPayeesView : BookAccountsView
    {
        public SupplierPayeesView()
        {
        }

        internal SupplierPayeesView(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        internal SupplierPayeesView(PvdCrmReponsitory reponsitory, IQueryable<BookAccount> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<BookAccount> GetIQueryable()
        {
            return from payee in base.GetIQueryable()
                   join enterprise in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Enterprises>() on payee.EnterpriseID equals enterprise.ID
                   where payee.RelationType == RelationType.Suppliers && payee.BookAccountType == BookAccountType.Payee
                   select new BookAccount
                   {
                       ID = payee.ID,
                       EnterpriseID = payee.EnterpriseID,
                       RelationType = payee.RelationType,
                       BookAccountType = payee.BookAccountType,
                       BookAccountMethord = payee.BookAccountMethord,
                       Bank = payee.Bank,
                       BankAddress = payee.BankAddress,
                       BankCode = payee.BankCode,
                       Account = payee.Account,
                       Currency = payee.Currency,
                       SwiftCode = payee.SwiftCode,
                       Transfer = payee.Transfer,
                       CreatorID = payee.CreatorID,
                       Status = payee.Status,
                       IsPersonal = payee.IsPersonal,
                       Enterprise = new Enterprise
                       {
                           ID = enterprise.ID,
                           Name = enterprise.Name
                       }
                   };
        }

        public object Single()
        {
            return (this.ToMyObject() as object[]).SingleOrDefault();
        }

        public object[] ToMyArray()
        {
            return this.ToMyObject() as object[];
        }

        /// <summary>
        /// 补全对象
        /// </summary>
        /// <returns></returns>
        public object ToMyObject()
        {
            var iquery = this.IQueryable.Cast<BookAccount>();
            var ienum_iquery = iquery.ToArray();

            var linq = from payee in ienum_iquery
                       select new
                       {
                           payee.ID,
                           payee.EnterpriseID,
                           EnterpriseName = payee.Enterprise.Name,
                           payee.BookAccountMethord,
                           payee.Bank,
                           payee.BankAddress,
                           payee.BankCode,
                           payee.Account,
                           payee.Currency,
                           payee.SwiftCode,
                           payee.Transfer,
                           CreatorID = payee.CreatorID,
                           IsPersonal = payee.IsPersonal,
                       };

            return linq.ToArray();
        }

        #region 搜索方法

        /// <summary>
        /// 根据供应商ID查询
        /// </summary>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        public SupplierPayeesView SearchBySupplierID(string supplierID)
        {
            var linq = from payee in this.IQueryable
                       where payee.EnterpriseID == supplierID
                       select payee;

            var view = new SupplierPayeesView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据供应商名称查询
        /// </summary>
        /// <param name="supplierName"></param>
        /// <returns></returns>
        public SupplierPayeesView SearchBySupplierName(string supplierName)
        {
            var linq = from payee in this.IQueryable
                       where payee.Enterprise.Name == supplierName
                       select payee;

            var view = new SupplierPayeesView(this.Reponsitory, linq);
            return view;
        }

        #endregion
    }
}
