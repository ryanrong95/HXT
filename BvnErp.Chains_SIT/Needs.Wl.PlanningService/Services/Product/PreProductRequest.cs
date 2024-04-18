using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    public class IcgooRequest : PreProductRequest, IPreProductRequest
    {
        public IcgooRequest() : base("icgoo")
        {

        }

        public override void Process()
        {
            var client = this.Client;
            var Api = base.ApiSetting.Apis[ApiType.PreProduct];

            int page = 0;
            int pagesize = 200;
            string json = "";
            while (true)
            {
                try
                {
                    page++;
                    string requesturl = string.Format(Api.Url, 0, pagesize, page);

                    HttpRequest request = new HttpRequest
                    {
                        Timeout = this.ApiSetting.Timeout,
                        Headers = this.ApiSetting.Headers
                    };

                    json = request.Get(requesturl);
                    if (json.Equals("{\"detail\": \"\\u65e0\\u6548\\u9875\\u9762\\u3002\"}"))
                    {
                        break;
                    }
                    List<IcgooPreProduct> partnos = JsonConvert.DeserializeObject<List<IcgooPreProduct>>(json);
                   
                   var partnoHY = partnos.Where(t => t.customs_company.ToUpper() != "XDT").ToList();                  

                    List<PreProduct> pres = partnoHY.Select(item =>
                      new PreProduct
                      {
                          ClientID = client.ID,
                          ProductUnionCode = item.sale_orderline_id,
                          Model = item.partno,
                          Manufacturer = item.mfr,
                          Qty = item.product_qty,
                          Price = item.price,
                          Currency = item.currency_code,
                          Supplier = item.supplier,
                          CompanyType = Ccs.Services.Enums.CompanyTypeEnums.Icgoo,
                          DueDate = item.arrival_date,
                      }).ToList();

                    foreach (var t in pres)
                    {
                        t.Enter();
                    }

                    if (partnos.Count != pagesize)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    //using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    //{
                    //    string now = System.DateTime.Now.ToString("yyyy-MM-dd");
                    //    DateTime dtStart = Convert.ToDateTime(now + " 00:00");
                    //    DateTime dtEnd = Convert.ToDateTime(now + " 23:59:59");
                    //    int count = responsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EmailNotice>().Where(item => item.Summary == ex.ToString() && item.Createdate > dtStart && item.Createdate < dtEnd).Count();
                    //    if (count == 0)
                    //    {
                    //        string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                    //        SmtpContext.Current.Send(receivers, "获取Icgoo预归类产品异常", json);

                    //        responsitory.Insert(new Layer.Data.Sqls.ScCustoms.EmailNotice
                    //        {
                    //            ProductUniqueCode = ChainsGuid.NewGuidUp(),
                    //            Createdate = DateTime.Now,
                    //            Updatedate = DateTime.Now,
                    //            Summary = ex.ToString(),
                    //        });
                    //    }
                    //}
                    if (ex.ToString().Contains("Cannot deserialize"))
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(2000);
                    continue;
                }
            }
        }
    }

    public class KbRequest : PreProductRequest, IPreProductRequest
    {
        public KbRequest() : base("kb")
        {

        }

        public override void Process()
        {
            var client = this.Client;
            var Api = base.ApiSetting.Apis[ApiType.PreProduct];

            int page = 0;
            int pagesize = 50;
            string json = "";
            while (true)
            {
                try
                {
                    page++;
                    string url = string.Format(Api.Url, 0, pagesize, page);

                    HttpRequest request = new HttpRequest
                    {
                        Timeout = this.ApiSetting.Timeout,
                        Headers = this.ApiSetting.Headers
                    };
                    json = request.Get(url);
                    List<KbPreProduct> partnos = JsonConvert.DeserializeObject<List<KbPreProduct>>(json);

                    List<PreProduct> pres = partnos.Select(item =>
                      new PreProduct
                      {
                          ClientID = client.ID,
                          ProductUnionCode = item.sale_orderline_id,
                          Model = item.partno,
                          Manufacturer = item.mfr,
                          Qty = item.product_qty,
                          Price = item.price,
                          Currency = item.currency_code,
                          Supplier = item.supplier,
                          CompanyType = Ccs.Services.Enums.CompanyTypeEnums.FastBuy,
                          DueDate = item.arrival_date,
                      }).ToList();

                    foreach (var t in pres)
                    {
                        t.Enter();
                    }

                    if (partnos.Count != pagesize)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    //using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    //{
                    //    string now = System.DateTime.Now.ToString("yyyy-MM-dd");
                    //    DateTime dtStart = Convert.ToDateTime(now + " 00:00");
                    //    DateTime dtEnd = Convert.ToDateTime(now + " 23:59:59");
                    //   int count = responsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EmailNotice>().Where(item => item.Summary == ex.ToString()&&item.Createdate>dtStart&&item.Createdate<dtEnd).Count();
                    //    if (count == 0)
                    //    {
                    //        string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                    //        SmtpContext.Current.Send(receivers, "获取快包预归类产品异常", json);

                    //        responsitory.Insert(new Layer.Data.Sqls.ScCustoms.EmailNotice
                    //        {
                    //            ProductUniqueCode = ChainsGuid.NewGuidUp(),
                    //            Createdate = DateTime.Now,
                    //            Updatedate = DateTime.Now,
                    //            Summary = ex.ToString(),
                    //        });
                    //    }
                    //}
                    System.Threading.Thread.Sleep(2000);
                    continue;
                }
            }
        }
    }

    public class DyjRequest : PreProductRequest, IPreProductRequest
    {
        public DyjRequest() : base("dyj")
        {

        }

        public override void Process()
        {
            var client = this.Client;
            var Api = base.ApiSetting.Apis[ApiType.PreProduct];
            List<ApiOrderCompany> apiOrderCompanies = ApiService.Current.OrderCompanies;

            HttpRequest request = new HttpRequest
            {
                Timeout = this.ApiSetting.Timeout
            };

            string json = request.Get(Api.Url);
            //将json序列化为对象
            var list = this.ToList(json);
            foreach (var product in list)
            {
                if (product.报关公司 == client.DeclareCompany)
                {
                    try
                    {
                        string url = this.ApiSetting.Apis[ApiType.PreProduct2].Url;
                        request = new HttpRequest
                        {
                            Timeout = this.ApiSetting.Timeout
                        };
                        json = request.Get(string.Format(url, product.ID));

                        this.Enter(product, json, apiOrderCompanies, client.ID);
                    }
                    catch (Exception ex)
                    {
                        //string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                        //SmtpContext.Current.Send(receivers, "获取大赢家预归类产品异常", ex.Message);
                        System.Threading.Thread.Sleep(2000);
                        continue;
                    }
                }
            }   
        }

        private IEnumerable<DyjPreProduct> ToList(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            JObject jsonObject = (JObject)JToken.Parse(json);
            return JsonConvert.DeserializeObject<IEnumerable<DyjPreProduct>>(jsonObject["list"].ToString());
        }

        private void Enter(DyjPreProduct product, string json, List<ApiOrderCompany> OrderCompanies,string clientID)
        {
            JObject jsonObject = (JObject)JToken.Parse(json);
            var dyjPreProduct1 = JsonConvert.DeserializeObject<IEnumerable<DyjSinglePreProduct>>(jsonObject["list"].ToString());
            var dyjPreProduct2 = dyjPreProduct1.FirstOrDefault();
            if (dyjPreProduct2 != null)
            {
                PreProduct preProduct = new PreProduct();
                preProduct.ProductUnionCode = dyjPreProduct2.单据号;
                preProduct.Model = dyjPreProduct2.型号;
                preProduct.Manufacturer = dyjPreProduct2.厂家;
                preProduct.Price = product.报关价格;
                preProduct.Qty = product.数量;
                preProduct.Currency = "USD";
                preProduct.CompanyType = CompanyTypeEnums.Inside;
                preProduct.TaxCode = dyjPreProduct2.信息归类码;
                preProduct.TaxName = dyjPreProduct2.信息归类值;
                preProduct.HSCode = dyjPreProduct2.型号归类编码;
                preProduct.Elements = dyjPreProduct2.型号归类值;
                preProduct.BatchNo = dyjPreProduct2.批号;
                preProduct.Description = dyjPreProduct2.描述;
                preProduct.Pack = dyjPreProduct2.封装;
                preProduct.UseFor = dyjPreProduct2.用途;
                preProduct.AreaOfProduction = dyjPreProduct2.产地;
                preProduct.Source = product.来源;
                preProduct.Unit = dyjPreProduct2.单位;

                //preProduct.ClientID = clientID;

                if (dyjPreProduct2.委托公司 == Needs.Wl.PlanningService.Services.AgentCompanies.xdtSeries)
                {
                    var clientOrder = OrderCompanies.Where(t => t.Name == dyjPreProduct2.下单公司).FirstOrDefault();
                    if (clientOrder != null)
                    {
                        preProduct.ClientID = clientOrder.ID;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    var clientOrder = OrderCompanies.Where(t => t.Name == dyjPreProduct2.委托公司).FirstOrDefault();
                    if (clientOrder != null)
                    {
                        preProduct.ClientID = clientOrder.ID;
                    }
                    else
                    {
                        return;
                    }
                }

                if (string.IsNullOrEmpty(preProduct.ClientID))
                {
                    preProduct.ClientID = clientID;
                }

                DateTime dtArrive;
                if (DateTime.TryParse(product.ArrivalTime, out dtArrive))
                {
                    preProduct.DueDate = dtArrive;
                }
                preProduct.Enter();
            }
        }
    }

    public class IcgooInXDTRequest : PreProductRequest, IPreProductRequest
    {
        public IcgooInXDTRequest() : base("icgooXDT")
        {

        }

        public override void Process()
        {
            var client = this.Client;
            var Api = base.ApiSetting.Apis[ApiType.PreProduct];
            string CXZXBJ = System.Configuration.ConfigurationManager.AppSettings["CXZXBJ"];
            string CXZXBJID = System.Configuration.ConfigurationManager.AppSettings["CXZXBJID"];
            string CXZXSZ = System.Configuration.ConfigurationManager.AppSettings["CXZXSZ"];
            string CXZXSZID = System.Configuration.ConfigurationManager.AppSettings["CXZXSZID"];
            string SZCXZX = System.Configuration.ConfigurationManager.AppSettings["SZCXZX"];
            string SZCXZXID = System.Configuration.ConfigurationManager.AppSettings["SZCXZXID"];
            string CXZXSD = System.Configuration.ConfigurationManager.AppSettings["CXZXSD"];
            string CXZXSDID = System.Configuration.ConfigurationManager.AppSettings["CXZXSDID"];
            string BJXDN = System.Configuration.ConfigurationManager.AppSettings["BJXDN"];
            string BJXDNID = System.Configuration.ConfigurationManager.AppSettings["BJXDNID"];
            string FCGYL = System.Configuration.ConfigurationManager.AppSettings["FCGYL"];
            string FCGYLID = System.Configuration.ConfigurationManager.AppSettings["FCGYLID"];
            Dictionary<string, string> clientID = new Dictionary<string, string>();
            clientID.Add(CXZXBJ, CXZXBJID);
            clientID.Add(CXZXSZ, CXZXSZID);
            clientID.Add(SZCXZX, SZCXZXID);
            clientID.Add(CXZXSD, CXZXSDID);
            clientID.Add(BJXDN, BJXDNID);
            clientID.Add(FCGYL, FCGYLID);

            int page = 0;
            int pagesize = 200;
            string json = "";
            while (true)
            {
                try
                {
                    page++;
                    string requesturl = string.Format(Api.Url, 0, pagesize, page);

                    HttpRequest request = new HttpRequest
                    {
                        Timeout = this.ApiSetting.Timeout,
                        Headers = this.ApiSetting.Headers
                    };

                    json = request.Get(requesturl);                   
                    if (json.Equals("{\"detail\": \"\\u65e0\\u6548\\u9875\\u9762\\u3002\"}"))
                    {
                        break;
                    }
                    List<IcgooPreProduct> partnos = JsonConvert.DeserializeObject<List<IcgooPreProduct>>(json);
                                       
                    var partnoXDT = partnos.Where(t => t.customs_company.ToUpper() == "XDT").ToList();                    

                    List<PreProduct> pres = partnoXDT.Select(item =>
                      new PreProduct
                      {
                          ClientID = item.order_company,
                          ProductUnionCode = item.sale_orderline_id,
                          Model = item.partno,
                          Manufacturer = item.mfr,
                          Qty = item.product_qty,
                          Price = item.price,
                          Currency = item.currency_code,
                          Supplier = item.supplier,
                          CompanyType = Ccs.Services.Enums.CompanyTypeEnums.Icgoo,
                          DueDate = item.arrival_date,
                      }).ToList();

                    foreach (var t in pres)
                    {
                        if (!string.IsNullOrEmpty(t.ClientID)&&clientID.ContainsKey(t.ClientID))
                        {
                            t.ClientID = clientID[t.ClientID];
                        }
                        else
                        {
                            t.ClientID = client.ID;
                        }
                        t.Enter();
                    }

                    if (partnos.Count != pagesize)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    //using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    //{
                    //    string now = System.DateTime.Now.ToString("yyyy-MM-dd");
                    //    DateTime dtStart = Convert.ToDateTime(now + " 00:00");
                    //    DateTime dtEnd = Convert.ToDateTime(now + " 23:59:59");
                    //    int count = responsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EmailNotice>().Where(item => item.Summary == ex.ToString() && item.Createdate > dtStart && item.Createdate < dtEnd).Count();
                    //    if (count == 0)
                    //    {
                    //        string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                    //        SmtpContext.Current.Send(receivers, "获取Icgoo预归类产品异常", json);

                    //        responsitory.Insert(new Layer.Data.Sqls.ScCustoms.EmailNotice
                    //        {
                    //            ProductUniqueCode = ChainsGuid.NewGuidUp(),
                    //            Createdate = DateTime.Now,
                    //            Updatedate = DateTime.Now,
                    //            Summary = ex.ToString(),
                    //        });
                    //    }
                    //}
                    if (ex.ToString().Contains("Cannot deserialize"))
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(2000);
                    continue;
                }
            }
        }
    }

    public class IcgooConsultRequest : PreProductRequest, IPreProductRequest
    {
        public IcgooConsultRequest() : base("icgooXDT")
        {

        }

        public override void Process()
        {
            //var client = this.Client;
            var Api = base.ApiSetting.Apis[ApiType.Consult];           
            string CXZXBJID = System.Configuration.ConfigurationManager.AppSettings["CXZXBJID"];
          

            int page = 0;           
            string json = "";
            string state = "wait";
            while (true)
            {
                try
                {
                    page++;
                    string requesturl = string.Format(Api.Url, page,state);

                    HttpRequest request = new HttpRequest
                    {
                        Timeout = this.ApiSetting.Timeout,
                        Headers = this.ApiSetting.Headers
                    };

                    json = request.Get(requesturl);                   
                    if (json.Equals("{\"detail\": \"\\u65e0\\u6548\\u9875\\u9762\\u3002\"}") )
                    {
                        break;
                    }
                    try
                    {
                        JObject jsonObject = (JObject)JToken.Parse(json);
                        if (!jsonObject.ContainsKey("results")) 
                        {
                            break;
                        }
                        List<IcgooConsultProduct> partnos = JsonConvert.DeserializeObject<List<IcgooConsultProduct>>(jsonObject["results"].ToString());

                        List<PreProduct> pres = partnos.Select(item =>
                          new PreProduct
                          {
                              //ClientID = client.ID,
                              ClientID = CXZXBJID,
                              ProductUnionCode = item.sale_order_line_id,
                              Currency = item.currency_code,
                              Price = item.price,
                              Qty = item.qty,
                              Model = item.partno,
                              Manufacturer = item.mfr,
                              Supplier = item.supplier,
                              CompanyType = Ccs.Services.Enums.CompanyTypeEnums.Icgoo,
                              UseType = PreProductUserType.Consult,
                              IcgooAdmin = item.sales_name
                          }).ToList();

                        foreach (var t in pres)
                        {
                            t.Enter();
                        }
                    }
                    catch
                    {
                        break;
                    }
                   
                }
                catch (Exception ex)
                {
                    //using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    //{
                    //    string now = System.DateTime.Now.ToString("yyyy-MM-dd");
                    //    DateTime dtStart = Convert.ToDateTime(now + " 00:00");
                    //    DateTime dtEnd = Convert.ToDateTime(now + " 23:59:59");
                    //    int count = responsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EmailNotice>().Where(item => item.Summary == ex.ToString() && item.Createdate > dtStart && item.Createdate < dtEnd).Count();
                    //    if (count == 0)
                    //    {
                    //        string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                    //        SmtpContext.Current.Send(receivers, "获取Icgoo预归类产品异常", json);

                    //        responsitory.Insert(new Layer.Data.Sqls.ScCustoms.EmailNotice
                    //        {
                    //            ProductUniqueCode = ChainsGuid.NewGuidUp(),
                    //            Createdate = DateTime.Now,
                    //            Updatedate = DateTime.Now,
                    //            Summary = ex.ToString(),
                    //        });
                    //    }
                    //}
                    if (ex.ToString().Contains("Cannot deserialize"))
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(2000);
                    continue;
                }
            }
        }
    }
}