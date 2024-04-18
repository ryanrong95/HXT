using DBSApis.Models;
using DBSApis.Services;
using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DBSApis.Controllers
{
    public class DBSJsonController : System.Web.Http.ApiController
    {
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetABECNYJson")]
        public HttpResponseMessage GetABECNYJson()
        {
            ABERequest aBERequest = getABERequest();

            aBERequest.accountBalInfo = new ABEAccountBalInfo();
            aBERequest.accountBalInfo.accountNo = DBSConstConfig.DBSConstConfiguration.CNYAccountNo;           

            return new HttpResponseMessage()
            {
                Content = new StringContent(aBERequest.Json(), Encoding.UTF8, "application/json"),
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetABEUSDJson")]
        public HttpResponseMessage GetABEUSDJson()
        {
            ABERequest aBERequest = getABERequest();

            aBERequest.accountBalInfo = new ABEAccountBalInfo();
            aBERequest.accountBalInfo.accountNo = DBSConstConfig.DBSConstConfiguration.USDAccountNo;          

            return new HttpResponseMessage()
            {
                Content = new StringContent(aBERequest.Json(), Encoding.UTF8, "application/json"),
            };
        }

        private ABERequest getABERequest()
        {
            ABERequest aBERequest = new ABERequest();

            aBERequest.header = new ABEHeader();
            aBERequest.header.msgId = ChainsGuid.NewGuidUp();
            aBERequest.header.orgId = ApiService.Current.KeyConfigs.OrgId;
            aBERequest.header.timeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            aBERequest.header.ctry = DBSConstConfig.DBSConstConfiguration.Ctry;

            aBERequest.txnInfo = new ABETxnInfo();
            aBERequest.txnInfo.txnType = DBSConstConfig.DBSConstConfiguration.ABETxtType;

            return aBERequest;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetABEUrl")]
        public HttpResponseMessage GetABEUrl()
        {
            UrlConfig urlConfig = ApiService.Current.UrlConfigs;
            string postUrl = urlConfig.ApiServerUrl + urlConfig.ABEUrl;

            string json = JsonConvert.SerializeObject(new
            {
                Success = true,
                Code = "200",
                Data = postUrl
            });
            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetFXPricingJson")]
        public HttpResponseMessage GetFXPricingJson()
        {
            FXQuoteRequest fXQuoteRequest = new FXQuoteRequest();

            fXQuoteRequest.header = new FXQuoteHeader();
            fXQuoteRequest.header.msgId = ChainsGuid.NewGuidUp();
            fXQuoteRequest.header.orgId = ApiService.Current.KeyConfigs.OrgId;
            fXQuoteRequest.header.timeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");

            fXQuoteRequest.txnInfo = new FXQuoteTxnInfo();
            fXQuoteRequest.txnInfo.ccyPair = "USDCNY";
            fXQuoteRequest.txnInfo.dealtSide = "BUY";
            fXQuoteRequest.txnInfo.txnAmount = 1300m;
            fXQuoteRequest.txnInfo.txnCcy = "USD";
            fXQuoteRequest.txnInfo.tenor = "TODAY";
            fXQuoteRequest.txnInfo.clientTxnsId = ChainsGuid.NewGuidUp();

            return new HttpResponseMessage()
            {
                Content = new StringContent(fXQuoteRequest.Json(), Encoding.UTF8, "application/json"),
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetFXBookingJson")]
        public HttpResponseMessage GetFXBookingJson(string uid)
        {
            FXBookingRequest fXBookingRequest = new FXBookingRequest();

            fXBookingRequest.header = new FXQuoteHeader();
            fXBookingRequest.header.msgId = ChainsGuid.NewGuidUp();
            fXBookingRequest.header.orgId = ApiService.Current.KeyConfigs.OrgId;
            fXBookingRequest.header.timeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");

            fXBookingRequest.txnInfo = new FXBookingTxnInfo();
            fXBookingRequest.txnInfo.uid = uid;           
            fXBookingRequest.txnInfo.clientTxnsId = ChainsGuid.NewGuidUp();

            return new HttpResponseMessage()
            {
                Content = new StringContent(fXBookingRequest.Json(), Encoding.UTF8, "application/json"),
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetACTJson")]
        public HttpResponseMessage GetACTJson(decimal totalTxnAmount,string fxContractRef1)
        {
            ACTRequest aCTRequest = new ACTRequest();

            aCTRequest.header = new ACTRequestHeader();
            aCTRequest.header.msgId = ChainsGuid.NewGuidUp();
            aCTRequest.header.orgId = ApiService.Current.KeyConfigs.OrgId;
            aCTRequest.header.timeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            aCTRequest.header.ctry = DBSConstConfig.DBSConstConfiguration.Ctry;
            aCTRequest.header.noOfTxs = 1;
            aCTRequest.header.totalTxnAmount = totalTxnAmount;

            aCTRequest.txnInfoDetails = new ACTRequestTxnInfoDetails();
            aCTRequest.txnInfoDetails.txnInfo = new List<ACTRequestTxnInfo>();

            ACTRequestTxnInfo aCTRequestTxnInfo = new ACTRequestTxnInfo();
            aCTRequestTxnInfo.customerReference = ChainsGuid.NewGuidUp().Substring(0, 15);
            aCTRequestTxnInfo.txnType = DBSConstConfig.DBSConstConfiguration.ACTTxnType;
            aCTRequestTxnInfo.txnDate = DateTime.Now.ToString("yyyy-MM-dd");
            aCTRequestTxnInfo.txnCcy = DBSConstConfig.DBSConstConfiguration.USD;
            aCTRequestTxnInfo.txnAmount = totalTxnAmount;
            aCTRequestTxnInfo.debitAccountCcy = DBSConstConfig.DBSConstConfiguration.CNY;
            aCTRequestTxnInfo.fxContractRef1 = fxContractRef1;
            aCTRequestTxnInfo.fxAmountUtilized1 = totalTxnAmount;

            aCTRequestTxnInfo.senderParty = new ACTSenderParty();
            aCTRequestTxnInfo.senderParty.name = "XDT";
            aCTRequestTxnInfo.senderParty.accountNo = DBSConstConfig.DBSConstConfiguration.CNYAccountNo;
            aCTRequestTxnInfo.senderParty.bankCtryCode = DBSConstConfig.DBSConstConfiguration.Ctry;
            aCTRequestTxnInfo.senderParty.swiftBic = ApiService.Current.KeyConfigs.swiftBic;

            aCTRequestTxnInfo.receivingParty = new ACTReceivingParty();
            aCTRequestTxnInfo.receivingParty.name = "XDT";
            aCTRequestTxnInfo.receivingParty.accountNo = DBSConstConfig.DBSConstConfiguration.USDAccountNo;

            aCTRequest.txnInfoDetails.txnInfo.Add(aCTRequestTxnInfo);

            return new HttpResponseMessage()
            {
                Content = new StringContent(aCTRequest.Json(), Encoding.UTF8, "application/json"),
            };
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetCNAPSJson")]
        public HttpResponseMessage GetCNAPSJson(decimal totalTxnAmount,string receivingPartyName,string accountNo,string receivingPartybankName)
        {
            CNAPSRequest aCTRequest = new CNAPSRequest();

            aCTRequest.header = new ACTRequestHeader();
            aCTRequest.header.msgId = ChainsGuid.NewGuidUp();
            aCTRequest.header.orgId = ApiService.Current.KeyConfigs.OrgId;
            aCTRequest.header.timeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            aCTRequest.header.ctry = DBSConstConfig.DBSConstConfiguration.Ctry;
            aCTRequest.header.noOfTxs = 1;
            aCTRequest.header.totalTxnAmount = totalTxnAmount;

            aCTRequest.txnInfoDetails = new CNAPSRequestTxnInfoDetails();
            aCTRequest.txnInfoDetails.txnInfo = new List<CNAPSRequestTxnInfo>();

            CNAPSRequestTxnInfo aCTRequestTxnInfo = new CNAPSRequestTxnInfo();
            aCTRequestTxnInfo.customerReference = ChainsGuid.NewGuidUp().Substring(0, 15);
            aCTRequestTxnInfo.txnType = DBSConstConfig.DBSConstConfiguration.CNAPSTxnType;
            aCTRequestTxnInfo.txnDate = DateTime.Now.ToString("yyyy-MM-dd");
            aCTRequestTxnInfo.txnCcy = DBSConstConfig.DBSConstConfiguration.CNY;
            aCTRequestTxnInfo.txnAmount = totalTxnAmount;
            aCTRequestTxnInfo.debitAccountCcy = DBSConstConfig.DBSConstConfiguration.CNY;
           

            aCTRequestTxnInfo.senderParty = new CNAPSSenderParty();
            aCTRequestTxnInfo.senderParty.name = "XINDATONGSUPPLYCHAIN";
            aCTRequestTxnInfo.senderParty.accountNo = DBSConstConfig.DBSConstConfiguration.CNYAccountNo;
            aCTRequestTxnInfo.senderParty.bankCtryCode = DBSConstConfig.DBSConstConfiguration.Ctry;
            aCTRequestTxnInfo.senderParty.swiftBic = ApiService.Current.KeyConfigs.swiftBic;

            aCTRequestTxnInfo.receivingParty = new CNAPSReceivingParty();            
            aCTRequestTxnInfo.receivingParty.name = receivingPartyName;
            aCTRequestTxnInfo.receivingParty.accountNo = accountNo;
            aCTRequestTxnInfo.receivingParty.bankCtryCode = DBSConstConfig.DBSConstConfiguration.Ctry;            
            aCTRequestTxnInfo.receivingParty.bankName = receivingPartybankName;

            aCTRequest.txnInfoDetails.txnInfo.Add(aCTRequestTxnInfo);

            return new HttpResponseMessage()
            {
                Content = new StringContent(aCTRequest.Json(), Encoding.UTF8, "application/json"),
            };
        }

       

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetTTJson")]
        public HttpResponseMessage GetTTJson(decimal totalTxnAmount,string txnRefId,string chargeBearer)
        {
            string senderPartyName = "XINDATONGSUPPLYCHAIN";
            string receivingPartyName = "HONG KONG CHANGYUN INTERNATIONAL";
            string receivingPartyAccountNo = "016478000502260";
            string receivingPartySwiftBic = "DHBKHKH0XXX";
            string receivingBankName = "DBS Bank (Hong Kong) Limited";
            
            TTRequest aCTRequest = new TTRequest();

            aCTRequest.header = new ACTRequestHeader();
            aCTRequest.header.msgId = ChainsGuid.NewGuidUp();
            aCTRequest.header.orgId = System.Configuration.ConfigurationManager.AppSettings["Api_OrgId"];
            aCTRequest.header.timeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            aCTRequest.header.ctry = DBSConstConfig.DBSConstConfiguration.Ctry;
            aCTRequest.header.noOfTxs = 1;
            aCTRequest.header.totalTxnAmount = totalTxnAmount;

            aCTRequest.txnInfoDetails = new TTRequestTxnInfoDetails();
            aCTRequest.txnInfoDetails.txnInfo = new List<TTRequestTxnInfo>();

            TTRequestTxnInfo aCTRequestTxnInfo = new TTRequestTxnInfo();
            aCTRequestTxnInfo.customerReference = ChainsGuid.NewGuidUp().Substring(0, 15);
            aCTRequestTxnInfo.txnType = DBSConstConfig.DBSConstConfiguration.TTTxnType;
            aCTRequestTxnInfo.txnDate = DateTime.Now.ToString("yyyy-MM-dd");
            aCTRequestTxnInfo.txnCcy = DBSConstConfig.DBSConstConfiguration.USD;
            aCTRequestTxnInfo.txnAmount = totalTxnAmount;
            aCTRequestTxnInfo.debitAccountCcy = DBSConstConfig.DBSConstConfiguration.CNY;
            aCTRequestTxnInfo.fxContractRef1 = txnRefId;
            aCTRequestTxnInfo.fxAmountUtilized1 = totalTxnAmount;
            aCTRequestTxnInfo.chargeBearer = chargeBearer;


            aCTRequestTxnInfo.senderParty = new CNAPSSenderParty();
            aCTRequestTxnInfo.senderParty.name = senderPartyName;
            aCTRequestTxnInfo.senderParty.accountNo = DBSConstConfig.DBSConstConfiguration.CNYAccountNo;
            aCTRequestTxnInfo.senderParty.bankCtryCode = DBSConstConfig.DBSConstConfiguration.Ctry;
            aCTRequestTxnInfo.senderParty.swiftBic = System.Configuration.ConfigurationManager.AppSettings["Api_SwiftBic"];


            aCTRequestTxnInfo.receivingParty = new TTReceivingParty();
            aCTRequestTxnInfo.receivingParty.name = receivingPartyName;
            aCTRequestTxnInfo.receivingParty.accountNo = receivingPartyAccountNo;
            aCTRequestTxnInfo.receivingParty.bankCtryCode = DBSConstConfig.DBSConstConfiguration.HKCtry;
            aCTRequestTxnInfo.receivingParty.swiftBic = receivingPartySwiftBic;
            aCTRequestTxnInfo.receivingParty.bankName = receivingBankName;
            aCTRequestTxnInfo.receivingParty.beneficiaryAddresses = new List<TTBeneficiaryAddresses>();
            TTBeneficiaryAddresses addresses = new TTBeneficiaryAddresses();
            addresses.address = "LOGISTICS CO., LIMITED";
            aCTRequestTxnInfo.receivingParty.beneficiaryAddresses.Add(addresses);


            aCTRequestTxnInfo.bopInfo = new TTBopInfo();
            aCTRequestTxnInfo.bopInfo.specificPaymentPurpose = TTPaymentPurpose.PD.ToString();
            aCTRequestTxnInfo.bopInfo.taxFreeGoodsRelated = TTTaxFree.N.ToString();
            aCTRequestTxnInfo.bopInfo.paymentNature = TTPaymentNature.F.ToString();
            aCTRequestTxnInfo.bopInfo.bOPCode1PaymentCategory = TTPaymentCategory.TR.ToString();
            aCTRequestTxnInfo.bopInfo.bOPCode1SeriesCode = TTBOPCode.TT121010.ToString().Replace("TT", "");
            //aCTRequestTxnInfo.bopInfo.transactionRemarks1 = "一般贸易进口电子产品";
            aCTRequestTxnInfo.bopInfo.transactionRemarks1 = "General trading in import electronic goods";
            aCTRequestTxnInfo.bopInfo.counterPartyCtryCode = DBSConstConfig.DBSConstConfiguration.HKCtry;

            aCTRequest.txnInfoDetails.txnInfo.Add(aCTRequestTxnInfo);

            return new HttpResponseMessage()
            {
                Content = new StringContent(aCTRequest.Json(), Encoding.UTF8, "application/json"),
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetDecryptMessage")]
        public HttpResponseMessage GetDecryptMessage()
        {
            string EncryptMsg = "-----BEGIN PGP MESSAGE-----\nVersion: BCPG v1.60\n\nhQEMAzpt8q9W506tAQgAk7FZohl1WUR2NaNmOzYnwKWo+p0fXXiST5YXhKD98zn1\nGEp+10R0k29AZDm+3P+OvkO/oNYNi+j6vF3ZItFhRRAs+Qp9nAcdLixXpa5aqpHD\nneD7xbDAB6HFTxXklUap29BvgEP2sYur9L9GkYCXbBo0nwc6xF1WZ7dQcMjNAIXM\nqCvO6CDEYgJSZ6MEImzhLtav869zAbTvHfonCGSXDnhiUbilAl1NAQx6f2bG+BBw\ngQ9+64ka8xrZarjs51Fw//OoAqpxKGRCqAAEih7SpxxoUPQI4Z+Y5FVMeLlvbtqj\nR0Kk5b8TWBXZg1WyG23RPZJrk0KU0OOAZwlfX0I8NNLCOAE26DgPtE+A+Qq3OqGE\nDXSPlfhEIP66ydCNYj74tyCjr4UbgNXEfyASk7ON4FHpzwfyJZhIJVZg2mByRKEc\nSBSMJKDR4a3+tiX6h+k8nugHvSZHPw9Rp4cYfZVDO/ap1RAI0Uu8KmO4GFuJk6uL\nnzMo/wIgTiiBEU+rqyqtX44Opz//B/YYlsKfu262ISzBryj9S4JlBneKQePM5jTY\nmkPnUqCZQ0xxlaTksDt2L6a+xB4qKozHehbqxQvkz0Vcq9mZIb6NAb4A3kn/glLR\nXIaIA6yQmetSlPUDE3kJ+pbfSeY1xO1mHRyZ7yS8GkVemiKzTG8W7yFJOeg8/3bD\nu3l01xBRhxjTF6iOgWmbqKH1Ti2iCB00mVVw987c9NCrboDawM4CRWIR8AGSEFuT\nmT+g/gPI5JIBaoNjGn6LtBWZP8hwyiw4an0plPlZWPlrcP5vMM+CN7roo0Diub9/\nXu2w8+y5HOPWadJpt/qkn36Xc94S1Py7Rtb7rIvbNHEUv7HnZE5P8QzXffxStBKJ\noW00/IuF7+46ao7YGQ+HpWBf8DU5RQtdd8DMvIvqeRqyEcPOxzthNJT/OUgwxzga\nNV4UL0ptWS5j0P7BeiZXXSLJ30QO0bQG0jQOik3oG4ahsDOEnP4TwtC2YzZ/7u3a\ndyfPnbpedbmV323E9JwwtBPusjH5XOX6RJ/m13laWii2c+BvbpCN2DG41s/+mejO\neqn8FLI6MPsTiSDsJUuIL2mSjuDDkN8mN1Cd6H74CKc+tNDS7jOa51WNLgz1m7af\nRDXNQr7nyCWETf68KqECkwoTd9ncuhXax+sFwUkIDuG9vGKWG147W4Fcp3r2CQD9\nocIjY5iywXGnOM2216EXcIqItzN3udsKzg+Yi2l0SaztV3xeGAy/Hcg9MFa+YMn/\noSwUerGO2D1XZY/dGgBtE9uH75J2WNTzuMbrVgq/8lx+Zbro8gQtfTstdeAbRt37\nAD3kuWbyv9nqEx8uvGUqgfyc+0CuRLhF6Qo=\n=MUYX\n-----END PGP MESSAGE-----";
            KeyConfig Config = ApiService.Current.KeyConfigs;
            DecryptMessage decryptMessage = new DecryptMessage(Config, EncryptMsg);
            string decryptedMsg = decryptMessage.Decrypt();
            
            return new HttpResponseMessage()
            {
                Content = new StringContent(decryptedMsg, Encoding.UTF8, "application/json"),
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/dbs/testDB")]
        public HttpResponseMessage testDB()
        {
            try
            {
                using (var reponsitory = new Layer.Data.Sqls.ForicDBSReponsitory())
                {
                    reponsitory.Insert<Layer.Data.Sqls.foricDBS.ApiLogs>(new Layer.Data.Sqls.foricDBS.ApiLogs
                    {
                        ID = ChainsGuid.NewGuidUp(),
                        TransactionName = "ARE",
                        msgId = "test",
                        RequestContent = "dbtest",
                        Status = (int)Needs.Ccs.Services.Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    });
                }

                string json = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "200",
                    Data = "查询余额成功"
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
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

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("dbs/testSubstring")]
        public HttpResponseMessage testSubstring()
        {
            string test = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");

            string decryptedMsg = "2020-10-14T07:45:22.764";
            DateTime dt1 = Convert.ToDateTime(decryptedMsg);
            DateTime dt = Convert.ToDateTime(decryptedMsg.Substring(0, 23).Replace("T", " "));
            return new HttpResponseMessage()
            {
                Content = new StringContent(decryptedMsg, Encoding.UTF8, "application/json"),
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("dbs/testFile2Base64")]
        public HttpResponseMessage testFile2Base64()
        {
            File2Base64 file2Base64 = new File2Base64();
            string file64 = file2Base64.FileToBase64String();
            return new HttpResponseMessage()
            {
                Content = new StringContent(file64, Encoding.UTF8, "application/json"),
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("dbs/testBase642File")]
        public HttpResponseMessage testBase642File()
        {
            string fileBase64 = "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C3N5upi1mj09NfJ8fqGfdrJur9l9qNtuJAvpoNlonl9sLaYLC2lyurXUXlzUc6eh51oca/uYa0MYt1YHnZUkGSf9YT/rrjQHk8Z4uNjIp4NG/9UsTTvJKF5c0lZ6I1qI3ot+GI2jnZ2B2sh29AhnO4OhNhU7qVVeP90ZZ/qm3/uqavbH3xgvNltfbS5+tZlrKTd8VFZn/0LrZSOqmimPGFF2LNJdfiGJkxfiJF5c7MdZZ+GH/6Cw49/RXW61NAW5puB9LXv3RX/8k2ikpcXotN5H7kZP0f/lRz09A89oeTFyaK+aoONm3Jd0wtwZ82d5bP4Y0Ys2xomm+g91arLRXbdip8uvnxh3x93p1JhL/bhlOnZ8pG+2p2fmpydeur2++dXp+ku3lwdqmg++oWH+hduDRTuV+0ZIeo9rMfmfzFWLibZH3V/o/5CNyAiGG9almW7E8GsZMZy42Jw+cumSlp8vdbtZb0Er+UbzWG95Kc02JvOfiLO5RuNLi0uDXvd7/UGeNBYWtGQ0W3ra9rPldH3aaLe383h90DumudbRM9Zw7bHoX/4kerRgzUNG55lped7x6riZY0/qG91IdOUBa38m5nZiOTU0gz1cNrZoaBMKC3VpZbwRbWxcPL1xzg5/x4jERmPl4rlzF1e0hjZp5zULoycal1ZXXri9umPdldX5C5c2B2r+/gvfuLD5wu35wQu31XysHxbnc2f/tebT/7W8FKyEDdoujFC/NEFRMdMqbGyWWnJa2Jkabo/H76+v9FdbC43FublOrA3J6nDQ6Ta7c3PanDRXe6az4+5Dg41h3ojj+cX5hfmGZnN/3Bz02qOmnkNz2ndptDqDlXZntNha/f8A4Xbw+g0KZW5kc3RyZWFtDWVuZG9iag0yIDAgb2JqDTw8L0NvdW50IDEgL0tpZHMgWzYgMCBSXSAvVHlwZSAvUGFnZXM+Pg1lbmRvYmoNNCAwIG9iag08PC9OYW1lcyBbXT4+DWVuZG9iag01IDAgb2JqDTw8Pj4NZW5kb2JqDXhyZWYNCjAgMTUNCjAwMDAwMDAwMDAgNjU1MzUgZg0KMDAwMDAwMDAxNiAwMDAwMCBuDQowMDAwMDI3NjY4IDAwMDAwIG4NCjAwMDAwMDAxMDMgMDAwMDAgbg0KMDAwMDAyNzcyMyAwMDAwMCBuDQowMDAwMDI3NzUyIDAwMDAwIG4NCjAwMDAwMDA0NDIgMDAwMDAgbg0KMDAwMDAwMDU5NyAwMDAwMCBuDQowMDAwMDAwODUzIDAwMDAwIG4NCjAwMDAwMDA5OTAgMDAwMDAgbg0KMDAwMDAwMTQwNCAwMDAwMCBuDQowMDAwMDAxNTc1IDAwMDAwIG4NCjAwMDAwMDE4ODQgMDAwMDAgbg0KMDAwMDAwMDM3MCAwMDAwMCBuDQowMDAwMDAxMzMxIDAwMDAwIG4NCnRyYWlsZXI8PC9TaXplIDE1IC9Sb290IDEgMCBSIC9JbmZvIDMgMCBSIC9JRCBbPDAxMmY5MWVlNjI5MTRhMmFhZGZhMGM3ZmY0MDJkMDY1PjxiNGQwYTZiMzcwZmY0NTVlYjcyNzUyN2Q5NjY1YjJlMz5dPj4Nc3RhcnR4cmVmDTI3NzcyDSUlRU9GDQ==";
            File2Base64 file2Base64 = new File2Base64();
            string fileName = "Icgoo.pdf";
            file2Base64.Base64StringToFile(fileBase64, fileName);
            return new HttpResponseMessage()
            {
                Content = new StringContent("", Encoding.UTF8, "application/json"),
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetAREJson")]
        public HttpResponseMessage GetAREJson(string AccountNo,string ccy,string fromdate,string todate)
        {
            ARERequest aRERequest = new ARERequest();
            aRERequest.header = new AREHeader();
            aRERequest.header.msgId = ChainsGuid.NewGuidUp();
            aRERequest.header.orgId = ApiService.Current.KeyConfigs.OrgId;
            aRERequest.header.timeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            aRERequest.header.ctry = DBSConstConfig.DBSConstConfiguration.Ctry;

            aRERequest.accInfo = new AREAccInfo();
            aRERequest.accInfo.accountNo = AccountNo;
            aRERequest.accInfo.accountCcy = ccy;
            aRERequest.accInfo.fromDate = fromdate;
            aRERequest.accInfo.toDate = todate;

            return new HttpResponseMessage()
            {
                Content = new StringContent(aRERequest.Json(), Encoding.UTF8, "application/json"),
            };
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetEncrypetMsg")]
        public string EncryptMsg()
        {
            KeyConfig Config = ApiService.Current.KeyConfigs;
            UrlConfig urlConfig = ApiService.Current.UrlConfigs;
            string msg = "This is XDT SUPPLY CHAIN MANAGEMENT";

            EncryptMessage encryptMessage = new EncryptMessage(Config, msg);
            string encryptedMessage = encryptMessage.Encrypt();

            return encryptedMessage;
        }
    }
}