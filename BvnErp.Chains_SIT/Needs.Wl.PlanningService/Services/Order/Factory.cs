using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    public interface IOrderRequest
    {
        void Process();
    }

    /// <summary>
    /// 目前只主动获取大赢家的订单数据
    /// </summary>
    public class OrderRequestFactory
    {
        public static IOrderRequest Create(string client)
        {
            switch (client)
            {
                case "dyj":
                    return new DyjOrderRequest();

           
                default:
                    return null;
            }
        }
    }

    public abstract class OrderRequest : IOrderRequest
    {
        protected ApiClient Client { get; set; }

        protected ApiSetting ApiSetting { get; private set; }

        public OrderRequest(string key)
        {
            //根据 key 加载不同的信息
            this.Client = ApiService.Current.Clients[key];
            this.ApiSetting = ApiService.Current.ApiSettings[key];
        }

        public abstract void Process();
    }

    public interface IOrderCreate
    {
        void Process();
    }



    public abstract class OrderCreate : IOrderCreate
    {
        protected ApiClient Client { get; set; }

        protected ApiSetting ApiSetting { get; private set; }

        public OrderCreate(string key)
        {
            //根据 key 加载不同的信息
            this.Client = ApiService.Current.Clients[key];
            this.ApiSetting = ApiService.Current.ApiSettings[key];
        }

        public abstract void Process();
    }

    public interface IPIRequest
    {
        void Process();
    }

    public abstract class PIRequest : IPIRequest
    {
        protected ApiClient Client { get; set; }

        protected ApiSetting ApiSetting { get; private set; }

        public PIRequest(string key)
        {
            //根据 key 加载不同的信息
            this.Client = ApiService.Current.Clients[key];
            this.ApiSetting = ApiService.Current.ApiSettings[key];
        }

        public abstract void Process();
    }
}
