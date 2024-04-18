using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.ClientModels
{
    public class MobileLog// : Yahv.Linq.IUnique
    {

        //public string ID { get; set; }
        //public string Token { get; set; }
        //public string RequestContent { get; set; }
        //public string ResponseContent { get; set; }
        //public DateTime CreateDate { get; set; }

        public static void InsertLog(string token, string url, string requestContent, string responseContent)
        {
            try
            {
                using (var reponsitory = LinqFactory<ScCustomReponsitory>.Create())
                {
                    reponsitory.Insert(new Layers.Data.Sqls.ScCustoms.MobileLogs()
                    {
                        ID = Guid.NewGuid().ToString(),
                        Token = token,
                        RequestContent = requestContent,
                        ResponseContent = responseContent,
                        CreateDate = DateTime.Now,
                        Url = url,
                    });
                }
            }
            catch (Exception ex)
            {

            }
        }

    }
}
