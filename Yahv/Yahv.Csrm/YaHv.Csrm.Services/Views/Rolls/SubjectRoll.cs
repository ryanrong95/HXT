using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    /// <summary>
    /// 科目管理试图
    /// </summary>
    public class SubjectRoll : Yahv.Linq.UniqueView<Subject, PvbCrmReponsitory>
    {
        public SubjectRoll()
        {

        }

        internal SubjectRoll(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Subject> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Subjects>()
                select new Subject
                {
                    ID = entity.ID,
                    Catalog = entity.Catalog,
                    Conduct = entity.Conduct,
                    Currency = (Currency)entity.Currency,
                    IsCount = entity.IsCount,
                    IsToCustomer = entity.IsToCustomer,
                    Name = entity.Name,
                    Price = entity.Price,
                    Steps = entity.Steps,
                    Type = (Yahv.Underly.SubjectType)entity.Type,
                };
        }
    }
}
