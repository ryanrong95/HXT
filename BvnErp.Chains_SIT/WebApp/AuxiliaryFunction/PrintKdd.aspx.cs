using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YuanDa_Logistics.Utility;

namespace WebApp.AuxiliaryFunction
{
    public partial class PrintKdd : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
            }
            LoadData();
        }


        protected void LoadComboBoxData()
        {
            //快递公司
            this.Model.ExpressCompanyData = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressCompanies.Select(item => new { Value = item.ID, Text = item.Name }).Json();
            this.Model.PayType = EnumUtils.ToDictionary<PayType>().Select(item => new { item.Key, item.Value }).Json();
        }

        protected void ExpressSelect()
        {
            //快递公司名称
            string name = Request.Form["name"];
            //快递方式
            if (!string.IsNullOrEmpty(name))
            {
                string shipperCode = "";
                switch (name)
                {
                    case "顺丰":
                        shipperCode = "SF";
                        break;
                    case "跨越速运":
                        shipperCode = "KYSY";
                        break;
                }

                var types = new[] { typeof(SfExpType), typeof(KysyExpType) };
                var type = types.SingleOrDefault(item => item.Name.StartsWith(shipperCode,
                    StringComparison.OrdinalIgnoreCase));
                var ins = Activator.CreateInstance(type) as CodeType;

                var expressType = ins.Select(t => new
                {
                    Value = t.Value,
                    Text = t.Name
                });
                Response.Write(expressType.Select(item => new { Value = item, Text = item.Text }).Json());

            }
            //var expressType = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressTypes.Where(item => item.ExpressCompany.ID == id);
            //Response.Write(expressType.Select(item => new { Value = item.TypeValue, Text = item.TypeName }).Json());
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        protected void LoadData()
        {
            //参数
            string id = Request.QueryString["ID"];
            this.Model.ID = id;


            if (!string.IsNullOrEmpty(id)&& id!= "undefined")
            {
                var express = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.ExpressKdds[id];
                this.Model.ExpressData = new
                {
                    ID = express.ID,
                    express.Receiver,
                    express.ReceiverComp,
                    express.ReveiveAddress,
                    express.ReveiveMobile,
                    express.Sender,
                    express.SenderComp,
                    express.SenderAddress,
                    express.SenderMobile,
                    ExpressCompany = express.ExpressCompany.ID,
                    ExpressType = express.ExpressType.TypeValue,
                    PayType = (int)express.PayType,
                }.Json();
            }
            else
            {
                this.Model.ExpressData = new { }.Json();
            }
        }


        /// <summary>
        /// 生成快递面单
        /// </summary>
        protected void GenerateExpress()
        {
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            dynamic model = Model.JsonTo<dynamic>();
            string id = model.ID;
            string ExpressID = model.ExpressNameID;
            string SenderAddress = model.SenderAddress;
            string ReveiveAddress = model.ReveiveAddress;
            var expressCompany = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressCompanies.Where(item => item.ID == ExpressID).FirstOrDefault();
            #region KDDRequestModel数据填入

            var requestData = new KDDRequestModel();

            requestData.OrderCode = DateTime.Now.ToString("yyyyMMddHHmmss");
            requestData.Quantity = 1;
            requestData.ShipperCode = expressCompany.Code;
            requestData.CustomerName = expressCompany.CustomerName;
            requestData.CustomerPwd = expressCompany.CustomerPwd;
            requestData.MonthCode = expressCompany.MonthCode;
            requestData.PayType = (int)Needs.Ccs.Services.Enums.PayType.MonthlyPay;
            requestData.ExpType = model.ExpressTypeID;
            requestData.Cost = 0;
            requestData.OtherCost = 0;
            //寄件信息
            requestData.Sender = requestData.GetSender(SenderAddress);
            requestData.Sender.Name = model.Sender;//寄件人
            requestData.Sender.Mobile = model.SenderMobile;
            requestData.Sender.Company = model.SenderComp;//寄件公司
            //收货信息
            requestData.Receiver = requestData.GetReceiver(ReveiveAddress);
            requestData.Receiver.Company = model.SenderComp;
            requestData.Receiver.Name = model.Receiver;
            requestData.Receiver.Mobile = model.ReveiveMobile;
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

            //返还数据到前端
            Response.Write(new
            {
                success = ResponseResult.Success,
                message = ResponseResult.Reason,
                LogisticCode = ResponseResult.Order?.LogisticCode,
                PrintTemplate = ResponseResult.PrintTemplate
            }.Json());
        }
    }
}