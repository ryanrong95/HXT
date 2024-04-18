using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvWsOrder.WebApp.Invoice
{
    public partial class BatchPrintInvoiceConfirm : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void LoadData()
        {
            string IDs = Request.QueryString["IDs"];
            string[] idArray = IDs.Split(',');

            //var noticeitems = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNoticeItem.Where(x => idArray.Contains(x.InvoiceNoticeID)).Distinct().ToList();
            var noticeitems = new Yahv.PvWsOrder.Services.Views.InvoiceNoticeItemViewForBatchPrint(idArray).Distinct().ToArray();

            var groupedNoticeItems = noticeitems.GroupBy(t => t.ClientID);


            List<object> displayResults = new List<object>();

            foreach (var group in groupedNoticeItems)
            {
                var thisGroupNoticeitems = group.ToList();

                //发票号的集合
                var invoiceitem = thisGroupNoticeitems.Select(item => new
                {
                    InvoiceNo = item.InvoiceNo,
                }).Distinct();

                decimal totalAmout = 0;
                var InvoiceNo = "";
                var remak = "";
                int count = 0;
                //计算发票的张数
                foreach (var item in invoiceitem)
                {
                    if (item.InvoiceNo.Contains('/'))
                    {
                        var remaks = item.InvoiceNo.Split('/');
                        for (int i = 0; i < remaks.Length; i++)
                        {
                            var content = remaks[i].ToString();

                            if (content.Contains('-'))
                            {
                                var contentArr = content.Split('-');
                                int digits = contentArr[1].Trim().Length;
                                int InvoiceCount = Convert.ToInt32(contentArr[1].ToString()) - Convert.ToInt32(contentArr[0].Substring(contentArr[0].Length - digits));
                                if (InvoiceCount < 0)
                                {
                                    int remaks1 = Convert.ToInt32("1" + contentArr[1]);
                                    count += System.Math.Abs(remaks1 - Convert.ToInt32(contentArr[0].Substring(contentArr[0].Length - digits))) + 1;
                                }
                                else
                                {
                                    count += InvoiceCount + 1;
                                }
                            }
                            else
                            {
                                count += 1;

                            }
                        }
                    }
                    else if (item.InvoiceNo.Contains("-"))
                    {
                        var remaks = item.InvoiceNo.Split('-');
                        int digits = remaks[1].Trim().Length;
                        long InvoiceCount = Convert.ToInt64(remaks[1].ToString()) - Convert.ToInt64(remaks[0].Substring(remaks[0].Length - digits));
                        if (InvoiceCount < 0)
                        {
                            int remaks1 = Convert.ToInt32("1" + remaks[1]);
                            count += System.Math.Abs(remaks1 - Convert.ToInt32(remaks[0].Substring(remaks[0].Length - digits))) + 1;
                        }
                        else
                        {
                            count += Convert.ToInt32(InvoiceCount) + 1;
                        }
                    }
                    else
                    {
                        count += 1;
                    }
                    //totalAmout += item.Amount;
                    InvoiceNo += item.InvoiceNo + ",";
                    remak = count + "张";
                }

                //从 InvoiceNoticeItem 的集合中计算 totalAmout
                totalAmout = thisGroupNoticeitems.Sum(t => t.Amount).ToRound1(2);

                //this.Model.ConfirmInvoiceInfo = new
                //{
                //    InvoiceNo = InvoiceNo.TrimEnd(',').Split(',').Distinct(),
                //    TotalAmout = totalAmout,
                //    Summry = remak,
                //    DataTime = DateTime.Now.ToString("yyyy/MM/dd")
                //}.Json();

                displayResults.Add(new
                {
                    InvoiceNo = InvoiceNo.TrimEnd(',').Split(',').Distinct(),
                    TotalAmout = totalAmout,
                    Summry = remak,
                    DataTime = DateTime.Now.ToString("yyyy/MM/dd")
                });
            }

            this.Model.ConfirmInvoiceInfos = displayResults;
            this.Model.InvoiceUrl = "../Content/Themes/Images/" + ConfigurationManager.AppSettings["InvoiceImage"];
        }


    }
}