using Layers.Data.Sqls;
using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    public class ClientSupplierBankView : UniqueView<ClientSupplierBank, ScCustomReponsitory>
    {
        public ClientSupplierBankView()
        {

        }

        public ClientSupplierBankView(ScCustomReponsitory reponsitory, IQueryable<ClientSupplierBank> iQuery) : base(reponsitory, iQuery)
        {

        }

        protected override IQueryable<ClientSupplierBank> GetIQueryable()
        {
            return from bank in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.ClientSupplierBanks>()
                   join country in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.BaseCountries>() on bank.Place equals country.Code
                   where bank.Status == (int)GeneralStatus.Normal
                   select new ClientSupplierBank
                   {
                       ID = bank.ID,
                       ClientSupplierID = bank.ClientSupplierID,
                       BankAccount = bank.BankAccount,
                       BankAddress = bank.BankAddress,
                       BankName = bank.BankName,
                       SwiftCode = bank.SwiftCode,
                       Palce = bank.Place,
                       Type = country.Type.GetValueOrDefault(),
                       Status = bank.Status,
                       CreateDate = bank.CreateDate,
                       Summary = bank.Summary
                   };
        }

        /// <summary>
        /// 根据供应商ID查询数据
        /// </summary>
        /// <param name="supplierid"></param>
        /// <returns></returns>
        public ClientSupplierBankView SearchBySupplierID(string supplierid)
        {
            var linq = this.IQueryable.Where(item => item.ClientSupplierID == supplierid).OrderByDescending(item => item.CreateDate);

            return new ClientSupplierBankView(this.Reponsitory, linq);
        }
    }

    public class ClientSupplierBank : Yahv.Linq.IUnique
    {
        public string ID { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        public string Palce { get; set; }

        /// <summary>
        /// 类型（1:美国,2:欧盟）
        /// </summary>
        public int Type { get; set; }

        public string ClientSupplierID { get; set; }

        public string BankAccount { get; set; }

        public string BankAddress { get; set; }

        public string BankName { get; set; }

        public string SwiftCode { get; set; }

        public int Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }

        public ClientSupplierBank()
        {
            Status = (int)GeneralStatus.Normal;
            CreateDate = DateTime.Now;
        }
    }
}
