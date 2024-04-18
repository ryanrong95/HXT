using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class MatchPost2ClientDirectConfirm
    {
        public Order CurrentOrder { get; set; }
        public MatchPost2ClientDirectConfirm(Order currentOrder)
        {
            this.CurrentOrder = currentOrder;
        }
        public void Post()
        {
            try
            {
                var Orders = new Views.Orders2View().Where(item => item.MainOrderID == this.CurrentOrder.MainOrderID &&
                                                          item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Canceled &&
                                                          item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Returned).ToList();

                List<TinyOrderDeclareFlags> declareFlags = new List<TinyOrderDeclareFlags>();

                foreach (var order in Orders)
                {
                    if (order.ID != CurrentOrder.ID)
                    {
                        TinyOrderDeclareFlags tinyOrder = new TinyOrderDeclareFlags();
                        tinyOrder.TinyOrderID = order.ID;
                        tinyOrder.IsDeclare = false;
                        if (order.DeclareFlag == Needs.Ccs.Services.Enums.DeclareFlagEnums.Able)
                        {
                            tinyOrder.IsDeclare = true;
                        }
                        declareFlags.Add(tinyOrder);
                    }
                }

                TinyOrderDeclareFlags thistinyOrder = new TinyOrderDeclareFlags();
                thistinyOrder.TinyOrderID = CurrentOrder.ID;
                thistinyOrder.IsDeclare = true;
                declareFlags.Add(thistinyOrder);

                var ermAdminID = new Needs.Ccs.Services.Views.AdminsTopView2().FirstOrDefault(x => x.OriginID == CurrentOrder.Client.Merchandiser.ID)?.ID;

                var confirm = new ClientConfirm();
                confirm.OrderID = CurrentOrder.MainOrderID;
                confirm.AdminID = ermAdminID;
                confirm.Type = ConfirmType.DirectConfirm;
                confirm.DeclareFlags = declareFlags;

                var apisetting = new Needs.Ccs.Services.ApiSettings.PvWsOrderApiSetting();
                var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.ClientConfirm;

                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    OrderID = CurrentOrder.MainOrderID,
                    TinyOrderID = CurrentOrder.ID,
                    Url = apiurl,
                    RequestContent = confirm.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                };
                apiLog.Enter();


                var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, confirm);
                apiLog.ResponseContent = result;
                apiLog.Enter();

                var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Needs.Underly.JMessage>(result);

                if (message.code != 200)
                {
                    OrderLog log = new OrderLog();
                    log.OrderID = CurrentOrder.ID;
                    //log.Admin = Needs.Wl.Admin.Plat.AdminPlat.Current;
                    log.OrderStatus = Needs.Ccs.Services.Enums.OrderStatus.QuoteConfirmed;
                    log.Summary = "到货异常处理推送代仓储失败:" + message.data;
                    log.Enter();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
