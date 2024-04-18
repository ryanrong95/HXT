
using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WebApi.Models;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace WebApi.Controllers
{
    [CustomBasicAuthenticationFilter]
    public class PublicController : ApiController
    {
        [System.Web.Http.HttpGet]
        public HttpResponseMessage SwapLimitCountries()
        {
            try
            {
                var countries = new Needs.Ccs.Services.Views.SwapLimitCountriesView();

                var s = countries.GroupBy(t => new { t.Code, t.Name }).Select(g => g.First()).ToList();

                var list = new List<LimitCountry>();
                foreach (var item in s)
                {
                    list.Add(new LimitCountry()
                    {
                        Code = item.Code,
                        Name = item.Name
                    });
                }

                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8, "application/json"),
                };
            }
            catch
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent("提交数据不正确", Encoding.UTF8, "application/json"),
                };
            }
        }


        /// <summary>
        /// 提供报关产品数据查询接口
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage ProductDeclareDetail(string ID)
        {
            try
            {
                if (!new Needs.Ccs.Services.Views.OrderItemsView().CheckOrderItemsVaild(ID))
                {
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent("唯一号错误或未下单", Encoding.UTF8, "application/json"),
                    };
                }

                var details = new Needs.Ccs.Services.Views.ModelDetailView(ID).GetModelDetailPre().ToArray();
                if (details.Count() < 1)
                {
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent("唯一号还未报关", Encoding.UTF8, "application/json"),
                    };
                }
                else
                {
                    var view = new Needs.Ccs.Services.Views.ModelDetailView();
                    var viewInvoice = view.GetInvoiceInfo();
                    
                    foreach (var detail in details)
                    {
                        //补充发票信息
                        var invoice = viewInvoice.Where(t => t.OrderID.Contains(detail.OrderID))?.FirstOrDefault();
                        if (invoice != null)
                        {
                            detail.InvoiceType = invoice.InvoiceType.GetDescription();
                            detail.InvoiceTaxRate = invoice.InvoiceTaxRate;
                            detail.InvoiceDate = invoice.InvoiceDate == DateTime.Parse("2000-01-01T00:00:00") ? null : invoice.InvoiceDate.ToString();
                            detail.InvoiceNo = invoice.InvoiceNo;
                        }

                        //进行单双抬头的计算
                        view.CalculateTax(detail);
                    }
                }

                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"),
                };
            }
            catch(Exception ex)
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent("提交数据不正确", Encoding.UTF8, "application/json"),
                };
            }
        }


        /// <summary>
        /// 提供报关产品数据查询接口--订单号
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage DeclareDetailByOrderID(string ID)
        {
            try
            {
                var result = new List<IcgooModelDetail>();
                var IcgooMaps = new Needs.Ccs.Services.Views.IcgooMapView().Where(t => t.IcgooOrder == ID).ToList();
                if (IcgooMaps.Count < 1)
                {
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent("报关单号错误或未下单", Encoding.UTF8, "application/json"),
                    };
                }

                var details = new Needs.Ccs.Services.Views.IcgooGetModelByCMXXXView(ID).GetModelDetail();

                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"),
                };
            }
            catch
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent("Error,数据异常", Encoding.UTF8, "application/json"),
                };
            }
        }


        /// <summary>
        /// 提供报关产品数据查询接口--箱号
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage DeclareDetailByCartonNO(string ID)
        {
            try
            {
                var result = new List<IcgooModelDetail>();
                var IcgooCarton = new Needs.Ccs.Services.Views.PackingsView().Where(t => t.BoxIndex == ID).ToList();
                if (IcgooCarton.Count < 1)
                {
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent("箱号错误或未下单", Encoding.UTF8, "application/json"),
                    };
                }

                var details = new Needs.Ccs.Services.Views.IcgooGetModelByCartonView(IcgooCarton.FirstOrDefault().ID).GetModelDetail();

                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"),
                };
            }
            catch
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent("Error,数据异常", Encoding.UTF8, "application/json"),
                };
            }
        }

        /// <summary>
        /// 根据Icgoo的订单号，查询报关单状态，以及箱号
        /// </summary>
        /// <param name="FS_Number"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetDelerationStatus(string FS_Number)
        {
            List<IcgooDeclarationResponse> ResponseData = new List<IcgooDeclarationResponse>();
            string json = JsonConvert.SerializeObject(new
            {
                Success = false,
                Code = "100",
                Data = ResponseData
            });
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var orderIDs = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>().Where(t => t.IcgooOrder == FS_Number).
                                                                                                    OrderBy(t => t.CreateDate).
                                                                                                    Select(t => t.OrderID).ToList();
                   

                    if (orderIDs.Count() > 0)
                    {
                        foreach (var orderid in orderIDs)
                        {
                            string OrderStatus = "";
                            var order = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(t => t.ID == orderid).FirstOrDefault();
                            var declists = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>().Where(t => t.OrderID == orderid).ToList();
                            var packNos = declists.Select(t => t.CaseNo).Distinct();
                            OrderStatus = ((Needs.Ccs.Services.Enums.OrderStatus)order.OrderStatus).GetDescription();
                            if (order.Status == (int)Needs.Ccs.Services.Enums.OrderStatus.Declared)
                            {
                                var orderitemid = declists.FirstOrDefault().OrderItemID;
                                var storage = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.StoreStorages>().
                                                  Where(t => t.OrderItemID == orderitemid && t.Purpose == (int)Needs.Ccs.Services.Enums.StockPurpose.Storaged).FirstOrDefault();
                                if (storage != null)
                                {
                                    OrderStatus = "深圳已上架";
                                }
                            }

                            IcgooDeclarationResponse response = new IcgooDeclarationResponse();
                            response.OrderID = orderid;
                            response.PackNo = string.Join(";", packNos);
                            response.OrderStatus = OrderStatus;
                            ResponseData.Add(response);
                        }

                        json = JsonConvert.SerializeObject(new
                        {
                            Success = true,
                            Code = "200",
                            Data = ResponseData
                        });
                    }                    
                }

                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
            catch
            {
                string fail = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "400",
                    Data = "查询出错"
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(fail, Encoding.UTF8, "application/json"),
                };
            }
        }

        /// <summary>
        /// 缴款书PDF路径接口
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public HttpResponseMessage DeclareFile(DateTime StartDate,DateTime EndDate)
        {
            TimeSpan timeSpan = EndDate - StartDate;
            if (timeSpan.Days > 7||timeSpan.Days<0)
            {
                string json = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "400",
                    Data = "只能查询跨度一个星期的数据!"
                });

                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
          

            try
            {
                var tariffFiles = new Needs.Ccs.Services.Views.DecFileView().Where(t => t.CreateDate > StartDate && t.CreateDate < EndDate.AddDays(1) && t.FileType == FileType.DecHeadTariffFile).ToList();
                var vatFiles = new Needs.Ccs.Services.Views.DecFileView().Where(t => t.CreateDate > StartDate && t.CreateDate < EndDate.AddDays(1) && t.FileType == FileType.DecHeadVatFile).ToList();

                List<FileReturn> files = new List<FileReturn>();
                foreach(var item in tariffFiles)
                {
                    files.Add(new FileReturn
                    {
                        CusTaxNumber = item.Name,
                        CusTaxPdfPath = item.Url,
                    });
                }
                foreach (var item in vatFiles)
                {
                    files.Add(new FileReturn
                    {
                        CusTaxNumber = item.Name,
                        CusTaxPdfPath = item.Url,
                    });
                }
                string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrlForDyj"];
                string json = JsonConvert.SerializeObject(new
                {
                    Success = true,
                    Code = "200",
                    DomainURL = FileServerUrl,
                    Data = files.Json()
                });

                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
            catch
            {
                string fail = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "400",
                    Data = "查询出错"
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(fail, Encoding.UTF8, "application/json"),
                };
            }
        }
        
        /// <summary>
        /// 原产地加征排除-采购计划
        /// </summary>
        /// <param name="Month"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage ExcludeOriginHSCode(string Month = null)
        {
            try
            {
                var result = new Needs.Ccs.Services.Views.ExcludeOriginTariffsView().AsQueryable();
                if (Month != null)
                {
                    result = result.Where(t => t.ExclusionPeriod == Month);
                }
               
                string json = JsonConvert.SerializeObject(new
                {
                    Success = true,
                    Code = "200",
                    Data = result
                });

                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
            catch
            {
                string fail = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "400",
                    Data = "查询出错"
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(fail, Encoding.UTF8, "application/json"),
                };
            }
        }

        /// <summary>
        /// 实时汇率
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage RealTimeExRate()
        {
            try
            {
                var result = new Needs.Ccs.Services.Views.RealTimeExchangeRatesView().AsQueryable();
                var sss = result.Where(t => t.Code == "USD").ToArray().Select(t=> new { 
                    Date = t.UpdateDate.Value.ToString("yyyy-MM-dd"),
                    Currency = "USD",
                    Rate = t.Rate
                }).FirstOrDefault();

                string json = JsonConvert.SerializeObject(new
                {
                    Success = true,
                    Code = "200",
                    Data = sss
                });

                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
            catch
            {
                string fail = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "400",
                    Data = "查询出错"
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(fail, Encoding.UTF8, "application/json"),
                };
            }
        }
    }
}
