using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models.PveCrm;

namespace Yahv.Services.Views.PveCrm
{
    /// <summary>
    /// 内部公司视图
    /// </summary>
    public class CompaniesTopView<TReponsitory> : UniqueView<Company, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CompaniesTopView()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public CompaniesTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Company> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PveCrm.CompaniesTopView>()
                   select new Company
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Type = (Underly.CrmPlus.CompanyType)entity.Type,
                       Status = (Underly.DataStatus)entity.Status,
                       District = entity.District,
                       RegAddress = entity.RegAddress,
                       Uscc = entity.Uscc,
                       Corporation = entity.Corporation,
                   };
        }
    }
}
