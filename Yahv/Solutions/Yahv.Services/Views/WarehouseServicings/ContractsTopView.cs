using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    public class ContractsTopView<TReponsitory> : QueryView<Contract, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public ContractsTopView()
        {

        }
        public ContractsTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Contract> GetIQueryable()
        {
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.ContractsTopView>()
                       join maps in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>()
                      on entity.ID equals maps.SubID
                       where maps.Bussiness == (int)Business.WarehouseServicing && maps.Type == (int)MapsType.Contract
                       select new Contract
                       {
                           ID = entity.ID,
                           EnterpriseID = entity.EnterpriseID,//客户ID，合同的甲方
                           StartDate = entity.StartDate,
                           EndDate = entity.EndDate,
                           AgencyRate = entity.AgencyRate,
                           MinAgencyFee = entity.MinAgencyFee,
                           ExchangeMode = (ExchangeMode)entity.ExchangeMode,
                           InvoiceType = (BillingType)entity.InvoiceType,
                           InvoiceTaxRate = entity.InvoiceTaxRate,
                           Summary = entity.Summary,
                           Status = (GeneralStatus)entity.Status,
                           CompanyID = maps.EnterpriseID  //内部公司ID，合同的乙方：华芯通等
                       };
            return linq;
        }

    }
}
