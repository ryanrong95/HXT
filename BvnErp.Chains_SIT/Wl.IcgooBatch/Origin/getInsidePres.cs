using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wl.IcgooBatch
{
    public class getInsidePres
    {
        public string KeyValue { get; set; }

        public class InsideInfo
        {
            /// <summary>
            /// ID
            /// </summary>
            public string UnionCode { get; set; }
            /// <summary>
            /// 单价
            /// </summary>
            public decimal UnitPrice { get; set; }         
        }

        public getInsidePres()
        {
            this.KeyValue = System.Configuration.ConfigurationManager.AppSettings["keyValue"];
        }
        public void GetProduct()
        {
            List<InsideInfo> infos = RequestLists();
            RequestSingleProduct(infos);
        }

        private List<InsideInfo> RequestLists()
        {
            List<InsideInfo> infos = new List<InsideInfo>();
            bool requeststatus = true;
            string requesturl = System.Configuration.ConfigurationManager.AppSettings["ListUrl"];
            requesturl += "?key=" + this.KeyValue;
            HttpRequest httpRequest = new HttpRequest();            
            string result = httpRequest.GetRequest(requesturl, ref requeststatus);
            if (requeststatus)
            {
                InsidePreProducts partnos = JsonConvert.DeserializeObject<InsidePreProducts>(result);
                            
                foreach(var p in partnos.list)
                {
                    InsideInfo info = new InsideInfo();
                    info.UnionCode = p.ID;
                    info.UnitPrice = p.报关价格;                   
                    infos.Add(info);
                }
                return infos;              
            }

            return null;
        }

        private void RequestSingleProduct(List<InsideInfo> infos)
        {
            bool requeststatus = true;
            string prefixRequesturl = System.Configuration.ConfigurationManager.AppSettings["SingleProductUrl"];
            string requesturl = "";
            HttpRequest httpRequest = new HttpRequest();
            foreach (InsideInfo singleProduct in infos)
            {
                Console.WriteLine(singleProduct.UnionCode);
                requesturl = prefixRequesturl + "?id=" + singleProduct.UnionCode + "&key=" + this.KeyValue;
                string result = httpRequest.GetRequest(requesturl, ref requeststatus);
                if (requeststatus)
                {
                    InsidePreSingleProducts partnos = JsonConvert.DeserializeObject<InsidePreSingleProducts>(result);
                    foreach(var item in partnos.list)
                    {
                        if (item.型号归类编码 != "")
                        {
                            try
                            {
                                InsidePreProduct p = new InsidePreProduct();
                                p.sale_orderline_id = singleProduct.UnionCode;
                                p.partno = item.型号;
                                p.mfr = item.厂家;
                                p.price = singleProduct.UnitPrice;
                                p.currency_code = "USD";
                                p.CompanyType = CompanyTypeEnums.Inside;
                                p.UpdateTime = p.CreateTime = DateTime.Now;
                                p.Status = (int)Status.Normal;
                                p.supplier = "";
                                p.TaxCode = item.信息归类码;
                                p.TaxName = item.信息归类值;
                                p.HSCode = item.型号归类编码;
                                p.Elements = item.型号归类值;
                                p.BatchNo = item.批号;
                                p.Description = item.描述;
                                p.Pack = item.封装;
                                p.UseFor = item.用途;
                                p.AraeOfProduction = item.产地;
                                p.PreEnter();
                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                            
                        }                        
                    }
                }
            }
        }

    }
}
