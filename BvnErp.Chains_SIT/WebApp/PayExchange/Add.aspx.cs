using Layer.Data.Sqls.ScCustoms;
using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.PayExchange
{
    public partial class Add : Uc.PageBase
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
            //付款方式
            this.Model.PaymentTypeData = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.PaymentType>()
                .Select(item => new { item.Key, item.Value }).Json();

            #region 付汇供应商
            this.Model.SupplierData = "".Json();
            this.Model.IsadvanceMoney = "".Json();
            if (string.IsNullOrEmpty(Request.QueryString["ids"]))
            {
                return;
            }
            string[] orderIds = Request.QueryString["ids"].Split(',');
            var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders.Where(item => orderIds.Contains(item.ID));
            if (orders.Count() > 0)
            {
                // 查询此客户是否存在已审批生效的垫资申请  by 2020 - 12 - 23 yess
                var advanceMoneyApply = new Needs.Ccs.Services.Views.AdvanceMoneyApplyView().FirstOrDefault(x => x.ClientID == orders.FirstOrDefault().Client.ID && x.Status == AdvanceMoneyStatus.Effective);
                if (advanceMoneyApply != null)
                {
                    this.Model.IsadvanceMoney = 0;//有值就是存在垫资申请
                }
                else
                {
                    this.Model.IsadvanceMoney = "";
                }
                //查询可用垫资额度  by 2021-01-05 yess

                var availableProductFeeView = new Needs.Ccs.Services.Views.AvailableProductFeeView(orders.FirstOrDefault().Client.ID);
                decimal availableProductFee = availableProductFeeView.GetProductAdvanceMoneyApply();
                this.Model.AvailableProductFee = availableProductFee;

                if (orders.Count() == 1)
                {
                    //取第一个元素ID
                    var supplier = orders.FirstOrDefault().PayExchangeSuppliers;
                    //this.Model.SupplierData = supplier.Select(item => new
                    //{
                    //    ID = item.ClientSupplier.ID,
                    //    ChineseName = item.ClientSupplier.ChineseName,
                    //}).Json();
                    var places = supplier.Select(t => t.ClientSupplier.Place).ToArray();
                    var placeTypes = GetPlaceTypes(places);

                    for (int i = 0; i < supplier.Count; i++)
                    {
                        var thePlaceType = placeTypes.Where(t => t.Code == supplier[i].ClientSupplier.Place).FirstOrDefault();
                        if (thePlaceType != null)
                        {
                            supplier[i].ClientSupplier.PlaceType = thePlaceType.TypeInt;
                        }
                    }

                    this.Model.SupplierData = supplier.Select(item => new
                    {
                        ID = item.ClientSupplier.ID,
                        PlaceType = item.ClientSupplier.PlaceType,
                        ChineseName = item.ClientSupplier.ChineseName,
                    }).Json();
                }
                else
                {
                    //取第一个元素ID
                    var supplier = orders.FirstOrDefault().PayExchangeSuppliers.Select(item => item.ClientSupplier.ID);
                    //做交集
                    foreach (var order in orders)
                    {
                        supplier = supplier.Intersect(order.PayExchangeSuppliers.Select(item => item.ClientSupplier.ID));
                    }
                    //查询客户供应商
                    var suppliers = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientSuppliers.Where(item => supplier.Contains(item.ID)).ToList();

                    var places = suppliers.Select(t => t.Place).ToArray();
                    var placeTypes = GetPlaceTypes(places);

                    for (int i = 0; i < suppliers.Count; i++)
                    {
                        var thePlaceType = placeTypes.Where(t => t.Code == suppliers[i].Place).FirstOrDefault();
                        if (thePlaceType != null)
                        {
                            suppliers[i].PlaceType = thePlaceType.TypeInt;
                        }
                    }

                    this.Model.SupplierData = suppliers.Select(item => new
                    {
                        ID = item.ID,
                        PlaceType = item.PlaceType,
                        ChineseName = item.ChineseName,
                    }).Json();
                }
            }
            #endregion
        }

        /// <summary>
        /// 获取 PlaceType
        /// </summary>
        /// <param name="places"></param>
        /// <returns></returns>
        private List<Needs.Ccs.Services.Models.Country> GetPlaceTypes(string[] places)
        {
            var countries = new Needs.Ccs.Services.Views.Origins.BaseCountriesOrigin();

            var placeTypes = (from country in countries
                              where places.Contains(country.Code)
                              group country by new { country.Code, } into g
                              select new Needs.Ccs.Services.Models.Country
                              {
                                  Code = g.Key.Code,
                                  TypeInt = g.FirstOrDefault().TypeInt,
                              }).ToList();

            return placeTypes;
        }

        /// <summary>
        /// 加载付汇订单数据
        /// </summary>
        protected void data()
        {
            if (string.IsNullOrEmpty(Request.QueryString["ids"]))
            {
                return;
            }
            string[] orderIds = Request.QueryString["ids"].Split(',');
            //查询待付汇订单
            var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyUnPayExchangeOrders.Where(item => orderIds.Contains(item.ID));

            //取汇率数据
            var order = orders.FirstOrDefault();
            if (order == null)
            {
                return;
            }
            //汇率类型
            Needs.Ccs.Services.Enums.ExchangeRateType RateType = order.Client.Agreement.ProductFeeClause.ExchangeRateType;
            //汇率值
            decimal rate = 0M;
            ExchangeRate exchangeRate = null;

            if (RateType == Needs.Ccs.Services.Enums.ExchangeRateType.Agreed)
            {
                rate = order.Client.Agreement.ProductFeeClause.ExchangeRateValue.Value;
            }
            else if (RateType == Needs.Ccs.Services.Enums.ExchangeRateType.Custom)
            {
                exchangeRate = new Needs.Ccs.Services.Views.CustomExchangeRatesView(order.Currency).ToRate();
            }
            else if (RateType == Needs.Ccs.Services.Enums.ExchangeRateType.RealTime)
            {
                if (order.Client.Agreement.IsTen == PEIsTen.Nine)
                {
                    exchangeRate = new Needs.Ccs.Services.Views.NineRealTimeExchangeRatesView(order.Currency).ToRate();
                }
                else
                {
                    exchangeRate = new Needs.Ccs.Services.Views.RealTimeExchangeRatesView(order.Currency).ToRate();
                }
            }

            if (exchangeRate != null)
            {
                rate = exchangeRate.Rate; //实时汇率,海关汇率
            }

            Func<UnPayExchangeOrder, object> linq = item => new
            {
                ID = item.ID,
                Currency = item.Currency,
                DeclarePrice = item.DeclarePrice,
                PaidExchangeAmount = item.PaidExchangeAmount,
                PaidAmount = item.DeclarePrice - item.PaidExchangeAmount,//可付汇金额
                CurrentPaidAmount = item.DeclarePrice - item.PaidExchangeAmount,//本次付汇金额
                //扩展字段
                ClientID = item.Client.ID,
                ExchangeRateType = RateType,
                ExchangeRate = rate,
                IsMatchSupplier = false,  //默认都不匹配供应商
                MatchSupplierAmount = 0,  //如果匹配供应商，匹配的金额
                RmbAmount = (item.DeclarePrice - item.PaidExchangeAmount) * rate //人民币
            };
            Response.Write(new
            {
                rows = orders.Select(linq).ToArray()
            }.Json());
        }

        /// <summary>
        /// 加载PI文件
        /// </summary>
        protected void filedata()
        {
            if (string.IsNullOrEmpty(Request.QueryString["ids"]))
            {
                return;
            }
            string[] orderIds = Request.QueryString["ids"].Split(',');
            var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders.Where(item => orderIds.Contains(item.ID));
            List<MainOrderFile> fileList = new List<MainOrderFile>();

            //勾选的订单，是同一个主订单下的，则文件只要一份
            foreach (var order in orders)
            {
                foreach (var file in order.MainOrderFiles.Where(t => t.FileType == FileType.OriginalInvoice))
                {
                    if (!fileList.Any(t => t.ID == file.ID))
                    {
                        fileList.Add(file);
                    }
                }
                //if (order.MainOrderFiles.Count > 0)
                //{
                //    fileList.AddRange(order.MainOrderFiles.Where(t => t.FileType == FileType.OriginalInvoice));
                //}
            }

            fileList = fileList.DistinctBy(t => t.Name).ToList();
            var t1 = Convert.ToDateTime(FileDirectory.Current.IsChainsDate);
            Func<MainOrderFile, object> linq = item => new
            {
                FileName = item.Name,
                FileFormat = item.FileFormat,
                Url = item.Url,    //数据库相对路径
                WebUrl = DateTime.Compare(item.CreateDate, t1) > 0 ? FileDirectory.Current.PvDataFileUrl + "/" + item.Url.ToUrl() :
                      FileDirectory.Current.FileServerUrl + "/" + item.Url?.ToUrl(),
                //  WebUrl = FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),//查看路径

            };
            Response.Write(new
            {
                rows = fileList.Select(linq).ToArray(),
                total = fileList.Count(),
            }.Json());
        }

        /// <summary>
        /// 上传图片文件
        /// </summary>
        protected void UploadFiles()
        {
            try
            {
                List<dynamic> fileList = new List<dynamic>();
                HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            //文件保存
                            string fileName = file.FileName.ReName();

                            //创建文件目录
                            FileDirectory fileDic = new FileDirectory(fileName);
                            fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Order);
                            fileDic.CreateDataDirectory();
                            file.SaveAs(fileDic.FilePath);
                            fileList.Add(new
                            {
                                FileName = file.FileName,
                                FileFormat = file.ContentType,
                                Url = fileDic.VirtualPath,
                                WebUrl = fileDic.FileUrl,
                            });
                        }
                    }
                }
                if (fileList.Count() == 0)
                {
                    Response.Write((new { success = false, data = "上传文件为空" }).Json());
                }
                Response.Write((new { success = true, data = fileList }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, data = ex.Message }).Json());
            }
        }

        /// <summary>
        /// 选择供应商名称、订单本次申请金额
        /// </summary>
        protected void SelectSupplier()
        {
            #region 获取供应商银行+地址

            string clientID = Request.Form["ClientID"];
            if (string.IsNullOrEmpty(clientID))
            {
                Response.Write((new { success = false, data = "客户ID为空！" }).Json());
                return;
            }

            var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientSuppliers.
                    Where(item => item.ID == clientID).FirstOrDefault();
            if (client == null)
            {
                Response.Write((new { success = false, data = "客户供应商对象为空！" }).Json());
                return;
            }
            //获取供应商的银行信息
            var banks = client.Banks.Select(t => new
            {
                ID = t.ID,
                Name = t.BankName
            });
            var address = client.Addresses.Select(t => new
            {
                ID = t.ID,
                Name = t.Address
            });

            #endregion

            #region 订单本次申请金额

            string orderIDsStr = Request.Form["OrderIDs"];
            string supplierID = Request.Form["ClientID"];

            string[] orderIDs = orderIDsStr.Split(',');

            List<Needs.Wl.Models.OrderCurrentPayAmount> payAmounts = new List<Needs.Wl.Models.OrderCurrentPayAmount>();

            foreach (var orderID in orderIDs)
            {
                var view = new Needs.Wl.Models.Views.OrderCurrentPayAmountView(orderID, supplierID);
                var thisOrderCurrentPayAmount = view.FirstOrDefault();
                if (thisOrderCurrentPayAmount != null)
                {
                    payAmounts.Add(thisOrderCurrentPayAmount);
                }
            }

            var currentPayAmounts = (from orderID in orderIDs
                                     join payAmount in payAmounts on orderID equals payAmount.ID into payAmounts2
                                     from payAmount in payAmounts2.DefaultIfEmpty()
                                     select new Needs.Wl.Models.OrderCurrentPayAmount
                                     {
                                         ID = orderID,
                                         IsMatchSupplier = payAmount != null,
                                         CurrentPaidAmount = payAmount != null ? payAmount.CurrentPaidAmount : 0,
                                     }).ToList()
                                     .Select(t => new
                                     {
                                         OrderID = t.ID,
                                         IsMatchSupplier = t.IsMatchSupplier,
                                         CurrentPaidAmount = t.CurrentPaidAmount.ToRound(2),
                                     }).ToList();

            #endregion

            var data = new
            {
                EnglishName = client.Name,
                Banks = banks,
                Address = address,
                currentPayAmounts = currentPayAmounts,
            };
            Response.Write((new { success = true, data = data }).Json());
        }

        /// <summary>
        /// 检查供应商银行敏感性
        /// </summary>
        private bool CheckSupplierBankSensitive(string supplierBankAddress)
        {
            if (string.IsNullOrEmpty(supplierBankAddress))
            {
                return false;
            }

            supplierBankAddress = supplierBankAddress.Trim();
            var splittedStrs = Split(supplierBankAddress);

            splittedStrs = splittedStrs.Distinct().ToArray();
            splittedStrs = splittedStrs.Where(t => t.Length != 1).ToArray();

            int pageSize = 1000;
            int pageCount = (splittedStrs.Length / pageSize) + (splittedStrs.Length % pageSize > 0 ? 1 : 0);

            bool isSensitive = false;

            for (int i = 1; i <= pageCount; i++)
            {
                var theSplittedStrs = splittedStrs.Skip(pageSize * (i - 1)).Take(pageSize);

                var predicate = PredicateBuilder.Create<Needs.Ccs.Services.Views.PayExchangeSensitiveWordCheckModel>();
                predicate = predicate.And(t => t.WordContent == "qwertyuiopasdfg"); //这里是用来使得下面的 Or 生效的

                foreach (var splittedStr in theSplittedStrs)
                {
                    predicate = predicate.Or(t => t.WordContent == splittedStr);
                }

                var payExchangeSensitiveWordCheckView = new Needs.Ccs.Services.Views.PayExchangeSensitiveWordCheckView(Needs.Ccs.Services.Enums.PayExchangeSensitiveAreaType.Forbid);
                payExchangeSensitiveWordCheckView.AllowPaging = false;
                payExchangeSensitiveWordCheckView.Predicate = predicate;
                var oneSensitiveArea = payExchangeSensitiveWordCheckView.FirstOrDefault();

                if (oneSensitiveArea == null)
                {
                    isSensitive = false;
                }
                else
                {
                    isSensitive = true;
                    break;
                }
            }

            return isSensitive;
        }

        #region 切割字符串

        static string[] Split(string origin)
        {
            List<string> resultStrs = new List<string>();

            for (int i = 1; i <= origin.Length; i++)
            {
                var sdfsf = Split(origin, i);
                resultStrs.AddRange(sdfsf);
            }

            return resultStrs.ToArray();
        }

        static string[] Split(string origin, int len)
        {
            List<string> resultStrs = new List<string>();

            for (int i = 0; i <= origin.Length - len; i++)
            {
                resultStrs.Add(origin.Substring(i, len));
            }

            return resultStrs.ToArray();
        }

        #endregion

        /// <summary>
        /// 选择银行名称
        /// </summary>
        protected void SelectBank()
        {
            try
            {
                string bankID = Request.Form["BankID"];
                if (string.IsNullOrEmpty(bankID))
                {
                    Response.Write((new { success = false, data = "供应商银行ID为空" }).Json());
                    return;
                }
                var bank = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSupplierBanks.
                        Where(item => item.ID == bankID).FirstOrDefault();
                if (bank == null)
                {
                    Response.Write((new { success = false, data = "供应商银行对象为空" }).Json());
                    return;
                }

                bool isSensitive = CheckSupplierBankSensitive(bank.BankAddress);

                var data = new
                {
                    BankAddress = bank.BankAddress,
                    BankAccount = bank.BankAccount,
                    SwiftCode = bank.SwiftCode,
                    IsSensitive = isSensitive,
                };
                Response.Write((new { success = true, data = data }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, data = ex.Message }).Json());
            }
        }

        /// <summary>
        /// 提交付汇申请
        /// </summary>
        protected void Submit()
        {
            try
            {


                string Model = HttpUtility.UrlDecode(Request.Form["Model"]).Replace("&quot;", "\'").Replace("amp;", "");
                dynamic model = Model.JsonTo<dynamic>();

                string OrderData = HttpUtility.UrlDecode(Request.Form["OrderData"]).Replace("&quot;", "\'").Replace("amp;", "");
                string FileData = HttpUtility.UrlDecode(Request.Form["FileData"]).Replace("&quot;", "\'").Replace("amp;", "");
                IEnumerable<UnPayExchangeOrder> Orders = OrderData.JsonTo<IEnumerable<UnPayExchangeOrder>>();
                IEnumerable<PayExchangeApplyFile> files = FileData.JsonTo<IEnumerable<PayExchangeApplyFile>>();
                string SupplierEnglishName = model.SupplierEnglishName;
                string BankAddress = model.BankAddress;


                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                AdminPayExchangeApply payExchangeApply = new AdminPayExchangeApply(Orders);
                foreach (var file in files)
                {
                    var applyfile = new PayExchangeApplyFile();
                    applyfile.FileFormat = "";
                    applyfile.FileName = file.FileName;
                    applyfile.FileType = FileType.PIFiles;
                    //   var url = FileDirectory.Current.FilePath + @"\" + file.Url;
                    applyfile.Url = file.Url;
                    applyfile.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                    applyfile.ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                    payExchangeApply.PayExchangeApplyFiles.Add(applyfile);
                }
                payExchangeApply.ClientID = Orders.FirstOrDefault()?.ClientID;
                payExchangeApply.Currency = Orders.FirstOrDefault()?.Currency;
                payExchangeApply.ExchangeRateType = (ExchangeRateType)Orders.FirstOrDefault().ExchangeRateType;
                payExchangeApply.ExchangeRate = Orders.FirstOrDefault().ExchangeRate;
                payExchangeApply.Admin = admin;
                payExchangeApply.SupplierName = model.SupplierName;
                payExchangeApply.SupplierAddress = model.SupplierAddress;
                payExchangeApply.SupplierEnglishName = SupplierEnglishName.Replace("&#39", "'");
                payExchangeApply.BankName = model.BankName;
                payExchangeApply.BankAddress = BankAddress.Replace("&#39", "'");
                payExchangeApply.BankAccount = model.BankAccount;
                payExchangeApply.SwiftCode = model.SwiftCode;
                payExchangeApply.OtherInfo = model.OtherInfo;
                payExchangeApply.Summary = model.Summary;
                payExchangeApply.ABA = model.ABA;
                payExchangeApply.IBAN = model.IBAN;
                payExchangeApply.IsAdvanceMoney = model.IsAdvanceMoney;

                if (!string.IsNullOrEmpty((string)model.ExpectPayDate))
                {
                    payExchangeApply.ExpectPayDate = Convert.ToDateTime(model.ExpectPayDate);
                }
                payExchangeApply.SettlemenDate = DateTime.Now.AddDays(90);
                payExchangeApply.PaymentType = model.PaymentTypeID;
                //操作人
                payExchangeApply.SetOperator(admin);
                payExchangeApply.Enter();

                Response.Write((new { success = true, message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "提交失败：" + ex.Message }).Json());
            }
        }
    }
}