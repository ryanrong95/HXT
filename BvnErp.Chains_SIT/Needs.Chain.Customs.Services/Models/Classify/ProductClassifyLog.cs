using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 产品归类变更日志
    /// </summary>
    public class ProductClassifyLog : IUnique
    {
        #region 属性

        public string ID { get; set; }

        /// <summary>
        /// 归类产品ID
        /// </summary>
        public string ClassifyProductID { get; set; }

        /// <summary>
        /// 报关员
        /// </summary>
        public Admin Declarant { get; set; }

        /// <summary>
        /// Log类型
        /// </summary>
        public LogTypeEnums LogType { get; set; }

        /// <summary>
        /// 操作记录
        /// </summary>
        public string OperationLog { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public Status Stauts { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 摘要备注
        /// </summary>
        public string Summary { get; set; }

        #endregion

        public ProductClassifyLog()
        {
           this.UpdateDate = this.CreateDate = DateTime.Now;
           this.Stauts = Status.Normal;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //主键ID（ProductClassifyChangeLog +8位年月日+10位流水号）
                this.ID = ChainsGuid.NewGuidUp();
                reponsitory.Insert(this.ToLinq());
            }
        }
    }
}
