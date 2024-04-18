using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.Models.Origin
{
    public class PayeeRight : IUnique
    {
        #region 属性

        public string ID { get; set; }
        public string LeftID { get; set; }
        public Underly.Currency Currency { get; set; }
        public decimal Price { get; set; }
        public DateTime CreateDate { get; set; }
        public string AdminID { get; set; }
        public string ReviewerID { get; set; }
        public string FlowFormCode { get; set; }

        #endregion

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (var repository = new Layers.Data.Sqls.PsOrderRepository())
            {
                if (!repository.ReadTable<Layers.Data.Sqls.PsOrder.PayeeRights>().Any(t => t.ID == this.ID))
                {
                    this.ID = Layers.Data.PKeySigner.Pick(PKeyType.PayeeRight);
                    repository.Insert(new Layers.Data.Sqls.PsOrder.PayeeRights()
                    {
                        ID = this.ID,
                        LeftID = this.LeftID,
                        Currency = (int)this.Currency,
                        Price = this.Price,
                        AdminID = this.AdminID,
                        ReviewerID = this.ReviewerID,
                        CreateDate = this.CreateDate,
                        FlowFormCode = this.FlowFormCode,
                    });
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PsOrder.PayeeRights>(new
                    {
                        this.LeftID,
                        Currency = (int)this.Currency,
                        this.Price,
                        this.AdminID,
                        this.ReviewerID,
                        this.FlowFormCode,
                    }, t => t.ID == this.ID);
                }
            }
        }
    }
}
