using Needs.Linq;
using Needs.Utils.Converters;
using NtErp.Wss.Oss.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Models
{
    /// <summary>
    /// 产品
    /// </summary>
    public class StandardProduct : IUnique, IPersist
    {
        public StandardProduct()
        {

        }

        #region 属性

        string id;

        /// <summary>
        /// 销项ID
        /// </summary>
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Name, this.Manufacturer.Name,
                     this.PackageCase, this.Packaging, this.Batch, this.DateCode).MD5();
            }
            set
            {
                this.id = value;
            }
        }
        /// <summary>
        /// 标记编码 [产品编码]
        /// </summary>
        public string SignCode { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 封装
        /// </summary>
        public string PackageCase { get; set; }
        /// <summary>
        /// 包装
        /// </summary>
        public string Packaging { get; set; }
        /// <summary>
        /// 批次
        /// </summary>
        public string Batch { get; set; }
        public string DateCode { get; set; }
        /// <summary>
        /// 产品描述
        /// </summary>
        public string Description { get; set; }

        #endregion

        #region 扩展属性

        public Company Manufacturer { get; set; }

        #endregion 

        #region 持久化

        public void Enter()
        {
            this.Manufacturer.Enter();

            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.CvOss.StandardProducts>().Count(item => item.ID == this.ID);

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

        #endregion
    }
}
