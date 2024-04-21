using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using YaHv.CrmPlus.Service.Views.Origins;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class AddressesRoll : AddressesOrigin
    {

        //Enterprise enterprise;
        //RelationType relationType;
        //public AddressesRoll()
        //{


        //}

        //public AddressesRoll(Enterprise enterprise, RelationType type)
        // {
        //     this.enterprise = enterprise;
        //     this.relationType = type;

        // }
        //protected override IQueryable<Models.Origins.Address> GetIQueryable()
        //{
        //    return from item in base.GetIQueryable()
        //           where item.EnterpriseID == this.enterprise.ID && item.RelationType == this.relationType
        //           select item;
        //}

        protected override IQueryable<Models.Origins.Address> GetIQueryable()
        {
            return  base.GetIQueryable();
                
        }
        public IQueryable<Models.Origins.Address> this[string enterpriseid, RelationType type]
        {
            get {
                return this.Where(x => x.EnterpriseID == enterpriseid && x.RelationType == type);
        }
        }
        public IQueryable<Models.Origins.Address> this[string enterpriseid, RelationType type,AddressType addressType]
        {
            get
            {
                return this.Where(x => x.EnterpriseID == enterpriseid && x.RelationType == type&&x.AddressType==addressType);
            }
        }
    }
}
