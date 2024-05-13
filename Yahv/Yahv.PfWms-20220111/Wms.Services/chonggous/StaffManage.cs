using Layers.Data.Sqls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.chonggous
{

    public class Staff
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class StaffManage 
    {

        IQueryable<Layers.Data.Sqls.PvWms.Staff> codes;
        public StaffManage()
        {
            using (var repository = new PvWmsRepository())
            {
                var staffs=this.codes = repository.ReadTable<Layers.Data.Sqls.PvWms.Staff>();
            }
        }

        public static Layers.Data.Sqls.PvWms.Staff[] GetData()
        {
            using (var repository = new PvWmsRepository())
            {
                var staffs  = repository.ReadTable<Layers.Data.Sqls.PvWms.Staff>().ToArray();
                return staffs;
            }
        }

        
    }
}
