using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.Declare.PackingData
{
    public class FooterDataTable : PackingDataTable
    {
        public FooterDataTable(string 数量, string 净重, string 毛重)
        {
            DataRow dr = base.dt.NewRow();
            dr["箱号"] = "合计:";
            dr["序号"] = "";
            dr["货物名称"] = "";
            dr["货物型号"] = "";
            dr["品牌"] = "";
            dr["数量(PCS)"] = 数量;
            dr["净重(KGS)"] = 净重;
            dr["毛重(KGS)"] = 毛重;
            base.dt.Rows.Add(dr);
        }
    }
}
