using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Invoice
{
    public partial class BatchPrintInvoiceConfirm : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        protected void LoadData()
        {
            string IDs = Request.QueryString["IDs"];
            string[] idArray = IDs.Split(',');

            var noticeitems = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNoticeItem
               .Where(x => idArray.Contains(x.InvoiceNoticeID)).Distinct().ToList();

            var groupedNoticeItems = noticeitems.GroupBy(t => t.ClientID);
            List<object> displayResults = new List<object>();

            using(var clientView = new Needs.Ccs.Services.Views.ClientsView())
            using (var query = new Needs.Ccs.Services.Views.InvoiceXmlSplitShowView())
            {
                var view = query;

                foreach (var group in groupedNoticeItems)
                {
                    var thisGroupNoticeitems = group.ToList();
                    List<string> invoiceNoticeIDs = thisGroupNoticeitems.Select(item => item.InvoiceNoticeID).Distinct().ToList();


                    //发票号的集合
                    var invoiceitem = thisGroupNoticeitems.Select(item => new
                    {
                        InvoiceNo = item.InvoiceNo,
                    }).Distinct();

                    decimal totalAmout = 0;
                    var InvoiceNo = "";
                    var remak = "";
                    int count = 0;
                    string clientName = "";
                    string clientID = group.FirstOrDefault().ClientID;
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
                    totalAmout = thisGroupNoticeitems.Sum(t => t.Amount).ToRound(2);

                    //根据生成的发票信息，取出开票金额                  
                    view = view.SearchByInvoiceNoticeIDs(invoiceNoticeIDs);
                    string invoices = view.ToMyPage(1, 500).Json();
                    JObject jsonObject = (JObject)JToken.Parse(invoices);
                    List<InvoiceXmlVo> xmls = JsonConvert.DeserializeObject<List<InvoiceXmlVo>>(jsonObject["rows"].ToString());
                    if (xmls.Count > 0)
                    {
                        totalAmout = xmls.Sum(t => t.XmlPrice);
                    }

                    var client = clientView.Where(t => t.ID == clientID).FirstOrDefault();
                    if (client != null)
                    {
                        clientName = client.Company.Name;
                    }

                    displayResults.Add(new
                    {
                        InvoiceNo = InvoiceNo.TrimEnd(',').Split(',').Distinct(),
                        TotalAmout = totalAmout,
                        Summry = remak,
                        DataTime = DateTime.Now.ToString("yyyy/MM/dd"),
                        ClientName = clientName,
                    });
                }

                

               
            }

            

            

            this.Model.ConfirmInvoiceInfos = displayResults.Json();
            this.Model.InvoiceUrl = ("../../" + PurchaserContext.Current.InvoiceStamp.ToUrl()).Json();

        }

    }
}