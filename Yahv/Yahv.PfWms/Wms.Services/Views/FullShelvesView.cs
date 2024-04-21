using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Models;
using Yahv.Linq;

namespace Wms.Services.Views
{

    /// <summary>
    /// 专属于库房的视图
    /// </summary>
    public class FullShelvesView : UniqueView<Models.Warehouse, PvWmsRepository>
    {
        protected override IQueryable<Warehouse> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Shelves>()
                   join warehouse in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Warehouses>() on entity.ID equals warehouse.ID
                   select new Warehouse
                   {
                       ID = entity.ID,
                       FatherID = entity.FatherID,
                       Type = (Enums.ShelvesType)entity.Type,
                       Purpose = (Enums.ShelvesPurpose)entity.Purpose,
                       Addible = entity.Addible,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Status = (Enums.ShelvesStatus)entity.Status,
                       SpecID = entity.SpecID,
                       Summary = entity.Summary,
                       ManagerID = entity.ManagerID,
                       EnterpriseID = entity.EnterpriseID,
                       ClerkID = entity.ClerkID,
                       ContractID = entity.ContractID,
                       Name = warehouse.Name,
                       Address = warehouse.Address,
                       CrmCode = warehouse.CrmCode
                   };
        }
    }
}
