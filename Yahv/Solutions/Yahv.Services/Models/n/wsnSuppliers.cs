using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 客户的供应商
    /// </summary>
    public class wsnSuppliers : IUnique
    {
        #region 属性
        public string ID { set; get; }

        /// <summary>
        /// 供应商企业名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 中文名称
        /// </summary>
        public string ChineseName { set; get; }

        /// <summary>
        /// 英文名称
        /// </summary>
        public string EnglishName { set; get; }

        /// <summary>
        /// 所属企业ID：客户ID
        /// </summary>
        public string OwnID { set; get; }
        /// <summary>
        /// 所属企业的企业名称
        /// </summary>
        public string OwnName { set; get; }
        /// <summary>
        /// 真正的企业ID：供应商的企业ID
        /// </summary>
        public string RealEnterpriseID { set; get; }
        /// <summary>
        /// 真正的企业名称：供应商的企业名称
        /// </summary>
        public string RealEnterpriseName { set; get; }
        /// <summary>
        /// 供应商等级
        /// </summary>
        public SupplierGrade nGrade { set; get; }
        /// <summary>
        /// 国家或地区
        /// </summary>
        public string Place { set; get; }
        /// <summary>
        /// 客户的入仓号
        /// </summary>
        public string EnterCode { set; get; }
        /// <summary>
        /// 客户等级
        /// </summary>
        public ClientGrade ClientGrade { set; get; }
        /// <summary>
        /// 供应商等级
        /// </summary>
        public GeneralStatus Status { set; get; }
        /// <summary>
        /// 法人
        /// </summary>
        public string Corporation { get; set; }
        /// <summary>
        /// 社会统一码
        /// </summary>
        public string Uscc { get; set; }
        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegAddress { get; set; }
        /// <summary>
        /// 中文简称
        /// </summary>
        public string CHNabbreviation { set; get; }
        #endregion
    }
}
