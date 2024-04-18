using DBSApis.Models;
using DBSApis.Services;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WebApi.Models;

namespace DBSApis.Controllers
{
    [CustomBasicAuthenticationFilter]
    public class DBSAuthController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("ABE")]
        public HttpResponseMessage GetABE(ABERequest aBERequest)
        {
            try
            {
                ABEService aBEService = new ABEService(aBERequest);
                ReturnType returnType = aBEService.PostMsg();
                if (returnType.IsSuccess)
                {
                    ABESuccResponse reponse = (ABESuccResponse)returnType.Data;
                    string json = JsonConvert.SerializeObject(new
                    {
                        Success = true,
                        Code = "200",
                        Data = reponse.accountBalResponse.Json()
                    });
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                    };
                }
                else
                {
                    string json = JsonConvert.SerializeObject(new
                    {
                        Success = false,
                        Code = "400",
                        Data = returnType.Msg
                    });
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                    };
                }
            }
            catch
            {
                string json = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "400",
                    Data = "查询余额失败"
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("FXPricing")]
        public HttpResponseMessage FXPricing(FXQuoteRequest fXQuoteRequest)
        {
            try
            {
                FXQuoteService fXQuoteService = new FXQuoteService(fXQuoteRequest);
                ReturnType returnType = fXQuoteService.PostMsg();

                if (returnType.IsSuccess)
                {
                    FXQuoteResponse reponse = (FXQuoteResponse)returnType.Data;
                    string json = JsonConvert.SerializeObject(new
                    {
                        Success = true,
                        Code = "200",
                        Data = reponse.txnResponse.Json()
                    });
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                    };
                }
                else
                {
                    string json = JsonConvert.SerializeObject(new
                    {
                        Success = false,
                        Code = "400",
                        Data = returnType.Msg
                    });
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                    };
                }
            }
            catch (Exception ex)
            {
                string test = ex.ToString();
                string json = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "400",
                    Data = test
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("FXBooking")]
        public HttpResponseMessage FXBooking(FXBookingRequest fXBookingRequest)
        {
            try
            {
                FXBookingService fXQuoteService = new FXBookingService(fXBookingRequest);
                ReturnType returnType = fXQuoteService.PostMsg();

                if (returnType.IsSuccess)
                {
                    FXBookingResponse reponse = (FXBookingResponse)returnType.Data;
                    string json = JsonConvert.SerializeObject(new
                    {
                        Success = true,
                        Code = "200",
                        Data = reponse.txnResponse.Json()
                    });
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                    };
                }
                else
                {
                    string json = JsonConvert.SerializeObject(new
                    {
                        Success = false,
                        Code = "400",
                        Data = returnType.Msg
                    });
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                    };
                }
            }
            catch (Exception ex)
            {
                string test = ex.ToString();
                string json = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "400",
                    Data = test
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("ACT")]
        public HttpResponseMessage ACT(ACTRequest aCTRequest)
        {
            try
            {
                ACTService fXQuoteService = new ACTService(aCTRequest);
                ReturnType returnType = fXQuoteService.PostMsg();

                if (returnType.IsSuccess)
                {
                    ACTACK1Response reponse = (ACTACK1Response)returnType.Data;
                    ACTACK1ResponseTxnResponses ack1Response = reponse.txnResponses.FirstOrDefault();
                    string json = JsonConvert.SerializeObject(new
                    {
                        Success = true,
                        Code = "200",
                        Data = ack1Response.Json()
                    });
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                    };
                }
                else
                {
                    string json = JsonConvert.SerializeObject(new
                    {
                        Success = false,
                        Code = "400",
                        Data = returnType.Msg
                    });
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                    };
                }
            }
            catch (Exception ex)
            {
                string test = ex.ToString();
                string json = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "400",
                    Data = test
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("CNAPS")]
        public HttpResponseMessage CNAPS(CNAPSRequest cNAPSTRequest)
        {
            try
            {
                CNAPSService fXQuoteService = new CNAPSService(cNAPSTRequest);
                ReturnType returnType = fXQuoteService.PostMsg();

                if (returnType.IsSuccess)
                {
                    ACTACK1Response reponse = (ACTACK1Response)returnType.Data;
                    ACTACK1ResponseTxnResponses ack1Response = reponse.txnResponses.FirstOrDefault();
                    string json = JsonConvert.SerializeObject(new
                    {
                        Success = true,
                        Code = "200",
                        Data = ack1Response.Json()
                    });
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                    };
                }
                else
                {
                    string json = JsonConvert.SerializeObject(new
                    {
                        Success = false,
                        Code = "400",
                        Data = returnType.Msg
                    });
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                    };
                }
            }
            catch (Exception ex)
            {
                string test = ex.ToString();
                string json = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "400",
                    Data = test
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("TT")]
        public HttpResponseMessage TT(TTRequest tTRequest)
        {
            try
            {
                TTService fXQuoteService = new TTService(tTRequest);
                ReturnType returnType = fXQuoteService.PostMsg();

                if (returnType.IsSuccess)
                {
                    ACTACK1Response reponse = (ACTACK1Response)returnType.Data;
                    ACTACK1ResponseTxnResponses ack1Response = reponse.txnResponses.FirstOrDefault();
                    string json = JsonConvert.SerializeObject(new
                    {
                        Success = true,
                        Code = "200",
                        Data = ack1Response.Json()
                    });
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                    };
                }
                else
                {
                    string json = JsonConvert.SerializeObject(new
                    {
                        Success = false,
                        Code = "400",
                        Data = returnType.Msg
                    });
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                    };
                }
            }
            catch (Exception ex)
            {
                string test = ex.ToString();
                string json = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "400",
                    Data = test
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("ARE")]
        public HttpResponseMessage ARE(ARERequest aRERequest)
        {
            try
            {
                AREService fXQuoteService = new AREService(aRERequest);
                ReturnType returnType = fXQuoteService.SearchFlow();

                if (returnType.IsSuccess)
                {
                    string json = JsonConvert.SerializeObject(new
                    {
                        Success = true,
                        Code = "200",
                        Data = returnType.Msg
                    });
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                    };
                }
                else
                {
                    string json = JsonConvert.SerializeObject(new
                    {
                        Success = false,
                        Code = "400",
                        Data = returnType.Msg
                    });
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                    };
                }
            }
            catch (Exception ex)
            {
                string test = ex.ToString();
                string json = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "400",
                    Data = test
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
        }
      
    }
}