using Needs.Ccs.Services;
using Needs.Ccs.Services.Views;
using Needs.Model;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;

namespace WebApp.SysConfig.ExcludeOriginTariffs
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string HSCode = Request.QueryString["HSCode"];
            string ExclusionPeriod = Request.QueryString["ExclusionPeriod"];

            using (var query = new Needs.Ccs.Services.Views.ExcludeOriginTariffsView())
            {
                var view = query;
                if (!string.IsNullOrEmpty(HSCode))
                {
                    view = view.SearchByHSCode(HSCode);
                }

                if (!string.IsNullOrEmpty(ExclusionPeriod))
                {
                    view = view.SearchByExclusionPeriod(ExclusionPeriod);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

        protected void ImportAccountFlow()
        {
            try
            {
                HttpPostedFile file = Request.Files["uploadExcel"];
                string ext = Path.GetExtension(file.FileName);
                if (ext != ".xls" && ext != ".xlsx")
                {
                    Response.Write((new { success = false, message = "文件格式错误，请上传.xls或.xlsx文件！" }).Json());
                    return;
                }

                //文件保存
                string fileName = file.FileName.ReName();

                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Import);
                fileDic.CreateDataDirectory();
                file.SaveAs(fileDic.FilePath);

                DataTable dt = Ccs.Utils.NPOIHelper.ExcelToDataTable(fileDic.FilePath, false);
              
                string exclusionPeriod="";
                DateTime dtexclusionPeriod;
                for (int i = 2; i < dt.Rows.Count; i++)
                {
                    if (string.IsNullOrEmpty(dt.Rows[i][2].ToString()))
                    {
                        break;
                    }
                    exclusionPeriod = dt.Rows[i][4].ToString().Replace("年","").Replace("月","");
                    if(DateTime.TryParse(exclusionPeriod,out dtexclusionPeriod))
                    {
                        exclusionPeriod = dtexclusionPeriod.ToString("yyyyMM");
                    }

                    Needs.Ccs.Services.Models.ExcludeOriginTariffs excludeOrigin = new Needs.Ccs.Services.Models.ExcludeOriginTariffs();
                    excludeOrigin.ID = ChainsGuid.NewGuidUp();
                    excludeOrigin.HSCode = dt.Rows[i][2].ToString();
                    excludeOrigin.Name = dt.Rows[i][3].ToString();
                    excludeOrigin.ExclusionPeriod = exclusionPeriod;
                    excludeOrigin.Enter();
                }

                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("Authorization", "Basic YW5kYWVsZWM6YmlnYmFuZzIwMTg=");
                string url = System.Configuration.ConfigurationManager.AppSettings["icgooExcludeTariffUrl"];
                HttpResponseMessage response = new HttpGet().HttpClient(url, headers);
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    Response.Write((new { success = false, message = "请求会员接口失败：" }).Json());
                    return;
                }
              
                Response.Write((new { success = true, message = "导入成功" }).Json());
                
            }
            catch(Exception ex)
            {
                Response.Write((new { success = false, message = "导入失败：" + ex.Message }).Json());
            }
        }
    }
}