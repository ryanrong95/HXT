using Needs.Ccs.Services.Enums;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.Finance.DyjFinance
{
    public class DyjPayment
    {
        // { id:"单据号",userID:"操作人员ID",checkInfo:"原因",data:"",list:"",key:"8c2b75ad115b467a8e976123033319f2"}
        public int id { get; set; }

        public int userID { get; set; }

        public string checkInfo { get; set; }

        public DyjPaymentData data { get; set; }

        public List<DyjPaymentList> list { get; set; }

        public string key { get; set; }

        public DyjPayment(PaymentNotice notice)
        {
            //所有员工
            var AdminStaff = new Views.AdminStaffsTopView().ToList();
            //基础信息返回值
            var BaseInfo = DyjBaseInfo.GetBaseInfo();
            //金库、账户
            var vault = new Views.FinanceVaultsView().FirstOrDefault(t=>t.ID == notice.FinanceVault.ID);
            var createDyjCode = AdminStaff.FirstOrDefault(t => t.OriginID == notice.PayExchangeApply.Client.Merchandiser.ID).DyjCode;

            data = new DyjPaymentData();
            data.FiskID = int.Parse(vault.BigWinVaultID);
            data.AccountID = string.IsNullOrEmpty(notice.FinanceAccount.BigWinAccountID) ? 0 : int.Parse(notice.FinanceAccount.BigWinAccountID);
            data.CreatorID = string.IsNullOrEmpty(createDyjCode) ? 0 : int.Parse(createDyjCode);
            data.Provider = notice.PayExchangeApply.SupplierEnglishName;
            data.BalanceStyle = 45;//付款类型，45：预收付汇
            data.Note = "付款流水号：" + notice.SeqNo;

            //正常为[45 - 预收付汇];
            //利息为[24 - 利息];
            //银行费用为[25 - 银行费用];
            //汇兑损益为[26 - 汇兑损益];
            //税金及附加为[28 - 税金及附加];
            list = new List<DyjPaymentList>();
            var Operator = AdminStaff.FirstOrDefault(t => t.OriginID == notice.GetOperator().ID);
            var oper = Operator == null ? new XDTAdminStaff { DyjCompanyCode = "100", DyjDepartmentCode = "136", DyjCode = "3806" } : Operator;//如果为空，默认杨艳梦

            #region 25-银行费用
            if (notice.Poundage.HasValue)
            {
                var bankfee = new DyjPaymentList();
                bankfee.Summary = "银行费用";
                bankfee.AmortizeCount = 1;
                bankfee.BillType = 25;
                bankfee.CorpID = int.Parse(oper.DyjCompanyCode);
                bankfee.DeptID = int.Parse(oper.DyjDepartmentCode);
                bankfee.EmployeeID = int.Parse(oper.DyjCode);
                bankfee.Note = "手续费流水号：" + notice.SeqNoPoundage;
                bankfee.ForeignAmount = notice.Poundage.Value;
                bankfee.Amount = notice.Amount;
                list.Add(bankfee);
            }
            #endregion

            //其它信息
            this.key = DyjCwConfig.Key;
            this.userID = int.Parse(oper.DyjCode);
            this.checkInfo = "";
        }

        public void PostToDYJ()
        {
            var api = System.Configuration.ConfigurationManager.AppSettings["DYJFeeApplyApiURL"];
            //调用接口
            var post = DyjBaseInfo.PostDYJ(api + DyjCwConfig.PayFeeApplyAPass, "POST", this.Json());
            //格式化json字符串
            var result = new Newtonsoft.Json.JsonSerializer().Deserialize(new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(post)));
            //反序列化Response
            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<DyjResponse>(result.ToString());
            if (!response.isSuccess || response.status != "200")
            {
                var ex = new Exception(response.message);
                ex.Source += "DyjPayment.PostToDYJ()";
                ex.CcsLog("付汇付款调用大赢家错误");
            }
        }

    }

    /// <summary>
    /// 付汇申请功能类
    /// </summary>
    public class DyjPayExchange
    {
        public string key { get; set; }

        public DyjPayExchangeData data { get; set; }

        //付汇委托书
        public string pic { get; set; }

        public DyjPayExchange(PayExchangeApply apply)
        {
            this.key = DyjCwConfig.Key;

            //华芯通所有员工
            var AdminStaff = new Views.AdminStaffsTopView().ToList();
            //基础信息返回值
            var BaseInfo = DyjBaseInfo.GetBaseInfo();
            
            //转换币种
            DyjCurrency currency;
            switch (apply.Currency)
            {
                case "CNY":
                    currency = DyjCurrency.CNY;
                    break;
                case "USD":
                    currency = DyjCurrency.USD;
                    break;
                case "HKD":
                    currency = DyjCurrency.HKD;
                    break;
                case "EUR":
                    currency = DyjCurrency.EUR;
                    break;
                case "GBP":
                    currency = DyjCurrency.GBP;
                    break;
                case "JPY":
                    currency = DyjCurrency.JPY;
                    break;
                default:
                    currency = DyjCurrency.USD;
                    break;
            }

            //美元汇率
            var USDRate = BaseInfo.币种.FirstOrDefault(t => t.名称 == currency.GetDescription()).汇率;
            //制单人
            var user = AdminStaff.FirstOrDefault(t => t.OriginID == apply.Client.Merchandiser.ID);

            //境外付款公司：畅运 万路通
            var vendor = new VendorContext(VendorContextInitParam.Pointed, apply.Client.Company.Name).Current1;
            var Consignor = BaseInfo.付款公司.FirstOrDefault(t => t.名称 == vendor.CompanyName);

            //付款备注
            var recMessage = "";
            var receipts = new Views.OrderReceivedsView().Where(t => t.FeeSourceID == apply.ID).ToList();
            var r_g = from rec in receipts
                     group rec by new { rec.SeqNo, rec.DyjID } into g
                     select new {
                         SeqNo = g.Key.SeqNo,
                         DyjID = g.Key.DyjID,
                         Amount = g.Sum(t => t.Amount)
                     };
            //外币总金额
            var totalAmount = apply.PayExchangeApplyItems.Sum(item => item.Amount);

            foreach (var rec in r_g)
            {
                recMessage += "Seq:" + rec.SeqNo + " p" + rec.DyjID + " RMB:" + rec.Amount + "\r\n";
            }
            //客户付 MISsIon ELECTRONICS INTERNATIONAL LIMITE 20, 098.07 p527709
            //需付出5940美金到以上帐户，谢谢!
            var message = string.Format(" 客户付 {0} {1} {2} 需付出{3}美金到以上帐户，谢谢!", apply.SupplierEnglishName, recMessage, string.IsNullOrEmpty(recMessage) ? "\r\n" : "", totalAmount);


            var payex = new DyjPayExchangeData();
            payex.CurrencyID = currency.GetHashCode();
            payex.FeeType = 0;//：汇兑（后台有赋值） 
            payex.CorpID = Int32.Parse(user.DyjCompanyCode);
            payex.DeptID = Int32.Parse(user.DyjDepartmentCode);
            payex.UserID = Int32.Parse(user.DyjCode);
            payex.UserName = user.RealName;
            payex.Amount = (totalAmount * apply.ExchangeRate).ToRound(2);
            payex.FAmount = totalAmount;
            payex.HL = apply.ExchangeRate;//目前使用的是华芯通实际的付汇汇率
            payex.State = "";// 状态（默认为等待审批）可以为空（后台有赋值） 状态：新增、等待审批、等待付款
            payex.CheckID = 3793;
            payex.CheckName = "尹荣荣";
            payex.Note = apply.Summary + ((apply.Summary != null && apply.Summary.Contains("代付")) ? "" : message);
            payex.PayType = 0;// 开票类型(3)（后台有赋值） 
            payex.PayComID = Consignor != null ? Consignor.编号 : 0;
            payex.PayComName = Consignor != null ? Consignor.名称 : vendor.CompanyName;

            //收款客户信息
            payex.ClientID = 0;
            payex.ClientName = apply.Client.Company.Name;
            payex.ClientLinkName = "";
            payex.ClientBank = "";
            payex.ClientBankNum = "";
            payex.ClientBankAddress = "";

            //付汇供应商信息
            payex.Provider = 0;
            payex.ProviderName = apply.SupplierEnglishName;
            payex.ProviderLinkName = "";
            payex.ProviderBank = apply.BankName;
            payex.ProviderBankNum = apply.BankAccount;
            payex.ProviderBankAddress = apply.BankAddress;

            payex.XDTID = apply.ID;
            this.data = payex;

            //付汇委托书
            var applyFile = new Needs.Ccs.Services.Views.PayExchangeApplyFileView().
                Where(item => item.PayExchangeApplyID == apply.ID && item.FileType == Needs.Ccs.Services.Enums.FileType.PayExchange).FirstOrDefault();
            if (applyFile != null)
            {
                this.pic = FileDirectory.Current.PvDataFileUrl + "/" + applyFile.Url.ToUrl();
            }
        }

        public string PostToDYJ()
        {
            var api = System.Configuration.ConfigurationManager.AppSettings["DYJFeeApplyApiURL"];
            //调用接口
            var post = DyjBaseInfo.PostDYJ(api + DyjCwConfig.SetFeeApplyAHD, "POST", this.Json());
            //格式化json字符串
            var result = new Newtonsoft.Json.JsonSerializer().Deserialize(new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(post)));
            //反序列化Response
            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<DyjResponse>(result.ToString());
            //记录日志
            Needs.Ccs.Services.Models.Logs log = new Needs.Ccs.Services.Models.Logs()
            {
                ID = Guid.NewGuid().ToString("N"),
                Name = "付汇申请同步大赢家",
                MainID = this.data.XDTID,
                AdminID = this.data.UserName,
                Summary = "大赢家返回结果：" + result.ToString(),
                Json = this.Json(),
                CreateDate = DateTime.Now
            };
            log.Enter();

            if (!response.isSuccess || response.status != "200")
            {
                var ex = new Exception(response.message);
                ex.Source += "DyjPayExchange.PostToDYJ()";
                ex.CcsLog("付汇申请调用大赢家错误,接口返回");
                return "";
            }
            else
            {
                return response.data.ToString();
            }
        }
    }

    /// <summary>
    /// 费用申请功能类
    /// </summary>
    public class DyjFeeApply
    {
        public string key { get; set; }

        public DyjFeeApplyMain data { get; set; }

        public List<DyjFeeApplyList> list { get; set; }

        public DyjFeeApply(CostApply costApply)
        {
            //华芯通所有员工
            var AdminStaff = new Views.XDTAdminStaffsTopView().ToList();
            //基础信息返回值
            var BaseInfo = DyjBaseInfo.GetBaseInfo();
            //美元汇率
            var USDRate = BaseInfo.币种.FirstOrDefault(t => t.名称 == "美元").汇率;

            this.key = DyjCwConfig.Key;

            #region 主表数据对象

            this.data = new DyjFeeApplyMain();
            this.data.ID = 0;
            this.data.CurrencyID = costApply.Currency == "CNY" ? 1 : 2;//费用类的默认RMB
            this.data.FeeType = "";//付款和预付款，为空的时候默认为付款
            //转换大赢家人员ID
            var user = AdminStaff.FirstOrDefault(t => t.OriginID == costApply.AdminID);
            this.data.UserID = user == null ? 3422 : int.Parse(user.DyjCode);
            this.data.UserName = user == null ? "成金霖" : user.RealName;


            this.data.Amount = (costApply.Currency == "CNY" ? costApply.Amount : costApply.Amount * USDRate).ToRound(2);
            this.data.FAmount = (costApply.Currency == "CNY" ? costApply.Amount / USDRate : costApply.Amount).ToRound(2);
            this.data.HL = USDRate;
            this.data.State = ""; //状态（默认为等待审批）可以为空

            var checker = GetBaseInfoChecker(TransToDyjFeeType(costApply.Items.FirstOrDefault().FeeType));
            this.data.CheckID = checker.编号;
            this.data.CheckName = checker.名称;
            this.data.Note = "";
            //付款对象，有Id给ID，没有给具体值，由大赢家进行匹配或生成
            this.data.Provider = 0;
            this.data.ProviderName = costApply.PayeeName;
            this.data.ProviderLinkName = "";
            this.data.ProviderBank = costApply.PayeeBank;
            this.data.ProviderBankNum = costApply.PayeeAccount;
            this.data.ProviderBankAddress = "";

            this.data.PayType = "";
            this.data.PayComID = "";
            this.data.PayComName = "";

            #endregion

            #region 明细数据对象集合

            this.list = new List<DyjFeeApplyList>();
            foreach (var item in costApply.Items)
            {
                //付款类型转换
                var dyjType = TransToDyjFeeType(item.FeeType);
                //大赢家人
                var emp = AdminStaff.FirstOrDefault(t => t.OriginID == item.EmployeeID);

                var detail = new DyjFeeApplyList();
                detail.ID = 0;
                detail.PayType = dyjType.编号;
                detail.PayTypeName = dyjType.名称;
                detail.CorpID = emp.DyjCompanyCode;
                detail.CorpName = emp.DyjCompany;
                detail.DeptID = emp.DyjDepartmentCode;
                detail.DeptName = emp.DyjDepartment;
                detail.EmployeeID = emp.DyjCode;
                detail.EmployeeName = emp.RealName;

                detail.Summary = item.FeeDesc;
                detail.Amount = (costApply.Currency == "CNY" ? item.Amount : item.Amount * USDRate).ToRound(2);
                detail.Note = item.FeeType.GetDescription();
                detail.FAmount = (costApply.Currency == "CNY" ? item.Amount / USDRate : item.Amount).ToRound(2);

                this.list.Add(detail);
            }

            #endregion
        }

        public string PostToDYJ()
        {
            var api = System.Configuration.ConfigurationManager.AppSettings["DYJFeeApplyApiURL"];
            //调用接口
            var post = DyjBaseInfo.PostDYJ(api + DyjCwConfig.SetFeeApplyA, "POST", this.Json());
            //格式化json字符串
            var result = new Newtonsoft.Json.JsonSerializer().Deserialize(new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(post)));
            //反序列化Response
            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<DyjResponse>(result.ToString());
            if (!response.isSuccess || response.status != "200")
            {
                var ex = new Exception(response.message);
                ex.Source += "DyjFeeApply.PostToDYJ()";
                ex.CcsLog("付款调用大赢家错误");
                return "";
            }
            else
            {
                return response.data + "|" + this.data.CheckID;
            }
        }


        /// <summary>
        /// 转换费用类型与指定审批人
        /// </summary>
        /// <returns></returns>
        public DyjBaseInfoData付款类型 TransToDyjFeeType(FinanceFeeType feeType)
        {
            var 编号 = 1;
            var 名称 = "";
            var 类型 = "";
            switch (feeType)
            {
                case FinanceFeeType.Product:
                case FinanceFeeType.PayGoods:
                    编号 = 1;
                    名称 = "正常采购";
                    类型 = "付款";
                    break;
                case FinanceFeeType.Tariff:
                case FinanceFeeType.PayTariff:
                    编号 = 63;
                    名称 = "关税";
                    类型 = "付款";
                    break;
                case FinanceFeeType.AddedValueTax:
                case FinanceFeeType.PayAddedValue:
                    编号 = 53;
                    名称 = "海关增值税";
                    类型 = "付款";
                    break;
                case FinanceFeeType.Incidental:
                    编号 = 16;
                    名称 = "运杂费";
                    类型 = "付款";
                    break;
                case FinanceFeeType.Poundage:
                case FinanceFeeType.PayBankPaypal:
                    编号 = 50;
                    名称 = "手续费";
                    类型 = "付款";
                    break;
                case FinanceFeeType.Refund://退款
                case FinanceFeeType.PaySaleAgentBack://代销退款
                case FinanceFeeType.PayDespoistBack://退定金
                    编号 = 65;
                    名称 = "退定金";
                    类型 = "付款";
                    break;
                case FinanceFeeType.FundExchange://资金往来
                case FinanceFeeType.FundTransfer://自己调拨
                case FinanceFeeType.PayCallout:
                    编号 = 4;
                    名称 = "调出";
                    类型 = "付款";
                    break;

                case FinanceFeeType.Tax:
                case FinanceFeeType.PayTax:
                    编号 = 28;
                    名称 = "税金及附加";
                    类型 = "付款";
                    break;
                case FinanceFeeType.BankInterest:
                case FinanceFeeType.PayInterest:
                case FinanceFeeType.PayAcceptance://贴现利息
                case FinanceFeeType.PayDepositInterest://存款利息
                case FinanceFeeType.PayInterInterest://内部利息
                    编号 = 24;
                    名称 = "利息";
                    类型 = "付款";
                    break;
                case FinanceFeeType.ExchangeLoss:
                    编号 = 26;
                    名称 = "汇兑损益";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PayPreBill:
                    编号 = 12;
                    名称 = "预付货款";
                    类型 = "预付款";
                    break;
                case FinanceFeeType.PayIncomeTax:
                    编号 = 64;
                    名称 = "所得税";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PaySalary:
                    编号 = 13;
                    名称 = "工资";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PayEndowment:
                    编号 = 23;
                    名称 = "劳动保险费";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PayIncidentals:
                    编号 = 16;
                    名称 = "运杂费";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PayBussiness:
                    编号 = 15;
                    名称 = "差旅费";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PayAdvertisement:
                    编号 = 17;
                    名称 = "广告费";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PayPropertyMan:
                    编号 = 38;
                    名称 = "物业费";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PayEntertain:
                    编号 = 39;
                    名称 = "业务活动费用";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PayService:
                    编号 = 52;
                    名称 = "报关服务费";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PayWarehouse:
                    编号 = 54;
                    名称 = "仓储保管费";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PayNet:
                    编号 = 9;
                    名称 = "邮电费";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PayPackage:
                    编号 = 55;
                    名称 = "包装费";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PayAudit:
                    编号 = 56;
                    名称 = "审计及会计服务费";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PayTrade:
                    编号 = 57;
                    名称 = "贸易通清关费";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PayStampTax:
                    编号 = 58;
                    名称 = "印花税";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PayHouse:
                    编号 = 59;
                    名称 = "房产税";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PayWater:
                    编号 = 20;
                    名称 = "水电费";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PayLawsuit:
                    编号 = 61;
                    名称 = "诉讼费";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PayBadBorrow:
                    编号 = 62;
                    名称 = "坏账损失";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PayOffice:
                    编号 = 11;
                    名称 = "办公设备";
                    类型 = "付款";
                    break;
                case FinanceFeeType.PayCar:
                    编号 = 22;
                    名称 = "汽车支出";
                    类型 = "付款";
                    break;
                case FinanceFeeType.Fee://费用
                case FinanceFeeType.Loan://借款
                case FinanceFeeType.Repayment://还款
                case FinanceFeeType.Other:
                case FinanceFeeType.DeclareService://报关服务费
                case FinanceFeeType.PayOther://其它支出
                case FinanceFeeType.PayBorrowAdded://借款附加费
                case FinanceFeeType.PayInjury://残保金
                case FinanceFeeType.PayOtherFee:
                default:
                    编号 = 27;
                    名称 = "其他费用";
                    类型 = "付款";
                    break;

                case FinanceFeeType.PayDespoist://押金？？？
                case FinanceFeeType.PayBank://银行还贷？？
                case FinanceFeeType.PayCompany://还单位借款？？
                case FinanceFeeType.PayEmployee://员工借款？？
                case FinanceFeeType.PaySaleAgent://代销付款？？
                case FinanceFeeType.PayHousingProvidentFund://住房公积金？？
                    break;

                    //case FinanceFeeType.DepositReceived://预收账款
                    //case FinanceFeeType.Warehouse://仓储收入
                    //case FinanceFeeType.GoodsAgent://代发收入
                    //case FinanceFeeType.SpecialPack://理货异常收入
                    //case FinanceFeeType.Rent://租赁收入
                    //case FinanceFeeType.BankBorrow://银行借款
                    //case FinanceFeeType.Interest://存款利息
                    //case FinanceFeeType.CompanyBorrow://单位借款
                    //case FinanceFeeType.Deposit://收回押金
                    //case FinanceFeeType.EmployeePayback://员工还款
                    //case FinanceFeeType.CallIn://调入
                    //case FinanceFeeType.GenereOther://综合其他收入
                    //case FinanceFeeType.SaleAgent://代销收款
                    //case FinanceFeeType.Guarantee://认证保证金
                    //case FinanceFeeType.ThirdService://第三方收款
            }

            return new DyjBaseInfoData付款类型()
            {
                编号 = 编号,
                名称 = 名称,
                类型 = 类型
            };
        }

        /// <summary>
        /// 根据大赢家费用类型获得审批人
        /// </summary>
        /// <param name="dyjFeeType"></param>
        /// <returns></returns>
        public DyjBaseInfoData指定审批人 GetBaseInfoChecker(DyjBaseInfoData付款类型 dyjFeeType)
        {
            int 编号;
            string 名称;
            switch (dyjFeeType.编号)
            {
                case 13:
                    编号 = 359;
                    名称 = "张良宏";
                    break;
                case 11:
                case 12:
                case 15:
                case 16:
                case 17:
                case 20:
                case 22:
                case 23:
                case 28:
                case 39:
                case 50:
                case 55:
                case 56:
                case 57:
                case 62:
                    //case 11:邮电费属于自动扣款
                    编号 = 558;
                    名称 = "孙善华";
                    break;
                case 1:
                case 4:
                case 9:
                case 24:
                case 25:
                case 26:
                case 27:
                case 38:
                case 52:
                case 53:
                case 54:
                case 58:
                case 59:
                case 61:
                case 63:
                case 64:
                case 65:
                default:
                    编号 = 3793;
                    名称 = "尹荣荣";
                    break;
            }

            return new DyjBaseInfoData指定审批人()
            {
                编号 = 编号,
                名称 = 名称
            };
        }

    }

    /// <summary>
    /// 费用审核
    /// </summary>
    public class DyjPayApprove { 
    
        public DyjPayApproveData Data { get; set; }

        //付汇审批
        public DyjPayApprove(UnApprovalPayExchangeApply Apply,string Summary,bool IsPass)
        {
            //所有员工
            var AdminStaff = new Views.AdminStaffsTopView().ToList();

            //转换大赢家人员ID
            var payer = IsPass ? AdminStaff.FirstOrDefault(t => t.OriginID == Apply.GetPayer().ID) : null;
            var payerc = payer == null ? new XDTAdminStaff { DyjCode = "3806", RealName = "杨艳梦" } : payer;
            var user = AdminStaff.FirstOrDefault(t => t.OriginID == Apply.GetOperator().ID);

            Data = new DyjPayApproveData();
            Data.key = DyjCwConfig.Key;
            Data.id = int.Parse(Apply.DyjID.Split('.')[0]);
            Data.payerID = int.Parse(payerc.DyjCode);
            Data.payerName = payerc.RealName;
            Data.userID = int.Parse(user.DyjCode);
            Data.checkInfo = Summary;
            Data.isPass = IsPass;
        }

        //费用审批
        public DyjPayApprove(Views.CostApplyDetailViewModel CostApply, Admin payer, string Summary, bool IsPass)
        {
            //所有员工
            var AdminStaff = new Views.AdminStaffsTopView().ToList();

            //转换大赢家人员ID
            var payer1 = IsPass ? AdminStaff.FirstOrDefault(t => t.OriginID == payer.OriginID) : null;
            var payerc = payer1 == null ? new XDTAdminStaff { DyjCode = "632", RealName = "郝红梅" } : payer1;
            var user = AdminStaff.FirstOrDefault(t => t.OriginID == payer.OriginID);


            Data = new DyjPayApproveData();
            Data.key = DyjCwConfig.Key;
            Data.id = CostApply.DyjID.Value;
            Data.payerID = int.Parse(payerc.DyjCode);
            Data.payerName = payerc.RealName;
            Data.userID = CostApply.DyjCheckID.Value;
            Data.checkInfo = Summary;
            Data.isPass = IsPass;
        }

        public void PostToDYJ()
        {
            var api = System.Configuration.ConfigurationManager.AppSettings["DYJFeeApplyApiURL"];
            //调用接口
            var post = DyjBaseInfo.PostDYJ(api + DyjCwConfig.CheckFeeApplyA, "POST", this.Data.Json());
            //格式化json字符串
            var result = new Newtonsoft.Json.JsonSerializer().Deserialize(new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(post)));
            //反序列化Response
            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<DyjResponse>(result.ToString());

            //记录日志
            Needs.Ccs.Services.Models.Logs log = new Needs.Ccs.Services.Models.Logs()
            {
                ID = Guid.NewGuid().ToString("N"),
                Name = "付款/汇审批同步大赢家",
                MainID = this.Data.id.ToString(),
                AdminID = this.Data.userID.ToString(),
                Summary = "大赢家返回结果：" + result.ToString(),
                Json = this.Json(),
                CreateDate = DateTime.Now
            };
            log.Enter();

            if (!response.isSuccess || response.status != "200")
            {
                var ex = new Exception(response.message);
                ex.Source += "DyjPayApprove.PostToDYJ() " + this.Data.id;
                ex.CcsLog("付款/汇审批调用大赢家错误");
            }
        }

    }

    /// <summary>
    /// 删除申请
    /// </summary>
    public class DyjDeleteFeeApplyA
    {
        public DyjDeleteFeeApplyData Data { get; set; }

        //付汇审批 拒绝
        public DyjDeleteFeeApplyA(UnApprovalPayExchangeApply Apply)
        {
            //所有员工
            var AdminStaff = new Views.AdminStaffsTopView().ToList();
            //转换大赢家人员ID
            var user = AdminStaff.FirstOrDefault(t => t.OriginID == Apply.GetOperator().ID);

            Data = new DyjDeleteFeeApplyData();
            Data.key = DyjCwConfig.Key;
            Data.id = int.Parse(Apply.DyjID.Split('.')[0]);
            Data.userID = int.Parse(user.DyjCode);
        }

        public void PostToDYJ()
        {
            var api = System.Configuration.ConfigurationManager.AppSettings["DYJFeeApplyApiURL"];
            //调用接口
            var post = DyjBaseInfo.PostDYJ(api + DyjCwConfig.DeleteFeeApplyA, "POST", this.Data.Json());
            //格式化json字符串
            var result = new Newtonsoft.Json.JsonSerializer().Deserialize(new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(post)));
            //反序列化Response
            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<DyjResponse>(result.ToString());

            //记录日志
            Needs.Ccs.Services.Models.Logs log = new Needs.Ccs.Services.Models.Logs()
            {
                ID = Guid.NewGuid().ToString("N"),
                Name = "付款/汇 删除 同步大赢家",
                MainID = this.Data.id.ToString(),
                AdminID = this.Data.userID.ToString(),
                Summary = "大赢家返回结果：" + result.ToString(),
                Json = this.Json(),
                CreateDate = DateTime.Now
            };
            log.Enter();

            if (!response.isSuccess || response.status != "200")
            {
                var ex = new Exception(response.message);
                ex.Source += "DyjPayApprove.PostToDYJ() " + this.Data.id;
                ex.CcsLog("付款/汇 删除 调用大赢家错误");
            }
        }

    }


    #region 费用申请

    /// <summary>
    /// 费用data
    /// </summary>
    public class DyjFeeApplyMain
    {
        /// <summary>
        /// 单据号，新增的时候为0
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 币种 1为人民币，2位美元
        /// </summary>
        public int CurrencyID { get; set; }

        /// <summary>
        /// 付款和预付款，为空的时候默认为付款
        /// </summary>
        public string FeeType { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 制单人名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 金额（人民币）
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 金额外币（外币）
        /// </summary>
        public decimal FAmount { get; set; }

        /// <summary>
        /// 汇率（人民币为1:；其他的为两位数小数点的汇率） 
        /// </summary>
        public decimal HL { get; set; }

        /// <summary>
        /// 状态（默认为等待审批）可以为空
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// 指定审核人
        /// </summary>
        public int CheckID { get; set; }

        /// <summary>
        /// 指定审核人名称
        /// </summary>
        public string CheckName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 供应商ID
        /// </summary>
        public int Provider { get; set; }

        /// <summary>
        /// 收款单位名称
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string ProviderLinkName { get; set; }

        /// <summary>
        /// 银行
        /// </summary>
        public string ProviderBank { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string ProviderBankNum { get; set; }

        /// <summary>
        /// 银行地址
        /// </summary>
        public string ProviderBankAddress { get; set; }

        /// <summary>
        /// 开票类型
        /// </summary>
        public string PayType { get; set; }

        /// <summary>
        /// 开票公司ID
        /// </summary>
        public string PayComID { get; set; }

        /// <summary>
        /// 开票公司名称
        /// </summary>
        public string PayComName { get; set; }
    }

    /// <summary>
    /// 费用list
    /// </summary>
    public class DyjFeeApplyList
    {

        /// <summary>
        /// 单据号，新增的时候为0
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 付款类型ID
        /// </summary>
        public int PayType { get; set; }

        /// <summary>
        /// 付款类型名称
        /// </summary>
        public string PayTypeName { get; set; }

        /// <summary>
        /// 公司ID
        /// </summary>
        public string CorpID { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CorpName { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public string DeptID { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 业务员ID（部门ID和业务员ID不能同时为空或者为0） 
        /// </summary>
        public string EmployeeID { get; set; }

        /// <summary>
        /// 业务员名称
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// 摘要（不能为空） 
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 外币金额
        /// </summary>
        public decimal FAmount { get; set; }
    }

    #endregion

    #region 供应商列表返回值

    public class DyjProviderInfoResponse : DyjResponse
    {

        public new List<DyjProviderInfo> data { get; set; }
    }

    public class DyjProviderInfo
    {

    }

    #endregion

    #region 付款列表返回值

    public class DyjFeeApplyResponse : DyjResponse
    {

        public List<FeeApply> data { get; set; }
    }

    public class FeeApply
    {
        /// <summary>
        /// "编号": 4
        /// </summary>
        public string 编号 { get; set; }

        /// <summary>
        /// "制单日期": "2022-03-29"
        /// </summary>
        public string 制单日期 { get; set; }

        /// <summary>
        /// "费用类型": "付款"
        /// </summary>
        public string 费用类型 { get; set; }

        /// <summary>
        /// "制单人": "管理员"
        /// </summary>
        public string 制单人 { get; set; }

        /// <summary>
        /// "付款公司": "Glitter electronics group co.ltd"
        /// 我方公司
        /// </summary>
        public string 付款公司 { get; set; }

        /// <summary>
        /// "币种": "美元"
        /// </summary>
        public string 币种 { get; set; }

        /// <summary>
        /// "收款单位": "广州明泰装饰工程有限公司"
        /// </summary>
        public string 收款单位 { get; set; }

        /// <summary>
        /// "申请金额": 1274.00
        /// </summary>
        public decimal 申请金额 { get; set; }

        /// <summary>
        /// "外币金额": 200.00
        /// </summary>
        public string 外币金额 { get; set; }

        /// <summary>
        /// "状态": "付款完成"
        /// </summary>
        public string 状态 { get; set; }

        /// <summary>
        /// "公司": "总公司"
        /// </summary>
        public string 公司 { get; set; }

        /// <summary>
        /// "部门": "公司"
        /// </summary>
        public string 部门 { get; set; }

        /// <summary>
        /// "员工": "管理员"
        /// </summary>
        public string 员工 { get; set; }

        /// <summary>
        /// "摘要": "海关增值税"
        /// </summary>
        public string 摘要 { get; set; }

        /// <summary>
        /// 明细金额
        /// </summary>
        public decimal 明细金额 { get; set; }

        /// <summary>
        /// "明细外币金额": 100.00
        /// </summary>
        public decimal 明细外币金额 { get; set; }

        /// <summary>
        /// "剩余金额": 0.00
        /// </summary>
        public decimal 剩余金额 { get; set; }

        /// <summary>
        /// "剩余外币金额": 0.00
        /// </summary>
        public decimal 剩余外币金额 { get; set; }
    }

    #endregion

    #region 付款明细返回值

    public class DyjFeeApplyDetailResponse : DyjResponse
    {
        public new DyjFeeApplyDetailData data { get; set; }

        public new List<DyjFeeApplyDetailList> list { get; set; }
    }

    /// <summary>
    /// 付款申请主表信息
    /// </summary>
    public class DyjFeeApplyDetailData
    {
        /// <summary>
        /// 4
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// "2022-03-29T17:35:37.28"
        /// </summary>
        public DateTime PublicDate { get; set; }

        /// <summary>
        /// 2
        /// </summary>
        public int CurrencyID { get; set; }

        /// <summary>
        /// "付款"
        /// </summary>
        public string FeeType { get; set; }

        /// <summary>
        /// 101
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// "管理员"
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 1274.00
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 200.00
        /// </summary>
        public decimal FAmount { get; set; }

        /// <summary>
        /// 汇率：6.3700
        /// </summary>
        public decimal HL { get; set; }

        /// <summary>
        /// "付款完成"
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// 101
        /// </summary>
        public int CheckID { get; set; }

        /// <summary>
        /// "管理员"
        /// </summary>
        public string CheckName { get; set; }

        /// <summary>
        /// 101
        /// </summary>
        public int PayerID { get; set; }

        /// <summary>
        /// "管理员"
        /// </summary>
        public string PayerName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 173487
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// ""
        /// </summary>
        public string PayType { get; set; }

        /// <summary>
        /// "79"
        /// </summary>
        public string PayComID { get; set; }

        /// <summary>
        /// "Glitter electronics group co.ltd"
        /// </summary>
        public string PayComName { get; set; }

        /// <summary>
        /// 1274.00
        /// </summary>
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 200.00
        /// </summary>
        public decimal PayFAmount { get; set; }

        /// <summary>
        /// "729644,729645"
        /// </summary>
        public string PayMentIDs { get; set; }
    }

    /// <summary>
    /// 付款申请子表
    /// </summary>
    public class DyjFeeApplyDetailList
    {

        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 53
        /// </summary>
        public int PayType { get; set; }

        /// <summary>
        /// 海关增值税
        /// </summary>
        public string PayTypeName { get; set; }

        /// <summary>
        /// 4
        /// </summary>
        public string MainID { get; set; }

        /// <summary>
        /// 100
        /// </summary>
        public int CorpID { get; set; }

        /// <summary>
        /// 总公司
        /// </summary>
        public string CorpName { get; set; }

        /// <summary>
        /// 101
        /// </summary>
        public int DeptID { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 101
        /// </summary>
        public int EmployeeID { get; set; }

        /// <summary>
        /// 管理员
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// 海关增值税
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 637.00
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 0.00
        /// </summary>
        public decimal BalanceAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 100.00
        /// </summary>
        public decimal FAmount { get; set; }

        /// <summary>
        /// 0.00
        /// </summary>
        public decimal BalanceFAmount { get; set; }
    }

    #endregion

    #region 付汇申请

    public class DyjPayExchangeData
    {
        /// <summary>
        /// 大赢家币种*
        /// </summary>
        public int CurrencyID { get; set; }

        /// <summary>
        /// 汇兑（后台有赋值）
        /// </summary>
        public int FeeType { get; set; }

        /// <summary>
        /// 制单人公司ID*
        /// </summary>
        public int CorpID { get; set; }

        /// <summary>
        /// 制单人部门ID*
        /// </summary>
        public int DeptID { get; set; }

        /// <summary>
        /// 制单人ID*
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 制单人名称*
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 金额（人民币）* 
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 金额外币（外币）*
        /// </summary>
        public decimal FAmount { get; set; }

        /// <summary>
        /// 汇率（人民币为1:；其他的为两位数小数点的汇率）
        /// </summary>
        public decimal HL { get; set; }

        /// <summary>
        /// 状态（默认为等待审批）可以为空（后台有赋值）
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// 指定审核人*
        /// </summary>
        public int CheckID { get; set; }

        /// <summary>
        /// 指定审核人名称*
        /// </summary>
        public string CheckName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 开票类型(3)（后台有赋值） 
        /// </summary>
        public int PayType { get; set; }

        /// <summary>
        /// 开票公司ID*
        /// </summary>
        public int PayComID { get; set; }

        /// <summary>
        /// 开票公司名称*
        /// </summary>
        public string PayComName { get; set; }

        /// <summary>
        /// 收款单位ID【收款单位ID和收款单位不能同时为空】
        /// </summary>
        public int ClientID { get; set; }

        /// <summary>
        /// 收款单位名称【收款单位ID和收款单位不能同时为空】
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string ClientLinkName { get; set; }

        /// <summary>
        /// 银行
        /// </summary>
        public string ClientBank { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string ClientBankNum { get; set; }

        /// <summary>
        /// 银行地址
        /// </summary>
        public string ClientBankAddress { get; set; }

        /// <summary>
        /// （付汇单位ID）*int【付汇单位ID和付汇单位不能同时为空】
        /// </summary>
        public int Provider { get; set; }

        /// <summary>
        /// 付汇单位名称【付汇单位ID和付汇单位不能同时为空】 
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// 付汇联系人
        /// </summary>
        public string ProviderLinkName { get; set; }

        /// <summary>
        /// 付汇银行
        /// </summary>
        public string ProviderBank { get; set; }

        /// <summary>
        /// 付汇银行账号
        /// </summary>
        public string ProviderBankNum { get; set; }

        /// <summary>
        /// 付汇银行地址
        /// </summary>
        public string ProviderBankAddress { get; set; }

        /// <summary>
        /// 华芯通付汇申请ID
        /// </summary>
        public string XDTID { get; set; }
    }

    #endregion

    #region 付款审批

    public class DyjPayApproveData
    {
        /// <summary>
        /// 单据号
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 指定付款人ID
        /// </summary>
        public int payerID { get; set; }

        /// <summary>
        /// 指定付款人名称
        /// </summary>
        public string payerName { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public int userID { get; set; }

        /// <summary>
        /// 审核信息
        /// </summary>
        public string checkInfo { get; set; }

        /// <summary>
        /// 布尔类型，是否通过
        /// </summary>
        public bool isPass { get; set; }

        public string key { get; set; }

    }

    #endregion

    #region 删除付款/汇申请

    public class DyjDeleteFeeApplyData
    {
        /// <summary>
        /// 单据号
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public int userID { get; set; }

        public string key { get; set; }

    }

    #endregion

    #region 付款

    public class DyjPaymentData
    {
        /// <summary>
        /// 金库
        /// </summary>
        public int FiskID { get; set; }

        /// <summary>
        /// 账户
        /// </summary>
        public int AccountID { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>
        public int CreatorID { get; set; }

        /// <summary>
        /// 供应商名称(付汇公司名称)*
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// 付款类型
        /// </summary>
        public int BalanceStyle { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

    }

    public class DyjPaymentList
    {

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 默认值为1
        /// </summary>
        public int AmortizeCount { get; set; }

        /// <summary>
        /// BillType
        /// 正常为[45 - 预收付汇];
        /// 利息为[24 - 利息];
        /// 银行费用为[25 - 银行费用];
        /// 汇兑损益为[26 - 汇兑损益];
        /// 税金及附加为[28 - 税金及附加];
        /// </summary>
        public int BillType { get; set; }

        /// <summary>
        /// 付款人员公司ID
        /// </summary>
        public int CorpID { get; set; }

        /// <summary>
        /// 付款人员部门ID
        /// </summary>
        public int DeptID { get; set; }

        /// <summary>
        /// 付款人员ID
        /// </summary>
        public int EmployeeID { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 外币金额
        /// </summary>
        public decimal ForeignAmount { get; set; }

        /// <summary>
        /// 人民币金额
        /// </summary>
        public decimal Amount { get; set; }
    }

    #endregion
}
