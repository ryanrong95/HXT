using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.MakeAccount
{
    public partial class DeclareImportQuery : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
            }
        }

        protected void LoadComboBoxData()
        {
            //凭证类型
            this.Model.DeclareImportType = EnumUtils.ToDictionary<DeclareImportType>().Select(item => new { Value = item.Key, Text = item.Value }).Json();
            //开票类型
            this.Model.InvoiceTypeData = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.InvoiceType>().Select(item => new { Value = item.Key, Text = item.Value }).Json();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string declareImportType = Request.QueryString["DeclareImportType"];
            string companyName = Request.QueryString["CompanyName"];
            string tian = Request.QueryString["Tian"];
            string declareDate = Request.QueryString["DeclareDate"];


  
                var view = new Needs.Ccs.Services.Views.MKDeclareImportQueryView().AsQueryable();

                if (!string.IsNullOrEmpty(declareImportType))
                {
                    int type = int.Parse(declareImportType);
                    view = view.Where(t => t.Type == type);
                }
                if (!string.IsNullOrEmpty(companyName))
                {
                    companyName = companyName.Trim();
                    view = view.Where(t=> t.Sanfang.Contains(companyName));
                }
                if (!string.IsNullOrEmpty(tian))
                {
                    var date = int.Parse(tian.Trim());
                    view = view.Where(t => t.Tian == date);
                }
                if (!string.IsNullOrEmpty(declareDate))
                {
                    //var from = DateTime.Parse(declareDate);
                    view = view.Where(t=> t.DeclareDate == declareDate);
                }

            Func<Needs.Ccs.Services.Models.MKDeclareImport, object> convert = mk => new
            {
                ID = mk.ID,
                TemplateCode = mk.TemplateCode,
                SchemeCode = mk.SchemeCode,
                Type = ((DeclareImportType)mk.Type).GetDescription(),
                InvoiceType = ((Needs.Ccs.Services.Enums.InvoiceType)mk.InvoiceType).GetDescription(),
                DeclareDate = mk.DeclareDate,
                Tian = mk.Tian,
                Jinkou = mk.Jinkou,
                Huokuan = mk.Huokuan,
                Yunbaoza = mk.Yunbaoza,
                Guanshui = mk.Guanshui,
                GuanshuiShijiao = mk.GuanshuiShijiao,
                Xiaofeishui = mk.Xiaofeishui,
                XiaofeishuiShijiao = mk.XiaofeishuiShijiao,
                Shui = mk.Shui,
                Jinxiangshui = mk.Jinxiangshui,
                HuiduiSanfang = mk.HuiduiSanfang,
                Sanfang = mk.Sanfang,
                HuiduiWofang = mk.HuiduiWofang,
                Huilv = mk.Huilv,
                YingfuSanfang = mk.YingfuSanfang,
                Wuliufang = mk.Wuliufang,
                YingfuWofang = mk.YingfuWofang,
                Currency = mk.Currency,
                PingzhengZi = mk.PingzhengZi,
                PingzhengHao = mk.PingzhengHao,
                Summary = mk.Summary,
                RequestID = mk.RequestID,
            };

            this.Paging(view, convert);

        }

    }
}