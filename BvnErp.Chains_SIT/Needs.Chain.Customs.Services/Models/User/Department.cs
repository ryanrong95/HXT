using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class Department : IUnique
    {
        public string ID { get; set; }

        public string FatherID { get; set; }

        public string Name { get; set; }

        public string LeaderID { get; set; }

        public Enums.Status Status { get; set; }

        public int? OrderIndex { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }




        public void SetDepartmentLeader()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                bool exsit = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Departments>().
                    Any(item => item.ID == this.ID);
                if (exsit)
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.Departments
                    {
                        ID = this.ID,
                        LeaderID = this.LeaderID,
                    }, item => item.ID == this.ID);
                }
            }
        }
    }
}
