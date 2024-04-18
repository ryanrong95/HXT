using Yahv.Models;
using Layers.Data.Sqls;
using Yahv.Linq;
using System.Linq;
using Yahv.Underly;

namespace Yahv.Views
{
    class StaffsAlls : QueryView<ErmStaff, PvbErmReponsitory>
    {
        protected internal StaffsAlls()
        {
        }

        internal StaffsAlls(PvbErmReponsitory reponsitory) : base(reponsitory)
        {
        }


        protected override IQueryable<ErmStaff> GetIQueryable()
        {
            return from staff in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Staffs>()
                   select new ErmStaff
                   {
                       ID = staff.ID,
                       DYJID = staff.DyjCode
                   };
        }
    }
}

