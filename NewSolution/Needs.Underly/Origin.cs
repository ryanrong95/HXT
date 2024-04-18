using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Underly.Attributes;
using Needs.Utils.Descriptions;

namespace Needs.Underly
{
    /// <summary>
    /// 原产地
    /// </summary>
    public enum Origin
    {
        [Description("乌干达")]
        [Origin("UGA", "乌干达", "Uganda")]
        UGA = 1,
        [Description("坦桑尼亚")]
        [Origin("TZA", "坦桑尼亚", "Tanzania, United Republic of")]
        TZA = 2,
        [Description("英属印度洋领地")]
        [Origin("IOT", "英属印度洋领地", "British Indian Ocean Territory ")]
        IOT = 3,
        [Description("开曼群岛")]
        [Origin("CYM", "开曼群岛", "Cayman Islands ")]
        CYM = 4,
        [Description("巴哈马")]
        [Origin("BHS", "巴哈马", "Bahamas")]
        BHS = 5,
        [Description("波兰")]
        [Origin("POL", "波兰", "Poland")]
        POL = 6,
        [Description("阿尔巴尼亚")]
        [Origin("ALB", "阿尔巴尼亚", "Albania")]
        ALB = 7,
        [Description("文莱")]
        [Origin("BRN", "文莱", "Brunei Darussalam")]
        BRN = 8,
        [Description("中国")]
        [Origin("CHN", "中国", "China")]
        CHN = 9,
        [Description("圣马丁(法国)")]
        [Origin("MAF", "圣马丁(法国)", "Saint Martin")]
        MAF = 10,

        [Description("韩国")]
        [Origin("KOR", "韩国", "Korea")]
        KOR = 11,
        [Description("匈牙利")]
        [Origin("HUN", "匈牙利", "Hungary")]
        HUN = 12,
        [Description("克罗地亚")]
        [Origin("HRV", "克罗地亚", "Croatia")]
        HRV = 13,
        [Description("福克兰群岛(马尔维纳斯)")]
        [Origin("FLK", "福克兰群岛(马尔维纳斯)", "Falkland Islands")]
        FLK = 14,
        [Description("马提尼克")]
        [Origin("MTQ", "马提尼克", "Martinique")]
        MTQ = 15,
        [Description("安圭拉")]
        [Origin("AIA", "安圭拉", "Anguilla")]
        AIA = 16,
        [Description("塞浦路斯")]
        [Origin("CYP", "塞浦路斯", "Cyprus")]
        CYP = 17,
        [Description("利比亚")]
        [Origin("LBY", "利比亚", "Libya")]
        LBY = 18,
        [Description("阿联酋")]
        [Origin("ARE", "阿联酋", "United Arab Emirates")]
        ARE = 19,
        [Description("马来西亚")]
        [Origin("MYS", "马来西亚", "Malaysia")]
        MYS = 20,

        [Description("安哥拉")]
        [Origin("AGO", "安哥拉", "Angola")]
        AGO = 21,
        [Description("比利时")]
        [Origin("BEL", "比利时", "Belgium")]
        BEL = 22,
        [Description("南极洲")]
        [Origin("ATA", "南极洲", "Antarctica")]
        ATA = 23,
        [Description("美国本土外小岛屿")]
        [Origin("UMI", "美国本土外小岛屿", "United States Minor Outlying Islands")]
        UMI = 24,
        [Description("玻利维亚")]
        [Origin("BOL", "玻利维亚", "Bolivia")]
        BOL = 25,
        [Description("波多黎各")]
        [Origin("PRI", "波多黎各", "Puerto Rico")]
        PRI = 26,
        [Description("国(地)别不详")]
        [Origin("ZZZ", "国(地)别不详", "Countries(reg.) unknow")]
        ZZZ = 27,
        [Description("圣赫勒拿")]
        [Origin("SHN", "圣赫勒拿", "Saint Helena, Ascension and Tristan da Cunha")]
        SHN = 28,
        [Description("塞舌尔")]
        [Origin("SYC", "塞舌尔", "Seychelles")]
        SYC = 29,
        [Description("特克斯和凯科斯群岛")]
        [Origin("TCA", "特克斯和凯科斯群岛", "Turks and Caicos Islands ")]
        TCA = 30,

