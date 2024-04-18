using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    public interface IPreProductRequest
    {
        void Process();
    }

    public partial class PreProductRequestFactory
    {
        public static IPreProductRequest Create(string client)
        {
            switch (client)
            {
                case "icgoo":
                    return new IcgooRequest();

                case "dyj":
                    return new DyjRequest();

                case "kb":
                    return new KbRequest();

                case "icgooxdt":
                    return new IcgooInXDTRequest();                

                default:
                    return null;
            }
        }
    }

    public abstract class PreProductRequest : IPreProductRequest
    {
        protected ApiClient Client { get; set; }

        protected ApiSetting ApiSetting { get; private set; }

        public PreProductRequest(string key)
        {
            //根据 key 加载不同的信息
            this.Client = ApiService.Current.Clients[key];
            this.ApiSetting = ApiService.Current.ApiSettings[key];
        }

        public abstract void Process();
    }
}