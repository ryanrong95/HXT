using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Views
{
    /// <summary>
    /// 颗粒化视图
    /// </summary>
    public class UnitesAllsView : UniqueView<RoleUnite, BvnErpReponsitory>
    {
        public UnitesAllsView()
        {
           
        }

        protected override IQueryable<RoleUnite> GetIQueryable()
        {
             return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.RoleUnites>()
                        select new RoleUnite
                        {
                            ID = entity.ID,
                            Type = (RoleUniteType)entity.Type,
                            Menu = entity.Menu,
                            Name = entity.Name,
                            Title = entity.Title,
                            Url = entity.Url,
                            CreateDate = entity.CreateDate
                        };

        }
    }
}
