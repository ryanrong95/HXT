using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.Declare.PackingData
{
    public class PackingDataTable
    {
        protected DataTable dt = new DataTable();

        public PackingDataTable()
        {
            dt.Columns.Add("箱号", typeof(string));
            dt.Columns.Add("序号", typeof(string));
            dt.Columns.Add("货物名称", typeof(string));
            dt.Columns.Add("货物型号", typeof(string));
            dt.Columns.Add("品牌", typeof(string));
            dt.Columns.Add("数量(PCS)", typeof(string));
            dt.Columns.Add("净重(KGS)", typeof(string));
            dt.Columns.Add("毛重(KGS)", typeof(string));
        }

        public int ColumnsCount
        {
            get { return dt.Columns.Count; }
        }

        public DataRowCollection Rows
        {
            get { return dt.Rows; }
        }
    }
}
