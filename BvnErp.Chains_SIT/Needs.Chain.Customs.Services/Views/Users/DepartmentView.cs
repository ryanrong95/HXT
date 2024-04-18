using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DepartmentView : UniqueView<Models.Department, ScCustomsReponsitory>
    {
        public DepartmentView()
        {
        }

        internal DepartmentView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Department> GetIQueryable()
        {
            return from depart in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Departments>()
                   where depart.Status == (int)Enums.Status.Normal
                   orderby depart.OrderIndex
                   select new Models.Department
                   {
                       ID = depart.ID,
                       FatherID = depart.FatherID,
                       Name = depart.Name,
                       LeaderID = depart.LeaderID,
                       OrderIndex = depart.OrderIndex,
                       Status = (Enums.Status)depart.Status,
                       CreateDate = depart.CreateDate,
                       Summary = depart.Summary
                   };
        }
    }
}
