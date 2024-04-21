using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class MyContactsRoll : Origins.ContactsOrigin
    {
        IErpAdmin Admin;
        
        public MyContactsRoll(IErpAdmin admin)
        {
            this.Admin = admin;

        }
        protected override IQueryable<Models.Origins.Contact> GetIQueryable()
        {
            return from entity in base.GetIQueryable()
                   where entity.OwnerID==this.Admin.ID
                   select entity;
        }
        public IQueryable<Contact> this[string EnterpriseID, RelationType relationType]
        {
            get
            {
                return
                    this.Where(
                        item => item.EnterpriseID == EnterpriseID && item.RelationType == relationType);
            }
        }
        /// <summary>
        ///过滤多个枚举
        /// </summary>
        /// <param name="EnterpriseID"></param>
        /// <param name="relationTypes"></param>
        /// <returns></returns>
        //public IQueryable<Contact> this[string EnterpriseID, RelationType[] relationTypes]
        //{
        //    get
        //    {
        //        return
        //            this.Where(
        //                item => item.EnterpriseID == EnterpriseID && relationTypes.Contains(item.RelationType));
        //    }

        //}
    }
}
