using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMvc.Models
{
    /// <summary>
    /// 申请数据模型
    /// </summary>
    public class WebApiApply
    { 
        /// <summary>
        /// 所有地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string Company_Name { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contacts { get; set; }

        /// <summary>
        /// 联系人手机
        /// </summary>
        public string Contacts_Moblie { get; set; }

        /// <summary>
        /// 固定电话
        /// </summary>
        public string Phone { get; set; }
    }

    /// <summary>
    /// 归类查询数据模型
    /// </summary>
    public class WebApiQuery
    {
        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProName { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string ProModel { get; set; }

        /// <summary>
        /// 第一单位
        /// </summary>
        public string FirstLegalUnit { get; set; }

        /// <summary>
        /// 第二单位
        /// </summary>
        public string SecondLegalUnit { get; set; }

        /// <summary>
        /// 监管条件
        /// </summary>
        public string monCon { get; set; }

        /// <summary>
        /// 检验检疫
        /// </summary>
        public string InsQua { get; set; }

        /// <summary>
        /// 进口最惠圆税率
        /// </summary>
        public string MFN { get; set; }

        /// <summary>
        /// 普通税率
        /// </summary>
        public string General { get; set; }

        /// <summary>
        /// 增值税率
        /// </summary>
        public string AddedValue { get; set; }

        /// <summary>
        /// 消费税率
        /// </summary>
        public string Consume { get; set; }

        /// <summary>
        /// 申报要素
        /// </summary>
        public string[] Elements { get; set; }

        /// <summary>
        /// CIQ
        /// </summary>
        public string CIQCode { get; set; }

        /// <summary>
        /// 是否3c
        /// </summary>
        public string IsCCC { get; set; }
    }
}