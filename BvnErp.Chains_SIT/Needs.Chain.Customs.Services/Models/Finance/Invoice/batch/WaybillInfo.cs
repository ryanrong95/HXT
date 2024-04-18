using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using Needs.Underly;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class WaybillInfo
    {
        public List<string> UnInvoiceNoticeID { get; set; }

        public List<XmlWaybillResposenVo> WaybillResponses { get; set; }

        public List<XmlInvoiceResponseVo> InvoiceResponses { get; set; }

        public WaybillInfo()
        {
            this.UnInvoiceNoticeID = new List<string>();
            this.WaybillResponses = new List<XmlWaybillResposenVo>();
            this.InvoiceResponses = new List<XmlInvoiceResponseVo>();
        }

        public void doWaybill()
        {
            GetUnInvoiceNoticeID();
            FetchWaybillInfo();
            //UpdateWaybillInfo();
            FetchInvoiceInfo();
            UpdateInvoice();
        }

        private void GetUnInvoiceNoticeID()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var InvoiceXmls = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmls>().Where(t => t.InvoiceNo == null).Select(t => t.InvoiceNoticeID).Distinct();
                var InvoiceNotices = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>().
                                                            Where(t => InvoiceXmls.Contains(t.ID) && t.Status == (int)InvoiceNoticeStatus.Auditing).Select(t => t.ID).Distinct();
                foreach (var item in InvoiceNotices)
                {
                    this.UnInvoiceNoticeID.Add(item);
                }
            }
        }

        private void FetchWaybillInfo()
        {
            foreach (var item in this.UnInvoiceNoticeID)
            {
                try
                {
                    InvoiceXmlRequestModel xmlRequestModel = new InvoiceXmlRequestModel();
                    xmlRequestModel.request_service = InvoiceApiSetting.ServiceName;
                    xmlRequestModel.request_item = InvoiceApiSetting.XmlRequestRecord;

                    InvoiceXmlWaybillRequestVo waybillRequestVo = new InvoiceXmlWaybillRequestVo();
                    waybillRequestVo.发票标识 = item;                    
                    xmlRequestModel.data = waybillRequestVo;

                    var apiurl = System.Configuration.ConfigurationManager.AppSettings[InvoiceApiSetting.ApiName] + InvoiceApiSetting.GenerateXmlUrl;
                    var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, xmlRequestModel);

                    var jResult = JsonConvert.DeserializeObject<JSingle<dynamic>>(result);

                    if (jResult.success)
                    {
                        JObject jsonObject = (JObject)JToken.Parse(result);
                        List<XmlWaybillResposenVo> waybillResponse = JsonConvert.DeserializeObject<List<XmlWaybillResposenVo>>(jsonObject["data"].ToString());
                        foreach (var waybill in waybillResponse)
                        {                           
                             this.WaybillResponses.Add(waybill);                            
                        }
                    }

                }
                catch (Exception ex)
                {
                    ex.CcsLog("获取dyj发票信息出错：" + item);
                    continue;
                }
            }
        }

        private void UpdateWaybillInfo()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var Express = (from carrier in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Carriers>()
                               where carrier.Status == (int)Status.Normal && (carrier.Name == "EMS" || carrier.Name == "顺丰")
                               select new
                               {
                                   ID = carrier.ID,
                                   code = carrier.Name,
                               }).ToList();
                string EMSID = Express.Where(t => t.code == "EMS").FirstOrDefault().ID;
                string SFID = Express.Where(t => t.code == "顺丰").FirstOrDefault().ID;

                foreach (var waybill in this.WaybillResponses)
                {
                    string carrierID = EMSID;
                    if (waybill.快递类型 == "顺丰")
                    {
                        carrierID = SFID;
                    }

                    var waybillInfo = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceWaybills>().Where(t => t.InvoiceNoticeID == waybill.发票标识).FirstOrDefault();
                    if (waybillInfo != null)
                    {
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.InvoiceWaybills
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            InvoiceNoticeID = waybill.发票标识,
                            CompanyName = carrierID,
                            WaybillCode = waybill.运单号,
                            CreateDate = DateTime.Now,
                        });
                    }
                }
            }
        }

        private void FetchInvoiceInfo()
        {
            foreach (var waybill in this.WaybillResponses)
            {
                try
                {
                    InvoiceXmlRequestModel xmlRequestModel = new InvoiceXmlRequestModel();
                    xmlRequestModel.request_service = InvoiceApiSetting.ServiceName;
                    xmlRequestModel.request_item = InvoiceApiSetting.XmlRequestDetail;

                    XmlInvoiceRequestVo xmlInvoiceRequestVo = new XmlInvoiceRequestVo();
                    xmlInvoiceRequestVo.MainID = waybill.ID;
                    xmlRequestModel.data = xmlInvoiceRequestVo;

                    var apiurl = System.Configuration.ConfigurationManager.AppSettings[InvoiceApiSetting.ApiName] + InvoiceApiSetting.GenerateXmlUrl;
                    var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, xmlRequestModel);

                    var jResult = JsonConvert.DeserializeObject<JSingle<dynamic>>(result);

                    if (jResult.success)
                    {
                        JObject jsonObject = (JObject)JToken.Parse(result);
                        List<XmlInvoiceResponseVo> invoiceResponse = JsonConvert.DeserializeObject<List<XmlInvoiceResponseVo>>(jsonObject["data"].ToString());

                        foreach (var inv in invoiceResponse)
                        {
                            if (inv.发票号 != null)
                            {
                                this.InvoiceResponses.Add(inv);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.CcsLog("获取dyj发票号等信息出错：" + waybill.发票标识);
                    continue;
                }
            }
        }

        private void UpdateInvoice()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                string InvoiceNo;
                DateTime InvoiceDate;
                foreach (var inv in this.InvoiceResponses)
                {
                    InvoiceNo = inv.发票号;
                    string InvoiceDateFomate = Convert.ToDateTime(inv.开票时间).ToString("yyyy-MM-dd");
                    InvoiceDate = Convert.ToDateTime(InvoiceDateFomate);
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmls>(new
                    {
                        InvoiceNo = InvoiceNo,
                        InvoiceDate = InvoiceDate
                    }, t => t.ID == inv.XML标识);

                    string InvoiceNoticeID = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmls>().
                                                Where(t => t.ID == inv.XML标识 && t.Status == (int)Status.Normal).FirstOrDefault().InvoiceNoticeID;
                    if (string.IsNullOrEmpty(InvoiceNoticeID))
                    {
                        return;
                    }

                    //获取开票类型
                    var invNotice = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>().Where(t => t.ID == InvoiceNoticeID).FirstOrDefault();

                    if (invNotice.InvoiceType == (int)InvoiceType.Full)
                    {
                        //全额发票
                        var InvoiceNoticeItems = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmlItems>().
                            Where(t => t.InvoiceNoticeXmlID == inv.XML标识).Select(t => t.InvoiceNoticeItemID).Distinct().ToList();

                        foreach (var item in InvoiceNoticeItems)
                        {
                            if (string.IsNullOrEmpty(item))
                            {
                                continue;
                            }
                            var noticeItem = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>().Where(t => t.ID == item).FirstOrDefault();
                            string nowInvoiceNo = InvoiceNo;
                            if (!string.IsNullOrEmpty(noticeItem.InvoiceNo))
                            {
                                nowInvoiceNo = noticeItem.InvoiceNo + "," + InvoiceNo;
                            }

                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>(new
                            {
                                InvoiceNo = nowInvoiceNo,
                                InvoiceDate = InvoiceDate
                            }, t => t.ID == item);
                        }

                    }
                    else
                    {
                        //服务费发票
                        var invNoticeItems = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>().Where(t => t.InvoiceNoticeID == InvoiceNoticeID).FirstOrDefault();
                        string nowInvoiceNo = InvoiceNo;
                        if (!string.IsNullOrEmpty(invNoticeItems.InvoiceNo))
                        {
                            nowInvoiceNo = invNoticeItems.InvoiceNo + "," + InvoiceNo;
                        }
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>(new
                        {
                            InvoiceNo = nowInvoiceNo,
                            InvoiceDate = InvoiceDate
                        }, t => t.InvoiceNoticeID == InvoiceNoticeID);
                    }
                   
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNotices>(new
                    {
                        Status = InvoiceStatus.Invoiced
                    }, t => t.ID == InvoiceNoticeID);
                }
            }
        }
    }
}
