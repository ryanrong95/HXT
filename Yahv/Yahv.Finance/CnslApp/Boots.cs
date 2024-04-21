using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv;
using Yahv.Finance.Services;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using PKeyType = Yahv.Finance.Services.PKeyType;

namespace CnslApp
{
    public class Boots
    {
        private const string luyahui = "Admin00523";
        private const string haohongmei = "Admin00530";
        private const string yangyanmeng = "Admin00390";
        private const string CompanyName = "深圳市芯达通供应链管理有限公司,香港畅运国际物流有限公司,香港万路通国际物流有限公司";

        public static void Init()
        {
            using (var reponstory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                InitGoldStores(reponstory);
                InitAccounts(reponstory, GetAccounts());
                InitAccounts(reponstory, GetAccountsSimple());
            }
        }

        /// <summary>
        /// 初始化金库
        /// </summary>
        /// <param name="reponsitory"></param>
        private static void InitGoldStores(PvFinanceReponsitory reponsitory)
        {
            new GoldStore()
            {
                ID = PKeySigner.Pick(PKeyType.GoldStore),
                Name = "深圳金库",
                IsSpecial = false,
                OwnerID = luyahui,
                CreatorID = luyahui,
                ModifierID = luyahui,
            }.Enter();

            new GoldStore()
            {
                ID = PKeySigner.Pick(PKeyType.GoldStore),
                Name = "香港金库",
                IsSpecial = false,
                OwnerID = luyahui,
                CreatorID = luyahui,
                ModifierID = luyahui,
            }.Enter();

            new GoldStore()
            {
                Name = "承兑金库",
                IsSpecial = false,
                OwnerID = haohongmei,
                CreatorID = haohongmei,
                ModifierID = haohongmei,
            }.Enter();
        }

        private static void InitAccounts(PvFinanceReponsitory reponsitory, List<AccountInputDto> inputs)
        {
            foreach (var input in inputs)
            {
                var entity = new Account()
                {
                    NatureType = input.NatureType,
                    Name = input.CompanyName ?? input.AccountName,
                    ShortName = input.AccountName,
                    BankName = GetSimilarBank(input.BankName),
                    Code = input.BankAccount,
                    ManageType = ManageType.Normal,
                    Currency = CurrencyHelper.GetCurrency(input.Currency),
                    OpeningBank = input.BankName,
                    BankAddress = input.BankAddress,
                    District = GetDistrict(input.Region),
                    SwiftCode = input.SwiftCode,
                    IsHaveU = false,
                    OwnerID = input.Owner,
                    GoldStoreID = GetGoldStoreID(input.VaultName),
                    EnterpriseID = GetEnterpriseID(input.CompanyName),
                    CreatorID = input.CreatorID,
                    ModifierID = input.CreatorID,
                    Source = (AccountSource)input.AccountSource,
                };
                entity.Enter();
                InsertAccountType(entity.ID, input.AccountType);

                //初始化余额
                if (input.Balance != null && input.Balance > 0)
                {
                    var rate = ExchangeRates.Universal[entity.Currency, Currency.CNY];
                    //收款流水
                    new FlowAccount()
                    {
                        ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc),
                        AccountMethord = AccountMethord.Receipt,
                        AccountID = entity.ID,
                        Currency = entity.Currency,
                        Price = input.Balance.Value,
                        PaymentDate = DateTime.Now,
                        FormCode = "余额初始化",
                        Currency1 = Currency.CNY,
                        ERate1 = rate,
                        Price1 = input.Balance.Value * rate,
                        CreatorID = input.CreatorID,
                        PaymentMethord = PaymentMethord.BankTransfer,     //收款方式
                    }.Enter();
                }
            }
        }

