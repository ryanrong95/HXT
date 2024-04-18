using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;
using YuanDa_Logistics.Utility;

namespace WebApp.Finance
{
    public partial class PrintInvoice : Uc.PageBase
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
            this.Model.ExpressCompanyData = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressCompanies.Where(t=>t.Code=="SF" || t.Code == "EMS").
                Select(item => new { Value = item.ID, Text = item.Name }).Json();
        }

        protected void ExpressSelect()
        {
            //快递公司ID
            string id = Request.Form["ID"];
            //快递方式
            var expressType = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressTypes.Where(item => item.ExpressCompany.ID == id);
            Response.Write(expressType.Select(item => new { Value = item.TypeValue, Text = item.TypeName }).Json());
        }

        /// <summary>
        /// 生成快递面单-快递鸟 20211014 过期
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
                var invoiceNotices = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNotice.Where(item => InvoiceNotices.Contains(item.ID));
                var invoiceNotice = invoiceNotices.First();
                var expressCompany = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressCompanies.Where(item => item.ID == ExpressName).FirstOrDefault();

                #region KDDRequestModel数据填入

                var requestData = new KDDRequestModel();

                //快递鸟的长度只支持30的长
                requestData.OrderCode = InvoiceNotices[0];
                requestData.Quantity = 1;
                requestData.ShipperCode = expressCompany.Code;
                requestData.CustomerName = expressCompany.CustomerName;
                requestData.CustomerPwd = expressCompany.CustomerPwd;
                requestData.MonthCode = expressCompany.MonthCode;
                requestData.PayType = (int)Needs.Ccs.Services.Enums.PayType.MonthlyPay;
                requestData.ExpType = int.Parse(ExpressType);
                requestData.Cost = 0;
                requestData.OtherCost = 0;
                requestData.Sender = new Sender()
                {
                    Company = PurchaserContext.Current.CompanyName,
                    Name = invoiceNotice.Client.ServiceManager.RealName,
                    Mobile = invoiceNotice.Client.ServiceManager.Mobile,
                    ProvinceName = "广东省",
                    CityName = "深圳市",
                    ExpAreaName = "龙岗区",
                    Address = "坂田吉华路393号英达丰科技园",
                };
                requestData.Receiver = requestData.GetReceiver(invoiceNotice.MailAddress, invoiceNotice.Client.Company.Name);
                requestData.Receiver.Company = invoiceNotice.Client.Company.Name;
                requestData.Receiver.Name = invoiceNotice.MailName;
                requestData.Receiver.Mobile = invoiceNotice.MailMobile;
                requestData.Commodity = new Commodity[]{
                    new Commodity()
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
                    InvoiceContext context = new InvoiceContext();
                    context.notices = invoiceNotices;
                    context.WaybillCode = ResponseResult.Order?.LogisticCode;
                    context.CompanyName = ExpressName;
                    context.GInvoiceWaybill();
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
                //返还数据
                Response.Write(new
                {
                    success = false,
                    message = "运单生成失败" + ex.Message,
                }.Json());
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
                var invoiceNotices = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNotice.Where(item => InvoiceNotices.Contains(item.ID));
                var invoiceNotice = invoiceNotices.First();
                var expressCompany = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressCompanies.Where(item => item.ID == ExpressName).FirstOrDefault();


                #region 顺丰

                if (expressCompany.Code == "SF")
                {
                    var sender = new Sender()
                    {
                        Company = PurchaserContext.Current.CompanyName,
                        Name = invoiceNotice.Client.ServiceManager.RealName,
                        Mobile = invoiceNotice.Client.ServiceManager.Mobile,
                        ProvinceName = "广东省",
                        CityName = "深圳市",
                        ExpAreaName = "龙岗区",
                        Address = "坂田吉华路393号英达丰科技园",
                    };

                    var requestData = new KDDRequestModel();
                    var receiver = requestData.GetReceiver(invoiceNotice.MailAddress, invoiceNotice.Client.Company.Name);
                    receiver.Company = invoiceNotice.Client.Company.Name;
                    receiver.Name = invoiceNotice.MailName;
                    receiver.Mobile = invoiceNotice.MailMobile;


                    SFRequestPara para = new SFRequestPara();
                    para.OrderID = InvoiceNotices[0];
                    //para.OrderID = "SFOrder00012021101805";                
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
                        InvoiceContext context = new InvoiceContext();
                        context.notices = invoiceNotices;
                        context.WaybillCode = ResponseResult.WaybillNo;
                        context.CompanyName = ExpressName;
                        context.GInvoiceWaybill();
                    }

                    //返还数据到前端
                    Response.Write(new
                    {
                        success = ResponseResult.Success,
                        message = ResponseResult.FilePath,
                        LogisticCode = ResponseResult.WaybillNo,
                        PrintTemplate = ResponseResult.FilePath,
                        ShipperCode = expressCompany.Code
                    }.Json());
                }

                #endregion

                #region EMS

                if (expressCompany.Code == "EMS")
                {

                    var emsApi = new EmsApiHelper();
                    var kk = new EmsRequestModel();

                    kk.LogisticsOrderNo = InvoiceNotices[0];

                    kk.Sender = new EmsSender();
                    kk.Sender.Name = invoiceNotice.Client.ServiceManager.RealName;
                    kk.Sender.PostCode = "";
                    kk.Sender.Phone = "";
                    kk.Sender.Mobile = invoiceNotice.Client.ServiceManager.Mobile;
                    kk.Sender.Prov = "广东";
                    kk.Sender.City = "深圳";
                    kk.Sender.County = "龙岗区";
                    kk.Sender.Address = "坂田吉华路393号英达丰科技园";


                    var address = emsApi.HandleAddress(invoiceNotice.MailAddress);
                    kk.Receiver = new EmsSender();
                    kk.Receiver.Name = invoiceNotice.MailName;
                    kk.Receiver.PostCode = "";
                    kk.Receiver.Phone = "";
                    kk.Receiver.Mobile = invoiceNotice.MailMobile;
                    kk.Receiver.Prov = address["Province"];
                    kk.Receiver.City = address["City"];
                    kk.Receiver.County = address["Area"];
                    kk.Receiver.Address = address["DetailsAddress"] + "(" + invoiceNotice.Client.Company.Name + ")";

                    kk.Cargos = new Cargos();
                    kk.Cargos.Cargo = new List<Cargo>();
                    kk.Cargos.Cargo.Add(new Cargo()
                    {
                        CargoName = "文件票据"
                    });

                    var result = emsApi.EmsXmlGenerate(kk);

                    //调用运单接口成功
                    if (result.ResponseItems.Response.Success)
                    {
                        //保存运单数据
                        InvoiceContext context = new InvoiceContext();
                        context.notices = invoiceNotices;
                        context.WaybillCode = result.ResponseItems.Response.WaybillNo;
                        context.CompanyName = ExpressName;
                        context.GInvoiceWaybill();

                        //组织数据 返回前台
                        Response.Write(new
                        {
                            success = true,
                            message = new {
                                RouteCode = result.ResponseItems.Response.RouteCode,

                                SName = kk.Sender.Name,
                                SMobile = kk.Sender.Mobile,
                                SAddress = "广东 " + "深圳 " + "龙岗区 " + "坂田吉华路393号英达丰科技园",

                                RName = kk.Receiver.Name,
                                RMobile = kk.Receiver.Mobile,
                                RAddress = invoiceNotice.MailAddress + " " + invoiceNotice.Client.Company.Name,
                            },
                            LogisticCode = result.ResponseItems.Response.WaybillNo,
                            ShipperCode = expressCompany.Code
                        }.Json());
                    }
                    else {
                        //返还数据
                        Response.Write(new
                        {
                            success = false,
                            message = "运单生成失败:" + result.ResponseItems.Response.Reason,
                        }.Json());
                    }

                }

                #endregion

            }
            catch (Exception ex)
            {
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