        [Description("格陵兰")]
        [Origin("GRL", "格陵兰", "Greenland")]
        GRL = 31,
        [Description("加纳")]
        [Origin("GHA", "加纳", "Ghana")]
        GHA = 32,
        [Description("尼日尔")]
        [Origin("NER", "尼日尔", "Niger")]
        NER = 33,
        [Description("南乔治亚岛和南桑德韦奇岛")]
        [Origin("SGS", "南乔治亚岛和南桑德韦奇岛", "South Georgia and the South Sandwich Islands")]
        SGS = 34,
        [Description("帕劳")]
        [Origin("PLW", "帕劳", "Palau")]
        PLW = 35,
        [Description("斯洛伐克")]
        [Origin("SVK", "斯洛伐克", "Slovakia")]
        SVK = 36,
        [Description("毛里求斯")]
        [Origin("MUS", "毛里求斯", "Mauritius")]
        MUS = 37,
        [Description("奥地利")]
        [Origin("AUT", "奥地利", "Austria")]
        AUT = 38,
        [Description("白俄罗斯")]
        [Origin("BLR", "白俄罗斯", "Belarus")]
        BLR = 39,
        [Description("阿曼")]
        [Origin("OMN", "阿曼", "Oman")]
        OMN = 40,

        [Description("所罗门群岛")]
        [Origin("SLB", "所罗门群岛", "Solomon Islands")]
        SLB = 41,
        [Description("马克萨斯群岛")]
        [Origin("MAI", "马克萨斯群岛", "Marquesas Islands")]
        MAI = 42,
        [Description("赤道几内亚")]
        [Origin("GNQ", "赤道几内亚", "Equatorial Guinea")]
        GNQ = 43,
        [Description("美国")]
        [Origin("USA", "美国", "United States of America ")]
        USA = 44,
        [Description("马其顿")]
        [Origin("MKD", "马其顿", "Macedonia")]
        MKD = 45,
        [Description("塔吉克斯坦")]
        [Origin("TJK", "塔吉克斯坦", "Tajikistan")]
        TJK = 46,
        [Description("印度尼西亚")]
        [Origin("IDN", "印度尼西亚", "Indonesia")]
        IDN = 47,
        [Description("萨摩亚")]
        [Origin("WSM", "萨摩亚", "Samoa")]
        WSM = 48,
        [Description("罗马尼亚")]
        [Origin("ROU", "罗马尼亚", "Romania")]
        ROU = 49,
        [Description("摩洛哥")]
        [Origin("MAR", "摩洛哥", "Morocco")]
        MAR = 50,

        [Description("新喀里多尼亚")]
        [Origin("NCL", "新喀里多尼亚", "New Caledonia")]
        NCL = 51,
        [Description("摩纳哥")]
        [Origin("MCO", "摩纳哥", "Monaco")]
        MCO = 52,
        [Description("百慕大")]
        [Origin("BMU", "百慕大", "Bermuda")]
        BMU = 53,
        [Description("圭亚那")]
        [Origin("GUY", "圭亚那", "Guyana")]
        GUY = 54,
        [Description("孟加拉")]
        [Origin("BGD", "孟加拉", "Bangladesh")]
        BGD = 55,
        [Description("卡塔尔")]
        [Origin("QAT", "卡塔尔", "Qatar")]
        QAT = 56,
        [Description("东帝汶")]
        [Origin("TLS", "东帝汶", "Timor-Leste")]
        TLS = 57,
        [Description("波斯尼亚和黑塞哥维那")]
        [Origin("BIH", "波斯尼亚和黑塞哥维那", "Bosnia and Herzegovina")]
        BIH = 58,
        [Description("乌兹别克斯坦")]
        [Origin("UZB", "乌兹别克斯坦", "Uzbekistan")]
        UZB = 59,
        [Description("赞比亚")]
        [Origin("ZMB", "赞比亚", "Zambia")]
        ZMB = 60,

