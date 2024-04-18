using Layers.Data.Sqls.PvbCrm;
using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 供应商视图
    /// </summary>
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

        virtual protected IQueryable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView> GetMapIQueryable()
        {
            return null;
        }


        IQueryable<Supplier> getIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<SuppliersTopView>()
                   select new Supplier
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Type = (SupplierType)entity.Type,
                       Grade = (SupplierGrade)entity.Grade,
                       Status = (ApprovalStatus)entity.Status,
                       TaxperNumber = entity.TaxperNumber,
                       District = entity.District,//废弃
                       AreaType = (AreaType)entity.AreaType,
                       InvoiceType = (InvoiceType)entity.InvoiceType,
                       Nature = (SupplierNature)entity.Nature,
                       IsFactory = entity.IsFactory,
                       AgentCompany = entity.AgentCompany,
                       DyjCode = entity.DyjCode,
                       RepayCycle = entity.RepayCycle,
                       Currency = (Currency)entity.Currency,
                       Price = entity.Price
                   };
        }
        protected override IQueryable<Supplier> GetIQueryable()
        {
            var mapsView = this.GetMapIQueryable();
            if (mapsView == null)
            {
                return this.getIQueryable();
            }
            else
                return from entity in this.getIQueryable()
                       join map in mapsView on entity.ID equals map.EnterpriseID
                       select entity;
        }

    }
}
