using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.ApiSettings
{
    public class DBSApiSetting
    {
        public string ApiName { get; set; }
        public string ABE { get; set; }
        public string FXPricing { get; set; }
        public string FXBooking { get; set; }
        public string ACT { get; set; }
        public string TT { get; set; }
        public string CNAPS { get; set; }
        public string ARE { get; set; }

        public DBSApiSetting()
        {
            this.ApiName = "DBSApiUrl";
            this.ABE = "dbs/ABE";
            this.FXPricing = "dbs/FXPricing";
            this.FXBooking = "dbs/FXBooking";
            this.ACT = "dbs/ACT";
            this.TT = "dbs/TT";
            this.CNAPS = "dbs/CNAPS";
            this.ARE = "dbs/ARE";
        }
    }
}
