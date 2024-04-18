using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.InsideOrder
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void GetOrder()
        {
            string OrderId = Request.Form["OrderId"];
            string AdditionWeight = Request.Form["AdditionWeight"];          

            string url = Needs.Ccs.Services.InsideOrder.GetUrl.Replace("idpara", OrderId);
            bool bSuccess = true;
            string result = Needs.Ccs.Services.HttpRequest.GetRequest(url, ref bSuccess);
            if (bSuccess)
            {
                try
                {
                    var message = new Needs.Ccs.Services.Views.MessageView().Where(item => item.Summary == OrderId).FirstOrDefault();                   
                    if (message == null)
                    {
                        List<InsideOrderJsonItem> Items = JsonConvert.DeserializeObject<List<InsideOrderJsonItem>>(result);
                        var CaseCount = Items.ToList().Select(item => item.箱号).Distinct().Count();
                        var Prices = Items.ToList().Select(item => new { price = Convert.ToDecimal(item.金额) });
                        var TotalPrice = Prices.Sum(t=>t.price);

                        //创建InsidePost记录，并持久化
                        IcgooMQ mq = new IcgooMQ();
                        mq.ID = ChainsGuid.NewGuidUp();                     
                        mq.PostData = result;
                        mq.IsAnalyzed = true;
                        mq.Status = Needs.Ccs.Services.Enums.Status.Normal;
                        mq.UpdateDate = mq.CreateDate = DateTime.Now;
                        mq.CompanyType = Needs.Ccs.Services.Enums.CompanyTypeEnums.Inside;
                        mq.Summary = OrderId;
                        mq.AdditionWeight = Convert.ToInt16(AdditionWeight);
                       
                        mq.Enter();

                        string UserName = System.Configuration.ConfigurationManager.AppSettings["UserName"];
                        string Password = System.Configuration.ConfigurationManager.AppSettings["Password"];
                        string HostName = System.Configuration.ConfigurationManager.AppSettings["HostName"];
                        string Port = System.Configuration.ConfigurationManager.AppSettings["Port"];
                        string VirtualHost = System.Configuration.ConfigurationManager.AppSettings["VirtualHost"];
                        //加入队列
                        MQMethod mqMethod = new MQMethod(UserName, Password, HostName, Convert.ToInt16(Port), VirtualHost);
                        string returnmsg = "";
                        bool isSuccess = mqMethod.ProduceInside(mq.ID, ref returnmsg);

                        Response.Write((new { success = isSuccess, message = returnmsg + "!共" + Items.Count + "个型号,共" + CaseCount + "件,总金额:" + TotalPrice + "美元" }).Json());
                    }
                    else
                    {
                        Response.Write((new { success = false, message = "该A类订单已经生成了订单!"}).Json());
                    }                          
                }
                catch(Exception ex)
                {
                    Response.Write((new { success = false, message = ex.Message }).Json());
                }               
            }
            else
            {
                Response.Write((new { success = false, message = "获取A类订单数据失败" }).Json());
            }
        }

        protected void data()
        {
            Response.Write(new
            {
                rows = 0,
                total = 0
            }.Json());
        }
    }
}