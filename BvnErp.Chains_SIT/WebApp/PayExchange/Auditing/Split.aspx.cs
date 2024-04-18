using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.PayExchange.Auditing
{
    public partial class Split : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        protected void LoadData()
        {
            //付款方式
            this.Model.PaymentTypeData = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.PaymentType>()
                .Select(item => new { item.Key, item.Value }).Json();

            this.Model.PayExchangeApplyData = "".Json();
            string ID = Request.QueryString["ID"];
            string AdvanceMoney = Request.QueryString["AdvanceMoney"];
            string FileUrl = Request.QueryString["FileUrl"];
            if (string.IsNullOrEmpty(ID))
            {
                return;
            }

            var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyUnAuditedPayExchangeApply
               .Where(item => item.ID == ID).FirstOrDefault();
            if (apply != null)
            {
                this.Model.PayExchangeApplyData = new
                {
                    ID = apply.ClientID,
                    SupplierName = apply.SupplierName,
                    SupplierAddress = apply.SupplierAddress,
                    SupplierEnglishName = apply.SupplierEnglishName.Replace("'", "&#39"),
                    BankName = apply.BankName,
                    BankAddress = apply.BankAddress.Replace("'", "&#39"),
                    BankAccount = apply.BankAccount,
                    SwiftCode = apply.SwiftCode,
                    PaymentType = apply.PaymentType.GetDescription(),
                    PaymentTypeInt = apply.PaymentType,
                    ExpectPayDate = apply.ExpectPayDate?.ToString("yyyy-MM-dd"),
                    OtherInfo = apply.OtherInfo,
                    Summary = apply.Summary,
                    CreateDate = apply.CreateDate.ToString(),
                    SettlemenDate = apply.SettlemenDate.ToString(),
                    Currency = apply.Currency,
                    ClientName = apply.Client.Company.Name,
                    ClientCode = apply.Client.ClientCode,
                    Merchandiser = apply.Client.Merchandiser.RealName,
                    ExchangeRateType = apply.ExchangeRateType.GetDescription(),
                    ExchangeRate = apply.ExchangeRate,
                    Price = ((decimal)apply.TotalAmount).ToRound(4),
                    RmbPrice = ((decimal)(apply.TotalAmount * apply.ExchangeRate)).ToRound(2),//人民币金额
                    ABA = apply.ABA ?? "",
                    IBAN = apply.IBAN ?? "",
                    FileUrl = FileUrl,
                    AdvanceMoney = AdvanceMoney
                }.Json();

            }

            #region 付汇供应商

            this.Model.SupplierData = "".Json();
            string ClientID = apply.ClientID;

            //查询客户供应商
            var suppliers = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientSuppliers.Where(item => item.ClientID == ClientID && item.Status == Needs.Ccs.Services.Enums.Status.Normal).ToList();
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
                ChineseName = item.Name
            }).Json();

            #endregion

        }
        protected void data()
        {
            string ID = Request.QueryString["ID"];
            if (string.IsNullOrEmpty(ID))
            {
                return;
            }
            var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyUnAuditedPayExchangeApply
                .Where(item => item.ID == ID).FirstOrDefault();
            Func<PayExchangeApplyItem, object> convert = item => new
            {
                OrderID = item.OrderID,
                Currency = apply.Currency,
                DeclarePrice = item.DeclarePrice.ToRound(4),
                PaidPrice = (item.PaidExchangeAmount - item.Amount).ToRound(4),
                Amount = item.Amount.ToRound(4),
                CreateDate = item.CreateDate.ToShortDateString(),
                OldAmount = item.Amount.ToRound(4),
            };
            Response.Write(new
            {
                rows = apply.PayExchangeApplyItems.Select(convert).ToArray()
            }.Json());
        }

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
                var data = new
                {
                    BankAddress = bank.BankAddress,
                    BankAccount = bank.BankAccount,
                    SwiftCode = bank.SwiftCode,
                };
                Response.Write((new { success = true, data = data }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, data = ex.Message }).Json());
            }
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
        /// 选择供应商名称、订单本次申请金额
        /// </summary>
        protected void SelectSupplier()
        {
            #region 获取供应商银行+地址

            string supplierID = Request.Form["SupplierID"];
            if (string.IsNullOrEmpty(supplierID))
            {
                Response.Write((new { success = false, data = "供应商ID为空！" }).Json());
                return;
            }
            //获取供应商的银行信息
            var bankName = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSupplierBanks.
                        Where(item => item.ClientSupplierID == supplierID && item.Status == Needs.Ccs.Services.Enums.Status.Normal).ToList();
            if (bankName == null)
            {
                Response.Write((new { success = false, data = "供应商的银行名称为空！" }).Json());
                return;
            }

            #endregion

            var data = bankName.Select(item => new
            {
                ID = item.ID,
                BankName = item.BankName,
                BankAddress = item.BankAddress
            }).Json();
            Response.Write((new { success = true, data = data }).Json());
        }

        protected void SplitCheck()
        {
            try
            {
                string Model = HttpUtility.UrlDecode(Request.Form["Model"]).Replace("&quot;", "\'").Replace("amp;", "");
                dynamic model = Model.JsonTo<dynamic>();
                int count = model.Count;
                int rows = (model[0])["RowsCount"];
                string fileUrl = Request.Form["FileUrl"];
                if (count != 0)
                {
                    var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                    //需要拆分的订单自动生成新的内部付汇申请，需要修改新生成的付汇申请表里的FatherID（FatherID等于拆分前的付汇申请ID）
                    //获取供应商信息
                    string supplierID = (model[0])["SupplierID"];
                    //获取供应商的银行信息
                    string bankID = (model[0])["BankID"];
                    //获取订单信息
                    string OrderId = "";
                    string payExchangeApplyID = (model[0])["ID"];
                    string AdvanceMoney = (model[0])["AdvanceMoney"];

                    if (count == 1 && rows == 1)
                    {
                        string NewPayExchangeApplyID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApply);
                        SplitPayExchangeApplies splitPayExchangeApply = new SplitPayExchangeApplies();
                        splitPayExchangeApply.ID = NewPayExchangeApplyID;
                        splitPayExchangeApply.Admin = admin;
                        splitPayExchangeApply.SupplierID = supplierID;
                        splitPayExchangeApply.BankID = bankID;
                        splitPayExchangeApply.ABA = (model[0])["ABA"];
                        splitPayExchangeApply.IBAN = (model[0])["IBAN"];
                        splitPayExchangeApply.FatherID = payExchangeApplyID;
                        splitPayExchangeApply.ExpectPayDate = (model[0])["ExpectPayDate"] == "" ? null : (model[0])["ExpectPayDate"];
                        splitPayExchangeApply.PaymentType = (model[0])["PaymentType"];
                        splitPayExchangeApply.AdvanceMoney = Convert.ToInt32(AdvanceMoney);
                        splitPayExchangeApply.ExchangeRate = model[0]["NewExchangeRate"];
                        splitPayExchangeApply.Enter();

                        SplitPayExchangeApplies splitPayExchangeApplyItem = new SplitPayExchangeApplies();
                        splitPayExchangeApplyItem.PayExchangeApplyID = NewPayExchangeApplyID;
                        splitPayExchangeApplyItem.OrderID = (model[0])["OrderID"];
                        splitPayExchangeApplyItem.TotalAmount = (model[0])["Amount"];
                        splitPayExchangeApplyItem.Admin = admin;
                        splitPayExchangeApplyItem.ApplyItemEnter();

                        //关联付汇委托书
                        FileDescription(payExchangeApplyID, NewPayExchangeApplyID);

                        //只有一笔订单被拆分以后，剩余可申请金额大于已申请金额时，才能自动生成新的内部付汇申请
                        string unNewPayExchangeApplyID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApply);
                        SplitPayExchangeApplies unSplitPayExchangeApply = new SplitPayExchangeApplies();
                        unSplitPayExchangeApply.ID = unNewPayExchangeApplyID;
                        unSplitPayExchangeApply.FatherID = payExchangeApplyID;
                        unSplitPayExchangeApply.Admin = admin;
                        unSplitPayExchangeApply.AdvanceMoney = Convert.ToInt32(AdvanceMoney);
                        unSplitPayExchangeApply.ExchangeRate = model[0]["NewExchangeRate"];
                        unSplitPayExchangeApply.UnSplitEnter();

                        unSplitPayExchangeApply.PayExchangeApplyID = unNewPayExchangeApplyID;
                        unSplitPayExchangeApply.OrderID = (model[0])["OrderID"];
                        unSplitPayExchangeApply.TotalAmount = (model[0])["PaidAmount"] - (model[0])["Amount"];

                        unSplitPayExchangeApply.UnSplitItemEnter();

                        //关联付汇委托书
                        FileDescription(payExchangeApplyID, unNewPayExchangeApplyID);

                        //更新I类的付汇申请状态
                        SplitPayExchangeApplies PayExchangeApplyStatus = new SplitPayExchangeApplies();
                        PayExchangeApplyStatus.FatherID = payExchangeApplyID;
                        PayExchangeApplyStatus.AdvanceMoney = Convert.ToInt32(AdvanceMoney);
                        PayExchangeApplyStatus.ExchangeRate = model[0]["NewExchangeRate"];
                        PayExchangeApplyStatus.StatusEnter();

                    }
                    else if (count == 1 && rows > 1)
                    {
                        string NewPayExchangeApplyID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApply);
                        SplitPayExchangeApplies splitPayExchangeApply = new SplitPayExchangeApplies();
                        splitPayExchangeApply.ID = NewPayExchangeApplyID;
                        splitPayExchangeApply.Admin = admin;
                        splitPayExchangeApply.SupplierID = supplierID;
                        splitPayExchangeApply.BankID = bankID;
                        splitPayExchangeApply.ABA = (model[0])["ABA"];
                        splitPayExchangeApply.IBAN = (model[0])["IBAN"];
                        splitPayExchangeApply.FatherID = payExchangeApplyID;
                        splitPayExchangeApply.ExpectPayDate = (model[0])["ExpectPayDate"] == "" ? null : (model[0])["ExpectPayDate"];
                        splitPayExchangeApply.PaymentType = (model[0])["PaymentType"];
                        splitPayExchangeApply.AdvanceMoney = Convert.ToInt32(AdvanceMoney);
                        splitPayExchangeApply.ExchangeRate = model[0]["NewExchangeRate"];
                        splitPayExchangeApply.Enter();

                        SplitPayExchangeApplies splitPayExchangeApplyItem = new SplitPayExchangeApplies();
                        splitPayExchangeApplyItem.PayExchangeApplyID = NewPayExchangeApplyID;
                        splitPayExchangeApplyItem.OrderID = (model[0])["OrderID"];
                        splitPayExchangeApplyItem.TotalAmount = (model[0])["Amount"];
                        splitPayExchangeApplyItem.Admin = admin;
                        splitPayExchangeApplyItem.ApplyItemEnter();

                        //关联付汇委托书
                        FileDescription(payExchangeApplyID, NewPayExchangeApplyID);

                        //pay2
                        string unNewPayExchangeApplyID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApply);
                        SplitPayExchangeApplies unSplitPayExchangeApply = new SplitPayExchangeApplies();
                        unSplitPayExchangeApply.ID = unNewPayExchangeApplyID;
                        unSplitPayExchangeApply.FatherID = payExchangeApplyID;
                        unSplitPayExchangeApply.Admin = admin;
                        unSplitPayExchangeApply.AdvanceMoney = Convert.ToInt32(AdvanceMoney);
                        unSplitPayExchangeApply.UnSplitEnter();

                        unSplitPayExchangeApply.PayExchangeApplyID = unNewPayExchangeApplyID;
                        unSplitPayExchangeApply.OrderID = (model[0])["OrderID"];
                        unSplitPayExchangeApply.TotalAmount = (model[0])["PaidAmount"] - (model[0])["Amount"];
                        unSplitPayExchangeApply.ExchangeRate = model[0]["NewExchangeRate"];
                        unSplitPayExchangeApply.UnSplitItemEnter();

                        string orderID = (model[0])["OrderID"];
                        var unpaymentApplyItem = new Needs.Ccs.Services.Views.SplitPayExchangeAppliesView()
                            .Where(item => item.ID == payExchangeApplyID && item.OrderID != orderID).ToList();
                        if (unpaymentApplyItem.Count != 0)
                        {
                            for (int i = 0; i < unpaymentApplyItem.Count; i++)
                            {
                                splitPayExchangeApplyItem.PayExchangeApplyID = unNewPayExchangeApplyID;
                                splitPayExchangeApplyItem.OrderID = unpaymentApplyItem[i].OrderID;
                                splitPayExchangeApplyItem.TotalAmount = unpaymentApplyItem[i].TotalAmount;
                                splitPayExchangeApplyItem.Admin = admin;
                                splitPayExchangeApplyItem.UnSplitItemEnter();
                            }
                        }

                        //关联付汇委托书
                        FileDescription(payExchangeApplyID, unNewPayExchangeApplyID);

                        //更新I类的付汇申请状态
                        SplitPayExchangeApplies PayExchangeApplyStatus = new SplitPayExchangeApplies();
                        PayExchangeApplyStatus.FatherID = payExchangeApplyID;
                        PayExchangeApplyStatus.AdvanceMoney = Convert.ToInt32(AdvanceMoney);
                        PayExchangeApplyStatus.ExchangeRate = model[0]["NewExchangeRate"];
                        PayExchangeApplyStatus.StatusEnter();
                    }
                    else if (count > 1 && rows > 1)
                    {
                        string NewPayExchangeApplyID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApply);
                        SplitPayExchangeApplies splitPayExchangeApply = new SplitPayExchangeApplies();
                        splitPayExchangeApply.ID = NewPayExchangeApplyID;
                        splitPayExchangeApply.Admin = admin;
                        splitPayExchangeApply.SupplierID = supplierID;
                        splitPayExchangeApply.BankID = bankID;
                        splitPayExchangeApply.ABA = (model[0])["ABA"];
                        splitPayExchangeApply.IBAN = (model[0])["IBAN"];
                        splitPayExchangeApply.FatherID = payExchangeApplyID;
                        splitPayExchangeApply.ExpectPayDate = (model[0])["ExpectPayDate"] == "" ? null : (model[0])["ExpectPayDate"];
                        splitPayExchangeApply.PaymentType = (model[0])["PaymentType"];
                        splitPayExchangeApply.AdvanceMoney = Convert.ToInt32(AdvanceMoney);
                        splitPayExchangeApply.ExchangeRate = model[0]["NewExchangeRate"];
                        splitPayExchangeApply.Enter();
                        for (int i = 0; i < count; i++)
                        {
                            SplitPayExchangeApplies splitPayExchangeApplyItem = new SplitPayExchangeApplies();
                            splitPayExchangeApplyItem.PayExchangeApplyID = NewPayExchangeApplyID;
                            splitPayExchangeApplyItem.OrderID = (model[i])["OrderID"];
                            splitPayExchangeApplyItem.TotalAmount = (model[i])["Amount"];
                            splitPayExchangeApplyItem.Admin = admin;
                            splitPayExchangeApplyItem.ApplyItemEnter();
                        }
                        //关联付汇委托书
                        FileDescription(payExchangeApplyID, NewPayExchangeApplyID);

                        //pay2

                        string unNewPayExchangeApplyID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApply);
                        SplitPayExchangeApplies unSplitPayExchangeApply = new SplitPayExchangeApplies();
                        unSplitPayExchangeApply.ID = unNewPayExchangeApplyID;
                        unSplitPayExchangeApply.FatherID = payExchangeApplyID;
                        unSplitPayExchangeApply.Admin = admin;
                        unSplitPayExchangeApply.AdvanceMoney = Convert.ToInt32(AdvanceMoney);
                        unSplitPayExchangeApply.ExchangeRate = model[0]["NewExchangeRate"];
                        unSplitPayExchangeApply.UnSplitEnter();

                        var unPaymentApplyItem = new Needs.Ccs.Services.Views.SplitPayExchangeAppliesView()
                            .Where(item => item.ID == unNewPayExchangeApplyID).ToList();

                        for (int k = 0; k < count; k++)
                        {
                            if ((model[k])["PaidAmount"] - (model[k])["Amount"] != 0)
                            {
                                unSplitPayExchangeApply.PayExchangeApplyID = unNewPayExchangeApplyID;
                                unSplitPayExchangeApply.OrderID = (model[k])["OrderID"];
                                unSplitPayExchangeApply.TotalAmount = (model[k])["PaidAmount"] - (model[k])["Amount"];
                                unSplitPayExchangeApply.Admin = admin;
                                unSplitPayExchangeApply.UnSplitItemEnter();
                            }
                            else
                            {
                                OrderId = (model[k])["OrderID"];
                            }

                        }
                        var unItem = new Needs.Ccs.Services.Views.SplitPayExchangeAppliesView()
                        .Where(item => item.ID == unNewPayExchangeApplyID).Select(item => item.OrderID).ToArray();

                        var Item = new Needs.Ccs.Services.Views.SplitPayExchangeAppliesView()
                        .Where(item => item.ID == payExchangeApplyID && item.OrderID != OrderId).Select(item => item.OrderID).ToArray();

                        var Items = Item.Except(unItem).ToList();
                        for (int j = 0; j < Items.Count; j++)
                        {
                            var ItemAmount = new Needs.Ccs.Services.Views.SplitPayExchangeAppliesView().
                                Where(item => item.ID == payExchangeApplyID && item.OrderID == Items[j]).ToList();

                            SplitPayExchangeApplies splitPayExchangeApplyItem = new SplitPayExchangeApplies();
                            splitPayExchangeApplyItem.PayExchangeApplyID = unNewPayExchangeApplyID;
                            splitPayExchangeApplyItem.OrderID = Items[j];
                            splitPayExchangeApplyItem.TotalAmount = ItemAmount[j].TotalAmount;
                            splitPayExchangeApplyItem.Admin = admin;
                            splitPayExchangeApplyItem.UnSplitItemEnter();
                        }

                        //关联付汇委托书
                        FileDescription(payExchangeApplyID, unNewPayExchangeApplyID);

                        //更新I类的付汇申请状态
                        SplitPayExchangeApplies PayExchangeApplyStatus = new SplitPayExchangeApplies();
                        PayExchangeApplyStatus.FatherID = payExchangeApplyID;
                        PayExchangeApplyStatus.AdvanceMoney = Convert.ToInt32(AdvanceMoney);
                        PayExchangeApplyStatus.ExchangeRate = model[0]["NewExchangeRate"];
                        PayExchangeApplyStatus.StatusEnter();
                    }
                }

                Response.Write((new { success = true, message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new
                {
                    success = false,
                    message = "提交失败：" + ex.Message
                }).Json());
            }
        }

        protected void FileDescription(string payExchangeApplyID, string NewPayExchangeApplyID)
        {
            var applyFile = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.PayExchangeApplyFile.
                Where(item => item.PayExchangeApplyID == payExchangeApplyID && item.FileType == Needs.Ccs.Services.Enums.FileType.PayExchange).FirstOrDefault();
            if (applyFile != null)
            {
                #region 关联付汇委托书上传中心
                var entity = new CenterFileDescription();
                entity.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                entity.ApplicationID = NewPayExchangeApplyID;
                entity.Type = (int)applyFile.FileType;
                entity.Url = applyFile.Url;
                entity.Status = FileDescriptionStatus.Normal;
                entity.CreateDate = DateTime.Now;
                entity.CustomName = applyFile.FileName;

                DateTime liunxStart = new DateTime(1970, 1, 1);
                var linuxtime = (DateTime.Now - liunxStart).Ticks;
                string topID = "F" + linuxtime;

                new CenterFilesTopView().Insert(entity, topID);
                #endregion
            }
        }

    }
}
