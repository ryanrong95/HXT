//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Yahv.Services.Json;
//using Yahv.Utils.Http;

//namespace Yahv.Services
//{
//    public static class AdminJsonView
//    {
//        public static AdminJson Get(string token)
//        {
//            string url = $"{Yahv.Underly.DomainConfig.ErmApi}Admins/Token/{token}";
//            return ApiHelper.Current[Agent.Warehouse].Get<AdminJson>(url);
//        }
//    }
//}
