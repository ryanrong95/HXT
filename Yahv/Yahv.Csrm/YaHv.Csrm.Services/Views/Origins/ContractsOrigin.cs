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
    public class ContractsOrigin : Yahv.Linq.UniqueView<Models.Origins.Contract, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal ContractsOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal ContractsOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Contract> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);

            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Contracts>()
                       join maps in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>()
                      on entity.ID equals maps.SubID
                       where maps.Bussiness == (int)Business.WarehouseServicing && maps.Type == (int)MapsType.Contract
                       join company in enterpriseView on maps.EnterpriseID equals company.ID
                       join client in enterpriseView on entity.EnterpriseID equals client.ID

                       join admin in adminsView on entity.Creator equals admin.ID into _admin
                       from admin in _admin.DefaultIfEmpty()
                       select new Contract
                       {
                           ID = entity.ID,
                           StartDate = entity.StartDate,
                           EndDate = entity.EndDate,
                           AgencyRate = entity.AgencyRate,
                           MinAgencyFee = entity.MinAgencyFee,
                           ExchangeMode = (Yahv.Underly.ExchangeMode)entity.ExchangeMode,
                           InvoiceType = (Yahv.Underly.BillingType)entity.InvoiceType,
                           InvoiceTaxRate = entity.InvoiceTaxRate,
                           Enterprise = client,
                           Company = company,
                           CreatorID = entity.Creator,
                           Creator = admin,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           Summary = entity.Summary,
                           Status = (GeneralStatus)entity.Status,
                           CompanyID = maps.EnterpriseID

                       };
            return linq;
            //return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Contracts>()
            //       join enterprises in enterpriseView on entity.EnterpriseID equals enterprises.ID
            //       join admin in adminsView on entity.Creator equals admin.ID into _admin
            //       from admin in _admin.DefaultIfEmpty()
            //       where entity.Status == (int)Status.Normal
            //       select new Contract()
            //       {
            //           ID = entity.ID,
            //           StartDate = entity.StartDate,
            //           EndDate = entity.EndDate,
            //           AgencyRate = entity.AgencyRate,
            //           MinAgencyFee = entity.MinAgencyFee,
            //           ExchangeMode = (Yahv.Underly.ExchangeMode)entity.ExchangeMode,
            //           InvoiceType = (Yahv.Underly.BillingType)entity.InvoiceType,
            //           InvoiceTaxRate = entity.InvoiceTaxRate,
            //           Enterprise = enterprises,
            //           Creator = admin,
            //           CreateDate = entity.CreateDate,
            //           UpdateDate = entity.UpdateDate,
            //           Summary = entity.Summary
            //       };
        }
    }
}
