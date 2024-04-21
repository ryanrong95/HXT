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
    public class ClientContactsView : UniqueView<Contact, PvdCrmReponsitory>
    {
        public ClientContactsView()
        {
        }

        internal ClientContactsView(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        internal ClientContactsView(PvdCrmReponsitory reponsitory, IQueryable<Contact> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Contact> GetIQueryable()
        {
            return from contact in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Contacts>()
                   where contact.Status == (int)DataStatus.Normal
                   select new Contact
                   {
                       ID = contact.ID,
                       EnterpriseID = contact.EnterpriseID,
                       Name = contact.Name,
                       Department = contact.Department,
                       Positon = contact.Positon,
                       Mobile = contact.Mobile,
                       Tel = contact.Tel,
                       Email = contact.Email,
                       Gender = contact.Gender,
                       QQ = contact.qq,
                       Skype = contact.Skype,
                       Character = contact.Character,
                       Taboo = contact.Taboo,
                       RelationType = (RelationType)contact.RelationType,
                       OwnerID = contact.OwnerID,
                       Summary = contact.Summary,
                       Wx = contact.Wx,
                       CreateDate = contact.CreateDate,
                       Status = (DataStatus)contact.Status,
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
            var iquery = this.IQueryable.Cast<Contact>();
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

            var linq = from contact in ienum_iquery
                       join enterprise in ienum_enterprises on contact.EnterpriseID equals enterprise.ID
                       select new
                       {
                           contact.ID,
                           EnterpriseID = enterprise.ID,
                           EnterpriseName = enterprise.Name,
                           contact.Name,
                           contact.Department,
                           contact.Positon,
                           contact.Mobile,
                           contact.Tel,
                           contact.Email,
                           contact.Gender,
                           contact.QQ,
                           Skype = contact.Skype,
                           contact.Character,
                           contact.Taboo,
                           contact.RelationType,
                           contact.OwnerID,
                           contact.Summary,
                           contact.Wx,
                       };

            return linq.ToArray();
        }

        #region 搜索方法

        /// <summary>
        /// 根据客户ID查询
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public ClientContactsView SearchByClientID(string clientID)
        {
            var linq = from contact in this.IQueryable
                       where contact.EnterpriseID == clientID
                       select contact;

            var view = new ClientContactsView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据业务类型查询
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ClientContactsView SearchByRelationType(RelationType type)
        {
            var linq = from contact in this.IQueryable
                       where contact.RelationType == type
                       select contact;

            var view = new ClientContactsView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据创建人查询
        /// </summary>
        /// <param name="ownerID"></param>
        /// <returns></returns>
        public ClientContactsView SearchByOwnerID(string ownerID)
        {
            var linq = from contact in this.IQueryable
                       where contact.OwnerID == ownerID
                       select contact;

            var view = new ClientContactsView(this.Reponsitory, linq);
            return view;
        }

        #endregion
    }
}
