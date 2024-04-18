using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseDomesticPortView : UniqueView<Models.BaseDomesticPort, ScCustomsReponsitory>
    {
        public BaseDomesticPortView()
        { }

        internal BaseDomesticPortView(ScCustomsReponsitory reponsitory) : base(reponsitory)
            { }

        protected override IQueryable<BaseDomesticPort> GetIQueryable()
        {
            //return from unit in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseDomesticPorts>()
            //       select new Models.BaseDomesticPort
            //       {
            //           ID = unit.ID,
            //           Code = unit.Code,
            //           Name = unit.Name,
            //       };
            return null;
        }
    }
}
