using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Client
{
    #region 京东万象-失效

    public class BaseInfo
    {
        /// <summary>
        /// 开业
        /// </summary>
        public string regStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string estiblishTime { get; set; }
        /// <summary>
        /// 5000.000000万人民币
        /// </summary>
        public string regCapital { get; set; }
        /// <summary>
        /// 北京
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// 北京市工商行政管理局海淀分局
        /// </summary>
        public string regInstitute { get; set; }
        /// <summary>
        /// 技术开发、技术推广、技术转让、技术咨询、技术服务；计算机系统服务；网上销售软件及辅助设备、机械设备、电子产品、通讯设备；货物进出口；信息服务业务（仅限互联网信息服务业务）不含信息搜索查询业务、信息社区服务、信息即时交互服务和信息保护和加工处理服务（增值电信业务经营许可证有效期至2025年01月16日）。（市场主体依法自主选择经营项目，开展经营活动；依法须经批准的项目，经相关部门批准后依批准的内容开展经营活动；不得从事国家和本市产业政策禁止和限制类项目的经营活动。）
        /// </summary>
        public string businessScope { get; set; }
        /// <summary>
        /// 北京市海淀区彩和坊路10号1号楼506室
        /// </summary>
        public string regLocation { get; set; }
        /// <summary>
        /// 陈保民
        /// </summary>
        public string legalPersonName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string regNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string companyId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string phoneNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string creditCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fromTime { get; set; }
        /// <summary>
        /// 北京远大创新科技有限公司
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string approvedTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string percentileScore { get; set; }
        /// <summary>
        /// 有限责任公司(自然人投资或控股)
        /// </summary>
        public string companyOrgType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string orgNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string correctCompanyId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string toTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// 北京
        /// </summary>
        public string @base { get; set; }
    }

    public class InvestorListItem
    {
        /// <summary>
        /// 陈保民
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string capitalActl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// [{"amomon":"4875万元","time":"2022-12-31","paymet":"货币"}]
        /// </summary>
        public string capital { get; set; }
        /// <summary>
        /// 个人
        /// </summary>
        public string type { get; set; }
    }

    public class InvestListItem
    {
        /// <summary>
        /// 存续（在营、开业、在册）
        /// </summary>
        public string regStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string estiblishTime { get; set; }
        /// <summary>
        /// 300.000000万人民币
        /// </summary>
        public string regCapital { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string pencertileScore { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string percent { get; set; }
        /// <summary>
        /// 乔首全
        /// </summary>
        public string legalPersonName { get; set; }
        /// <summary>
        /// 电子科技领域内的技术开发、技术服务、技术咨询及技术转让；计算机维修；计算机软件及辅助设备、电子产品、机械设备、通讯设备的销售。【依法须经批准的项目，经相关部门批准后方可开展经营活动】
        /// </summary>
        public string business_scope { get; set; }
        /// <summary>
        /// 有限责任公司（非自然人投资或控股的法人独资）
        /// </summary>
        public string orgType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string legalPersonId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string creditCode { get; set; }
        /// <summary>
        /// 上海比亿电子技术有限公司
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 专业技术服务业
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string personType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string @base { get; set; }
    }

    public class StaffListItem
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> staffName { get; set; }
        /// <summary>
        /// 朱文利
        /// </summary>
        public string name { get; set; }
    }

    public class BranchListItem
    {
        /// <summary>
        /// 北京远大创新科技有限公司销售部
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
    }

    public class InfoResult
    {
        /// <summary>
        /// 
        /// </summary>
        public BaseInfo baseInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<InvestorListItem> investorList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<InvestListItem> investList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<StaffListItem> staffList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<BranchListItem> branchList { get; set; }
    }

    public class RootResult
    {
        /// <summary>
        /// 
        /// </summary>
        public int error_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string reason { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public InfoResult result { get; set; }
    }

    public class SearchByNameModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string charge { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string remain { get; set; }
        /// <summary>
        /// 查询成功
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public RootResult result { get; set; }
    }

    public class SearchCompanyByNameModel
    {
        public bool success { get; set; }

        public int code { get; set; }

        public string data { get; set; }
    }

    #endregion

    #region 天眼查-工商基础信息

    public class TianyanchaBase
    {
        public TianyanchaResult result { get; set; }

        public string reason { get; set; }

        public int error_code { get; set; }
    }

    public class TianyanchaResult
    {
        public string historyNames { get; set; }
        public string cancelDate { get; set; }
        public string regStatus { get; set; }
        public string regCapital { get; set; }
        public string city { get; set; }
        public string compForm { get; set; }
        public string staffNumRange { get; set; }
        public string bondNum { get; set; }
        public List<string> historyNameList { get; set; }
        public string industry { get; set; }
        public string bondName { get; set; }
        public string revokeDate { get; set; }
        public int type { get; set; }
        public long updateTimes { get; set; }
        public string legalPersonName { get; set; }
        public string revokeReason { get; set; }
        public string regNumber { get; set; }
        public string creditCode { get; set; }
        public string property3 { get; set; }
        public string usedBondName { get; set; }
        public long? approvedTime { get; set; }
        public long? fromTime { get; set; }
        public int? socialStaffNum { get; set; }
        public string actualCapitalCurrency { get; set; }
        public string alias { get; set; }
        public string companyOrgType { get; set; }
        public long id { get; set; }
        public string cancelReason { get; set; }
        public string orgNumber { get; set; }
        public string toTime { get; set; }
        public string actualCapital { get; set; }
        public long estiblishTime { get; set; }
        public string regInstitute { get; set; }
        public string businessScope { get; set; }
        public string taxNumber { get; set; }
        public string regLocation { get; set; }
        public string regCapitalCurrency { get; set; }
        public string tags { get; set; }
        public string district { get; set; }
        public string bondType { get; set; }
        public string name { get; set; }
        public long percentileScore { get; set; }
        public int isMicroEnt { get; set; }
        // bj
        // public string base { get; set; }

        //"industryAll":{"categoryMiddle":"组织管理服务","categoryBig":"商务服务业","category":"租赁和商务服务业","categorySmall":""},
        public TianyanchaIndustryResult industryAll { get; set; }
    }

    public class TianyanchaIndustryResult
    {
        public string categoryMiddle { get; set; }
        public string categoryBig { get; set; }
        public string category { get; set; }
        public string categorySmall { get; set; }
    }

    #endregion

    #region 天眼查-企业风险

    public class TycRisk
    {

        public RiskResult result { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string reason { get; set; }

        /// <summary>
        /// 状态码 0:请求成功
        /// </summary>
        public string error_code { get; set; }
    }


    public class RiskResult
    {
        /// <summary>
        /// 风险等级
        /// </summary>
        public string riskLevel { get; set; }
        /// <summary>
        /// 风险列表
        /// </summary>
        public List<RiskItem> riskList { get; set; }
    }


    public class RiskItem
    {
        //"count": 0,
        //    "name": "历史风险",
        //    "list": [],
        //    "type": 3

        /// <summary>
        /// 总数
        /// </summary>
        public int count { get; set; }

        /// <summary>
        /// RiskCategory.Description
        /// 风险分类（周边风险，预警提醒，自身风险，历史风险）
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 风险分类（周边风险，预警提醒，自身风险，历史风险）
        /// </summary>
        public RiskCategory type { get; set; }

        /// <summary>
        /// 风险明细 RiskDetail
        /// </summary>
        public List<RiskDetail> list { get; set; }
    }


    public class RiskDetail
    {
        /// <summary>
        /// 总数
        /// </summary>
        public int total { get; set; }

        /// <summary>
        /// 风险标签（高风险，警示，提示信息）
        /// </summary>
        public string tag { get; set; }

        /// <summary>
        /// 风险类型
        /// </summary>
        public RiskType type { get; set; }

        /// <summary>
        /// 标题（风险类型的名称）
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 风险详细列表cell
        /// </summary>
        public List<RiskCell> list { get; set; }
    }


    public class RiskCell
    {
        //"companyId": 2358160158,
        //"companyName": "上海比亿电子技术有限公司",
        //"id": 356481853,
        //"riskCount": 2,
        //"title": "该公司投资的上海比亿电子技术有限公司起诉他人或公司的立案信息",
        //"type": 27,
        //"desc": "立案信息"

        /// <summary>
        /// 公司id（风险为自身信息，该字段为空）
        /// </summary>
        public string companyId { get; set; }

        /// <summary>
        /// 公司名（风险为自身信息，该字段为空）
        /// </summary>
        public string companyName { get; set; }

        /// <summary>
        /// 企业风险详情信息id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public string riskCount { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 风险类型
        /// </summary>
        public RiskType type { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string desc { get; set; }
    }

    #region RiskCategory 风险分类

    public enum RiskCategory
    {

        /// <summary>
        /// 预警提醒
        /// </summary>
        [Description("预警提醒")]
        预警提醒 = 0,

        /// <summary>
        /// 自身风险
        /// </summary>
        [Description("自身风险")]
        自身风险 = 1,

        /// <summary>
        /// 周边风险
        /// </summary>
        [Description("周边风险")]
        周边风险 = 2,

        /// <summary>
        /// 历史风险
        /// </summary>
        [Description("历史风险")]
        历史风险 = 3

    }

    #endregion

    #region RiskType 风险类型

    public enum RiskType
    {

        /// <summary>
        /// 严重违法
        /// </summary>
        [Description("严重违法")]
        严重违法 = 1,

        /// <summary>
        /// 失信被执行人（公司）
        /// </summary>
        [Description("失信被执行人（公司）")]
        失信被执行人_公司 = 3,

        /// <summary>
        /// 被执行人（公司）
        /// </summary>
        [Description("被执行人（公司）")]
        被执行人_公司 = 5,

        /// <summary>
        /// 行政处罚
        /// </summary>
        [Description("行政处罚")]
        行政处罚 = 6,

        /// <summary>
        /// 经营异常
        /// </summary>
        [Description("经营异常")]
        经营异常 = 7,

        /// <summary>
        /// 法律诉讼
        /// </summary>
        [Description("法律诉讼")]
        法律诉讼 = 8,

        /// <summary>
        /// 股权出质(公司)
        /// </summary>
        [Description("股权出质(公司)")]
        股权出质_公司 = 9,

        /// <summary>
        /// 动产抵押
        /// </summary>
        [Description("动产抵押")]
        动产抵押 = 10,

        /// <summary>
        /// 欠税公告
        /// </summary>
        [Description("欠税公告")]
        欠税公告 = 11,

        /// <summary>
        /// 名称变更
        /// </summary>
        [Description("名称变更")]
        名称变更 = 12,

        /// <summary>
        /// 开庭公告
        /// </summary>
        [Description("开庭公告")]
        开庭公告 = 13,

        /// <summary>
        /// 法院公告
        /// </summary>
        [Description("法院公告")]
        法院公告 = 14,

        /// <summary>
        /// 法人变更
        /// </summary>
        [Description("法人变更")]
        法人变更 = 15,


        /// <summary>
        /// 投资人变更
        /// </summary>
        [Description("投资人变更")]
        投资人变更 = 16,

        /// <summary>
        /// 主要人员变更
        /// </summary>
        [Description("主要人员变更")]
        主要人员变更 = 17,

        /// <summary>
        /// 注册资本变更
        /// </summary>
        [Description("注册资本变更")]
        注册资本变更 = 18,

        /// <summary>
        /// 注册地址变更
        /// </summary>
        [Description("注册地址变更")]
        注册地址变更 = 19,

        /// <summary>
        /// 出资情况变更
        /// </summary>
        [Description("出资情况变更")]
        出资情况变更 = 20,

        /// <summary>
        /// 司法协助(公司)
        /// </summary>
        [Description("司法协助(公司)")]
        司法协助_公司 = 21,

        /// <summary>
        /// 清算信息
        /// </summary>
        [Description("清算信息")]
        清算信息 = 22,

        /// <summary>
        /// 知识产权出质
        /// </summary>
        [Description("知识产权出质")]
        知识产权出质 = 23,

        /// <summary>
        /// 环保处罚
        /// </summary>
        [Description("环保处罚")]
        环保处罚 = 24,

        /// <summary>
        /// 公示催告
        /// </summary>
        [Description("公示催告")]
        公示催告 = 25,

        /// <summary>
        /// 送达公告
        /// </summary>
        [Description("送达公告")]
        送达公告 = 26,

        /// <summary>
        /// 立案信息
        /// </summary>
        [Description("立案信息")]
        立案信息 = 27,

        /// <summary>
        /// 税收违法
        /// </summary>
        [Description("税收违法")]
        税收违法 = 28,

        /// <summary>
        /// 司法拍卖
        /// </summary>
        [Description("司法拍卖")]
        司法拍卖 = 29,

        /// <summary>
        /// 土地抵押
        /// </summary>
        [Description("土地抵押")]
        土地抵押 = 30,

        /// <summary>
        /// 简易注销
        /// </summary>
        [Description("简易注销")]
        简易注销 = 31,

        /// <summary>
        /// 限制消费令（公司）
        /// </summary>
        [Description("限制消费令（公司）")]
        限制消费令_公司 = 32,

        /// <summary>
        /// 终本案件
        /// </summary>
        [Description("终本案件")]
        终本案件 = 34,

        /// <summary>
        /// 股权出质(人)
        /// </summary>
        [Description("股权出质(人)")]
        股权出质_人 = 35,

        /// <summary>
        /// 司法协助(人)
        /// </summary>
        [Description("司法协助(人)")]
        司法协助_人 = 36,

        /// <summary>
        /// 强制清算
        /// </summary>
        [Description("强制清算")]
        强制清算 = 45,

        /// <summary>
        /// 涉金融黑名单
        /// </summary>
        [Description("涉金融黑名单")]
        涉金融黑名单 = 62,

        /// <summary>
        /// 破产案件
        /// </summary>
        [Description("破产案件")]
        破产案件 = 38,

        /// <summary>
        /// 违规处理
        /// </summary>
        [Description("违规处理")]
        违规处理 = 42,

        /// <summary>
        /// 对外担保
        /// </summary>
        [Description("对外担保")]
        对外担保 = 41,

        /// <summary>
        /// 抽查检查
        /// </summary>
        [Description("抽查检查")]
        抽查检查 = 40,

        /// <summary>
        /// 食品安全
        /// </summary>
        [Description("食品安全")]
        食品安全 = 64,

        /// <summary>
        /// 注销备案
        /// </summary>
        [Description("注销备案")]
        注销备案 = 63,

        /// <summary>
        /// 询价评估
        /// </summary>
        [Description("询价评估")]
        询价评估 = 39,

        /// <summary>
        /// 限制消费令（人）
        /// </summary>
        [Description("限制消费令（人）")]
        限制消费令_人 = 33,

        /// <summary>
        /// 终本案件（人）
        /// </summary>
        [Description("终本案件（人）")]
        终本案件_人 = 46,

        /// <summary>
        /// 开庭公告（人）
        /// </summary>
        [Description("开庭公告（人）")]
        开庭公告_人 = 47,

        /// <summary>
        /// 法律诉讼（人）
        /// </summary>
        [Description("法律诉讼（人）")]
        法律诉讼_人 = 48,

        /// <summary>
        /// 送达公告（人）
        /// </summary>
        [Description("送达公告（人）")]
        送达公告_人 = 49,

        /// <summary>
        /// 立案信息（人）
        /// </summary>
        [Description("立案信息（人）")]
        立案信息_人 = 50,

        /// <summary>
        /// 股权质押
        /// </summary>
        [Description("股权质押")]
        股权质押 = 51,

        /// <summary>
        /// 股权质押（人）
        /// </summary>
        [Description("股权质押（人）")]
        股权质押_人 = 37,

        /// <summary>
        /// 历史失信被执行人
        /// </summary>
        [Description("历史失信被执行人")]
        历史失信被执行人 = 71,

        /// <summary>
        /// 历史被执行人
        /// </summary>
        [Description("历史被执行人")]
        历史被执行人 = 72,

        /// <summary>
        /// 历史限制消费令
        /// </summary>
        [Description("历史限制消费令")]
        历史限制消费令 = 73,

        /// <summary>
        /// 历史终本案件
        /// </summary>
        [Description("历史终本案件")]
        历史终本案件 = 74,

        /// <summary>
        /// 历史司法协助
        /// </summary>
        [Description("历史司法协助")]
        历史司法协助 = 75,

        /// <summary>
        /// 历史经营异常
        /// </summary>
        [Description("历史经营异常")]
        历史经营异常 = 76,

        /// <summary>
        /// 历史行政处罚
        /// </summary>
        [Description("历史行政处罚")]
        历史行政处罚 = 70,

        /// <summary>
        /// 历史股权出质
        /// </summary>
        [Description("历史股权出质")]
        历史股权出质 = 77,

        /// <summary>
        /// 历史动产抵押
        /// </summary>
        [Description("历史动产抵押")]
        历史动产抵押 = 78,

        /// <summary>
        /// 历史欠税公告
        /// </summary>
        [Description("历史欠税公告")]
        历史欠税公告 = 79,

        /// <summary>
        /// 产品召回
        /// </summary>
        [Description("产品召回")]
        产品召回 = 65,

        /// <summary>
        /// 严重违法（已移出）
        /// </summary>
        [Description("严重违法（已移出）")]
        严重违法_已移出 = 53,

        /// <summary>
        /// 经营异常（已移出）
        /// </summary>
        [Description("经营异常（已移出）")]
        经营异常_已移出 = 55
    }

    #endregion

    #endregion
}