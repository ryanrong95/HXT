using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PvWsClient.WebAppNew.Models
{
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
}