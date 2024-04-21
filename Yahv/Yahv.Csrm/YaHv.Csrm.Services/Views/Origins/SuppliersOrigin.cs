using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    public class SuppliersOrigin : Yahv.Linq.UniqueView<Models.Origins.Supplier, PvbCrmReponsitory>
    {
        public SuppliersOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal SuppliersOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Supplier> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            var businessadmin = new Rolls.BusinessAdminsRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Suppliers>()
                   join enterprises in enterprisesView on entity.ID equals enterprises.ID
                   join admin in adminsView on entity.AdminID equals admin.ID
                   select new Supplier()
                   {
                       ID = entity.ID,
                       Enterprise = enterprises,
                       Grade = (SupplierGrade)entity.Grade,
                       Type = (SupplierType)entity.Type,
                       DyjCode = entity.DyjCode,
                       TaxperNumber = entity.TaxperNumber,
                       Nature = (SupplierNature)entity.Nature,
                       AreaType = (AreaType)entity.AreaType,
                       InvoiceType = (InvoiceType)entity.InvoiceType,
                       IsFactory = entity.IsFactory,
                       AgentCompany = entity.AgentCompany,
                       SupplierStatus = (ApprovalStatus)entity.Status,
                       RepayCycle = entity.RepayCycle,
                       Currency = (Currency)entity.Currency,
                       Price = entity.Price,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       CreatorID = entity.AdminID,
                       Creator = admin,
                       Place = entity.Place,
                       IsForwarder=entity.IsForwarder
                   };

        }
    }
    public class TradingSuppliersOrigin : Yahv.Linq.UniqueView<Models.Origins.TradingSupplier, PvbCrmReponsitory>
    {
        public TradingSuppliersOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal TradingSuppliersOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<TradingSupplier> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            var purchasersView = new Rolls.TradingAdminsRoll(this.Reponsitory, MapsType.Supplier);

            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Suppliers>()

                       join enterprises in enterprisesView on entity.ID equals enterprises.ID

                       join admin in adminsView on entity.AdminID equals admin.ID

                       join maps in purchasersView on entity.ID equals maps.EnterpriseID into purchasers
                       select new TradingSupplier
                       {
                           ID = entity.ID,
                           Enterprise = enterprises,
                           Grade = (SupplierGrade)entity.Grade,
                           Type = (SupplierType)entity.Type,
                           DyjCode = entity.DyjCode,
                           TaxperNumber = entity.TaxperNumber,
                           Nature = (SupplierNature)entity.Nature,
                           AreaType = (AreaType)entity.AreaType,
                           InvoiceType = (InvoiceType)entity.InvoiceType,
                           IsFactory = entity.IsFactory,
                           AgentCompany = entity.AgentCompany,
                           SupplierStatus = (ApprovalStatus)entity.Status,
                           RepayCycle = entity.RepayCycle,
                           Currency = (Currency)entity.Currency,
                           Price = entity.Price,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           CreatorID = admin.ID,
                           Creator = admin,
                           Purchasers = purchasers,
                           Place = entity.Place,
                           IsForwarder = entity.IsForwarder
                       };
            return linq;

        }
    }
    #region 合作公司
    //public class TradingSuppliersOrigin : Yahv.Linq.UniqueView<Models.Origins.TradingSupplier, PvbCrmReponsitory>
    //{
    //    public TradingSuppliersOrigin()
    //    {
    //    }
    //    /// <summary>
    //    /// 构造函数
    //    /// </summary>
    //    /// <param name="reponsitory">数据库连接</param>
    //    internal TradingSuppliersOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
    //    {
    //    }
    //    protected override IQueryable<TradingSupplier> GetIQueryable()
    //    {
    //        var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
    //        var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
    //        var purchasersView = new Rolls.TradingAdminsRoll(this.Reponsitory, MapsType.Supplier, Business.Trading_Purchase);

    //        var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Suppliers>()

    //                   join enterprises in enterprisesView on entity.ID equals enterprises.ID

    //                   join admin in adminsView on entity.AdminID equals admin.ID

    //                   join maps in purchasersView on entity.ID equals maps.EnterpriseID into purchasers

    //                   select new TradingSupplier
    //                   {
    //                       ID = entity.ID,
    //                       Enterprise = enterprises,
    //                       Grade = (SupplierGrade)entity.Grade,
    //                       Type = (SupplierType)entity.Type,
    //                       DyjCode = entity.DyjCode,
    //                       TaxperNumber = entity.TaxperNumber,
    //                       Nature = (SupplierNature)entity.Nature,
    //                       AreaType = (AreaType)entity.AreaType,
    //                       InvoiceType = (InvoiceType)entity.InvoiceType,
    //                       IsFactory = entity.IsFactory,
    //                       AgentCompany = entity.AgentCompany,
    //                       SupplierStatus = (ApprovalStatus)entity.Status,
    //                       RepayCycle = entity.RepayCycle,
    //                       Currency = (Currency)entity.Currency,
    //                       Price = entity.Price,
    //                       CreateDate = entity.CreateDate,
    //                       UpdateDate = entity.UpdateDate,
    //                       Creator = admin,
    //                       Purchasers = purchasers
    //                   };
    //        return linq;

    //    }
    //}
    #endregion

}
