using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.Declare.PackingData
{
    public class BodyDataTable : PackingDataTable
    {
        public BodyDataTable(IEnumerable<DecProduct> items) : base()
        {
            int rownumber = 1;

            var packs = items.Select(t => t.BoxIndex).Distinct().OrderBy(item => item);
            int[] rowmerge = new int[packs.Count()];

            var i = 0;
            foreach (var pack in packs)
            {
                rowmerge[i] = rownumber;

                foreach (var m in items.Where(t => t.BoxIndex == pack))
                {
                    //PdfGridRow row = grid.Rows.Add();
                    DataRow dr = base.dt.NewRow();

                    dr["箱号"] = m.BoxIndex;
                    dr["序号"] = rownumber.ToString();
                    dr["货物名称"] = m.Name;
                    dr["货物型号"] = m.Model;
                    dr["品牌"] = m.Manufacturer;
                    dr["数量(PCS)"] = m.Quantity.ToString("0.####");
                    dr["净重(KGS)"] = m.NetWeight.ToRound(2).ToString("0.##");
                    dr["毛重(KGS)"] = m.GrossWeight.ToRound(2).ToString("0.##");

                    //row.Cells[0].Value = m.BoxIndex;
                    //row.Cells[1].Value = rownumber.ToString();
                    //row.Cells[2].Value = m.Name;
                    //row.Cells[3].Value = m.Model;
                    //row.Cells[4].Value = m.Manufacturer;
                    //row.Cells[5].Value = m.Quantity.ToString("0.####");
                    //row.Cells[6].Value = m.NetWeight.ToRound(2).ToString("0.##");
                    //row.Cells[7].Value = m.GrossWeight.ToRound(2).ToString("0.##");

                    ++rownumber;
                    //row.Style.Font = new PdfTrueTypeFont(new Font("SimSun", 7f, FontStyle.Regular), true);
                    base.dt.Rows.Add(dr);
                }
                ++i;
            }






            //for (int i = 0; i < 26; i++)
            //{
            //    DataRow dr = base.dt.NewRow();
            //    dr["箱号"] = "001";
            //    dr["序号"] = i + 1;
            //    dr["货物名称"] = "片式多层瓷介电容器";
            //    dr["货物型号"] = "ABM10-27.000MHZ-E20-T";
            //    dr["品牌"] = "Panasonic";
            //    dr["数量(PCS)"] = "4500";
            //    dr["净重(KGS)"] = "0.34";
            //    dr["毛重(KGS)"] = "3.23";
            //    base.dt.Rows.Add(dr);
            //}

            //for (int i = 26; i < 50; i++)
            //{
            //    DataRow dr = base.dt.NewRow();
            //    dr["箱号"] = "002";
            //    dr["序号"] = i + 1;
            //    dr["货物名称"] = "片式多层瓷介电容器";
            //    dr["货物型号"] = "ABM10-27.000MHZ-E20-T";
            //    dr["品牌"] = "Panasonic";
            //    dr["数量(PCS)"] = "4500";
            //    dr["净重(KGS)"] = "0.34";
            //    dr["毛重(KGS)"] = "4.89";
            //    base.dt.Rows.Add(dr);
            //}
        }
    }
}
