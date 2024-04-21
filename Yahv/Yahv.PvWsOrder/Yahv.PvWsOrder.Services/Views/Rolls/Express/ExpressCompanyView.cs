using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views
{
    public class ExpressCompanyView : UniqueView<ExpressCompanyModel, PvWsOrderReponsitory>
    {
        public ExpressCompanyView()
        {
        }

        internal ExpressCompanyView(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ExpressCompanyModel> GetIQueryable()
        {
            var expressCompanys = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ExpressCompanys>();
            var carriers = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Carriers>();

            return from expressCompany in expressCompanys
                   join carrier in carriers on expressCompany.ID equals carrier.ID
                   where carrier.Status == (int)GeneralStatus.Normal && carrier.CarrierType == (int)Enums.ExpressCarrierType.DomesticExpress
                   select new ExpressCompanyModel
                   {
                       ID = carrier.ID,
                       //Contact = carrier.Contact,
                       CarrierType = (Enums.ExpressCarrierType)carrier.CarrierType,
                       Name = carrier.Name,
                       Code = carrier.Code,
                       Status = (GeneralStatus)carrier.Status,
                       Summary = carrier.Summary,
                       CreateDate = carrier.CreateDate,
                       QueryMark = carrier.QueryMark,
                       CustomerName = expressCompany.CustomerName,
                       CustomerPwd = expressCompany.CustomerPwd,
                       MonthCode = expressCompany.MonthCode,
                   };
        }
    }

    public class ExpressCompanyModel : CarrierModel
    {
        /// <summary>
        /// 账号名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 账号密码
        /// </summary>
        public string CustomerPwd { get; set; }

        /// <summary>
        /// 月结账号
        /// </summary>
        public string MonthCode { get; set; }
    }

    public class CarrierModel : IUnique
    {
        public string ID { get; set; }

        /// <summary>
        /// 承运商名称(简称)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 查询标记
        /// </summary>
        public string QueryMark { get; set; }

        /// <summary>
        /// 承运商名称(全称)
        /// </summary>
        public string Name { get; set; }

        public Enums.ExpressCarrierType CarrierType { get; set; }

        public GeneralStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        ///// <summary>
        ///// 联系人
        ///// </summary>
        //public Contact Contact { get; set; }

        public string Summary { get; set; }

        public string Address { get; set; }
    }
}
