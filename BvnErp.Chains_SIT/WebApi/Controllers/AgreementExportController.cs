using Needs.Underly;
using Needs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace WebApi.Controllers
{
    /// <summary>
    /// 导出客户协议
    /// </summary>
    /// <returns></returns>
    [System.Web.Http.Route("/")]
    public class AgreementExportController : ApiController
    {
        /// <summary>
        /// 下载
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/agreement/download")]
        public JMessage Download(string ClientID)
        {
            try
            {
                var agreement = new Needs.Ccs.Services.Views.ClientAgreementsView().Where(item => item.ClientID == ClientID && item.Status == Needs.Ccs.Services.Enums.Status.Normal).FirstOrDefault();

                var fileName = DateTime.Now.Ticks + "服务协议草书.docx";
                FileDirectory file = new FileDirectory(fileName);
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                file.CreateDataDirectory();

                agreement.SaveAs(file.FilePath);

                var returnUrl = System.Configuration.ConfigurationManager.AppSettings["APIFileServerUrl"] + "/" + file.VirtualPath.Replace(@"\", "/");

                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = returnUrl
                };
                return json;
            }
            catch (Exception ex)
            {
                return new JMessage() { code = 300, success = false, data = ex.Message };
            }
        }

        /// <summary>
        /// 下载仓储协议
        /// </summary>
        /// <param name="ClientID"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/storageagreement/download")]
        public JMessage ExportStorageFile(string ClientID)
        {
            try
            {
                if (string.IsNullOrEmpty(ClientID))
                {
                    var json = new JMessage()
                    {
                        code = 100,
                        success = false,
                        data = "ClientID 为空"
                    };
                    return json;
                }
                var client = new Needs.Ccs.Services.Views.ClientsView()[ClientID];
                //创建文件夹
                var fileName = DateTime.Now.Ticks + "香港本地交货协议.docx";
                FileDirectory file = new FileDirectory(fileName);
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                file.CreateDataDirectory();
                //保存文件
                client.SaveStorageAgreement(file.FilePath);
                var returnUrl = System.Configuration.ConfigurationManager.AppSettings["APIFileServerUrl"] + "/" + file.VirtualPath.Replace(@"\", "/");
                return new JMessage() { code = 200, success = true, data = returnUrl };
            }
            catch (Exception ex)
            {

                return new JMessage() { code = 300, success = false, data = ex.Message };
            }
          


        }
    }
}