using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models.PveCrm;
using Yahv.Underly;

namespace Yahv.Services.Views.PveCrm
{
    /// <summary>
    /// 供应商通用视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class SuppliersTopView<TReponsitory> : UniqueView<Supplier, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SuppliersTopView()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public SuppliersTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Supplier> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PveCrm.SuppliersTopView>()
                   join area in this.Reponsitory.ReadTable<Layers.Data.Sqls.PveCrm.EnumsDictionariesTopView>() on entity.AreaType equals area.ID
                   select new Supplier
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Type = entity.Type.HasValue ? (Underly.CrmPlus.SupplierType)entity.Type : Underly.CrmPlus.SupplierType.UnKnown,
                       Nature = entity.Nature,
                       Grade = (SupplierGrade)entity.Grade,
                       Status = (AuditStatus)entity.Status,
                       TaxperNumber = entity.TaxperNumber,
                       District = entity.District,//废弃
                       InvoiceType = (InvoiceType)entity.InvoiceType,
                       IsFactory = entity.IsFactory == 1,
                       AgentCompany = entity.AgentCompany,
                       DyjCode = entity.DyjCode,
                       RepayCycle = entity.RepayCycle,
                       Currency = (Currency)entity.Currency,
                       Price = entity.Price
                   };
        }
    }
}
