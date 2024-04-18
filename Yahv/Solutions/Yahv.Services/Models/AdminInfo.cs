using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 管理员信息
    /// </summary>
    public class AdminInfo : Admin
    {
        #region 属性
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IDCard { get; set; }

        /// <summary>
        /// 肖像照
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 籍贯
        /// </summary>
        public string NativePlace { get; set; }

        /// <summary>
        /// 家庭地址
        /// </summary>
        public string HomeAddress { get; set; }

        /// <summary>
        /// 户口所在地
        /// </summary>
        public string PassAddress { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        public string Volk { get; set; }

        /// <summary>
        /// 政治面貌
        /// </summary>
        public string PoliticalOutlook { get; set; }

        /// <summary>
        /// 身高
        /// </summary>
        public double? Height { get; set; }

        /// <summary>
        /// 体重
        /// </summary>
        public double? Weight { get; set; }

        /// <summary>
        /// 血型
        /// </summary>
        public string Blood { get; set; }

        /// <summary>
        /// 学历
        /// </summary>
        public string Education { get; set; }

        /// <summary>
        /// 毕业院校
        /// </summary>
        public string GraduatInstitutions { get; set; }

        /// <summary>
        /// 所学专业
        /// </summary>
        public string Major { get; set; }

        /// <summary>
        /// 婚否
        /// </summary>
        public bool? IsMarry { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        #endregion
    }
}
