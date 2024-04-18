using Layers.Data.Sqls.PvbCrm;
using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Services.Views
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
            //var enterpriseView = this.Reponsitory.ReadTable<EnterprisesTopView>();

            return from entity in this.Reponsitory.ReadTable<CompaniesTopView>()
                   select new Company
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Type = (Underly.CompanyType)entity.Type,
                       Status = (Underly.ApprovalStatus)entity.Status,
                       District = entity.District,//废弃
                       Range = (Underly.AreaType)entity.Range,
                       RegAddress = entity.RegAddress,
                       Uscc = entity.Uscc,
                       Corporation = entity.Corporation,
                   };
        }
    }
}
