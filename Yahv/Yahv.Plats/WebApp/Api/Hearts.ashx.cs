using System;
using System.Web;
using Yahv.Utils.Serializers;

namespace WebApp.Api
{
    /// <summary>
    /// Hearts 的摘要说明
    /// </summary>
    public class Hearts : Yahv.Web.Handlers.JsonHandler
    {

        static Random random = new Random();

        override protected void OnProcessRequest(HttpContext context)
        {
            if (false)
            {
                #region 数据模拟

                var normal1 = new
                {
                    time = DateTime.Now.ToString(),
                    status = 200,
                };
                var normal2 = new
                {
                    time = DateTime.Now.ToString(),
                    status = 200,
                    message = "恭喜发财！"
                };

                var busy = new
                {
                    lastTime = DateTime.Now.AddMilliseconds(random.Next(0, 1000)).ToString(),
                    count = random.Next(1, 200)
                };

                string json;

                if (DateTime.Now.Second > 5 && DateTime.Now.Second < 15)
                {
                    json = busy.Json();
                }
                else
                {
                    if (DateTime.Now.Second % 6 == 0)
                    {
                        json = normal1.Json();
                    }
                    else
                    {
                        json = normal2.Json();
                    }
                }

                context.Response.Write(json);

                #endregion
            }

            //实际过程

            if (true)
            {
                #region 实际过程

                int count = Yahv.Plats.Services.Heart.Current.Execute(Yahv.Erp.Current);

                string json;

                if (count > 0)
                {
                    json = new
                    {
                        lastTime = DateTime.Now.ToString(),
                        count = count
                    }.Json();
                }
                else
                {
                    json = new
                    {
                        time = DateTime.Now.ToString(),
                        status = 200,
                    }.Json();
                }

                context.Response.Write(json);

                #endregion
            }
        }
    }
}