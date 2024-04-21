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
    /// <summary>
    /// 客户地址视图
    /// </summary>
    public class ClientAddressesView : UniqueView<Address, PvdCrmReponsitory>
    {
        public ClientAddressesView()
        {
        }

        internal ClientAddressesView(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        internal ClientAddressesView(PvdCrmReponsitory reponsitory, IQueryable<Address> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Address> GetIQueryable()
        {
            return from address in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Addresses>()
                   where address.Status == (int)AuditStatus.Waiting || address.Status == (int)AuditStatus.Normal
                   select new Address
                   {
                       ID = address.ID,
                       EnterpriseID = address.EnterpriseID,
                       RelationType = (RelationType)address.RelationType,
                       AddressType = (AddressType)address.Type,
                       Title = address.Title,
                       District = address.District,
                       Context = address.Context,
                       Contact = address.Contact,
                       Phone = address.Phone,
                       PostZip = address.PostZip,
                       DyjCode = address.DyjCode,
                       Status = (AuditStatus)address.Status,
                       CreatorID = address.CreatorID,
                       CreateDate = address.CreateDate,
                       ModifyDate = address.ModifyDate,
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
            var iquery = this.IQueryable.Cast<Address>();
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

            var linq = from address in ienum_iquery
                       join enterprise in ienum_enterprises on address.EnterpriseID equals enterprise.ID
                       select new
                       {
                           address.ID,
                           EnterpriseID = enterprise.ID,
                           EnterpriseName = enterprise.Name,
                           Title = address.Title,
                           District = address.District,
                           Context = address.Context,
                           Contact = address.Contact,
                           Phone = address.Phone,
                           PostZip = address.PostZip,
                           DyjCode = address.DyjCode,
                           address.CreatorID,
                       };

            return linq.ToArray();
        }

        #region 搜索方法

        /// <summary>
        /// 根据客户ID查询
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public ClientAddressesView SearchByClientID(string clientID)
        {
            var linq = from address in this.IQueryable
                       where address.EnterpriseID == clientID
                       select address;

            var view = new ClientAddressesView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据业务类型查询
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ClientAddressesView SearchByRelationType(RelationType type)
        {
            var linq = from address in this.IQueryable
                       where address.RelationType == type
                       select address;

            var view = new ClientAddressesView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据地址类型查询
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ClientAddressesView SearchByAddressType(AddressType type)
        {
            var linq = from address in this.IQueryable
                       where address.AddressType == type
                       select address;

            var view = new ClientAddressesView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据创建人查询
        /// </summary>
        /// <param name="creatorID"></param>
        /// <returns></returns>
        public ClientAddressesView SearchByCreatorID(string creatorID)
        {
            var linq = from address in this.IQueryable
                       where address.CreatorID == creatorID
                       select address;

            var view = new ClientAddressesView(this.Reponsitory, linq);
            return view;
        }

        #endregion
    }
}
