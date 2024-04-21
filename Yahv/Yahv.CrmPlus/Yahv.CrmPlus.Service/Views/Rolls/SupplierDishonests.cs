using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Models.Origins.Rolls;
using Yahv.Linq;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class SupplierDishonestsRoll : Origins.SupplierDishonestsOrigin
    {
        public SupplierDishonestsRoll()
        {
        }

        public SupplierDishonestsRoll(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<SupplierDisHonest> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.Status == DataStatus.Normal);
        }
    }
}
