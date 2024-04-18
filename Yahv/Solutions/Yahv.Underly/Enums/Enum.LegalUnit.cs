using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 海关计量单位
    /// </summary>
    public enum LegalUnit
    {
        [Description("台")]
        [Unit("001", "台")]
        台 = 1,
        [Description("座")]
        [Unit("002", "座")]
        座 = 2,
        [Description("辆")]
        [Unit("003", "辆")]
        辆 = 3,
        [Description("艘")]
        [Unit("004", "艘")]
        艘 = 4,
        [Description("架")]
        [Unit("005", "架")]
        架 = 5,
        [Description("套")]
        [Unit("006", "套")]
        套 = 6,
        [Description("个")]
        [Unit("007", "个")]
        个 = 7,
        [Description("只")]
        [Unit("008", "只")]
        只 = 8,
        [Description("头")]
        [Unit("009", "头")]
        头 = 9,
        [Description("张")]
        [Unit("010", "张")]
        张 = 10,

        [Description("件")]
        [Unit("011", "件")]
        件 = 11,
        [Description("支")]
        [Unit("012", "支")]
        支 = 12,
        [Description("枝")]
        [Unit("013", "枝")]
        枝 = 13,
        [Description("根")]
        [Unit("014", "根")]
        根 = 14,
        [Description("条")]
        [Unit("015", "条")]
        条 = 15,
        [Description("把")]
        [Unit("016", "把")]
        把 = 16,
        [Description("块")]
        [Unit("017", "块")]
        块 = 17,
        [Description("卷")]
        [Unit("018", "卷")]
        卷 = 18,
        [Description("副")]
        [Unit("019", "副")]
        副 = 19,
        [Description("片")]
        [Unit("020", "片")]
        片 = 20,

        [Description("组")]
        [Unit("021", "组")]
        组 = 21,
        [Description("份")]
        [Unit("022", "份")]
        份 = 22,
        [Description("幅")]
        [Unit("023", "幅")]
        幅 = 23,
        [Description("双")]
        [Unit("025", "双")]
        双 = 25,
        [Description("对")]
        [Unit("026", "对")]
        对 = 26,
        [Description("棵")]
        [Unit("027", "棵")]
        棵 = 27,
        [Description("株")]
        [Unit("028", "株")]
        株 = 28,
        [Description("井")]
        [Unit("029", "井")]
        井 = 29,
        [Description("米")]
        [Unit("030", "米")]
        米 = 30,

        [Description("盘")]
        [Unit("031", "盘")]
        盘 = 31,
        [Description("平方米")]
        [Unit("032", "平方米")]
        平方米 = 32,
        [Description("立方米")]
        [Unit("033", "立方米")]
        立方米 = 33,
        [Description("筒")]
        [Unit("034", "筒")]
        筒 = 34,
        [Description("千克")]
        [Unit("035", "千克")]
        千克 = 35,
        [Description("克")]
        [Unit("036", "克")]
        克 = 36,
        [Description("盆")]
        [Unit("037", "盆")]
        盆 = 37,
        [Description("万个")]
        [Unit("038", "万个")]
        万个 = 38,
        [Description("具")]
        [Unit("039", "具")]
        具 = 39,
        [Description("百副")]
        [Unit("040", "百副")]
        百副 = 40,

        [Description("百支")]
        [Unit("041", "百支")]
        百支 = 41,
        [Description("百把")]
        [Unit("042", "百把")]
        百把 = 42,
        [Description("百个")]
        [Unit("043", "百个")]
        百个 = 43,
        [Description("百片")]
        [Unit("044", "百片")]
        百片 = 44,
        [Description("刀")]
        [Unit("045", "刀")]
        刀 = 45,
        [Description("疋")]
        [Unit("046", "疋")]
        疋 = 46,
        [Description("公担")]
        [Unit("047", "公担")]
        公担 = 47,
        [Description("扇")]
        [Unit("048", "扇")]
        扇 = 48,
        [Description("百枝")]
        [Unit("049", "百枝")]
        百枝 = 49,
        [Description("千只")]
        [Unit("050", "千只")]
        千只 = 50,

        [Description("千块")]
        [Unit("051", "千块")]
        千块 = 51,
        [Description("千盒")]
        [Unit("052", "千盒")]
        千盒 = 52,
        [Description("千枝")]
        [Unit("053", "千枝")]
        千枝 = 53,
        [Description("千个")]
        [Unit("054", "千个")]
        千个 = 54,
        [Description("亿支")]
        [Unit("055", "亿支")]
        亿支 = 55,
        [Description("亿个")]
        [Unit("056", "亿个")]
        亿个 = 56,
        [Description("万套")]
        [Unit("057", "万套")]
        万套 = 57,
        [Description("千张")]
        [Unit("058", "千张")]
        千张 = 58,
        [Description("万张")]
        [Unit("059", "万张")]
        万张 = 59,
        [Description("千伏安")]
        [Unit("060", "千伏安")]
        千伏安 = 60,

        [Description("千瓦")]
        [Unit("061", "千瓦")]
        千瓦 = 61,
        [Description("千瓦时")]
        [Unit("062", "千瓦时")]
        千瓦时 = 62,
        [Description("千升")]
        [Unit("063", "千升")]
        千升 = 63,
        [Description("英尺")]
        [Unit("067", "英尺")]
        英尺 = 67,
        [Description("吨")]
        [Unit("070", "吨")]
        吨 = 70,

        [Description("长吨")]
        [Unit("071", "长吨")]
        长吨 = 71,
        [Description("短吨")]
        [Unit("072", "短吨")]
        短吨 = 72,
        [Description("司马担")]
        [Unit("073", "司马担")]
        司马担 = 73,
        [Description("司马斤")]
        [Unit("074", "司马斤")]
        司马斤 = 74,
        [Description("斤")]
        [Unit("075", "斤")]
        斤 = 75,
        [Description("磅")]
        [Unit("076", "磅")]
        磅 = 76,
        [Description("担")]
        [Unit("077", "担")]
        担 = 77,
        [Description("英担")]
        [Unit("078", "英担")]
        英担 = 78,
        [Description("短担")]
        [Unit("079", "短担")]
        短担 = 79,
        [Description("两")]
        [Unit("080", "两")]
        两 = 80,

        [Description("市担")]
        [Unit("081", "市担")]
        市担 = 81,
        [Description("盎司")]
        [Unit("083", "盎司")]
        盎司 = 83,
        [Description("克拉")]
        [Unit("084", "克拉")]
        克拉 = 84,
        [Description("市尺")]
        [Unit("085", "市尺")]
        市尺 = 85,
        [Description("码")]
        [Unit("086", "码")]
        码 = 86,
        [Description("英寸")]
        [Unit("088", "英寸")]
        英寸 = 88,
        [Description("寸")]
        [Unit("089", "寸")]
        寸 = 89,

        [Description("升")]
        [Unit("095", "升")]
        升 = 95,
        [Description("毫升")]
        [Unit("096", "毫升")]
        毫升 = 96,
        [Description("英加仑")]
        [Unit("097", "英加仑")]
        英加仑 = 97,
        [Description("美加仑")]
        [Unit("098", "美加仑")]
        美加仑 = 98,
        [Description("立方英尺")]
        [Unit("099", "立方英尺")]
        立方英尺 = 99,

        [Description("立方尺")]
        [Unit("101", "立方尺")]
        立方尺 = 101,
        [Description("平方码")]
        [Unit("110", "平方码")]
        平方码 = 110,

        [Description("平方英尺")]
        [Unit("111", "平方英尺")]
        平方英尺 = 111,
        [Description("平方尺")]
        [Unit("112", "平方尺")]
        平方尺 = 112,
        [Description("英制马力")]
        [Unit("115", "英制马力")]
        英制马力 = 115,
        [Description("公制马力")]
        [Unit("116", "公制马力")]
        公制马力 = 116,
        [Description("令")]
        [Unit("118", "令")]
        令 = 118,
        [Description("箱")]
        [Unit("120", "箱")]
        箱 = 120,

        [Description("批")]
        [Unit("121", "批")]
        批 = 121,
        [Description("罐")]
        [Unit("122", "罐")]
        罐 = 122,
        [Description("桶")]
        [Unit("123", "桶")]
        桶 = 123,
        [Description("扎")]
        [Unit("124", "扎")]
        扎 = 124,
        [Description("包")]
        [Unit("125", "包")]
        包 = 125,
        [Description("箩")]
        [Unit("126", "箩")]
        箩 = 126,
        [Description("打")]
        [Unit("127", "打")]
        打 = 127,
        [Description("筐")]
        [Unit("128", "筐")]
        筐 = 128,
        [Description("罗")]
        [Unit("129", "罗")]
        罗 = 129,
        [Description("匹")]
        [Unit("130", "匹")]
        匹 = 130,

        [Description("册")]
        [Unit("131", "册")]
        册 = 131,
        [Description("本")]
        [Unit("132", "本")]
        本 = 132,
        [Description("发")]
        [Unit("133", "发")]
        发 = 133,
        [Description("枚")]
        [Unit("134", "枚")]
        枚 = 134,
        [Description("捆")]
        [Unit("135", "捆")]
        捆 = 135,
        [Description("袋")]
        [Unit("136", "袋")]
        袋 = 136,
        [Description("粒")]
        [Unit("139", "粒")]
        粒 = 139,
        [Description("盒")]
        [Unit("140", "盒")]
        盒 = 140,

        [Description("合")]
        [Unit("141", "合")]
        合 = 141,
        [Description("瓶")]
        [Unit("142", "瓶")]
        瓶 = 142,
        [Description("千支")]
        [Unit("143", "千支")]
        千支 = 143,
        [Description("万双")]
        [Unit("144", "万双")]
        万双 = 144,
        [Description("万粒")]
        [Unit("145", "万粒")]
        万粒 = 145,
        [Description("千粒")]
        [Unit("146", "千粒")]
        千粒 = 146,
        [Description("千米")]
        [Unit("147", "千米")]
        千米 = 147,
        [Description("千英尺")]
        [Unit("148", "千英尺")]
        千英尺 = 148,
        [Description("百万贝可")]
        [Unit("149", "百万贝可")]
        百万贝可 = 149,

        [Description("部")]
        [Unit("163", "部")]
        部 = 163,
        [Description("亿株")]
        [Unit("164", "亿株")]
        亿株 = 164,
    }
}