        [Description("美属维尔京群岛")]
        [Origin("VIR", "美属维尔京群岛", "Virgin Islands")]
        VIR = 61,
        [Description("南非及非洲其它国际地区")]
        [Origin("ZAF", "南非及非洲其它国际地区", "South Africa")]
        ZAF = 62,
        [Description("圣基茨和尼维斯")]
        [Origin("KNA", "圣基茨和尼维斯", "Saint Kitts and Nevis")]
        KNA = 63,
        [Description("塞拉利昂")]
        [Origin("SLE", "塞拉利昂", "Sierra Leone")]
        SLE = 64,
        [Description("加勒比海圣巴特岛")]
        [Origin("BLM", "加勒比海圣巴特岛", "Saint Barthélemy")]
        BLM = 65,
        [Description("墨西哥")]
        [Origin("MEX", "墨西哥", "Mexico")]
        MEX = 66,
        [Description("加拿大")]
        [Origin("CAN", "加拿大", "Canada")]
        CAN = 67,
        [Description("阿尔及利亚")]
        [Origin("DZA", "阿尔及利亚", "Algeria")]
        DZA = 68,
        [Description("瑞士")]
        [Origin("CHE", "瑞士", "Switzerland")]
        CHE = 69,
        [Description("拉丁美洲其他国家(地区)")]
        [Origin("ZSA", "拉丁美洲其他国家(地区)", "South America other")]
        ZSA = 70,

        [Description("汤加")]
        [Origin("TON", "汤加", "Tonga")]
        TON = 71,
        [Description("巴拿马")]
        [Origin("PAN", "巴拿马", "Panama")]
        PAN = 72,
        [Description("柬埔寨")]
        [Origin("KHM", "柬埔寨", "Cambodia")]
        KHM = 73,
        [Description("社会群岛")]
        [Origin("SOC", "社会群岛", "Society Islands")]
        SOC = 74,
        [Description("缅甸")]
        [Origin("MMR", "缅甸", "Myanmar")]
        MMR = 75,
        [Description("亚美尼亚")]
        [Origin("ARM", "亚美尼亚", "Armenia")]
        ARM = 76,
        [Description("圣诞岛")]
        [Origin("CXR", "圣诞岛", "Christmas Island")]
        CXR = 77,
        [Description("图瓦卢")]
        [Origin("TUV", "图瓦卢", "Tuvalu")]
        TUV = 78,
        [Description("澳大利亚")]
        [Origin("AUS", "澳大利亚", "Australia")]
        AUS = 79,
        [Description("梅利利亚")]
        [Origin("MEL", "梅利利亚", "Melilla")]
        MEL = 80,

        [Description("喀麦隆")]
        [Origin("CMR", "喀麦隆", "Cameroon")]
        CMR = 81,
        [Description("休达")]
        [Origin("CEU", "休达", "Ceuta")]
        CEU = 82,
        [Description("突尼斯")]
        [Origin("TUN", "突尼斯", "Tunisia")]
        TUN = 83,
        [Description("泽西岛")]
        [Origin("JEY", "泽西岛", "Jersey")]
        JEY = 84,
        [Description("尼泊尔")]
        [Origin("NPL", "尼泊尔", "Nepal")]
        NPL = 85,
        [Description("危地马拉")]
        [Origin("GTM", "危地马拉", "Guatemala")]
        GTM = 86,
        [Description("土布艾群岛")]
        [Origin("TUB", "土布艾群岛", "Tubai Islands")]
        TUB = 87,
        [Description("也门")]
        [Origin("YEM", "也门", "Yemen")]
        YEM = 88,
        [Description("联合国及机构和国际组织")]
        [Origin("ZUN", "联合国及机构和国际组织", "UN and oth.int'l org")]
        ZUN = 89,
        [Description("留尼汪")]
        [Origin("REU", "留尼汪", "Réunion")]
        REU = 90,

        [Description("英属维尔京群岛")]
        [Origin("VGB", "英属维尔京群岛", "Virgin Islands")]
        VGB = 91,
        [Description("阿鲁巴")]
        [Origin("ABW", "阿鲁巴", "Aruba")]
        ABW = 92,
        [Description("刚果（布）")]
        [Origin("COG", "刚果（布）", "Congo")]
        COG = 93,
        [Description("几内亚")]
        [Origin("GIN", "几内亚", "Guinea")]
        GIN = 94,
        [Description("吉布提")]
        [Origin("DJI", "吉布提", "Djibouti")]
        DJI = 95,
        [Description("库克群岛")]
        [Origin("COK", "库克群岛", "Cook Islands")]
        COK = 96,
        [Description("葡萄牙")]
        [Origin("PRT", "葡萄牙", "Portugal")]
        PRT = 97,
        [Description("根西岛")]
        [Origin("GGY", "根西岛", "Guernsey")]
        GGY = 98,
        [Description("冈比亚")]
        [Origin("GMB", "冈比亚", "Gambia")]
        GMB = 99,
        [Description("沙特阿拉伯")]
        [Origin("SAU", "沙特阿拉伯", "Saudi Arabia")]
        SAU = 100,

