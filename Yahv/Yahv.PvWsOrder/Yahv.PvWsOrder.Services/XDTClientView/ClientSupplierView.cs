using Layers.Data.Sqls;
using System;
using System.Linq;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.XDTModels;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    /// <summary>
    /// 客户供应商
    /// </summary>
    public class ClientSupplierView : UniqueView<ClientSupplier, ScCustomReponsitory>
    {
        protected override IQueryable<ClientSupplier> GetIQueryable()
        {
            return from supplier in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.ClientSuppliers>()
                   join country in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.BaseCountries>() on supplier.Place equals country.Code
                   where supplier.Status == (int)Status.Normal
                   select new ClientSupplier
                   {
                       ID = supplier.ID,
                       ClientID = supplier.ClientID,
                       Name = supplier.Name,
                       ChineseName = supplier.ChineseName,
                       Status = (Status)supplier.Status,
                       CreateDate = supplier.CreateDate,
                       UpdateDate = supplier.UpdateDate,
                       Summary = supplier.Summary,
                       Place = supplier.Place,
                       Type = country.Type.GetValueOrDefault()
                   };
        }

        /// <summary>
        /// 根据供应商名称查询数据
        /// </summary>
        /// <param name="supplierid"></param>
        /// <returns></returns>
        public ClientSupplier SearchBySupplierName(string name)
        {
            var linq = this.IQueryable.Where(item => item.Name == name).FirstOrDefault();

            return linq;
        }
    }

    /// <summary>
    /// 客户供应商
    /// </summary>
    public class ClientSupplier : IUnique
    {
        public string ID { get; set; }

        public string ClientID { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 中文名称
        /// </summary>
        public string ChineseName { get; set; }

        public Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }
        public string Place { get; set; }
        public int Type { get; set; }
    }
}
