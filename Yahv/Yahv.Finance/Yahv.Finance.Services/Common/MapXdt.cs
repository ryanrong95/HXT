using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums;

namespace Yahv.Finance.Services
{
    /// <summary>
    /// 芯达通映射
    /// </summary>
    public class MapXdt
    {
        #region 账户类型
        /// <summary>
        /// 根据中心账户类型映射芯达通
        /// </summary>
        /// <param name="accountTypes">账户类型数组</param>
        /// <returns></returns>
        public static string MapAccountTypeToXdt(string[] accountTypes)
        {
            string result = string.Empty;

            using (var accountsView = new AccountTypesRoll())
            {
                var accounts = accountsView.ToArray();

                if (accountTypes.Length <= 0)
                {
                    return string.Empty;
                }

                foreach (var accountType in accountTypes)
                {
                    switch (accountType)
                    {
                        case "基本账户":
                            result += ",1";
                            break;
                        case "一般账户":
                            result += ",2";
                            break;
                        case "现金账户":
                            result += ",4";
                            break;
                        case "银行承兑账户":
                            result += ",8";
                            break;
                        case "商业承兑账户":
                            result += ",16";
                            break;
                        case "微信账户":
                            result += ",32";
                            break;
                        case "支付宝账户":
                            result += ",64";
                            break;
                        case "离岸账户":
                            result += ",128";
                            break;
                        default:
                            break;

                    }
                }
            }

            return result.Trim(',');
        }

        /// <summary>
        /// 根据芯达通映射中心账户类型
        /// </summary>
        /// <param name="accountTypes">芯达通字符串（1,2）</param>
        /// <returns></returns>
        public static string[] MapAccountTypeFromXdt(string accountTypes)
        {
            var result = new List<string>();

            using (var accountsView = new AccountTypesRoll())
            {
                var accounts = accountsView.ToArray();

                if (string.IsNullOrEmpty(accountTypes))
                {
                    return result.ToArray();
                }

                foreach (var accountType in accountTypes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    switch (accountType)
                    {
                        case "1":
                            result.Add("基本账户");
                            break;
                        case "2":
                            result.Add("一般账户");
                            break;
                        case "4":
                            result.Add("现金账户");
                            break;
                        case "8":
                            result.Add("银行承兑账户");
                            break;
                        case "16":
                            result.Add("商业承兑账户");
                            break;
                        case "32":
                            result.Add("微信账户");
                            break;
                        case "64":
                            result.Add("支付宝账户");
                            break;
                        case "128":
                            result.Add("离岸账户");
                            break;
                        default:
                            break;

                    }
                }
            }

            return result.ToArray();
        }
        #endregion

        #region 收款类型
        /// <summary>
        /// 根据芯达通映射中心收款类型
        /// </summary>
        /// <param name="catalog"></param>
        /// <returns></returns>
        public static string MapPayeeAccountCatalog(int catalog)
        {
            string result = string.Empty;

            using (var view = new AccountCatalogsRoll())
            {
                var array = view.ToArray();
                string name = String.Empty;

                switch (catalog)
                {
                    //资金调入
                    case 10:
                        name = "调入";
                        break;
                    //银行利息
                    case 14:
                        name = "存款利息";
                        break;
                    ////借款
                    //case 12:

                    //    break;
                    //还款
                    case 11:
                        name = "员工还款";
                        break;
                    default:
                        name = "预收账款";
                        break;
                }

                foreach (var source in array.Where(item => item.Name == name))
                {
                    if (source.Name == AccountCatalogType.Input.GetDescription())
                    {
                        result = source.ID;
                        break;
                    }

                    if (!string.IsNullOrWhiteSpace(GetFather(array, source.FatherID, AccountCatalogType.Input.GetDescription())))
                    {
                        result = source.ID;
                        break;
                    }
                }
            }

            return result;
        }

        static string GetFather(AccountCatalog[] data, string fatherId, string catalog)
        {
            string result = string.Empty;

            if (!string.IsNullOrWhiteSpace(fatherId))
            {
                var accountCatalog = data.FirstOrDefault(item => item.ID == fatherId);

                if (accountCatalog?.Name != catalog)
                {
                    result = GetFather(data, accountCatalog?.FatherID, catalog);
                }
                else
                {
                    result = accountCatalog?.ID;
                }
            }

            return result;
        }

        #endregion

        #region 收款方式
        /// <summary>
        /// 根据芯达通映射中心收款方式
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PaymentMethord MapReceiveMethord(int name)
        {
            switch (name)
            {
                //支票
                case 1:
                    return PaymentMethord.Cheque;
                //现金
                case 2:
                    return PaymentMethord.Cheque;
                //承兑
                case 4:
                    return PaymentMethord.BankAcceptanceBill;
                //转账
                //case 3:
                //    return PaymentMethord.BankAcceptanceBill;
                default:
                    return PaymentMethord.BankTransfer;
            }
        }
        #endregion

        #region 费用附件类型
        /// <summary>
        /// 根据芯达通费用类型获取对应的枚举
        /// </summary>
        /// <param name="fileType">芯达通附件值</param>
        /// <returns></returns>
        public static FileDescType MapFileDescTypeFromXdt(int fileType)
        {
            var reuslt = FileDescType.Other;

            switch (fileType)
            {
                case 1:
                    reuslt = FileDescType.ChargeApply;
                    break;
                case 2:
                    reuslt = FileDescType.ChargeApplyVoucher;
                    break;
                default:
                    break;
            }

            return reuslt;
        }

        /// <summary>
        /// 根据中心的费用附件类型获取对应芯达通费用附件
        /// </summary>
        /// <param name="type">中心费用附件枚举</param>
        /// <returns></returns>
        public static int MapFileDescTypeToXdt(FileDescType type)
        {
            var reuslt = 1;

            switch (type)
            {
                case FileDescType.ChargeApply:
                    reuslt = 1;
                    break;
                case FileDescType.ChargeApplyVoucher:
                    reuslt = 2;
                    break;
                default:
                    break;
            }

            return reuslt;
        }

        /// <summary>
        /// 根据承兑汇票类型转换芯达通附件类型
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static int MapMoFileDescTypeToXdt(FileDescType fileType)
        {
            var reuslt = 1;

            switch (fileType)
            {
                case FileDescType.MoneyOrder:
                    reuslt = 1;
                    break;
                default:
                    break;
            }

            return reuslt;
        }
        #endregion
    }
}