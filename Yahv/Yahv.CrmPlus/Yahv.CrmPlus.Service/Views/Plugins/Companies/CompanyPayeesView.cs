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
    public class CompanyPayeesView : BookAccountsView
    {
        public CompanyPayeesView()
        {
        }

        internal CompanyPayeesView(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        internal CompanyPayeesView(PvdCrmReponsitory reponsitory, IQueryable<BookAccount> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<BookAccount> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.RelationType == RelationType.Own && item.BookAccountType == BookAccountType.Payee);
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

            //企业
            var enterpriseIDs = ienum_iquery.Select(item => item.EnterpriseID).Distinct().ToArray();
            var enterprisesView = from enterprise in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Enterprises>()
                                  where enterpriseIDs.Contains(enterprise.ID)
                                  select new
                                  {
                                      enterprise.ID,
                                      enterprise.Name
                                  };
            var ienum_enterprises = enterprisesView.ToArray();

            var linq = from payee in ienum_iquery
                       join enterprise in ienum_enterprises on payee.EnterpriseID equals enterprise.ID
                       select new
                       {
                           payee.ID,
                           EnterpriseID = enterprise.ID,
                           EnterpriseName = enterprise.Name,
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
        /// 根据公司ID查询
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public CompanyPayeesView SearchByCompanyID(string companyID)
        {
            var linq = from payee in this.IQueryable
                       where payee.EnterpriseID == companyID
                       select payee;

            var view = new CompanyPayeesView(this.Reponsitory, linq);
            return view;
        }

        #endregion
    }
}
