using Needs.Ccs.Services.Models;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.WebService
{
    public partial class DownLoadInvoiceListWebFormSolo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Stream sream = Request.InputStream;
            StreamReader sr = new StreamReader(sream);
            string search = sr.ReadToEnd();
            sr.Close();
            var list = JsonConvert.DeserializeObject<IEnumerable<DecTaxFlowForUser>>(search);

            var file = list.FirstOrDefault();
            if (file != null)
            {
                string fileServerUrl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];
                var filepath = (fileServerUrl + @"\" + file.InvoiceFile.Url).ToUrl();

                Response.Write((new { success = true, message = "导出成功", url = filepath }).Json());
            }
            else
            {
                Response.Write((new { success = false, message = "导出失败，不存在文件", }).Json());
            }
            
        }
    }
}