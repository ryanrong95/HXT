using Yahv.Underly.Attributes;

namespace Yahv.Erm.Services
{
    /// <summary>
    /// 政治面貌
    /// </summary>
    public enum PoliticType
    {
        [Description("群众")]
        Masses = 1,

        [Description("党员")]
        PartyMember = 2,

        [Description("团员")]
        LeagueMember = 3,

        [Description("其它")]
        Others = 99,
    }

    /// <summary>
    /// 中华名族类型
    /// </summary>
    public enum ChineseNationType
    {
        //汉族、蒙古族、回族、藏族、维吾尔族、苗族、彝族、壮族、布依族、朝鲜族、满族、侗族、瑶族、
        //白族、土家族、哈尼族、哈萨克族、傣族、黎族、僳僳族、佤族、畲族、高山族、拉祜族、水族、
        //东乡族、纳西族、景颇族、柯尔克孜族、土族、达斡尔族、仫佬族、羌族、布朗族、撒拉族、
        //毛南族、仡佬族、锡伯族、阿昌族、普米族、塔吉克族、怒族、乌孜别克族、俄罗斯族、鄂温克族、
        //德昂族、保安族、裕固族、京族、塔塔尔族、独龙族、鄂伦春族、赫哲族、门巴族、珞巴族、基诺族。

        [Description("汉族")]
        汉族 = 1,
        [Description("蒙古族")]
        蒙古族 = 2,
        [Description("回族")]
        回族 = 3,
        [Description("藏族")]
        藏族 = 4,
        [Description("维吾尔族")]
        维吾尔族 = 5,
        [Description("苗族")]
        苗族 = 6,
        [Description("彝族")]
        彝族 = 7,
        [Description("壮族")]
        壮族 = 8,
        [Description("布依族")]
        布依族 = 9,
        [Description("朝鲜族")]
        朝鲜族 = 10,

        [Description("满族")]
        满族 = 11,
        [Description("侗族")]
        侗族 = 12,
        [Description("瑶族")]
        瑶族 = 13,
        [Description("白族")]
        白族 = 14,
        [Description("土家族")]
        土家族 = 15,
        [Description("哈尼族")]
        哈尼族 = 16,
        [Description("哈萨克族")]
        哈萨克族 = 17,
        [Description("傣族")]
        傣族 = 18,
        [Description("黎族")]
        黎族 = 19,
        [Description("僳僳族")]
        僳僳族 = 20,

        [Description("佤族")]
        佤族 = 21,
        [Description("畲族")]
        畲族 = 22,
        [Description("高山族")]
        高山族 = 23,
        [Description("拉祜族")]
        拉祜族 = 24,
        [Description("水族")]
        水族 = 25,
        [Description("东乡族")]
        东乡族 = 26,
        [Description("纳西族")]
        纳西族 = 27,
        [Description("景颇族")]
        景颇族 = 28,
        [Description("柯尔克孜族")]
        柯尔克孜族 = 29,
        [Description("土族")]
        土族 = 30,

        [Description("达斡尔族")]
        达斡尔族 = 31,
        [Description("仫佬族")]
        仫佬族 = 32,
        [Description("羌族")]
        羌族 = 33,
        [Description("布朗族")]
        布朗族 = 34,
        [Description("撒拉族")]
        撒拉族 = 35,
        [Description("毛南族")]
        毛南族 = 36,
        [Description("仡佬族")]
        仡佬族 = 37,
        [Description("锡伯族")]
        锡伯族 = 38,
        [Description("阿昌族")]
        阿昌族 = 39,
        [Description("普米族")]
        普米族 = 40,

        [Description("塔吉克族")]
        塔吉克族 = 41,
        [Description("怒族")]
        怒族 = 42,
        [Description("乌孜别克族")]
        乌孜别克族 = 43,
        [Description("俄罗斯族")]
        俄罗斯族 = 44,
        [Description("鄂温克族")]
        鄂温克族 = 45,
        [Description("德昂族")]
        德昂族 = 46,
        [Description("保安族")]
        保安族 = 47,
        [Description("裕固族")]
        裕固族 = 48,
        [Description("京族")]
        京族 = 49,
        [Description("塔塔尔族")]
        塔塔尔族 = 50,

        [Description("独龙族")]
        独龙族 = 51,
        [Description("鄂伦春族")]
        鄂伦春族 = 52,
        [Description("赫哲族")]
        赫哲族 = 53,
        [Description("门巴族")]
        门巴族 = 54,
        [Description("珞巴族")]
        珞巴族 = 55,
        [Description("基诺族")]
        基诺族 = 56,
    }

    /// <summary>
    /// 性别
    /// </summary>
    public enum Gender
    {
        [Description("男")]
        Male = 1,
        [Description("女")]
        Female = 0
    }

    /// <summary>
    /// 健康状态
    /// </summary>
    public enum HealthyType
    {
        [Description("优")]
        Excellent = 1,

        [Description("良")]
        Good = 2,

        [Description("差")]
        Poor = 3,
    }

    /// <summary>
    /// 血型类型
    /// </summary>
    public enum BloodType
    {
        [Description("A型")]
        A = 1,
        [Description("B型")]
        B = 2,
        [Description("AB型")]
        AB = 3,
        [Description("O型")]
        O = 4,
        [Description("其它")]
        Others = 99,
    }

    /// <summary>
    /// 学历类型
    /// </summary>
    public enum EducationType
    {
        [Description("本科")]
        Bachelor = 1,

        [Description("专科")]
        College = 2,

        [Description("硕士研究生")]
        Master = 3,

        [Description("博士研究生")]
        Doctor = 4,

        //[Description("博士后")]
        //PostDoctor =5,

        [Description("其它")]
        Others = 99
    }

    /// <summary>
    /// 婚姻状况
    /// </summary>
    public enum MaritalStatus
    {
        [Description("未婚")]
        Unmarried = 0,

        [Description("已婚")]
        Married = 1,
    }

    /// <summary>
    /// 银行代码
    /// </summary>
    public enum Bank
    {
        [Description("北京银行")]
        BOB = 100,

        [Description("中国工商银行")]
        ICBC = 102,

        [Description("中国农业银行")]
        ABC = 103,

        [Description("农商银行")]
        RCC = 304,

        [Description("中国建设银行")]
        CCB = 105,

        [Description("上海银行")]
        BOS = 004,

        [Description("招商银行")]
        CMB = 308,

        [Description("中国民生银行")]
        CMBC = 309,

        [Description("交通银行")]
        BCM = 301,

        [Description("平安银行")]
        PABC = 302,

        [Description("中国银行")]
        BC = 310,

        [Description("兴业银行")]
        CIB = 311,
    }
}
