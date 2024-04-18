using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 税务归类
    /// </summary>
    public class TaxCategory : IUnique, IPersist
    {
        #region 属性

        string id;
        public string ID
        {
            get
            {
                return this.id ?? this.TaxCode;
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWords { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否电子产品
        /// </summary>
        public bool IsElectronic { get; set; }

        public DateTime CreateDate { get; set; }

        #endregion

        public TaxCategory()
        {
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TaxCategories>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }
        }
    }
}