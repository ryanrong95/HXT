using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.VcCsrm.Service.Models;

namespace YaHv.VcCsrm.Service.Views.Origins
{
    public class ContractsOrigin : Yahv.Linq.UniqueView<Contract, PvcCrmReponsitory>
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
        internal ContractsOrigin(PvcCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Contract> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);

            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvcCrm.Contracts>()

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
                           //Enterprise = client,
                           //Company = company,
                           CreatorID = entity.Creator,
                           // Creator = admin,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           Summary = entity.Summary,
                           Status = (GeneralStatus)entity.Status,
                           // CompanyID = maps.EnterpriseID

                       };
            return linq;

        }
    }
}
