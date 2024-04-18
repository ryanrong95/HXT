using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.Bill
{
    /// <summary>
    /// 修改海关汇率/实时汇率
    /// </summary>
    public partial class ExchangeRateEdit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            List<ReGenerateBill> bills = new List<ReGenerateBill>();
            var OrderIDs = new Orders2View().Where(item => item.MainOrderID == id && item.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Quoted
                            && item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Returned && item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Canceled)
                           .Select(item => item.ID).ToList();
            foreach (var orderid in OrderIDs)
            {
                ReGenerateBill item = new ReGenerateBill();
                var bill = new Needs.Ccs.Services.Views.OrderBillsView2(orderid).FirstOrDefault();
                item.OrderID = orderid;
                item.CustomsExchangeRate = bill.CustomsExchangeRate;
                item.RealExchangeRate = bill.RealExchangeRate;
                item.OrderBillType = bill.Order.OrderBillType;
                item.RealAgencyFee = bill.Order.OrderBillType == OrderBillType.Pointed ? bill.AgencyFee : 0;
                bills.Add(item);
            }

            this.Model.ExchangeRateValue = bills.Json();
            this.Model.MainOrderID = id;
        }

        /// <summary>
        /// 变更汇率
        /// </summary>
        protected void ChangeRate()
        {
            try
            {                
                string mainOrderID = Request.Form["MainOrderID"];

                string FormData = Request.Form["Orders"].Replace("&quot;", "\"");
                FormData = "{\"Model\":" + FormData + "}";
                JObject jsonObject = (JObject)JToken.Parse(FormData);
                var models = JsonConvert.DeserializeObject<IEnumerable<ReGenerateBill>>(jsonObject["Model"].ToString());

                string orderid = "";
                string approveOnOff = ConfigurationManager.AppSettings["ApproveOnOff"];
                List<Needs.Ccs.Services.Models.OneTinyOrderInfo> tinyOrderInfos = new List<OneTinyOrderInfo>();
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                GroupPremiumsHelp[] group_premiums;
                var modelOrderIDs = models.Select(t => t.OrderID).ToList();

                //using (var orderBillView = new Needs.Ccs.Services.Views.OrderPremiumsView())
                using (var orderBillView = new Needs.Ccs.Services.Views.Origins.OrderPremiumsOrigin())
                {
                    group_premiums = (from item in orderBillView
                                      where modelOrderIDs.Contains(item.OrderID) && item.Status == Needs.Ccs.Services.Enums.Status.Normal
                                      group item by new { item.OrderID, item.Type } into groups
                                      select new GroupPremiumsHelp
                                      {
                                          OrderID = groups.Key.OrderID,
                                          Type = groups.Key.Type,
                                          TotalPrice = groups.Sum(item => item.Count * item.UnitPrice * item.Rate)
                                      }).ToArray();
                }              
                foreach (var item in models)
                {                   
                    orderid = item.OrderID;
                    var bill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyMainOrderBills[item.OrderID];
                    
                    bill.OldCustomsExchangeRate = bill.CustomsExchangeRate;  //传递旧的海关汇率
                    bill.OldRealExchangeRate = bill.RealExchangeRate;  //传递旧的实时汇率
                    bill.OldOrderBillType = bill.Order.OrderBillType;  //传递旧的代理费类型
                    //var oldAgencyFeePremiums = bill.Premiums.Where(t => t.Type == OrderPremiumType.AgencyFee).FirstOrDefault();
                    var oldAgencyFeePremiums = group_premiums.Where(t => t.OrderID == item.OrderID && t.Type == OrderPremiumType.AgencyFee).FirstOrDefault();
                    bill.OldAgencyFeeUnitPrice = oldAgencyFeePremiums != null ? oldAgencyFeePremiums.TotalPrice : 0;  //传递旧的代理费单价

                    bill.CustomsExchangeRate = item.CustomsExchangeRate;
                    bill.RealExchangeRate = item.RealExchangeRate;
                    bill.Order.SetAdmin(admin);
                   
                    if (string.IsNullOrEmpty(approveOnOff) || "no" == approveOnOff)
                    {                        
                       bill.MainChangeExchangeRate(item.OrderBillType, item.RealAgencyFee);                                               
                    }
                    else
                    {
                        tinyOrderInfos.Add(new OneTinyOrderInfo()
                        {
                            TinyOrderID = item.OrderID,
                            OldCustomsExchangeRate = bill.OldCustomsExchangeRate,
                            OldRealExchangeRate = bill.OldRealExchangeRate,
                            OldOrderBillTypeInt = (int)bill.OldOrderBillType,
                            OldAgencyFeeUnitPrice = bill.OldAgencyFeeUnitPrice,

                            NewCustomsExchangeRate = item.CustomsExchangeRate,
                            NewRealExchangeRate = item.RealExchangeRate,
                            NewOrderBillTypeInt = (int)item.OrderBillType,
                            NewAgencyFeeUnitPrice = item.RealAgencyFee,                                              
                        });
                    } 
                }
              
                var main = new MainOrdersView().FirstOrDefault(t => t.ID == mainOrderID);
                //外单等待系统执行完毕再返回  ryan 20200702
                if (main != null && main.Type == OrderType.Outside)
                {
                    #region 审批+对账单生成
                    //将收集好的小订单信息用于开始产生审核申请, 可能会自动审批
                    if (!string.IsNullOrEmpty(approveOnOff) && "no" != approveOnOff)
                    {
                        string eventInfo = JsonConvert.SerializeObject(new EventInfoGenerateBill
                        {
                            ApplyAdminName = admin.RealName,
                            MainOrderID = mainOrderID,
                            TinyOrderInfos = tinyOrderInfos,
                        });

                        var attachApproval = new Needs.Ccs.Services.Models.AttachApproval(
                                                    admin.ID,
                                                    Needs.Ccs.Services.Enums.ApprovalType.GenerateBillApproval,
                                                    mainOrderID,
                                                    null,
                                                    null,
                                                    eventInfo);
                        attachApproval.GenerateUnApprovalInfo();  //产生未审批消息

                        if ("auto" == approveOnOff) //自动审批
                        {
                            string approveManID = ConfigurationManager.AppSettings["ApproveManID"];
                            var XDTAdmin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(approveManID);
                            //string referenceInfo = attachApproval.GetReferenceInfoHtmlForGenerateBill(mainOrderID);                         
                            attachApproval.ApproveSuccess(XDTAdmin, attachApproval.ReferenceInfo, isAuto: true);  //审批通过                                                   
                            attachApproval.ExecuteTargetOperation();  //执行目标操作                          
                        }
                    }
                    #endregion
                }
                else
                {
                    //内单异步执行
                    Task.Run(() =>
                    {
                        #region 审批+对账单生成
                        //将收集好的小订单信息用于开始产生审核申请, 可能会自动审批
                        if (!string.IsNullOrEmpty(approveOnOff) && "no" != approveOnOff)
                        {
                            string eventInfo = JsonConvert.SerializeObject(new EventInfoGenerateBill
                            {
                                ApplyAdminName = admin.RealName,
                                MainOrderID = mainOrderID,
                                TinyOrderInfos = tinyOrderInfos,
                            });

                            var attachApproval = new Needs.Ccs.Services.Models.AttachApproval(
                                                        admin.ID,
                                                        Needs.Ccs.Services.Enums.ApprovalType.GenerateBillApproval,
                                                        mainOrderID,
                                                        null,
                                                        null,
                                                        eventInfo);
                            attachApproval.GenerateUnApprovalInfo();  //产生未审批消息

                            if ("auto" == approveOnOff) //自动审批
                            {
                                string approveManID = ConfigurationManager.AppSettings["ApproveManID"];
                                var XDTAdmin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(approveManID);
                                //string referenceInfo = attachApproval.GetReferenceInfoHtmlForGenerateBill(mainOrderID);
                                attachApproval.ApproveSuccess(XDTAdmin, attachApproval.ReferenceInfo, isAuto: true);  //审批通过
                                attachApproval.ExecuteTargetOperation();  //执行目标操作
                            }
                        }
                        #endregion
                    });
                }

                string rtnPageMessage = string.Empty;
                if (string.IsNullOrEmpty(approveOnOff) || "no" == approveOnOff)
                {
                    rtnPageMessage = "汇率修改成功，将重新生成对账单";
                }
                else
                {
                    rtnPageMessage = "待审批通过，即可执行修改汇率，并重新生成对账单";
                }
                ExpireUpdate(modelOrderIDs);
                Response.Write((new { success = true, message = rtnPageMessage }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "汇率修改失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// （注意，有地方反射调用这个方法）
        /// </summary>
        /// <param name="item"></param>
        /// <param name="admin"></param>
        public void DoGenerateBill(ReGenerateBill item, Needs.Ccs.Services.Models.Admin admin)
        {
            var bill = new Needs.Ccs.Services.Views.MainOrderBillsView()[item.OrderID];
            bill.CustomsExchangeRate = item.CustomsExchangeRate;
            bill.RealExchangeRate = item.RealExchangeRate;
            bill.Order.SetAdmin(admin);

            bill.MainChangeExchangeRate(item.OrderBillType, item.RealAgencyFee);           
        }

        private void ExpireUpdate(List<string> orderIDs)
        {
            Task.Run(() =>
            {
                foreach(var item in orderIDs)
                {
                    ExpireUpdate expire = new ExpireUpdate(item);
                    expire.Update();
                }
            });
        }


        /// <summary>
        /// 我的订单项附加费用帮助类
        /// </summary>
        class GroupPremiumsHelp
        {
            /// <summary>
            /// 订单ID
            /// </summary>
            public string OrderID { get; set; }

            /// <summary>
            /// 附加费用类型
            /// </summary>
            public OrderPremiumType Type { get; set; }

            /// <summary>
            /// 总值
            /// </summary>
            public decimal TotalPrice { get; set; }
        }

    }
}