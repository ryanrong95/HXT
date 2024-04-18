using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.HttpUtility
{ 
   public class HttpClientHelp
    {
        /// <summary>
        /// http请求
        /// </summary>
        /// <param name="requestType">请求方式：GET/POST</param>
        /// <param name="url">请求地址</param>
        /// <param name="body">请求体，默认为空值</param>
        /// <returns></returns>
        public HttpResponseMessage HttpClient(string requestType, string url, string body = "")
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {

                switch (requestType.ToUpper())
                {
                    case "POST":
                        HttpContent httpContent = new StringContent(body);
                        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        responseMessage = httpClient.PostAsync(url, httpContent).Result;
                        break;
                    case "GET":
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        responseMessage = httpClient.GetAsync(new Uri(url)).Result;
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                httpClient.Dispose();
            }
            return responseMessage;
        }
    }
}
