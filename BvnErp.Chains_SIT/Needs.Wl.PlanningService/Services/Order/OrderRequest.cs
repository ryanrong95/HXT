using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Wl.PlanningService.Services.Order;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    public class DyjOrderRequest : OrderRequest
    {
        public DyjOrderRequest() : base("dyj")
        {

        }
        public override void Process()
        {
            var client = this.Client;
            var Api = base.ApiSetting.Apis[ApiType.Order];
            string DateNow = DateTime.Now.ToString("yyyy-MM-dd");
            HttpRequest request = new HttpRequest
            {
                Timeout = this.ApiSetting.Timeout
            };

            string json = request.Get(string.Format(Api.Url, DateNow));
            //将json序列化为对象
            var list = this.ToOrderList(json);

            foreach (var order in list)
            {
                try
                {
                    if (order.状态 == "待报关")
                    {
                        string url = this.ApiSetting.Apis[ApiType.Order2].Url;
                        request = new HttpRequest
                        {
                            Timeout = this.ApiSetting.Timeout
                        };
                        string test = string.Format(url, order.报关单号);
                        json = request.Get(string.Format(url, order.报关单号));

                        List<InsideOrderJsonItem> models = JsonConvert.DeserializeObject<List<InsideOrderJsonItem>>(json.Replace("(PCS)", "").Replace("\"3C\":", "\"CCC\":"));
                        if (models[0].报关公司 == client.DeclareCompany)
                        {
                            this.Enter(json, order.报关单号, client.MQName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                    SmtpContext.Current.Send(receivers, "获取大赢家订单异常", ex.Message);
                    continue;
                }
            }
        }

        private IEnumerable<DyjOrderList> ToOrderList(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<IEnumerable<DyjOrderList>>(json);
        }

        private void Enter(string json, string orderno, string MQName)
        {
            var message = new Needs.Ccs.Services.Views.MessageView().Where(item => item.Summary == orderno).FirstOrDefault();
            if (message == null)
            {
                IcgooMQ mq = new IcgooMQ();
                mq.ID = ChainsGuid.NewGuidUp();
                mq.PostData = json;
                mq.IsAnalyzed = true;
                mq.Status = Needs.Ccs.Services.Enums.Status.Normal;
                mq.UpdateDate = mq.CreateDate = DateTime.Now;
                mq.CompanyType = Needs.Ccs.Services.Enums.CompanyTypeEnums.Inside;
                mq.Summary = orderno;
                mq.Enter();

                //生产者
                var publisher = new RabbitMQ(MQName, MQName);
                publisher.Publish(mq.ID);
            }
        }
    }

    public class DyjOrderCreate : OrderCreate
    {
        public DyjOrderCreate() : base("dyj")
        {

        }
        public override void Process()
        {
            var client = this.Client;
            var consumer = new RabbitMQ(client.MQName);
            consumer.Consume(message =>
            {
                Console.WriteLine(" [x] Received {0}", message);
                CenterCreateDyjOrder order = new CenterCreateDyjOrder(client.DeclareCompany, client.Name);
                order.CreateNew(message);
            });
        }
    }

    public class IcgooOrderCreate : OrderCreate
    {
        public IcgooOrderCreate() : base("icgoo")
        {

        }
        public override void Process()
        {
            var client = this.Client;
            var consumer = new RabbitMQ(client.MQName);

            //var clients = ApiService.Current.Clients.Where(item => item.Code == "WL888" || item.Code == "WL666").ToList();
            //Dictionary<string, string> PayExchangeSupplierMap = new Dictionary<string, string>();
            //PayExchangeSupplierMap.Add(clients[0].ID, clients[0].PayExchangeSupplier);
            //PayExchangeSupplierMap.Add(clients[1].ID, clients[1].PayExchangeSupplier);

            consumer.Consume(message =>
            {
                Console.WriteLine(" [x] Received {0}", message);
                CenterCreateIcgooOrder order = new CenterCreateIcgooOrder();
                order.Create(message);
            });
        }
    }


    public class IcgooInXDTOrderCreate : OrderCreate
    {
        public IcgooInXDTOrderCreate() : base("icgooXDT")
        {

        }
        public override void Process()
        {
            var client = this.Client;
            var consumer = new RabbitMQ(client.MQName);

            //var clients = ApiService.Current.Clients.Where(item => item.Code == "XL002").ToList();
            //Dictionary<string, string> PayExchangeSupplierMap = new Dictionary<string, string>();
            //PayExchangeSupplierMap.Add(clients[0].ID, clients[0].PayExchangeSupplier);
            //PayExchangeSupplierMap.Add(clients[1].ID, clients[1].PayExchangeSupplier);

            consumer.Consume(message =>
            {
                Console.WriteLine(" [x] Received {0}", message);
                CenterCreateIcgooOrder order = new CenterCreateIcgooOrder();
                order.Create(message);
            });
        }
    }

    /// <summary>
    /// 处理关税不一致消息
    /// </summary>
    public class IcgooTariffDiffCheck
    {
        public void Process()
        {
            var consumer = new RabbitMQ(Icgoo.IcgooTariffDiffMQName);
            consumer.Consume(message =>
            {
                Console.WriteLine(" [x] Received {0}", message);
                IcgooDiffTariffCheck icgooDiffTariffCheck = new IcgooDiffTariffCheck(message);
                icgooDiffTariffCheck.Check();
            });
        }
    }
}