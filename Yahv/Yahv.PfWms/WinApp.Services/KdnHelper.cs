using Kdn.Library;
using Kdn.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApp.Services.Controls;
using Yahv.Utils.Converters;
using Yahv.Utils.Extends;
using Yahv.Utils.Serializers;

namespace WinApp.Services
{
    public class KdnHelper
    {
        static public string GetCorrect(string shipperCode)
        {
            switch (shipperCode)
            {
                case ShipperCode.SF:
                    //return Properties.Resource.correctKdnSf;
                    #region 高会航测试用
                    return Properties.Resource.correctKdnSf1;
                #endregion
                case ShipperCode.KYSY:

                    int SW1 = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                    switch (SW1)
                    {
                        case 1920:
                            return $"<style>{ Properties.Resource.kysy_1920}</style>";
                        case 1440:
                            return $"<style>{ Properties.Resource.kysy_1440}</style>";
                        default:
                            return null;
                    }
                //return Properties.Resource.correctKdnKysy;
                default:
                    return null;
            }
        }

        static public string GetPrinterName(string shipperCode)
        {
            switch (shipperCode)
            {
                case ShipperCode.SF:
                    return PrinterConfigs.Current[PrinterConfigs.快递鸟顺丰].PrinterName;

                case ShipperCode.KYSY:
                    return PrinterConfigs.Current[PrinterConfigs.快递鸟跨越速递].PrinterName;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 打印面单
        /// </summary>
        /// <param name="shipperCode">快递商编号，请使用ShipperCode类中的常量</param>
        /// <param name="expType">快递类型，请使用与快递商号匹配的CodeType的派生类中的常量</param>
        /// <param name="sender">发货人</param>
        /// <param name="receiver">收货人</param>
        /// <param name="quantity">快递件数</param>
        /// <param name="commodities">产品列表可选</param>
        /// <param name="remark">客户备注</param>
        /// <param name="volume">体积</param>
        /// <param name="weight">重量</param>
        /// <remarks>
        /// 暂时：只支持顺丰、跨越
        /// </remarks>
        static public void FacePrint(string shipperCode, int expType, int exPayType, Sender sender, Receiver receiver, int quantity, string remark
            , double? volume, double? weight
            , params Commodity[] commodities)
        {

            //未实现
            //throw new Exception("8");
            string printerName = GetPrinterName(shipperCode);

            if (!PrinterConfigs.Connected(printerName))
            {
                MessageBox.Show($"请配置快递鸟打印机!");
                return;
            }

            if (string.IsNullOrWhiteSpace(printerName))
            {
                MessageBox.Show($"请配置快递鸟打印机!");
                return;
            }

            string correct = GetCorrect(shipperCode);
            if (string.IsNullOrWhiteSpace(correct))
            {
                MessageBox.Show($"快递鸟接口目前支持：{string.Join(",", new ShipperCode())}，请重试!");
                return;
            }

            #region 请求数据
            var request = new KdnRequest
            {
                OrderCode = string.Concat("WS", "-", shipperCode, "-", expType, "-", quantity, "-", DateTime.Now.LinuxTicks()),
                Quantity = shipperCode == ShipperCode.SF ? 1 : quantity,
                ShipperCode = shipperCode,
                //CustomerName = expressCompany.CustomerName,
                //CustomerPwd = expressCompany.CustomerPwd,
                //MonthCode = expressCompany.MonthCode,
                PayType = (PayType)exPayType,
                ExpType = expType,
                Cost = 0,
                OtherCost = 0,
                Sender = sender,
                Receiver = receiver,
                Commodity = commodities,
                Remark = remark,
                TemplateSize = shipperCode == ShipperCode.SF ? "21001" : "210",/*"21001"*/
                IsReturnPrintTemplate = "1",
                Volume = volume,
                Weight = weight,

            };
            #endregion

            //调用快递鸟方法
            var eOrder = new KdApiEOrder();
            eOrder.Requesting += EOrder_Requesting;
            eOrder.Responsed += EOrder_Responsed;

            var result = eOrder.orderTracesSubByJson(request);

            if (!result.Success)
            {
                return;
            }

            //获取返回结果
            var html = result.PrintTemplate.ToKdnHalfAngle();
            PrintKdnForm form = new PrintKdnForm();
            form.PrinterName = printerName;
            form.Show();

            form.Html(html, correct);
        }



        private static void EOrder_Requesting(object sender, RequestingEventArgs e)
        {
            //临时写的示例 ， 开发为接口调用
            //var request = e.Request;
            //string url = "";

            //Yahv.Utils.Http.ApiHelper.Current.JPost(url, new Yahv.Services.Models.KdnRequest
            //{
            //    OrderCode = request.OrderCode,
            //    ExpType = request.ExpType,
            //    Quantity = request.Quantity,
            //    PayType = (Yahv.Services.Models.PayType)request.PayType,
            //    MonthCode = request.MonthCode,
            //    SenderAddress = request.Sender.Address,
            //    SenderCompany = request.Sender.Company,
            //    SenderName = request.Sender.Name,
            //    SenderMobile = request.Sender.Mobile,
            //    SenderTel = request.Sender.Tel,
            //    ReceiverAddress = request.Receiver.Address,
            //    ReceiverCompany = request.Receiver.Company,
            //    ReceiverName = request.Receiver.Name,
            //    ReceiverMobile = request.Receiver.Mobile,
            //    ReceiverTel = request.Receiver.Tel,
            //    Remark = request.Remark,
            //    Currency = Yahv.Underly.Currency.CNY, // 现在只
            //    Cost = (decimal)request.Cost,
            //    OtherCost = (decimal)request.OtherCost,
            //});


            //using (var reponsitory = new KdnRequestTopView())
            //{
            //    reponsitory.Enter(new Yahv.Services.Models.KdnRequest
            //    {
            //        OrderCode = request.OrderCode,
            //        ExpType = request.ExpType,
            //        Quantity = request.Quantity,
            //        PayType = (Yahv.Services.Models.PayType)request.PayType,
            //        MonthCode = request.MonthCode,
            //        SenderAddress = request.Sender.Address,
            //        SenderCompany = request.Sender.Company,
            //        SenderName = request.Sender.Name,
            //        SenderMobile = request.Sender.Mobile,
            //        SenderTel = request.Sender.Tel,
            //        ReceiverAddress = request.Receiver.Address,
            //        ReceiverCompany = request.Receiver.Company,
            //        ReceiverName = request.Receiver.Name,
            //        ReceiverMobile = request.Receiver.Mobile,
            //        ReceiverTel = request.Receiver.Tel,
            //        Remark = request.Remark,
            //        Currency = Yahv.Underly.Currency.CNY, // 现在只
            //        Cost = (decimal)request.Cost,
            //        OtherCost = (decimal)request.OtherCost,
            //    });
            //}
        }

        private static void EOrder_Responsed(object sender, ResponsedEventArgs e)
        {
            //临时写的示例 ， 开发为接口调用

            var result = e.Result;
            var request = e.Request;

            if (!result.Success)
            {
                //以外情况处理
                MessageBox.Show($"快递鸟接口请求失败：{result.Reason}，请重试!");
                return;
            }

            string url = $"{Config.ApiUrlPrex}/Kdn/Enter";

            Yahv.Utils.Http.ApiHelper.Current.JPost(url, new
            {


                Request = new Yahv.Services.Models.KdnRequest
                {
                    OrderCode = request.OrderCode,
                    ExpType = request.ExpType,
                    Quantity = request.Quantity,
                    PayType = (Yahv.Services.Models.PayType)request.PayType,
                    MonthCode = request.MonthCode,
                    SenderAddress = request.Sender.Address,
                    SenderCompany = request.Sender.Company,
                    SenderName = request.Sender.Name,
                    SenderMobile = request.Sender.Mobile,
                    SenderTel = request.Sender.Tel,
                    ReceiverAddress = request.Receiver.Address,
                    ReceiverCompany = request.Receiver.Company,
                    ReceiverName = request.Receiver.Name,
                    ReceiverMobile = request.Receiver.Mobile,
                    ReceiverTel = request.Receiver.Tel,
                    Remark = request.Remark,
                    Currency = Yahv.Underly.Currency.CNY, // 现在只
                    Cost = (decimal)request.Cost,
                    OtherCost = (decimal)request.OtherCost,
                    ShipperCode = request.ShipperCode
                },
                Result = new Yahv.Services.Models.KdnResult
                {
                    OrderCode = result.Order.OrderCode,
                    LogisticCode = result.Order.LogisticCode,
                    KDNOrderCode = result.Order.KDNOrderCode,
                    DestinatioCode = result.Order.DestinatioCode,
                    OriginCode = result.Order.OriginCode,
                    ShipperCode = result.Order.ShipperCode,
                    Html = result.PrintTemplate
                }
            });



            //using (var reponsitory = new KdnResultTopView())
            //{
            //    reponsitory.Enter(new Yahv.Services.Models.KdnResult
            //    {
            //        OrderCode = result.Order.OrderCode,
            //        LogisticCode = result.Order.LogisticCode,
            //        KDNOrderCode = result.Order.KDNOrderCode,
            //        DestinatioCode = result.Order.DestinatioCode,
            //        OriginCode = result.Order.OriginCode,
            //        ShipperCode = result.Order.ShipperCode,
            //    });
            //}

            Gecko.GeckoWebBrowser firefox = SimHelper.Firefox;

            if (firefox == null)
            {
                return;
            }

            using (Gecko.AutoJSContext context = new Gecko.AutoJSContext(firefox.Window))
            {
                string _result;
                context.EvaluateScript($"this['KdnPrinted']('{result.Order.LogisticCode}');", firefox.Window.DomWindow, out _result);
            }
        }
    }
}