        [Description("巴勒斯坦")]
        [Origin("PSE", "巴勒斯坦", "Palestine, State of")]
        PSE = 101,
        [Description("蒙古")]
        [Origin("MNG", "蒙古", "Mongolia")]
        MNG = 102,
        [Description("博茨瓦纳")]
        [Origin("BWA", "博茨瓦纳", "Botswana")]
        BWA = 103,
        [Description("海地")]
        [Origin("HTI", "海地", "Haiti")]
        HTI = 104,
        [Description("哈萨克斯坦")]
        [Origin("KAZ", "哈萨克斯坦", "Kazakhstan")]
        KAZ = 105,
        [Description("新加坡")]
        [Origin("SGP", "新加坡", "Singapore")]
        SGP = 106,
        [Description("巴布亚新几内亚")]
        [Origin("PNG", "巴布亚新几内亚", "Papua New Guinea")]
        PNG = 107,
        [Description("巴巴多斯")]
        [Origin("BRB", "巴巴多斯", "Barbados")]
        BRB = 108,
        [Description("法属南部领地")]
        [Origin("ATF", "法属南部领地", "French Southern Territories")]
        ATF = 109,
        [Description("格鲁吉亚")]
        [Origin("GEO", "格鲁吉亚", "Georgia")]
        GEO = 110,

        [Description("多米尼克")]
        [Origin("DMA", "多米尼克", "Dominica")]
        DMA = 111,
        [Description("布基纳法索")]
        [Origin("BFA", "布基纳法索", "Burkina Faso")]
        BFA = 112,
        [Description("俄罗斯联邦")]
        [Origin("RUS", "俄罗斯联邦", "Russian Federation")]
        RUS = 113,
        [Description("荷属圣马丁岛")]
        [Origin("SXM", "荷属圣马丁岛", "Sint Maarten")]
        SXM = 114,
        [Description("古巴")]
        [Origin("CUB", "古巴", "Cuba")]
        CUB = 115,
        [Description("乌拉圭")]
        [Origin("URY", "乌拉圭", "Uruguay")]
        URY = 116,
        [Description("叙利亚")]
        [Origin("SYR", "叙利亚", "Syrian Arab Republic")]
        SYR = 117,
        [Description("西班牙")]
        [Origin("ESP", "西班牙", "Spain")]
        ESP = 118,
        [Description("托克劳")]
        [Origin("TKL", "托克劳", "Tokelau")]
        TKL = 119,
        [Description("挪威")]
        [Origin("NOR", "挪威", "Norway")]
        NOR = 120,

        [Description("中非")]
        [Origin("CAF", "中非", "Central African Republic")]
        CAF = 121,
        [Description("埃及")]
        [Origin("EGY", "埃及", "Egypt")]
        EGY = 122,
        [Description("澳门")]
        [Origin("MAC", "澳门", "Macao")]
        MAC = 123,
        [Description("土耳其")]
        [Origin("TUR", "土耳其", "Turkey")]
        TUR = 124,
        [Description("黑山共和国")]
        [Origin("MNE", "黑山共和国", "Montenegro")]
        MNE = 125,
        [Description("英国")]
        [Origin("GBR", "英国", "United Kingdom of Great Britain and Northern Ireland")]
        GBR = 126,
        [Description("梵蒂冈")]
        [Origin("VAT", "梵蒂冈", "Holy See")]
        VAT = 127,
        [Description("斯里兰卡")]
        [Origin("LKA", "斯里兰卡", "Sri Lanka")]
        LKA = 128,
        [Description("埃塞俄比亚")]
        [Origin("ETH", "埃塞俄比亚", "Ethiopia")]
        ETH = 129,
        [Description("圣卢西亚")]
        [Origin("LCA", "圣卢西亚", "Saint Lucia")]
        LCA = 130,

