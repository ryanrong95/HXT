using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 产品管控信息
    /// </summary>
    public class ProductControl : IUnique
    {
        #region 属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 管控类型： 商检、禁运
        /// </summary>
        public ControlType Type { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 制造商
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        #endregion

        /// <summary>
        /// 构造器，内部查询使用
        /// </summary>
        internal ProductControl()
        {
        }

        public ProductControl(string partNumber, ControlType type)
        {
            this.PartNumber = partNumber;
            this.Type = type;
        }

        #region 持久化

        public void Delete()
        {
            using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
            {
                reponsitory.Delete<Layers.Data.Sqls.PvData.ProductControls>(pc => pc.PartNumber == this.PartNumber && pc.Type == (int)this.Type);
            }
        }

        #endregion
    }
}
