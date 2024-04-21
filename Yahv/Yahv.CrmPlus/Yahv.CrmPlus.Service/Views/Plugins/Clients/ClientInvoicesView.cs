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
    public class ClientInvoicesView : UniqueView<Invoice, PvdCrmReponsitory>
    {
        public ClientInvoicesView()
        {
        }

        internal ClientInvoicesView(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        internal ClientInvoicesView(PvdCrmReponsitory reponsitory, IQueryable<Invoice> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Invoice> GetIQueryable()
        {
            return from invoice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Invoices>()
                   where invoice.Status == (int)DataStatus.Normal
                   orderby invoice.CreateDate
                   select new Invoice
                   {
                       ID = invoice.ID,
                       EnterpriseID = invoice.EnterpriseID,
                       RelationType = (RelationType)invoice.RelationType,
                       Address = invoice.Address,
                       Tel = invoice.Tel,
                       Bank = invoice.Bank,
                       Account = invoice.Account,
                       CreateDate = invoice.CreateDate,
                       CreatorID = invoice.CreatorID,
                       Status = (DataStatus)invoice.Status,
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
            var iquery = this.IQueryable.Cast<Invoice>();
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

            //注册信息
            var registersView = from register in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.MapsEnterprise>()
                                where enterpriseIDs.Contains(register.ID)
                                select new
                                {
                                    register.ID
                                };
            var ienum_registers = registersView.ToArray();

            var linq = from invoice in ienum_iquery
                       join enterprise in ienum_enterprises on invoice.EnterpriseID equals enterprise.ID
                       select new
                       {
                           invoice.ID,
                           EnterpriseID = enterprise.ID,
                           EnterpriseName = enterprise.Name,
                           invoice.Address,
                           invoice.Tel,
                           invoice.Bank,
                           invoice.Account,
                       };

            return linq.ToArray();
        }

        #region 搜索方法

        /// <summary>
        /// 根据客户ID查询
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public ClientInvoicesView SearchByClientID(string clientID)
        {
            var linq = from invoice in this.IQueryable
                       where invoice.EnterpriseID == clientID
                       select invoice;

            var view = new ClientInvoicesView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据业务类型查询
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ClientInvoicesView SearchByRelationType(RelationType type)
        {
            var linq = from invoice in this.IQueryable
                       where invoice.RelationType == type
                       select invoice;

            var view = new ClientInvoicesView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据创建人查询
        /// </summary>
        /// <param name="ownerID"></param>
        /// <returns></returns>
        public ClientInvoicesView SearchByCreatorID(string creatorID)
        {
            var linq = from invoice in this.IQueryable
                       where invoice.CreatorID == creatorID
                       select invoice;

            var view = new ClientInvoicesView(this.Reponsitory, linq);
            return view;
        }

        #endregion
    }
}
