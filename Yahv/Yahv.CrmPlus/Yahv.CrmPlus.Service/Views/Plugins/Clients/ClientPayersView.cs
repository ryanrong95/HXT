using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.CrmPlus.Service.Views.Plugins
{
    public class ClientPayersView : BookAccountsView
    {
        public ClientPayersView()
        {
        }

        internal ClientPayersView(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        internal ClientPayersView(PvdCrmReponsitory reponsitory, IQueryable<BookAccount> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<BookAccount> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.BookAccountType == BookAccountType.Payer);
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

            var linq = from payer in ienum_iquery
                       join enterprise in ienum_enterprises on payer.EnterpriseID equals enterprise.ID
                       select new
                       {
                           payer.ID,
                           EnterpriseID = enterprise.ID,
                           EnterpriseName = enterprise.Name,
                           payer.BookAccountMethord,
                           payer.Bank,
                           payer.BankAddress,
                           payer.BankCode,
                           payer.Account,
                           payer.Currency,
                           payer.SwiftCode,
                           payer.Transfer,
                           CreatorID = payer.CreatorID,
                           IsPersonal = payer.IsPersonal,
                       };

            return linq.ToArray();
        }

        #region 搜索方法

        /// <summary>
        /// 根据客户ID查询
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public ClientPayersView SearchByClientID(string clientID)
        {
            var linq = from payer in this.IQueryable
                       where payer.EnterpriseID == clientID
                       select payer;

            var view = new ClientPayersView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据业务类型查询
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ClientPayersView SearchByRelationType(RelationType type)
        {
            var linq = from payer in this.IQueryable
                       where payer.RelationType == type
                       select payer;

            var view = new ClientPayersView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据创建人查询
        /// </summary>
        /// <param name="ownerID"></param>
        /// <returns></returns>
        public ClientPayersView SearchByCreatorID(string creatorID)
        {
            var linq = from payer in this.IQueryable
                       where payer.CreatorID == creatorID
                       select payer;

            var view = new ClientPayersView(this.Reponsitory, linq);
            return view;
        }

        #endregion
    }
}
