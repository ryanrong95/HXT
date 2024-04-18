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
    public class ProductClassifyChangeLog : IUnique
    {
        #region 属性

        public string ID { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 报关员
        /// </summary>
        public Admin Declarant { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 摘要备注
        /// </summary>
        public string Summary { get; set; }

        #endregion

        public ProductClassifyChangeLog()
        {
            this.CreateDate = DateTime.Now;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //主键ID（ProductClassifyChangeLog +8位年月日+10位流水号）
                this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.ProductClassifyChangeLog);
                reponsitory.Insert(this.ToLinq());
            }
        }
    }
}