        [Description("塞内加尔")]
        [Origin("SEN", "塞内加尔", "Senegal")]
        SEN = 131,
        [Description("蒙特塞拉特")]
        [Origin("MSR", "蒙特塞拉特", "Montserrat")]
        MSR = 132,
        [Description("特立尼达和多巴哥")]
        [Origin("TTO", "特立尼达和多巴哥", "Trinidad and Tobago")]
        TTO = 133,
        [Description("北美洲其他国家(地区)")]
        [Origin("ZNA", "北美洲其他国家(地区)", "North America other")]
        ZNA = 134,
        [Description("马恩岛")]
        [Origin("IMN", "马恩岛", "Isle of Man")]
        IMN = 135,
        [Description("拉脱维亚")]
        [Origin("LVA", "拉脱维亚", "Latvia")]
        LVA = 136,
        [Description("诺福克岛")]
        [Origin("NFK", "诺福克岛", "Norfolk Island")]
        NFK = 137,
        [Description("捷克")]
        [Origin("CZE", "捷克", "Czechia")]
        CZE = 138,
        [Description("苏里南")]
        [Origin("SUR", "苏里南", "Suriname")]
        SUR = 139,
        [Description("加蓬")]
        [Origin("GAB", "加蓬", "Gabon")]
        GAB = 140,

        [Description("南苏丹")]
        [Origin("SSD", "南苏丹", "South Sudan")]
        SSD = 141,
        [Description("伯利兹")]
        [Origin("BLZ", "伯利兹", "Belize")]
        BLZ = 142,
        [Description("刚果（金）")]
        [Origin("COD", "刚果（金）", "Congo (the Democratic Republic of the)")]
        COD = 143,
        [Description("伊朗")]
        [Origin("IRN", "伊朗", "Iran")]
        IRN = 144,
        [Description("德国")]
        [Origin("DEU", "德国", "Germany")]
        DEU = 145,
        [Description("朝鲜")]
        [Origin("PRK", "朝鲜", "Korea (the Democratic People's Republic of)")]
        PRK = 146,
        [Description("西撒哈拉")]
        [Origin("ESH", "西撒哈拉", "Western Sahara*")]
        ESH = 147,
        [Description("厄瓜多尔")]
        [Origin("ECU", "厄瓜多尔", "Ecuador")]
        ECU = 148,
        [Description("爱沙尼亚")]
        [Origin("EST", "爱沙尼亚", "Estonia")]
        EST = 149,
        [Description("马耳他")]
        [Origin("MLT", "马耳他", "Malta")]
        MLT = 150,

        [Description("瓜德罗普")]
        [Origin("GLP", "瓜德罗普", "Guadeloupe")]
        GLP = 151,
        [Description("北马里亚纳自由联邦")]
        [Origin("MNP", "北马里亚纳自由联邦", "Northern Mariana Islands")]
        MNP = 152,
        [Description("安提瓜和巴布达")]
        [Origin("ATG", "安提瓜和巴布达", "Antigua and Barbuda")]
        ATG = 153,
        [Description("列支敦士登")]
        [Origin("LIE", "列支敦士登", "Liechtenstein")]
        LIE = 154,
        [Description("土阿莫土群岛")]
        [Origin("TUA", "土阿莫土群岛", "Tuamotu Islands")]
        TUA = 155,
        [Description("吉尔吉斯斯坦")]
        [Origin("KGZ", "吉尔吉斯斯坦", "Kyrgyzstan")]
        KGZ = 156,
        [Description("亚洲其他国家(地区)")]
        [Origin("ZAS", "亚洲其他国家(地区)", "Asia other")]
        ZAS = 157,
        [Description("索马里")]
        [Origin("SOM", "索马里", "Somalia")]
        SOM = 158,
        [Description("伊拉克")]
        [Origin("IRQ", "伊拉克", "Iraq")]
        IRQ = 159,
        [Description("以色列")]
        [Origin("ISR", "以色列", "Israel")]
        ISR = 160,

        [Description("欧洲其他国家(地区)")]
        [Origin("ZEU", "欧洲其他国家(地区)", "Europe other")]
        ZEU = 161,
        [Description("塞尔维亚共和国")]
        [Origin("SRB", "塞尔维亚共和国", "Serbia")]
        SRB = 162,
        [Description("意大利")]
        [Origin("ITA", "意大利", "Italy")]
        ITA = 163,
        [Description("法属圭亚那")]
        [Origin("GUF", "法属圭亚那", "French Guiana")]
        GUF = 164,
        [Description("秘鲁")]
        [Origin("PER", "秘鲁", "Peru")]
        PER = 165,
        [Description("智利")]
        [Origin("CHL", "智利", "Chile")]
        CHL = 166,
        [Description("巴拉圭")]
        [Origin("PRY", "巴拉圭", "Paraguay")]
        PRY = 167,
        [Description("圣文森特和格林纳丁斯")]
        [Origin("VCT", "圣文森特和格林纳丁斯", "Saint Vincent and the Grenadines")]
        VCT = 168,
        [Description("马约特")]
        [Origin("MYT", "马约特", "Mayotte")]
        MYT = 169,
        [Description("斐济")]
        [Origin("FJI", "斐济", "Fiji")]
        FJI = 170,

