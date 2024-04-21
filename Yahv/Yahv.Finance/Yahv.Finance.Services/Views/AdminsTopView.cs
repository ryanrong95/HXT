using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views
{

    public class AdminsTopView : UniqueView<Admin, PvFinanceReponsitory>
    {
        public AdminsTopView()
        {

        }

        public AdminsTopView(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Admin> GetIQueryable()
        {
            return new Yahv.Services.Views.AdminsAll<PvFinanceReponsitory>(this.Reponsitory);
        }

        public IQueryable<Admin> GetApplyAdmins()
        {
            var roleIds = new string[]
            {
                FixedRole.NewStaff.GetFixedID(),
                FixedRole.Npc.GetFixedID(),
            };

            return this.Where(item => !roleIds.Contains(item.RoleID) && item.Status != AdminStatus.Closed && item.StaffID != null);
        }

        public IQueryable<Admin> GetApproveAdmins()
        {
            var roleIds = new string[]
            {
                FixedRole.NewStaff.GetFixedID(),
                FixedRole.Npc.GetFixedID(),
            };

            return this.Where(item => !roleIds.Contains(item.RoleID) && item.Status != AdminStatus.Closed);
        }
    }
}
