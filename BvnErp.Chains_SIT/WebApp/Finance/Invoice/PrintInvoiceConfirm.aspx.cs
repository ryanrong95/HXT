using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance
{
    public partial class PrintInvoiceConfirm : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            LoadData();

        }

        /// <summary>
        ///数据加载
        /// </summary>
        protected void LoadData()
        {
            string id = Request["ID"];
            var noticeitem = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNoticeItem
               .Where(x => x.InvoiceNoticeID == id).Distinct();

            this.Model.InvoiceUrl = ("../../" + PurchaserContext.Current.InvoiceStamp.ToUrl()).Json();

            //发票号的集合
            var invoiceitem = noticeitem.Select(item => new
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
                if (item.InvoiceNo.Contains('/') || item.InvoiceNo.Contains(','))
                {
                    var remaks = item.InvoiceNo.Split('/', ',');
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
                        else {
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
                        count +=Convert.ToInt32(InvoiceCount) + 1;
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
            totalAmout = noticeitem.Sum(t => t.Amount + t.Difference).ToRound(2);

            this.Model.ConfirmInvoiceInfo = new
            {
                InvoiceNo = InvoiceNo.TrimEnd(',').Split(',').Distinct(),
                TotalAmout = totalAmout,
                Summry = remak,
                DataTime = DateTime.Now.ToString("yyyy/MM/dd")
            }.Json();
        }

    }
}