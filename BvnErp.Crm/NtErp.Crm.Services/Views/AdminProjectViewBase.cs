using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using NtErp.Crm.Services.Models.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class AdminProjectViewBase : Generic.AdminClassicsAlls
    {
        JobType JobType;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        AdminProjectViewBase()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="jobType">职能类型</param>
        public AdminProjectViewBase(JobType jobType)
        {
            this.JobType = jobType;
        }

        /// <summary>
        /// 获取人员集合
        /// </summary>
        /// <param name="expression">查询条件</param>
        /// <param name="expressions">Lambda表达式集合</param>
        /// <returns></returns>
        protected override IQueryable<AdminDossier> GetIQueryable(Expression<Func<AdminDossier, bool>> expression, params LambdaExpression[] expressions)
        {
            return from entity in base.GetIQueryable(expression, expressions)
                   where entity.Admin.JobType == this.JobType
                   select entity;
        }
    }
}
