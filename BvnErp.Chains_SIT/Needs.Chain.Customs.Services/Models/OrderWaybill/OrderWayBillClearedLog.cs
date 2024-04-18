using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 已清关记录，点一键清关才会插入DB
    /// </summary>
    public class OrderWayBillClearedLog
    {
        /// <summary>
        /// ID GUID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 运单号
        /// </summary>
        public string WaybillCode { get; set; }
        /// <summary>
        /// 币制
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public Enums.Status Status { get; set; }
        /// <summary>
        /// 创新日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime UpdataDate { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { get; set; }

        public OrderWayBillClearedLog()
        {
            this.Status = Enums.Status.Normal;
            this.CreateDate = DateTime.Now;
            this.UpdataDate = DateTime.Now;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.OrderWayBillClearedLogs>(new Layer.Data.Sqls.ScCustoms.OrderWayBillClearedLogs
                {
                    ID = ChainsGuid.NewGuidUp(),
                    WaybillCode = this.WaybillCode,
                    Currency = this.Currency,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdataDate,
                    Summary = this.Summary
                });
            }
        }
    }
}
