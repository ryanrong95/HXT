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
    /// 产品税务归类记录
    /// </summary>
    public class ProductTaxCategory : IUnique, IPersist
    {
        #region 属性

        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Name, this.TaxCode, this.TaxName).MD5();
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
        /// 型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        public DateTime CreateDate { get; set; }

        #endregion

        public ProductTaxCategory()
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
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductTaxCategories>().Count(item => item.ID == this.ID);

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