        [Description("关岛")]
        [Origin("GUM", "关岛", "Guam")]
        GUM = 171,
        [Description("爱尔兰")]
        [Origin("IRL", "爱尔兰", "Ireland")]
        IRL = 172,
        //[Description("非洲其他国家(地区)")]
        //[Origin("ZAF", "非洲其他国家(地区)", "Africa other")]
        //ZAF = 170,
        [Description("法罗群岛")]
        [Origin("FRO", "法罗群岛", "Faroe Islands")]
        FRO = 174,
        [Description("中国台湾")]
        [Origin("TWN", "中国台湾", "Taiwan")]
        TWN = 175,
        [Description("丹麦")]
        [Origin("DNK", "丹麦", "Denmark")]
        DNK = 176,
        [Description("纽埃")]
        [Origin("NIU", "纽埃", "Niue")]
        NIU = 177,
        [Description("尼加拉瓜")]
        [Origin("NIC", "尼加拉瓜", "Nicaragua")]
        NIC = 178,
        [Description("安道尔")]
        [Origin("AND", "安道尔", "Andorra")]
        AND = 179,
        [Description("牙买加")]
        [Origin("JAM", "牙买加", "Jamaica")]
        JAM = 180,

        [Description("斯洛文尼亚")]
        [Origin("SVN", "斯洛文尼亚", "Slovenia")]
        SVN = 181,
        [Description("马绍尔群岛")]
        [Origin("MHL", "马绍尔群岛", "Marshall Islands")]
        MHL = 182,
        [Description("圣多美和普林西比")]
        [Origin("STP", "圣多美和普林西比", "Sao Tome and Principe")]
        STP = 183,
        [Description("黎巴嫩")]
        [Origin("LBN", "黎巴嫩", "Lebanon")]
        LBN = 184,
        [Description("乍得")]
        [Origin("TCD", "乍得", "Chad")]
        TCD = 185,
        [Description("毛里塔尼亚")]
        [Origin("MRT", "毛里塔尼亚", "Mauritania")]
        MRT = 186,
        [Description("肯尼亚")]
        [Origin("KEN", "肯尼亚", "Kenya")]
        KEN = 187,
        [Description("立陶宛")]
        [Origin("LTU", "立陶宛", "Lithuania")]
        LTU = 188,
        [Description("圣马力诺")]
        [Origin("SMR", "圣马力诺", "San Marino")]
        SMR = 189,
        [Description("圣皮埃尔和密克隆")]
        [Origin("SPM", "圣皮埃尔和密克隆", "Saint Pierre and Miquelon")]
        SPM = 190,

        [Description("大洋洲其他国家(地区)")]
        [Origin("ZOC", "大洋洲其他国家(地区)", "Oceania other")]
        ZOC = 191,
        [Description("瓦努阿图")]
        [Origin("VUT", "瓦努阿图", "Vanuatu")]
        VUT = 192,
        [Description("莱索托")]
        [Origin("LSO", "莱索托", "Lesotho")]
        LSO = 193,
        [Description("马尔代夫")]
        [Origin("MDV", "马尔代夫", "Maldives")]
        MDV = 194,
        [Description("瑞典")]
        [Origin("SWE", "瑞典", "Sweden")]
        SWE = 195,
        [Description("巴基斯坦")]
        [Origin("PAK", "巴基斯坦", "Pakistan")]
        PAK = 196,
        [Description("格林纳达")]
        [Origin("GRD", "格林纳达", "Grenada")]
        GRD = 197,
        [Description("阿兰群岛（波罗的海中芬兰所属群岛）")]
        [Origin("ALA", "阿兰群岛（波罗的海中芬兰所属群岛）", "Åland Islands")]
        ALA = 198,
        [Description("基里巴斯")]
        [Origin("KIR", "基里巴斯", "Kiribati")]
        KIR = 199,

