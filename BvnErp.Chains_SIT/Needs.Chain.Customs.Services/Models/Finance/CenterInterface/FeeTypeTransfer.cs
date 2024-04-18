using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class FeeTypeTransfer
    {
        static object locker = new object();
        private FeeTypeTransfer() { }

        static private FeeTypeTransfer current;

        static public FeeTypeTransfer Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new FeeTypeTransfer();
                        }
                    }
                }

                return current;
            }
        }
        public FinanceFeeType C2LInTransfer(string cataID)
        {
            string feeName =  AccountCatalogsAlls.Current.catalogName(cataID);
            FinanceFeeType feeType = FinanceFeeType.Other;
            switch (feeName)
            {
                case "预收账款":
                    feeType = FinanceFeeType.DepositReceived;
                    break;

                case "报关货款":
                    feeType = FinanceFeeType.Product;
                    break;

                case "增值税":
                    feeType = FinanceFeeType.AddedValueTax;
                    break;

                case "关税":
                    feeType = FinanceFeeType.Tariff;
                    break;

                case "报关服务费":
                    feeType = FinanceFeeType.DeclareService;
                    break;

                case "仓储收入":
                    feeType = FinanceFeeType.Warehouse;
                    break;

                case "代发货收入":
                    feeType = FinanceFeeType.GoodsAgent;
                    break;

                case "异常理货收入":
                    feeType = FinanceFeeType.SpecialPack;
                    break;

                case "其他收费收入":
                    feeType = FinanceFeeType.Other;
                    break;


                case "租赁收入":
                    feeType = FinanceFeeType.Rent;
                    break;

                case "银行借款":
                    feeType = FinanceFeeType.BankBorrow;
                    break;               

                case "存款利息":
                    feeType = FinanceFeeType.Interest;
                    break;

                case "单位借款":
                    feeType = FinanceFeeType.CompanyBorrow;
                    break;

                case "收回押金":
                    feeType = FinanceFeeType.Deposit;
                    break;

                case "员工还款":
                    feeType = FinanceFeeType.EmployeePayback;
                    break;

                case "调入":
                    feeType = FinanceFeeType.CallIn;
                    break;

                case "其他收入":
                    feeType = FinanceFeeType.GenereOther;
                    break;

                case "汇兑损益":
                    feeType = FinanceFeeType.ExchangeLoss;
                    break;

                case "代销收款":
                    feeType = FinanceFeeType.SaleAgent;
                    break;

                case "认证保证金":
                    feeType = FinanceFeeType.Guarantee;
                    break;

                case "第三方收款":
                    feeType = FinanceFeeType.ThirdService;
                    break;

                default:
                   
                    break;
            }

            return feeType;
        }

        public string L2CInTransfer(FinanceFeeType feetype)
        {
            string feeTypeName = feetype.GetDescription();
            switch (feeTypeName)
            {
                case "货款":
                    feeTypeName = "报关货款";
                    break;

                case "供应链其他收入":
                    feeTypeName = "其他收费收入";
                    break;

                case "综合其他收入":
                    feeTypeName = "其他收入";
                    break;

                default:
                    break;
            }
            string feeID = AccountCatalogsAlls.Current.catalogIDIn(feeTypeName);
            return feeID;
        }

        public FinanceFeeType C2LOutTransfer(string cataID)
        {
            string feeName = AccountCatalogsAlls.Current.catalogName(cataID);
            FinanceFeeType feeType = FinanceFeeType.PayOtherFee;
            switch (feeName)
            {
                case "预付账款":
                    feeType = FinanceFeeType.PayPreBill;
                    break;

                case "货款":
                    feeType = FinanceFeeType.PayGoods;
                    break;

                case "海关增值税":
                    feeType = FinanceFeeType.PayAddedValue;
                    break;

                case "关税":
                    feeType = FinanceFeeType.PayTariff;
                    break;

                case "缴纳所得税":
                    feeType = FinanceFeeType.PayIncomeTax;
                    break;



                case "押金":
                    feeType = FinanceFeeType.PayDespoist;
                    break;

                case "银行还贷":
                    feeType = FinanceFeeType.PayBank;
                    break;

                case "还单位借款":
                    feeType = FinanceFeeType.PayCompany;
                    break;

                case "员工借款":
                    feeType = FinanceFeeType.PayEmployee;
                    break;

                case "其他支出":
                    feeType = FinanceFeeType.PayOther;
                    break;



                case "调出":
                    feeType = FinanceFeeType.PayCallout;
                    break;

                case "代销付款":
                    feeType = FinanceFeeType.PaySaleAgent;
                    break;

                case "代销退款":
                    feeType = FinanceFeeType.PaySaleAgentBack;
                    break;

                case "退定金":
                    feeType = FinanceFeeType.PayDespoistBack;
                    break;

                case "职工薪酬":
                    feeType = FinanceFeeType.PaySalary;
                    break;



                case "劳动保险费":
                    feeType = FinanceFeeType.PayEndowment;
                    break;

                case "住房公积金":
                    feeType = FinanceFeeType.PayHousingProvidentFund;
                    break;

                case "运杂费":
                    feeType = FinanceFeeType.PayIncidentals;
                    break;

                case "差旅费":
                    feeType = FinanceFeeType.PayBussiness;
                    break;

                case "广告宣传费":
                    feeType = FinanceFeeType.PayAdvertisement;
                    break;


                case "租赁费及物业费":
                    feeType = FinanceFeeType.PayPropertyMan;
                    break;

                case "业务招待费":
                    feeType = FinanceFeeType.PayEntertain;
                    break;

                case "借款利息":
                    feeType = FinanceFeeType.PayInterest;
                    break;

                case "报关服务费":
                    feeType = FinanceFeeType.PayService;
                    break;

                case "仓储保管费":
                    feeType = FinanceFeeType.PayWarehouse;
                    break;

                case "电话及网络通信费":
                    feeType = FinanceFeeType.PayNet;
                    break;


                case "包装费":
                    feeType = FinanceFeeType.PayPackage;
                    break;

                case "审计及会计服务费":
                    feeType = FinanceFeeType.PayAudit;
                    break;

                case "税金及附加":
                    feeType = FinanceFeeType.PayTax;
                    break;

                case "贸易通清关费":
                    feeType = FinanceFeeType.PayTrade;
                    break;

                case "印花税":
                    feeType = FinanceFeeType.PayStampTax;
                    break;


                case "房产税":
                    feeType = FinanceFeeType.PayHouse;
                    break;

                case "残保金":
                    feeType = FinanceFeeType.PayInjury;
                    break;

                case "借款附加费":
                    feeType = FinanceFeeType.PayBorrowAdded;
                    break;

                case "银行手续费":
                    feeType = FinanceFeeType.PayBankPaypal;
                    break;

                case "贴现利息":
                    feeType = FinanceFeeType.PayAcceptance;
                    break;

                case "存款利息":
                    feeType = FinanceFeeType.PayDepositInterest;
                    break;


                case "内部利息":
                    feeType = FinanceFeeType.PayInterInterest;
                    break;

                case "水电费":
                    feeType = FinanceFeeType.PayWater;
                    break;

                case "诉讼费":
                    feeType = FinanceFeeType.PayLawsuit;
                    break;

                case "坏账损失":
                    feeType = FinanceFeeType.PayBadBorrow;
                    break;

                case "办公用品及设备购置费":
                    feeType = FinanceFeeType.PayOffice;
                    break;


                case "车辆支出":
                    feeType = FinanceFeeType.PayCar;
                    break;

                case "其他费用":
                    feeType = FinanceFeeType.PayOtherFee;
                    break;

               

              
                default:

                    break;
            }

            return feeType;
        }

        public string L2COutTransfer(FinanceFeeType feetype)
        {
            string feeTypeName = feetype.GetDescription();
            feeTypeName = feeTypeName.Replace("付款-", "");
            string feeID = AccountCatalogsAlls.Current.catalogIDOut(feeTypeName);
            return feeID;
        }
    }
}
