using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvWsOrder.WebApp.Invoice
{
    public partial class PrintInvoice : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
            }
        }

        protected void LoadComboBoxData()
        {
            //快递公司
            this.Model.ExpressCompanyData = new Yahv.PvWsOrder.Services.Views.ExpressCompanyView().ToArray()
                                                .Select(item => new { Value = item.ID, Text = item.Name }).ToArray();
        }

        protected void ExpressSelect()
        {
            //快递公司ID
            string id = Request.Form["ID"];
            //快递方式
            var expressType = new Yahv.PvWsOrder.Services.Views.ExpressTypeView().Where(item => item.ExpressCompanyID == id).OrderBy(item=>item.TypeValue);
            Response.Write(expressType.Select(item => new { Value = item.TypeValue, Text = item.TypeName }).Json());
        }

        /// <summary>
        /// 生成快递面单
        /// </summary>
        /// <returns></returns>
        protected void GenerateExpress()
        {
            try
            {
                string InvoiceNotice = Request["InvoiceNotice"];
                string[] InvoiceNotices = InvoiceNotice.Split(',');
                string ExpressName = Request["ExpressName"];
                string ExpressType = Request["ExpressType"];
                var invoiceNotices = new Yahv.PvWsOrder.Services.Views.InvoiceNoticeOriginView().Where(item => InvoiceNotices.Contains(item.ID)).ToArray();
                var invoiceNotice = invoiceNotices.First();
                var expressCompany = new Yahv.PvWsOrder.Services.Views.ExpressCompanyView().Where(item => item.ID == ExpressName).FirstOrDefault();

                var serviceManager = new Yahv.PvWsOrder.Services.Views.ServiceManagerByErmClientIDView(invoiceNotice.ClientID, invoiceNotice.FromType).GetServiceManagerInfo(); //业务员信息

                #region KDDRequestModel数据填入

                var requestData = new KDDRequestModel();

                //快递鸟的长度只支持30的长
                requestData.OrderCode = InvoiceNotices[0];
                requestData.Quantity = 1;
                requestData.ShipperCode = expressCompany.Code;
                requestData.CustomerName = expressCompany.CustomerName;
                requestData.CustomerPwd = expressCompany.CustomerPwd;
                requestData.MonthCode = expressCompany.MonthCode;
                requestData.PayType = (int)Yahv.PvWsOrder.Services.Enums.PayType.MonthlyPay;
                requestData.ExpType = int.Parse(ExpressType);
                requestData.Cost = 0;
                requestData.OtherCost = 0;
                requestData.Sender = new Services.Views.Sender()
                {
                    Company = "深圳市芯达通供应链管理有限公司",
                    Name = serviceManager?.RealName,
                    Mobile = serviceManager?.Mobile,
                    ProvinceName = "广东省",
                    CityName = "深圳市",
                    ExpAreaName = "龙岗区",
                    Address = "坂田吉华路393号英达丰科技园",
                };
                requestData.Receiver = requestData.GetReceiver(invoiceNotice.PostAddress, invoiceNotice.Title);
                requestData.Receiver.Company = invoiceNotice.Title;
                requestData.Receiver.Name = invoiceNotice.PostRecipient;
                requestData.Receiver.Mobile = invoiceNotice.PostTel;
                requestData.Commodity = new Services.Views.Commodity[]{
                    new Services.Views.Commodity()
                    {
                        GoodsName="客户发票",
                    }
                };
                requestData.TemplateSize = "210";
                requestData.IsReturnPrintTemplate = "1";

                #endregion

                //获取请求数据json
                string requestdata = requestData.Json();

                //调用快递鸟方法
                var bs = new KdApiEOrder();
                var result = bs.orderTracesSubByJson(requestdata);

                //获取返回结果
                var ResponseResult = result.JsonTo<KDDResultModel>();

                //保存运单数据到数据库
                if (ResponseResult.Success)
                {
                    WayBillCodeUpdater wayBillCodeUpdater = new WayBillCodeUpdater(ResponseResult.Order?.LogisticCode, ExpressName, invoiceNotices.Select(t => t.ID).ToArray());
                    wayBillCodeUpdater.Update();
                }

                //返还数据到前端
                Response.Write(new
                {
                    success = ResponseResult.Success,
                    message = ResponseResult.Reason,
                    LogisticCode = ResponseResult.Order?.LogisticCode,
                    PrintTemplate = ResponseResult.PrintTemplate,
                    ShipperCode = expressCompany.Code
                }.Json());
            }
            catch (Exception ex)
            {
                Response.Write(new { success = false, message = "运单生成失败" + ex.Message, }.Json());
            }
        }

        /// <summary>
        /// 自行调用快递接口
        /// </summary>
        protected void ExpressSelf()
        {
            try
            {
                string InvoiceNotice = Request["InvoiceNotice"];
                string[] InvoiceNotices = InvoiceNotice.Split(',');
                string ExpressName = Request["ExpressName"];
                string ExpressType = Request["ExpressType"];
                var invoiceNotices = new Yahv.PvWsOrder.Services.Views.InvoiceNoticeOriginView().Where(item => InvoiceNotices.Contains(item.ID)).ToArray();
                var invoiceNotice = invoiceNotices.First();
                var expressCompany = new Yahv.PvWsOrder.Services.Views.ExpressCompanyView().Where(item => item.ID == ExpressName).FirstOrDefault();

                var serviceManager = new Yahv.PvWsOrder.Services.Views.ServiceManagerByErmClientIDView(invoiceNotice.ClientID, invoiceNotice.FromType).GetServiceManagerInfo(); //业务员信息


                #region 顺丰

                if (expressCompany.Code == "SF")
                {
                    var sender = new Services.Models.Sender()
                    {
                        Company = "深圳市芯达通供应链管理有限公司",
                        Name = serviceManager?.RealName,
                        Mobile = serviceManager?.Mobile,
                        ProvinceName = "广东省",
                        CityName = "深圳市",
                        ExpAreaName = "龙岗区",
                        Address = "坂田吉华路393号英达丰科技园",
                    };

                    var requestData = new KDDRequestModel();
                    var receiver = requestData.GetSFReceiver(invoiceNotice.PostAddress, invoiceNotice.Title);
                    receiver.Company = invoiceNotice.Title;
                    receiver.Name = invoiceNotice.PostRecipient;
                    receiver.Mobile = invoiceNotice.PostTel;


                    SFRequestPara para = new SFRequestPara();
                    para.OrderID = InvoiceNotices[0];                              
                    para.ExpType = 1;
                    para.Sender = sender;
                    para.Receiver = receiver;

                    SFExpres sfExpres = new SFExpres();
                    sfExpres.sfPara = para;
                    SFResponse ResponseResult = sfExpres.Order();

                    ///////////////////////////////////////////////////////////////

                    //保存运单数据到数据库
                    if (ResponseResult.Success)
                    {
                        WayBillCodeUpdater wayBillCodeUpdater = new WayBillCodeUpdater(ResponseResult.WaybillNo, ExpressName, invoiceNotices.Select(t => t.ID).ToArray());
                        wayBillCodeUpdater.Update();
                    }

                    string fileServerUrl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];
                    string imageUrl = fileServerUrl + @"Files/DownLoad/" + ResponseResult.WaybillNo + ".jpg";
                    //返还数据到前端
                    Response.Write(new
                    {
                        success = ResponseResult.Success,
                        message = imageUrl,
                        LogisticCode = ResponseResult.WaybillNo,
                        PrintTemplate = imageUrl,
                        ShipperCode = expressCompany.Code
                    }.Json());
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.CcsLog("SF运单生成失败");
                //返还数据
                Response.Write(new
                {
                    success = false,
                    message = "运单生成失败" + ex.Message,
                }.Json());
            }
        }

    }
}