        #region 数据源
        private static List<AccountInputDto> GetAccounts()
        {
            return new List<AccountInputDto>()
            {
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    CreatorID = haohongmei,
                    Owner = haohongmei,
                    AccountName = "芯达通-星展银行账户",
                    AccountSource = 1,
                    AccountType = "1", //基本户
                    VaultName = "深圳金库",
                    BankAccount="30015588588",
                    BankName="星展银行",
                    BankAddress = "深圳市罗湖区深南东路5001号华润大厦18楼",
                    SwiftCode="DBSSCNSH",
                    Currency="CNY",
                    Balance=decimal.Parse("204280.35"),
                    CompanyName="深圳市芯达通供应链管理有限公司",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    CreatorID = haohongmei,
                    Owner = haohongmei,
                    AccountName = "芯达通-中国银行深圳罗岗支行",
                    AccountSource = 1,
                    AccountType = "1", //基本户
                    VaultName = "深圳金库",
                    BankAccount="764071904447",
                    BankName="中国银行深圳罗岗支行",
                    BankAddress = "广东省深圳市龙岗区布吉镇罗岗百鸽路满庭芳家居广场118-123号",
                    SwiftCode="BKCHCNBJ",
                    Currency="CNY",
                    Balance=decimal.Parse("7199377.65"),
                    CompanyName="深圳市芯达通供应链管理有限公司",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Owner = yangyanmeng,
                    AccountName = "万路通-华侨永亨",
                    AccountSource = 1,
                    AccountType = "1", //基本户
                    VaultName = "香港金库",
                    BankAccount="035802644077831",
                    BankName="华侨永亨银行",
                    BankAddress = "161 Queen's Road  Central",
                    SwiftCode="WIHBHKHHXXX",
                    Currency="USD",
                    Balance=decimal.Parse("1744890.19"),
                    CompanyName="香港万路通国际物流有限公司",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    CreatorID = haohongmei,
                    Owner = haohongmei,
                    AccountName = "芯达通-宁波银行人民币户",
                    AccountSource = 1,
                    AccountType = "1", //基本户
                    VaultName = "深圳金库",
                    BankAccount="73010122001748524",
                    BankName="宁波银行深圳分行",
                    BankAddress = "广东省深圳市福田区福华三路88号财富大厦裙楼1C、2-3层",
                    SwiftCode="BKNBCN2N",
                    Currency="CNY",
                    Balance=decimal.Parse("62756.36"),
                    CompanyName="深圳市芯达通供应链管理有限公司",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    CreatorID = haohongmei,
                    Owner = haohongmei,
                    AccountName = "芯达通-宁波银行美元户",
                    AccountSource = 1,
                    AccountType = "1", //基本户
                    VaultName = "深圳金库",
                    BankAccount="73012025000157244",
                    BankName="宁波银行",
                    BankAddress = "广东省深圳市福田区福华三路88号财富大厦裙楼1C、2-3层",
                    SwiftCode="BKNBCN2N",
                    Currency="USD",
                    Balance=decimal.Parse("0"),
                    CompanyName="深圳市芯达通供应链管理有限公司",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    CreatorID = haohongmei,
                    Owner = haohongmei,
                    AccountName = "芯达通-农业银行人民币账户",
                    AccountSource = 1,
                    AccountType = "1", //基本户
                    VaultName = "深圳金库",
                    BankAccount="41-034100040029140",
                    BankName="中国农业银行",
                    BankAddress = "广东省深圳市福田区福华一路6-1免税商务大厦裙楼首层107商铺",
                    SwiftCode="ABOCCNBJ",
                    Currency="CNY",
                    Balance=decimal.Parse("23974.78"),
                    CompanyName="深圳市芯达通供应链管理有限公司",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    CreatorID = haohongmei,
                    Owner = haohongmei,
                    AccountName = "芯达通-农业银行美元账户",
                    AccountSource = 1,
                    AccountType = "1", //基本户
                    VaultName = "深圳金库",
                    BankAccount="41-034100040029157",
                    BankName="中国农业银行",
                    BankAddress = "广东省深圳市福田区福华一路6-1免税商务大厦裙楼首层107商铺",
                    SwiftCode="ABOCCNBJ",
                    Currency="USD",
                    Balance=decimal.Parse("0.26"),
                    CompanyName="深圳市芯达通供应链管理有限公司",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Owner = yangyanmeng,
                    AccountName = "万路通-星展银行",
                    AccountSource = 1,
                    AccountType = "1", //基本户
                    VaultName = "香港金库",
                    BankAccount="016478000927794",
                    BankName="星展银行",
                    BankAddress = "11/F, The Center , 99 Queen’s Road Central, Central, Hong Kong",
                    SwiftCode="DHBKHKHHXXX",
                    Currency="USD",
                    Balance=decimal.Parse("46001781.65"),
                    CompanyName="香港万路通国际物流有限公司",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Owner = yangyanmeng,
                    AccountName = "畅运-星展银行",
                    AccountSource = 1,
                    AccountType = "1", //基本户
                    VaultName = "香港金库",
                    BankAccount="016478000502260",
                    BankName="星展银行",
                    BankAddress = "11/F, The Center , 99 Queen’s Road Central, Central, Hong Kong",
                    SwiftCode="DHBKHKHHXXX",
                    Currency="USD",
                    Balance=decimal.Parse("35644269.15"),
                    CompanyName="香港畅运国际物流有限公司",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    CreatorID = haohongmei,
                    Owner = haohongmei,
                    AccountName = "芯达通-兴业银行",
                    AccountSource = 1,
                    AccountType = "1", //基本户
                    VaultName = "深圳金库",
                    BankAccount="337030100100266982",
                    BankName="兴业银行",
                    BankAddress = "广东省深圳市罗湖区春风路3013号国晖大厦1层",
                    SwiftCode="FJIBCNBAXXX",
                    Currency="CNY",
                    Balance=decimal.Parse("625929.14"),
                    CompanyName="深圳市芯达通供应链管理有限公司",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Owner = yangyanmeng,
                    AccountName = "畅运-中国银行",
                    AccountSource = 1,
                    AccountType = "1", //基本户
                    VaultName = "香港金库",
                    BankAccount="01259120044968",
                    BankName="中国银行",
                    BankAddress = "1Garden Road Hong Kong",
                    SwiftCode="BKCHHKHHXXX",
                    Currency="USD",
                    Balance=decimal.Parse("6097342.14"),
                    CompanyName="香港畅运国际物流有限公司",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    CreatorID = haohongmei,
                    Owner = haohongmei,
                    AccountName = "芯达通-宁波银行承兑户",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    VaultName = "承兑金库",
                    BankAccount="000000",
                    BankName="宁波银行深圳分行",
                    BankAddress = "广东省深圳市福田区福华三路88号财富大厦裙楼1C-3层",
                    //SwiftCode="BKCHHKHHXXX",
                    Currency="CNY",
                    Balance=decimal.Parse("350000"),
                    CompanyName="深圳市芯达通供应链管理有限公司",
                },

                //荣检
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    AccountName = "360group-花旗",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                    BankAccount="250-390-16942094",
                    BankName="Citibank (HongKong) Limited",
                    BankAddress = "11/F Citi Tower, One Bay East 83 Hoi Bun Road, Kwun Tong, Kowloon HongKong",
                    SwiftCode="CITIHKAX",
                    Currency="USD",
                    //Balance=decimal.Parse("150000"),
                    CompanyName="IC360 GROUP LIMITED",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    AccountName = "360电子-永隆",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                    BankAccount="601-341-8746-1",
                    BankName="Wing Lung Bank Limited",
                    BankAddress = "45 Des Voeux Road Central HongKong",
                    SwiftCode="WUBAHKHH",
                    Currency="USD",
                    //Balance=decimal.Parse("150000"),
                    CompanyName="IC360 ELECTRONICS LIMITED",
                },
                new AccountInputDto()
                {
                    CompanyName="Anda International Trade Group Limited",
                    AccountName = "anda-星展",
                    BankName="DBS Bank (Hong Kong) Limited",
                    BankAccount="000421141",
                    SwiftCode="DHBKHKHH",
                    BankAddress = "12/F, Tower A, Mira Place, 132 Nathan Road, Tsim Sha Tsui, Kowloon, HK",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="KB ELECTRONICS DEVELOPMENT LIMITED",
                    AccountName = "KB-星展",
                    BankName="DBS Bank (Hong Kong) Limited",
                    BankAccount="001138105",
                    SwiftCode="DHBKHKHH",
                    BankAddress = "",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="HK HUANYU ELECTRONICS TECHNOLOGY CO., LIMITED",
                    AccountName = "环宇-中银",
                    BankName="BANK OF CHINA (HONG KONG) LIMITED",
                    BankAccount="01259120050705",
                    SwiftCode="bkchhkhh",
                    BankAddress = "",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="B ONE B ELECTRONIC CO., LIMITED",
                    AccountName = "B1B-中银",
                    BankName="BANK OF CHINA (HONG KONG) LIMITED",
                    BankAccount="01259120050598",
                    SwiftCode="bkchhkhh",
                    BankAddress = "",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="Anda International Trade Group Limited",
                    AccountName = "anda-中银",
                    BankName="BANK OF CHINA (HONG KONG) LIMITED",
                    BankAccount="01257320115266",
                    SwiftCode="bkchhkhh",
                    BankAddress = "",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="HK Lianchuang Electronics Co.,Limited",
                    AccountName = "联创-中银",
                    BankName="BANK OF CHINA (HONG KONG) LIMITED",
                    BankAccount="012-923-2-007792-0",
                    SwiftCode="bkchhkhh",
                    BankAddress = "",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="IC360 ELECTRONICS LIMITED",
                    AccountName = "360电子-汇丰",
                    BankName="THE HONGKONG AND SHANGHAI BANKING CORPORATION LIMITED",
                    BankAccount="038518908838",
                    SwiftCode="HSBCHKHHHKH",
                    BankAddress = "1 Queen's Road Central, Hong Kong",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="HK Lianchuang Electronics Co.,Limited",
                    AccountName = "联创-汇丰",
                    BankName="THE HONGKONG AND SHANGHAI BANKING CORPORATION LIMITED",
                    BankAccount="484288170838",
                    SwiftCode="HSBCHKHHHKH",
                    BankAddress = "1 Queen's Road Central, Hong Kong",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="Mouser Electronics, Inc",
                    AccountName = "Mouser",
                    BankName="JPMorgan Chase Bank NA, Hong Kong Branch",
                    BankAccount="6872262719",
                    SwiftCode="CHASHKHH",
                    BankAddress = "",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="Arrow Electronics Inc",
                    AccountName = "Arrow",
                    BankName="JP Morgan Chase Bank",
                    BankAccount="844028894",
                    SwiftCode="CHASUS33",
                    BankAddress = "10410 Highland Manor Drive Tampa Florida 33610",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="RS Components Ltd.",
                    AccountName = "RS",
                    BankName="The Hong Kong & Shanghai Banking Corporation Ltd",
                    BankAccount="004-181-890559-274",
                    SwiftCode="HSBCHKHHHKH",
                    BankAddress = "",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="Microchip Technology Ireland Limited",
                    AccountName = "Microchip Technology Ireland Limited",
                    BankName="HUA NAN COMMERCIAL BANK LTD",
                    BankAccount="700-99-0000889",
                    SwiftCode="HNBKTWTP700",
                    BankAddress = "OFFSHORE BANKING BRANCH 38, SEC.1CHUNG-KING SOUTH RD.,TAIPEI,TAIWAN",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="Avnet Europe Comm. VA",
                    AccountName = "Avnet欧洲",
                    BankName="JPMorgan Chase Bank NA",
                    BankAccount="GB19CHAS60924241322584",
                    SwiftCode="CHASGB2L",
                    BankAddress = "25 Bank Street, Canary Wharf E14 5JP London GB",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="AVNET E.M.",
                    AccountName = "AVNET E.M.",
                    BankName="JP Morgan Chase",
                    BankAccount="5937116",
                    SwiftCode="CHASUS33",
                    BankAddress = "CHICAGO, ILLINOIS",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="Avnet Technology Hong Kong Ltd",
                    AccountName = "Avnet Technology Hong Kong Ltd",
                    BankName="Standard Chartered Bank",
                    BankAccount="003-447-00611978",
                    SwiftCode="SCBLHKHHXXX",
                    BankAddress = "Shop 16，G/F & Lower G/F, New World Tower, 16-18 Queen's Road Central, Hong Kong",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="EBV Elektronik GmbH & Co. KG",
                    AccountName = "EBV Elektronik GmbH & Co. KG",
                    BankName="JPMorgan Chase Bank N.A",
                    BankAccount="41313953",
                    SwiftCode="CHASGB2LXXX",
                    BankAddress = "",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="Master Electronics Group Limited",
                    AccountName = "Master Electronics Group Limited",
                    BankName="Citibank N.A., Hong Kong Branch",
                    BankAccount="006-391-91113334",
                    SwiftCode="CITIHKHXXXX",
                    BankAddress = "3 Garden Road, Central, Hong Kong",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="ROCHESTER ELECTRONICS LLC",
                    AccountName = "ROCHESTER ELECTRONICS LLC",
                    BankName="Santander Bank",
                    BankAccount="84992016679",
                    SwiftCode="SVRN US 33",
                    BankAddress = "75 State Street Boston, Ma. USA 02109",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="Future Electronics Inc (Distribution) Pte Ltd",
                    AccountName = "Future Electronics Inc (Distribution) Pte Ltd",
                    BankName="The Hongkong and Shanghai Banking Corporation Limited",
                    BankAccount="260169818180",
                    SwiftCode="HSBCSGSG",
                    BankAddress = "21 Collyer Quay, #01-01 HSBC Building Singapore 049320",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="Waldom Electronics Asia Pacific(S)Pte Ltd",
                    AccountName = "Waldom Electronics Asia Pacific(S)Pte Ltd",
                    BankName="The Hongkong and Shanghai Banking Corporation Limited 21 Collyer Quay",
                    BankAccount="260-453592-178",
                    SwiftCode="HSBCSGSG",
                    BankAddress = "",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="CHIP ONE STOP(HONG KONG)LIMITED",
                    AccountName = "CHIP ONE STOP(HONG KONG)LIMITED",
                    BankName="Sumitomo Mitsui Banking Corporation",
                    BankAccount="10095801",
                    SwiftCode="SMBCHKHH",
                    BankAddress = "",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="Burklin GmbH & Co. KG",
                    AccountName = "Burklin GmbH & Co. KG",
                    BankName="Bayerische Hypo- und Vereinsbank AG Munich",
                    BankAccount="DE77 7002 0270 0000 4095 50",
                    SwiftCode="HYVEDEMM",
                    BankAddress = "",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="AIPCO INC",
                    AccountName = "AIPCO INC",
                    BankName="ROYAL BANK OF CANADA",
                    BankAccount="06032 400 512 0",
                    SwiftCode="ROYCCAT2",
                    BankAddress = "",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="LOUIS YEN  SINGAPORE PTE. LTD",
                    AccountName = "LOUIS YEN  SINGAPORE PTE. LTD",
                    BankName="HSBC HONG KONG",
                    BankAccount="023-690415-838",
                    SwiftCode="HSBCHKHHHKH",
                    BankAddress = "HSBC WAN CHAI COMMERCIAL SERVICE CENTRE SHOP 110-120, EMPEROR GROUP CENTRE, 288 HENNESSY ROAD, WAN CHAI, HONG KONG",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="Digi-Key Electronics Asia Pacific Ltd",
                    AccountName = "Digi-Key Electronics Asia Pacific Ltd",
                    BankName="Standard Chartered Bank (Hong Kong) Limited, Hong Kong",
                    BankAccount="44717966235",
                    SwiftCode="SCBLHKHHXXX",
                    BankAddress = "Standard Chartered Tower 15/Floor, 388 Kwun Tong Road",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="CoreStaff Hong Kong Limited",
                    AccountName = "CoreStaff Hong Kong Limited",
                    BankName="MUFG Bank, Ltd.",
                    BankAccount="8560005084",
                    SwiftCode="BOTKHKHH",
                    BankAddress = "15/F Peninsula Office Tower, 18 Middle Road, Kowloon, Hong Kong",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="transfer multisort elektronik sp.z o.o",
                    AccountName = "transfer multisort elektronik sp.z o.o",
                    BankName="mBank S.A.",
                    BankAccount="PL20114011080000377910001013",
                    SwiftCode="brexplpwlod",
                    BankAddress = "",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="TTI Hong Kong Limited",
                    AccountName = "TTI Hong Kong Limited",
                    BankName="JPMorgan Chase Bank, N.A. Hong Kong Branch",
                    BankAccount="6872254369",
                    SwiftCode="CHASHKHH",
                    BankAddress = "",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="Heilind Asia Pacific (HK) Limited",
                    AccountName = "Heilind Asia Pacific (HK) Limited",
                    BankName="Bank of America",
                    BankAccount="605580703023",
                    SwiftCode="BOFAHKHXXXX",
                    BankAddress = "20th Floor, Tower 2, Kowloon Commerce Centre, 51 Kwai Cheong Road,Kwai Chung, Hong Kong",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
                new AccountInputDto()
                {
                    CompanyName="Rutronik Elektronische Bauelemente Gmbh",
                    AccountName = "Rutronik Elektronische Bauelemente Gmbh",
                    BankName="DEUTSCHE BANK AG, branch Pforzheim",
                    BankAccount="28028900",
                    SwiftCode="DEUTDESM666",
                    BankAddress = "中转银行：Deutsche Bank AG, New York, BIC: DEUTUS33XXX",
                    
                    //Balance=decimal.Parse("150000"),
                    NatureType = NatureType.Public,
                    CreatorID = yangyanmeng,
                    Currency="USD",
                    AccountSource = 1,
                    AccountType = "8", //基本户
                    //VaultName = "香港金库",
                },
            };
        }

        private static List<AccountInputDto> GetAccountsSimple()
        {
            return new List<AccountInputDto>()
            {
                new AccountInputDto()
                {
                    NatureType = NatureType.Private,
                    AccountName = "邵晨华",
                    BankAccount="6226096557273037",
                    BankName="招商银行深圳振华支行",
                    CreatorID = luyahui,
                    Owner = luyahui,


                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "深圳市跨越速运有限公司",
                    CompanyName="深圳市跨越速运有限公司",

                    BankAccount="755933100910301",
                    BankName="招商银行深圳宝安支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "张家港创立文体商贸有限公司",
                    CompanyName="张家港创立文体商贸有限公司",

                    BankAccount="803000000366188",
                    BankName="张家港市农村商业银行东莱支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Private,
                    AccountName = "郝红梅",

                    BankAccount="6225887863861163",
                    BankName="招商银行深圳威盛大厦支行",
                    CreatorID = haohongmei,
                    Owner = haohongmei,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "亿企赢网络科技有限公司",
                    CompanyName="亿企赢网络科技有限公司",

                    BankAccount="1202023019900086246",
                    BankName="中国工商银行杭州延中支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "深圳市社保局",

                    BankAccount="2",
                    //BankName="中国工商银行杭州延中支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "顺丰速运有限公司",
                    CompanyName="顺丰速运有限公司",

                    BankAccount="4000025319200395130",
                    BankName="工商银行深圳车公庙支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "深圳市飞龙世纪物流有限公司",
                    CompanyName="深圳市飞龙世纪物流有限公司",

                    BankAccount="4000026609201306761",
                    BankName="中国工商银行深圳龙华支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Private,
                    AccountName = "王启慧",

                    BankAccount="6217852000025312342",
                    BankName="中国银行深圳坂田之行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "北京京东世纪信息技术有限公司",
                    CompanyName="北京京东世纪信息技术有限公司",

                    BankAccount="110907597010206",
                    BankName="招商银行北京青年路支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "深圳市住房公积金管理中心归集挂帐户",

                    BankAccount="442000034156313120001000057",
                    BankName="建行深圳分行营业部",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "北京驰疆知识产权代理有限公司",
                    CompanyName="北京驰疆知识产权代理有限公司",

                    BankAccount="3272 5789 7487",
                    BankName="中国银行股份有限公司北京上地信息路支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Private,
                    AccountName = "荣检",

                    BankAccount="6217856101007638252",
                    BankName="中国银行苏州东环路支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "工资账户",

                    BankAccount="1",
                    //BankName="中国银行苏州东环路支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Private,
                    AccountName = "李昌伟",

                    BankAccount="6222621310033493020",
                    BankName="交通银行深圳坪山支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Private,
                    AccountName = "陈保民",

                    BankAccount="6226090105668905",
                    BankName="招商银行北京中关村支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "中国移动通信集团广东有限公司",
                    CompanyName="中国移动通信集团广东有限公司",

                    BankAccount="41008900040154778",
                    BankName="中国农业银行国贸支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "深圳市咨询宝网络服务有限公司",
                    CompanyName="深圳市咨询宝网络服务有限公司",

                    BankAccount="4000 0427 0910 0527 364",
                    BankName="工商银行深圳水榭春天支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "深圳源丰会计师事务所有限公司",
                    CompanyName="深圳源丰会计师事务所有限公司",

                    BankAccount="1500 0062 3425 92",
                    BankName="平安银行总行营业部",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Private,
                    AccountName = "鲁亚慧",

                    BankAccount="6222621310011466527",
                    BankName="交通银行深圳红荔之行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "中检检通（深圳）实业有限公司",
                    CompanyName="中检检通（深圳）实业有限公司",

                    BankAccount="087818120100302012068",
                    BankName="光大银行蛇口支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Private,
                    AccountName = "陈保民(工资卡）",

                    BankAccount="6225887842904589",
                    BankName="招商银行深圳威盛大厦支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                //new AccountInputDto()
                //{
                //    NatureType = NatureType.Public,
                //    AccountName = "深圳市芯达通供应链管理有限公司",

                //    BankAccount="337030100100266982",
                //    BankName="兴业银行深圳罗湖支行",
                //    CreatorID = luyahui,
                //    Owner = luyahui,

                //    AccountSource = 2,
                //    AccountType = "1", //基本户
                //    Currency="CNY",
                //},
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "龙港市优择文化用品有限公司",
                    CompanyName="龙港市优择文化用品有限公司",

                    BankAccount="201000246669742",
                    BankName="浙江苍南农村商业银行股份有限公司龙港支行城西分理处",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Private,
                    AccountName = "卢晓玲",

                    BankAccount="6228270087535269071",
                    BankName="中国农业银行广州市黄埔区红山支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Private,
                    AccountName = "曹丽平",

                    BankAccount="6214 8378 4759 3096",
                    BankName="招商银行车公庙支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Private,
                    AccountName = "张孟雨",

                    BankAccount="6222621310031667112",
                    BankName="交通银行深圳布吉支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Private,
                    AccountName = "卢晓玲",

                    BankAccount="6228270087535269071",
                    BankName="中国农业银行",
                    CreatorID = "Admin01074",
                    Owner = "Admin01074",

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "深圳市快金数据技术服务有限公司",
                    CompanyName="深圳市快金数据技术服务有限公司",

                    BankAccount="7917 0155 2000 14836",
                    BankName="上海浦东发展银行深圳分行营业部",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                //new AccountInputDto()
                //{
                //    NatureType = NatureType.Public,
                //    AccountName = "深圳市芯达通供应链管理有限公司",

                //    BankAccount="73010122001748524",
                //    BankName="宁波银行深圳分行营业部",
                //    CreatorID = haohongmei,
                //    Owner = haohongmei,

                //    AccountSource = 2,
                //    AccountType = "1", //基本户
                //    Currency="CNY",
                //},
                //new AccountInputDto()
                //{
                //    NatureType = NatureType.Public,
                //    AccountName = "深圳市芯达通供应链管理有限公司",

                //    BankAccount="30015588588",
                //    BankName="星展银行（中国）有限公司深圳分行",
                //    CreatorID = haohongmei,
                //    Owner = haohongmei,

                //    AccountSource = 2,
                //    AccountType = "1", //基本户
                //    Currency="CNY",
                //},
                new AccountInputDto()
                {
                    NatureType = NatureType.Private,
                    AccountName = "刘振虎",

                    BankAccount="6214837862910183",
                    BankName="招商银行深圳分行梅龙支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "深圳市龙岗区益丰水店",
                    CompanyName="深圳市龙岗区益丰水店",

                    BankAccount="000286283399",
                    BankName="深圳市农村商业银行吉华支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "醴陵市鸿博瓷业有限公司",
                    CompanyName="醴陵市鸿博瓷业有限公司",

                    BankAccount="82010750001027558",
                    BankName="湖南醴陵农村商业银行股份有限公司孙家湾支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "深圳税务局",

                    BankAccount="无",
                    CreatorID = "Admin01074",
                    Owner = "Admin01074",

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Private,
                    AccountName = "张庆永",

                    BankAccount="6222621310004492233",
                    BankName="交通银行深圳金田支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "广州晶东贸易有限公司",
                    CompanyName="广州晶东贸易有限公司",

                    BankAccount="120906065810403",
                    BankName="招商银行北京青年路支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "深圳市腾讯计算机系统有限公司",
                    CompanyName="深圳市腾讯计算机系统有限公司",

                    BankAccount="1 8172 8229 9610 0010 0008 0696 6",
                    BankName="招商银行深圳威盛大厦支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "阿里云计算有限公司",
                    CompanyName="阿里云计算有限公司",

                    BankAccount="5719 0549 3610 7020 2005 7576",
                    BankName="招商银行股份有限公司杭州高新支行",
                    CreatorID = luyahui,
                    Owner = luyahui,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
                new AccountInputDto()
                {
                    NatureType = NatureType.Public,
                    AccountName = "未知",

                    BankAccount="未知",
                    CreatorID = haohongmei,
                    Owner = haohongmei,

                    AccountSource = 2,
                    AccountType = "1", //基本户
                    Currency="CNY",
                },
            };
        }
        #endregion

        #region 自定义函数
        /// <summary>
        /// 根据开户行获取银行名称
        /// </summary>
        /// <param name="name">银行名称</param>
        /// <returns></returns>
        private static string GetSimilarBank(string name)
        {
            var result = name;

            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            using (var view = new BanksRoll())
            {
                Dictionary<string, float> dic = new Dictionary<string, float>();
                var banks = view.Select(item => new { item.ID, item.Name, item.EnglishName }).ToArray();
                foreach (var bank in banks)
                {
                    var float1 = EditorDistance.Levenshtein(bank.Name, name);
                    var float2 = EditorDistance.Levenshtein(bank.EnglishName, name);

                    if (float1 >= float2)
                    {
                        dic.Add(bank.Name, float1);
                    }
                    else
                    {
                        dic.Add(bank.Name, float2);
                    }
                }

                result = dic.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            }

            return result;
        }

        /// <summary>
        /// 根据编码获取区域名称
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string GetDistrict(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return string.Empty;
            }

            return Enum.GetValues(typeof(Origin))
                .Cast<Origin>()
                .FirstOrDefault(item => item.GetOrigin().Code.ToLower() == code.ToLower())
                .GetDescription();
        }

        /// <summary>
        /// 根据库房名称获取ID
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string GetGoldStoreID(string name)
        {
            string result = string.Empty;

            using (var view = new GoldStoresRoll())
            {
                result = view.FirstOrDefault(item => item.Name == name && item.Status == GeneralStatus.Normal)?.ID;
            }

            return result;
        }

        private static string GetEnterpriseID(string name)
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            using (var view = new EnterprisesRoll())
            {
                var model = view.FirstOrDefault(item => item.Name == name);
                if (model != null && !string.IsNullOrWhiteSpace(model.ID))
                {
                    result = model.ID;
                }
                else
                {
                    model = new Enterprise()
                    {
                        Name = name.Trim(),
                        Type = CompanyName.Contains(name.Trim()) ? EnterpriseAccountType.Company : (EnterpriseAccountType.Client | EnterpriseAccountType.Supplier),
                        CreatorID = Npc.Robot.Obtain(),
                        ModifierID = Npc.Robot.Obtain(),
                    };
                    model.Enter();
                    result = model.ID;
                }
            }

            return result;
        }

        /// <summary>
        /// 增加账户类型
        /// </summary>
        /// <param name="accountId">账户ID</param>
        /// <param name="accountsType">类型</param>
        private static void InsertAccountType(string accountId, string accountsType)
        {
            using (var accountTypeView = new AccountTypesRoll())
            using (var maps = new MapsAccountTypeRoll())
            {
                if (!string.IsNullOrWhiteSpace(accountsType))
                {
                    var list = new List<MapsAccountType>();
                    var accountTypes = accountTypeView.Where(item => item.Status == GeneralStatus.Normal).ToArray();
                    var array = MapXdt.MapAccountTypeFromXdt(accountsType);
                    if (array.Length <= 0) return;
                    foreach (var type in array)
                    {
                        list.Add(new MapsAccountType()
                        {
                            AccountID = accountId,
                            AccountTypeID = accountTypes.FirstOrDefault(item => item.Name == type)?.ID
                        });
                    }

                    maps.BatchEnter(accountId, list.ToArray());
                }
            }
        }
        #endregion

        public class AccountInputDto
        {
            /// <summary>
            /// 金库名称
            /// </summary>
            public string VaultName { get; set; }
            /// <summary>
            /// 账户名称
            /// </summary>
            public string AccountName { get; set; }
            /// <summary>
            /// 账号
            /// </summary>
            public string BankAccount { get; set; }
            /// <summary>
            /// 银行名称
            /// </summary>
            public string BankName { get; set; }
            /// <summary>
            /// 银行地址
            /// </summary>
            public string BankAddress { get; set; }
            /// <summary>
            /// 银行代码
            /// </summary>
            public string SwiftCode { get; set; }
            /// <summary>
            /// 币制（CNY）
            /// </summary>
            public string Currency { get; set; }
            /// <summary>
            /// 备注
            /// </summary>
            public string Summary { get; set; }
            /// <summary>
            /// 公司名称
            /// </summary>
            public string CompanyName { get; set; }
            /// <summary>
            /// 账号类型
            /// </summary>
            public string AccountType { get; set; }
            /// <summary>
            /// 账号管理人
            /// </summary>
            public string Owner { get; set; }
            /// <summary>
            /// 创建人ID
            /// </summary>
            public string CreatorID { get; set; }

            /// <summary>
            /// 地区（CHN）
            /// </summary>
            public string Region { get; set; }

            /// <summary>
            /// 余额
            /// </summary>
            public decimal? Balance { get; set; }

            /// <summary>
            /// 账户来源 1标准 2简易
            /// </summary>
            public int AccountSource { get; set; }

            public NatureType NatureType { get; set; }
        }
    }
}