        [Description("尼日利亚")]
        [Origin("NGA", "尼日利亚", "Nigeria")]
        NGA = 201,
        [Description("厄立特里亚")]
        [Origin("ERI", "厄立特里亚", "Eritrea")]
        ERI = 202,
        [Description("科科斯（基林）群岛")]
        [Origin("CCK", "巴哈马", "Cocos (Keeling) Islands (the)")]
        CCK = 203,
        [Description("菲律宾")]
        [Origin("PHL", "菲律宾", "Philippines (the)")]
        PHL = 204,
        [Description("哥伦比亚")]
        [Origin("COL", "哥伦比亚", "Colombia")]
        COL = 205,
        [Description("阿根廷")]
        [Origin("ARG", "阿根廷", "Argentina")]
        ARG = 206,
        [Description("法属波利尼西亚")]
        [Origin("PYF", "法属波利尼西亚", "French Polynesia")]
        PYF = 207,
        [Description("摩尔多瓦")]
        [Origin("MDA", "摩尔多瓦", "Bahamas")]
        MDA = 208,
        [Description("博内尔岛、圣尤斯特歇斯岛和萨巴岛")]
        [Origin("BES", "博内尔岛、圣尤斯特歇斯岛和萨巴岛", "Bonaire, Sint Eustatius and Saba")]
        BES = 209,
        [Description("阿塞拜疆")]
        [Origin("AZE", "阿塞拜疆", "Azerbaijan")]
        AZE = 210,

        [Description("保加利亚")]
        [Origin("BGR", "保加利亚", "Bulgaria")]
        BGR = 211,
        [Description("约旦")]
        [Origin("JOR", "约旦", "Jordan")]
        JOR = 212,
        [Description("萨尔瓦多")]
        [Origin("SLV", "萨尔瓦多", "El Salvador")]
        SLV = 213,
        [Description("法国")]
        [Origin("FRA", "法国", "France")]
        FRA = 214,
        [Description("布隆迪")]
        [Origin("BDI", "布隆迪", "Burundi")]
        BDI = 215,
        [Description("不丹")]
        [Origin("BTN", "不丹", "Bhutan")]
        BTN = 216,
        [Description("土库曼斯坦")]
        [Origin("TKM", "土库曼斯坦", "Turkmenistan")]
        TKM = 217,
        [Description("新西兰")]
        [Origin("NZL", "新西兰", "New Zealand")]
        NZL = 218,
        [Description("科威特")]
        [Origin("KWT", "科威特", "Kuwait")]
        KWT = 219,
        [Description("多米尼加")]
        [Origin("DOM", "多米尼加", "Dominican Republic (the)")]
        DOM = 220,

        [Description("直布罗陀")]
        [Origin("GIB", "直布罗陀", "Gibraltar")]
        GIB = 221,
        [Description("巴林")]
        [Origin("BHR", "巴林", "Bahrain")]
        BHR = 222,
        [Description("斯瓦尔巴岛和扬马延岛")]
        [Origin("SJM", "斯瓦尔巴岛和扬马延岛", "Svalbard and Jan Mayen")]
        SJM = 223,
        [Description("荷兰")]
        [Origin("NLD", "荷兰", "Netherlands (the)")]
        NLD = 224,
        [Description("皮特凯恩")]
        [Origin("PCN", "皮特凯恩", "Pitcairn")]
        PCN = 225,
        [Description("多哥")]
        [Origin("TGO", "多哥", "Togo")]
        TGO = 226,
        [Description("科摩罗")]
        [Origin("COM", "科摩罗", "Comoros (the)")]
        COM = 227,
        [Description("马拉维")]
        [Origin("MWI", "马拉维", "Malawi")]
        MWI = 228,
        [Description("盖比群岛")]
        [Origin("GAM", "盖比群岛", "Gambier Islands")]
        GAM = 229,
        [Description("泰国")]
        [Origin("THA", "泰国", "Thailand")]
        THA = 230,

