using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Declare
{
    public partial class CustomsFiles : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        protected void data()
        {
            string DecHeadID = Request.QueryString["DecHeadID"];
            var DecHead = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[DecHeadID];
            var DecTax = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DecTax[DecHeadID];

            List<FileSearch> files = new List<FileSearch>();
            string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];
            string PvDataFileUrl = System.Configuration.ConfigurationManager.AppSettings["PvDataFileUrl"];


            foreach (var item in DecHead.EdocRealations)
            {
                if (item.Edoc.Name != "装箱单")
                {
                    FileSearch file = new FileSearch();
                    file.FileName = item.EdocCopId;
                    file.FileType = item.Edoc.Name;
                    file.FileUrl = FileServerUrl + @"/" + item.FileUrl.ToUrl();
                    files.Add(file);
                }                  
            }

            if (DecTax.TariffFile != null)
            {
                FileSearch TariffFile = new FileSearch();
                TariffFile.FileName = DecTax.TariffFile.Name;
                TariffFile.FileType = DecTax.TariffFile.FileType.GetDescription();
                TariffFile.FileUrl = FileServerUrl + @"/" + DecTax.TariffFile.Url.ToUrl();
                files.Add(TariffFile);
            }

            if (DecTax.ExciseTaxFile != null)
            {
                FileSearch ExciseTaxFile = new FileSearch();
                ExciseTaxFile.FileName = DecTax.ExciseTaxFile.Name;
                ExciseTaxFile.FileType = DecTax.ExciseTaxFile.FileType.GetDescription();
                ExciseTaxFile.FileUrl = FileServerUrl + @"/" + DecTax.ExciseTaxFile.Url.ToUrl();
                files.Add(ExciseTaxFile);
            }

            if (DecTax.VatFile != null)
            {
                FileSearch VatFile = new FileSearch();
                VatFile.FileName = DecTax.VatFile.Name;
                VatFile.FileType = DecTax.VatFile.FileType.GetDescription();
                VatFile.FileUrl = FileServerUrl + @"/" + DecTax.VatFile.Url.ToUrl();
                files.Add(VatFile);
            }

            if (DecTax.DecFile != null)
            {
                FileSearch DecFile = new FileSearch();
                DecFile.FileName = DecTax.DecFile.Name;
                DecFile.FileType = DecTax.DecFile.FileType.GetDescription();
                DecFile.FileUrl = FileServerUrl + @"/" + DecTax.DecFile.Url.ToUrl();
                files.Add(DecFile);
            }

           var agencyPro =  new Needs.Ccs.Services.Views.MainOrderAgentProxiesView().Where(t=>t.ID== DecHead.OrderID).FirstOrDefault();
            if (agencyPro != null&&agencyPro.MainFile!=null)
            {               
                FileSearch DecFile = new FileSearch();
                DecFile.FileName = agencyPro.MainFile.Name;
                DecFile.FileType = agencyPro.MainFile.FileType.GetDescription();
                DecFile.FileUrl = PvDataFileUrl + @"/" + agencyPro.MainFile.Url.ToUrl();
                files.Add(DecFile);                              
            }

            Func<FileSearch, object> convert = item => new
            {               
                FileName = item.FileName,
                FileType = item.FileType,
                FileUrl =  item.FileUrl.ToUrl()
            };

            Response.Write(new
            {
                rows = files.Select(convert).ToArray(),
                total = files.Count()
            }.Json());
        }

        private class FileSearch
        {          
            public string FileName { get; set; }
            public string FileType { get; set; }
            public string FileUrl { get; set; }
        }
    }
}