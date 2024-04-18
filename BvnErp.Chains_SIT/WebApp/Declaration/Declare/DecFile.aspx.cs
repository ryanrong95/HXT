using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Declare
{
    public partial class DecFile : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)  
        {
            LoadData();
        }

        protected void LoadData()
        {
            this.Model.ID = Request.QueryString["ID"];
        }

        /// <summary>
        /// 报关单附件
        /// </summary>
        protected void data()
        {   
            string ID = Request.QueryString["ID"];

            var decFile = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DecFileView.Where(t => t.DecHeadID == ID);

            string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];
            Func<Needs.Ccs.Services.Models.DecFile, object> convert = item => new
            {
                ID = item.ID,
                DecHeadID = item.DecHeadID,
                AdminID = item.AdminID,
                Name = item.Name,
                FileType = item.FileType.GetDescription(),
                FileFormat = item.FileFormat,
                Url = FileServerUrl + @"/" + item.Url.ToUrl(),
                Status = item.Status.GetDescription(),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                Summary = item.Summary
            };

            Response.Write(new
            {
                rows = decFile.Select(convert).ToArray(),
                total = decFile.Count()
            }.Json());
        }
    }
} 