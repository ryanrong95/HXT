using System;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 承兑调拨右表
    /// </summary>
    public class AcceptanceRight : IUnique
    {
        #region 数据库属性
        public string ID { get; internal set; }
        public string AcceptanceLeftID { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 流水表ID
        /// </summary>
        public string FlowID { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var repository = LinqFactory<PvFinanceReponsitory>.Create())
            {
                if (string.IsNullOrEmpty(this.ID))
                {
                    this.ID = PKeySigner.Pick(PKeyType.AcceptanceRights);
                    repository.Insert(new Layers.Data.Sqls.PvFinance.AcceptanceRights()
                    {
                        ID = this.ID,
                        AcceptanceLeftID = this.AcceptanceLeftID,
                        Price = this.Price,
                        FlowID = this.FlowID,
                        CreatorID = this.CreatorID,
                        CreateDate = DateTime.Now,
                    });
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PvFinance.AcceptanceRights>(new
                    {
                        AcceptanceLeftID = this.AcceptanceLeftID,
                        Price = this.Price,
                        FlowID = this.FlowID,
                    }, item => item.ID == this.ID);
                }
            }
        }
        #endregion
    }
}