using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 内部公司视图
    /// </summary>
    public class CompaniesTopView : UniqueView<Company, PvbCrmReponsitory>
    {
        public CompaniesTopView()
        {

        }

        public CompaniesTopView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<Company> GetIQueryable()
        {
            return new Yahv.Services.Views.CompaniesTopView<PvbCrmReponsitory>();
        }

        /// <summary>
        /// 索引器
        /// </summary>
        public Company this[string enterpriseId]
        {
            get { return this.SingleOrDefault(item => item.ID == enterpriseId); }
        }
    }
}
