using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.Declare.PackingData
{
    public class TitleDataTable : PackingDataTable
    {
        public TitleDataTable() : base()
        {
            DataRow dr = base.dt.NewRow();
            dr["箱号"] = "箱号";
            dr["序号"] = "序号";
            dr["货物名称"] = "货物名称";
            dr["货物型号"] = "货物型号";
            dr["品牌"] = "品牌";
            dr["数量(PCS)"] = "数量(PCS)";
            dr["净重(KGS)"] = "净重(KGS)";
            dr["毛重(KGS)"] = "毛重(KGS)";
            base.dt.Rows.Add(dr);
        }
    }
}
