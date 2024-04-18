using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.OrderChange
{
    public partial class ArrivalEdit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            string ID = Request.QueryString["ID"];
        }

        /// <summary>
        /// 加载产品信息
        /// </summary>
        protected void data()
        {
            var OrderChangeNoticeID = Request.QueryString["ID"];
            var OrderID = Request.QueryString["OrderID"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.OrderChangeNoticeLogsListView.OrderChangeNoticeLogsListModel, bool>>)(t => t.OrderID == OrderID));

            var logs = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderChangeNoticeLogsList.GetCommon(lamdas.ToArray()).AsQueryable();

            List<OrderChangeNoticeLog> orderItemMatchs = new List<OrderChangeNoticeLog>();
            foreach (var t in logs)
            {
                OrderChangeNoticeLog p = new OrderChangeNoticeLog();
                p.OrderID = t.OrderID;
                p.Summary = t.Summary;
                orderItemMatchs.Add(p);
            }

            Response.Write(new
            {
                rows = orderItemMatchs.ToArray(),
                total = orderItemMatchs.Count()
            }.Json());

        }

        protected void Post2Client()
        {
            try
            {
                string OrderID = Request.Form["OrderID"];

                var productList = new Needs.Ccs.Services.Views.OrderItemChangeNoticesView();
                List<LambdaExpression> lamdas = new List<LambdaExpression>();
                Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> expression = t => t.ProcessState != Needs.Ccs.Services.Enums.ProcessState.Processed;
                Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> lambda1 = t => t.OrderID == OrderID.Trim();
                lamdas.Add(lambda1);

                var products = productList.GetPageList(1, 20, expression, lamdas.ToArray());

                var Order = new OrdersView().Where(item => item.ID == OrderID).FirstOrDefault();
                int UnClassifiedCount = Order.Items.Where(t => t.ClassifyStatus != Needs.Ccs.Services.Enums.ClassifyStatus.Done).Count();

                if (products.Count() > 0 || UnClassifiedCount > 0)
                {
                    Response.Write(new { result = false, info = "该订单还有未处理的重新归类信息或有未归类产品，不能确认!" }.Json());
                }
                else
                {
                    var orderChange = new Needs.Ccs.Services.Views.OrderChangeView().GetTop(1, x => x.OrderID == OrderID).FirstOrDefault();
                    //更新OrderChanges表的状态;
                    orderChange.UpdateProcessState();
                    orderChange.UpdateDeclareFlag();

                    string[] mainOrderID = OrderID.Split('-');

                    var Orders = new Orders2View().Where(item => item.MainOrderID == mainOrderID[0] &&
                                                                 item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Canceled &&
                                                                 item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Returned).ToList();

                    List<TinyOrderDeclareFlags> declareFlags = new List<TinyOrderDeclareFlags>();

                    foreach (var order in Orders)
                    {
                        if (order.ID != OrderID)
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
                    thistinyOrder.TinyOrderID = OrderID;
                    thistinyOrder.IsDeclare = true;
                    declareFlags.Add(thistinyOrder);

                    var confirm = new ClientConfirm();
                    confirm.OrderID = mainOrderID[0];
                    confirm.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                    confirm.Type = ConfirmType.DeliveryConfirm;
                    confirm.DeclareFlags = declareFlags;

                    var apisetting = new Needs.Ccs.Services.ApiSettings.PvWsOrderApiSetting();
                    var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.ClientConfirm;

                    Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        OrderID = mainOrderID[0],
                        TinyOrderID = OrderID,
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
                        log.OrderID = OrderID;
                        //log.Admin = Needs.Wl.Admin.Plat.AdminPlat.Current;
                        log.OrderStatus = Needs.Ccs.Services.Enums.OrderStatus.QuoteConfirmed;
                        log.Summary = "到货异常处理推送代仓储失败:" + message.data;
                        log.Enter();
                    }

                    Order.OrderStatus = Needs.Ccs.Services.Enums.OrderStatus.QuoteConfirmed;
                    Order.UpdateOrderStatus();
                    var current = Needs.Wl.Admin.Plat.AdminPlat.Current;
                    Admin admin = new Admin
                    {
                        ID = current.ID,
                        RealName = current.RealName
                    };
                    Order.SetAdmin(admin);
                    Order.CancelHangUp();

                    Order.GenerateBill(Order.OrderBillType, Order.PointedAgencyFee);
                    Response.Write(new { result = true, info = "确认成功" }.Json());
                }
            }
            catch(Exception ex)
            {
                Response.Write(new { result = false, info = "确认失败:"+ex.ToString() }.Json());
            }
            
        }       
    }
}