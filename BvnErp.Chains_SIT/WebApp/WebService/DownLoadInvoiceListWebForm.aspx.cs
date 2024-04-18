using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.WebService
{
    public partial class DownLoadInvoiceListWebForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Stream sream = Request.InputStream;
            StreamReader sr = new StreamReader(sream);
            string search = sr.ReadToEnd();
            sr.Close();
            var list = JsonConvert.DeserializeObject<IEnumerable<DecTaxFlowForUser>>(search);

            //1.创建文件夹(文件压缩后存放的地址)
            FileDirectory file = new FileDirectory();
            file.SetChildFolder(Needs.Ccs.Services.SysConfig.ZipFiles);
            file.CreateDataDirectory();
            string filePrefix = file.RootDirectory;
            List<string> files = new List<string>();
            foreach (var item in list)
            {
                if (item.InvoiceFile != null)
                {
                    var filepath = (filePrefix + @"\" + item.InvoiceFile.Url).ToUrl();
                    files.Add(filepath);
                }
            }
            if (files.Count() == 0)
            {
                Response.Write((new { success = false, message = "勾选的税单没找到，请联系客服",url=""}).Json());
                return;
            }
            string zipFileName = "税单" + DateTime.Now.ToString("yyyyMMddHHmmss") + Needs.Ccs.Services.SysConfig.Postfix;
            ZipFile zip = new ZipFile(zipFileName);
            zip.SetFilePath(file.FilePath);
            zip.Files = files;
            zip.ZipFiles();
            var urlReturn = System.Configuration.ConfigurationManager.AppSettings["PvWsorderPdfDownLoad"] + "/" + file.VirtualPath.Replace(@"\", "/");
            Response.Write((new { success = true, message = "导出成功", url = urlReturn + zipFileName }).Json());
        }
    }
}