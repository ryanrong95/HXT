using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Express100
{

    public partial class WaybillTracking : Uc.PageBase
    {
        private static KuaiDi100Config config = new KuaiDi100Config()
        {
            key = "dXkSAzHt352",
            customer = "3DFD4439E485B20BD51FA05E83754DA5",
            secret = "d14e76c91cc94590975a4a8bf46651b7",
            userid = "f388e01a882f408abc9a08a9af64b54b",
            siid = "",
            tid = "",
        };
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }
        protected void LoadData()
        {
            var expCompany = new Needs.Ccs.Services.Views.WaybillTrackingCompanyView().ToArray();
            this.Model.ExpCompany = expCompany.Select(item => new { item.ExpCompanyCode, item.ExpCompanyName }).Json();
        }
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string num = "5572717931";//Request.QueryString["num"];
            string com = "dhl"; //Request.QueryString["com"];
            string phone = "13522790823";//Request.QueryString["Phone"];
            using (var query = new Needs.Ccs.Services.Views.WaybillTrackingView())
            {
                var view = query;
                view = view.SearchByNum(num).SearchByCom(com);
                if (view.Count() <= 0)
                {

                    //接口请求返回数据保存
                    //var queryTrackParam = new Needs.Ccs.Services.Models.QueryTrackParam()
                    //{
                    //    com = com,//"shunfeng",
                    //    num = num,//"SF1049192353823",
                    //    phone = phone,//"13522790823"
                    //};
                    var queryTrackParam = new QueryTrackParam()
                    {
                        com = com,
                        num = num,
                        phone = phone
                    };
                    var message = QueryTrack.query(new QueryTrackReq()
                    {
                        customer = config.customer,
                        sign = SignUtils.GetMD5(queryTrackParam.ToString() + config.key + config.customer),
                        param = queryTrackParam
                    });
                    //var json = JSON.parse(message);
                    JObject messageJson = JObject.Parse(message);

                    //请求返回的状态
                    int state = Convert.ToInt32(messageJson["state"].ToString());
                    string waybillTrackingID = Guid.NewGuid().ToString("N");
                    //遍历data信息
                    var datasList = messageJson["data"].AsEnumerable();

                    Needs.Ccs.Services.Models.WaybillTrackingModel waybillTrack = new Needs.Ccs.Services.Models.WaybillTrackingModel
                    {
                        ID = waybillTrackingID,
                        WaybillTrackingID = waybillTrackingID,
                        ExpNumber = num,
                        ExpCompanyCode = com,
                        Status = Needs.Ccs.Services.Enums.Status.Normal,
                        State = (Needs.Ccs.Services.Enums.State)state,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        DatasList = messageJson["data"].AsEnumerable()
                    };
                    waybillTrack.Enter();
                }

                //如果有数据插入，需要 重新查询
                view = new Needs.Ccs.Services.Views.WaybillTrackingView().SearchByCom(com).SearchByNum(num);

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }
    }
}