        [Description("马达加斯加")]
        [Origin("MDG", "马达加斯加", "Madagascar")]
        MDG = 231,
        [Description("库腊索岛")]
        [Origin("CUW", "库腊索岛", "Curacao")]
        CUW = 232,
        [Description("贝宁")]
        [Origin("BEN", "贝宁", "Benin")]
        BEN = 233,
        [Description("卢森堡")]
        [Origin("LUX", "卢森堡", "Luxembourg")]
        LUX = 234,
        [Description("莫桑比克")]
        [Origin("MOZ", "莫桑比克", "Mozambique")]
        MOZ = 235,
        [Description("几内亚比绍")]
        [Origin("GNB", "几内亚比绍", "Guinea-Bissau")]
        GNB = 236,
        [Description("哥斯达黎加")]
        [Origin("CRI", "哥斯达黎加", "Costa Rica")]
        CRI = 237,
        [Description("洪都拉斯")]
        [Origin("HND", "洪都拉斯", "Honduras")]
        HND = 238,
        [Description("日本")]
        [Origin("JPN", "日本", "Japan")]
        JPN = 239,
        [Description("阿富汗")]
        [Origin("AFG", "阿富汗", "Afghanistan")]
        AFG = 240,

        [Description("冰岛")]
        [Origin("ISL", "冰岛", "Iceland")]
        ISL = 241,
        [Description("越南")]
        [Origin("VNM", "越南", "Viet Nam")]
        VNM = 242,
        [Description("美属萨摩亚")]
        [Origin("ASM", "美属萨摩亚", "American Samoa")]
        ASM = 243,
        [Description("希腊")]
        [Origin("GRC", "希腊", "Greece")]
        GRC = 244,
        [Description("印度")]
        [Origin("IND", "印度", "India")]
        IND = 245,
        [Description("阿姆斯特朗")]
        [Origin("lol", "阿姆斯特朗", "amusing")]
        lol = 246,
        [Description("乌克兰")]
        [Origin("UKR", "乌克兰", "Pitcairn")]
        UKR = 247,
        [Description("加那利群岛")]
        [Origin("CAI", "加那利群岛", "Canary Islands")]
        CAI = 248,
        [Description("纳米比亚")]
        [Origin("NAM", "纳米比亚", "Namibia")]
        NAM = 249,
        [Description("赫德岛和麦克唐纳岛")]
        [Origin("HMD", "赫德岛和麦克唐纳岛", "Heard Island and McDonald Islands")]
        HMD = 250,

        [Description("布维岛")]
        [Origin("BVT", "布维岛", "Bouvet Island")]
        BVT = 251,
        [Description("科特迪瓦")]
        [Origin("CIV", "科特迪瓦", "C?te d'Ivoire")]
        CIV = 252,
        [Description("老挝")]
        [Origin("LAO", "老挝", "Lao People's Democratic Republic (the)")]
        LAO = 253,
        [Description("委内瑞拉")]
        [Origin("VEN", "委内瑞拉", "Venezuela (Bolivarian Republic of)")]
        VEN = 254,
        [Description("利比里亚")]
        [Origin("LBR", "利比里亚", "Liberia")]
        LBR = 255,
        [Description("佛得角")]
        [Origin("CPV", "佛得角", "Cabo Verde")]
        CPV = 256,
        [Description("巴西")]
        [Origin("BRA", "巴西", "Brazil")]
        BRA = 257,
        [Description("卢旺达")]
        [Origin("RWA", "卢旺达", "Rwanda")]
        RWA = 258,
        [Description("马里")]
        [Origin("MLI", "马里", "Mali")]
        MLI = 259,
        //[Description("阿姆斯特朗2")]
        //[Origin("loli", "阿姆斯特朗2", "amusing1")]
        //loli = 260,

        [Description("苏丹")]
        [Origin("SDN", "苏丹", "Sudan (the)")]
        SDN = 261,
        [Description("瑙鲁")]
        [Origin("NRU", "瑙鲁", "Nauru")]
        NRU = 262,
        [Description("中国香港")]
        [Origin("HKG", "中国香港", "Hong Kong")]
        HKG = 263,
        [Description("津巴布韦")]
        [Origin("ZWE", "津巴布韦", "Zimbabwe")]
        ZWE = 264,
        [Description("密克罗尼西亚(联邦)")]
        [Origin("FSM", "密克罗尼西亚(联邦)", "Micronesia (Federated States of)")]
        FSM = 265,
        [Description("斯威士兰")]
        [Origin("SWZ", "斯威士兰", "Swaziland")]
        SWZ = 266,
        [Description("瓦利斯和富图纳")]
        [Origin("WLF", "瓦利斯和富图纳", "Wallis and Futuna")]
        WLF = 267,
        [Description("芬兰")]
        [Origin("FIN", "芬兰", "Finland")]
        FIN = 268,
        [Description("***")]
        [Origin("***", "***", "Unknown")]
        Unknown = 9999999,
    }
}
