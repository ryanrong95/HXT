using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class CommonTabStru
    {
        public string ColVarchar { get; set; }

        public string ColVarcharNull { get; set; }

        public string ColNvarchar { get; set; }

        public string ColNvarcharNull { get; set; }

        public int ColInt { get; set; }

        public int? ColIntNull { get; set; }

        public DateTime ColDatetimeReal
        {
            get { return DateTime.Parse(this.ColDatetime); }
        }

        public DateTime? ColDatetimeNullReal
        {
            get { return this.ColDatetimeNull != null ? (DateTime?)DateTime.Parse(this.ColDatetimeNull) : null; }
        }

        public decimal ColDecimal { get; set; }

        public decimal? ColDecimalNull { get; set; }

        public bool ColBit { get; set; }

        public bool? ColBitNull { get; set; }

        public DateTime ColDateReal
        {
            get { return DateTime.Parse(this.ColDate); }
        }

        public DateTime? ColDateNullReal
        {
            get { return this.ColDateNull != null ? (DateTime?)DateTime.Parse(this.ColDateNull) : null; }
        }

        public long ColBigint { get; set; }

        public long? ColBigintNull { get; set; }

        #region 接收参数

        public string ColDatetime { get; set; }

        public string ColDatetimeNull { get; set; }

        public string ColDate { get; set; }

        public string ColDateNull { get; set; }

        #endregion
    }

    #region Insert Model

    public class NewTabModel : CommonTabStru
    {
        public string MainID0 { get; set; }
    }

    #endregion

    public class PriKeyModel
    {
        public string MainID0 { get; set; }
    }

    #region Update Model

    public class UpdateModel
    {
        public NewTabModel NewTabObj { get; set; }

        public PriKeyModel[] PriKeyObjs { get; set; }
    }

    #endregion
}