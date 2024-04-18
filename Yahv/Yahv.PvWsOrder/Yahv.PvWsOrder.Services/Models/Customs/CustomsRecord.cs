using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Models.Customs
{
    /// <summary>
    /// 清关记录
    /// </summary>
    public class CustomsRecord : Yahv.Linq.IUnique
    {
        #region 属性
        public string ID { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }
        /// <summary>
        /// 运单ID
        /// </summary>
        public string WaybillID { get; set; }
        /// <summary>
        /// 清关费
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 到货时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 确认时间
        /// </summary>
        public DateTime? ConfirmDate { get; set; }
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvWsOrderReponsitory>.Create())
            {
                if (
                    reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.CustomsRecords>()
                        .Any(item => item.WaybillID == this.WaybillID))
                {
                    throw new Exception("您已经确认清关费用!");
                }

                reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.CustomsRecords()
                {
                    ID = PKeySigner.Pick(PKeyType.CustomsRecords),
                    WaybillID = this.WaybillID,
                    ClientID = this.ClientID,
                    ConfirmDate = this.ConfirmDate,
                    CreateDate = DateTime.Now,
                    Price = this.Price,
                });
            }
        }
        #endregion
    }

}
