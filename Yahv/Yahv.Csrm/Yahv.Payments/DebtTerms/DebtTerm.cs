using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Payments
{
    public class DebtTerm : IUnique
    {
        #region 属性
        private string id;
        public string ID
        {
            get { return this.id ?? string.Join("", this.Payer, this.Payee, this.Business, this.Catalog).MD5(); }
            set { this.id = value; }
        }

        public string Payer { get; internal set; }
        public string Payee { get; internal set; }
        public string Business { get; internal set; }
        public string Catalog { get; internal set; }
        public string AdminID { get; internal set; }
        public DateTime CreateDate { get; internal set; }

        public SettlementType SettlementType { get; set; }
        public int Months { get; set; }
        public int Days { get; set; }
        public ExchangeType ExchangeType { get; set; }
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
            {
                //新增
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.DebtTerms>().Any(item => item.ID == this.ID))
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.DebtTerms()
                    {
                        Business = this.Business,
                        ID = this.ID,
                        Catalog = this.Catalog,
                        Payee = this.Payee,
                        Payer = this.Payer,
                        CreateDate = DateTime.Now,
                        AdminID = this.AdminID,
                        Days = this.Days,
                        ERateType = (int)this.ExchangeType,
                        Months = this.Months,
                        SettlementType = (int)this.SettlementType,
                    });
                }
                else
                {
                    var entity =
                        reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.DebtTerms>()
                            .FirstOrDefault(item => item.ID == this.ID);

                    //更新账期表
                    reponsitory.Update<Layers.Data.Sqls.PvbCrm.DebtTerms>(new
                    {
                        CreateDate = DateTime.Now,
                        AdminID = this.AdminID,
                        Days = this.Days,
                        ERateType = (int)this.ExchangeType,
                        Months = this.Months,
                        SettlementType = (int)this.SettlementType,
                    }, item => item.ID == this.ID);


                    //新增账期日志表
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Logs_DebtTerms()
                    {
                        ID = PKeySigner.Pick(PKeyType.LogsDebtTerms),
                        Business = entity.Business,
                        Catalog = entity.Catalog,
                        Payer = entity.Payer,
                        Payee = entity.Payee,
                        CreateDate = DateTime.Now,
                        AdminID = entity.AdminID,
                        Months = entity.Months,
                        SettlementType = entity.SettlementType,
                        ERateType = entity.ERateType,
                        Days = entity.Days,
                        OldID = entity.ID,
                        OriginDate = entity.CreateDate,
                    });
                }
            }
        }
        #endregion
    }
}
