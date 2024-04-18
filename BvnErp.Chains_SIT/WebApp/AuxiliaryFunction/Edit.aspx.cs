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

namespace WebApp.AuxiliaryFunction
{
    public partial class Edit : Uc.PageBase
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
            //快递公司ID
            //string id = Request.Form["ID"];
            //快递方式
            //var expressType = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressTypes.Where(item => item.ExpressCompany.ID == id);
            //Response.Write(expressType.Select(item => new { Value = item.TypeValue, Text = item.TypeName }).Json());
            string expressCompanyID = Request.Form["ID"];
            if (!string.IsNullOrEmpty(expressCompanyID))
            {
                string shipperCode = "";
                switch (expressCompanyID)
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
         
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        protected void LoadData()
        {
            //参数
            string id = Request.QueryString["ID"];
            this.Model.ID = id;


            if (!string.IsNullOrEmpty(id))
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
                    PayType=(int)express.PayType,
                }.Json();
            }
            else
            {
                this.Model.ExpressData = new { }.Json();
            }
        }

        /// <summary>
        /// 保存收件地址信息
        /// </summary>
        protected void Save()
        {
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            dynamic model = Model.JsonTo<dynamic>();
            string id = model.ID;
            int typeValue = model.ExpressTypeID;
            string expressCompanyID = model.ExpressNameID;
            var express = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.ExpressKdds[id] ?? new Needs.Ccs.Services.Models.ExpressKdd();
            var expresstype = new Needs.Ccs.Services.Views.ExpressTypeView().FirstOrDefault(x => x.TypeValue == typeValue && x.ExpressCompanyID == expressCompanyID);
            express.ExpressCompany = new Needs.Ccs.Services.Models.ExpressCompany();
            express.ExpressType = new Needs.Ccs.Services.Models.ExpressType();
            express.ExpressCompany.ID = model.ExpressNameID;
            express.ExpressCompany.Name = model.ExpressName;

            express.ExpressType.ID = expresstype.ID;
            express.ExpressType.ExpressCompanyID = expressCompanyID;
            express.ExpressType.TypeValue = expresstype.TypeValue;
            express.ExpressType.TypeName = expresstype.TypeName;

            express.PayType = model.PayTypeID;
            express.ReceiverComp = model.ReceiverComp;
            express.Sender = model.Sender;
            express.SenderAddress = model.SenderAddress;
            express.SenderMobile = model.SenderMobile;
            express.SenderComp = model.SenderComp;
            express.Receiver = model.Receiver;
            express.ReveiveAddress = model.ReveiveAddress;
            express.ReveiveMobile = model.ReveiveMobile;
            express.EnterError += ExpresseKdd_EnterError;
            express.EnterSuccess += Expresskdd_EnterSuccess;
            express.Enter();
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExpresseKdd_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Expresskdd_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }
    }
}