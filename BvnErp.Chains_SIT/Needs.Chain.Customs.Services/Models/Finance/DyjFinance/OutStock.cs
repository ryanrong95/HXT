using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.ApiSettings;
using Needs.Underly;
using Needs.Utils.Serializers;
using Newtonsoft.Json;

namespace Needs.Ccs.Services.Models
{
    public class OutStock
    {
        public InvoiceNotice notice;
        public Admin admin;

        public OutStock() { }

        public OutStock(InvoiceNotice notice,Admin admin) {
            this.notice = notice;
            this.admin = admin;
        }

        public string PushOutStock() 
        {
            var message = "";
            try {
                var model = new OutStockData();
                model.XML标识 = "";
                model.发票标识 = notice.ID;


                var SendJson = new
                {
                    request_service = OutStockApiSetting.request_service,
                    request_item = OutStockApiSetting.request_item,
                    token = OutStockApiSetting.token,
                    data = model,
                };

                var apiurl = System.Configuration.ConfigurationManager.AppSettings[OutStockApiSetting.ApiName] + OutStockApiSetting.OutStockHandle;

                var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, SendJson);

                var jResult = JsonConvert.DeserializeObject<OutStockResult>(result);

                using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {


                    //成功，更新apiNotice
                    if (jResult.success && jResult.data)
                    {
                        //写开票通知推送日志
                        responsitory.Insert(new Layer.Data.Sqls.ScCustoms.InvoiceNoticeLogs
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            InvoiceNoticeID = notice.ID,
                            AdminID = admin.ID,
                            Status = 0,
                            CreateDate = DateTime.Now,
                            Summary = "财务人员[" + admin.RealName + "]操作出库"
                        });

                        responsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNotices>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            IsExStock = true
                        }, item => item.ID == notice.ID);


                    }
                    else
                    {
                        responsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNotices>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            IsExStock = false
                        }, item => item.ID == notice.ID);
                    }
                }

                message += notice.ID + jResult.msg;
            }
            catch (Exception ex) {
                ex.CcsLog("财务出库调用大赢家");
            }

            return message;
        }

    }

    public class OutStockModel
    {
        public string request_service { get; set; }
        public string request_item { get; set; }
        public string token { get; set; }
        public PurPriceData data { get; set; }
    }

    public class OutStockData
    {

        public string XML标识 { get; set; }

        public string 发票标识 { get; set; }

    }


    public class OutStockResult
    {
        public bool success { get; set; }

        public int status_code { get; set; }

        public bool data { get; set; }

        public string msg { get; set; }

        public string user_host_address { get; set; }
    }
}
