using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Usually;

namespace Yahv.Plats.Services.Models.Origins
{
    /// <summary>
    /// 员工信息
    /// </summary>
    public class Staff : IUnique
    {
        #region 属性

        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 员工编码,全局唯一（例如：0001）
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 统一编码,全局唯一(自定义编码)
        /// </summary>
        public string SelCode { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// 大赢家ID
        /// </summary>
        public string DyjCode { get; set; }

        /// <summary>
        /// 是内部公司分类的方法 采用大赢家的编号
        /// </summary>
        public string DyjCompanyCode { get; set; }

        /// <summary>
        /// 是内部公司下部门分类的方法 采用大赢家的编号
        /// </summary>
        public string DyjDepartmentCode { get; set; }

        /// <summary>
        /// 组织ID （只做合同关系的）
        /// </summary>
        public string LeagueID { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>
        public string PostionID { get; set; }

        /// <summary>
        /// 员工部门编码
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// 员工职务编码
        /// </summary>
        public string PostionCode { get; set; }

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