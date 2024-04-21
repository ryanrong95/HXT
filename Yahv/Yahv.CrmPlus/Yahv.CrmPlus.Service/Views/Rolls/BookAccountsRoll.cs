using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class BookAccountsRoll : Origins.BookAccountsOrigin
    {

        IErpAdmin Admin;

        public BookAccountsRoll(IErpAdmin admin)
        {
            this.Admin = admin;

        }
        protected override IQueryable<Models.Origins.BookAccount> GetIQueryable()
        {
            return base.GetIQueryable();
        }
        public IQueryable<BookAccount> this[string EnterpriseID, RelationType relationType]
        {
            get
            {
                return
                    this.Where(
                        item => item.EnterpriseID == EnterpriseID && item.RelationType == relationType);
            }
        }
        public IQueryable<BookAccount> this[string EnterpriseID, RelationType relationType, BookAccountType type]
        {
            get
            {
                return
                    this.Where(
                        item => item.EnterpriseID == EnterpriseID && item.RelationType == relationType && item.BookAccountType == type);
            }
        }
    }
}
