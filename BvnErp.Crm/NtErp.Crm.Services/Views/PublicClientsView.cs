using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
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
    public class PublicClientsView : Generic.ClientClassicAlls
    {
        //人员对象
        IGenericAdmin Admin;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        PublicClientsView()
        {

        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员对象</param>
        public PublicClientsView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 获取客户数据集合
        /// </summary>
        /// <param name="expression">查询条件</param>
        /// <param name="expressions">lambda表达式</param>
        /// <returns></returns>
        protected override IQueryable<ClientDossier> GetIQueryable(Expression<Func<ClientDossier, bool>> expression, params LambdaExpression[] expressions)
        {
            return from clientDossier in base.GetIQueryable(expression, expressions)
                   where clientDossier.Client.IsSafe != Enums.IsProtected.Yes
                   orderby clientDossier.Client.UpdateDate descending
                   select clientDossier;
        }
    }
}