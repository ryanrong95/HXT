using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService.Services.DBS
{
    public class DBSAccountFlow
    {
        public string RequestUrl { get; set; }
        public string OrgId { get; set; }
        public string AccountNo { get; set; }
        public string ccy { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public void GetAccountFlow()
        {
            try
            {               
                string requesturl = RequestUrl;

                ARERequest aRERequest = new ARERequest();
                aRERequest.header = new AREHeader();
                aRERequest.header.msgId = ChainsGuid.NewGuidUp();
                aRERequest.header.orgId = OrgId;
                aRERequest.header.timeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                aRERequest.header.ctry = DBSConstConfig.DBSConstConfiguration.Ctry;

                aRERequest.accInfo = new AREAccInfo();
                aRERequest.accInfo.accountNo = AccountNo;
                aRERequest.accInfo.accountCcy = ccy;
                aRERequest.accInfo.fromDate = fromdate;
                aRERequest.accInfo.toDate = todate;

                HttpPostRequest request = new HttpPostRequest();
                request.Timeout = Needs.Ccs.Services.Models.DBSConstConfig.DBSConstConfiguration.TimeOut;
                request.ContentType = "application/json";
                var result = request.Post(RequestUrl, aRERequest.Json());

            }
            catch(Exception ex) 
            {

            }
        }
    }
}
