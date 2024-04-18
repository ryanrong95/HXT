using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Services.Views
{

    /// <summary>
    /// Staff视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class StaffsTopView<TReponsitory> : UniqueView<Staff, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {

        public StaffsTopView()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        public StaffsTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Staff> GetIQueryable()
        {

            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.StaffsTopView>()
                   select new Models.Staff
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Code = entity.Code,
                       DyjCompanyCode = entity.DyjCompanyCode,
                       DyjDepartmentCode = entity.DyjDepartmentCode,
                       DyjCode = entity.DyjCode,
                       AdminID = entity.AdminID,
                       WorkCity = entity.WorkCity,
                   };
        }
    }
}
