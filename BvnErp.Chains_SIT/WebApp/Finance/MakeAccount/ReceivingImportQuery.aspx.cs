using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.MakeAccount
{
    public partial class ReceivingImportQuery : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);
           
            string companyName = Request.QueryString["CompanyName"];

            using (var query = new Needs.Ccs.Services.Views.ReceivingImportQueryView())
            {
                var view = query;

                if (!string.IsNullOrEmpty(companyName))
                {
                    companyName = companyName.Trim();
                    view = view.SearchByClientName(companyName);
                }

               

                Response.Write(view.ToMyPage(page, rows).Json());
            }

            

           
           
           

            //Func<Needs.Ccs.Services.Models.ReImportModel, object> convert = mk => new
            //{
            //    ID = mk.ID,
            //    mk.RequestID,
            //    mk.OrderRecepitID,
            //    mk.PreMoney,
            //    mk.Diff,
            //    mk.GoodsMoney,
            //    mk.AddTax,
            //    mk.Tariff,
            //    mk.ExciseTax,
            //    mk.Agency,
            //    mk.ClientName,
            //    mk.ReCreNo,
            //    mk.ReCreWord
            //};

        }
    }
}