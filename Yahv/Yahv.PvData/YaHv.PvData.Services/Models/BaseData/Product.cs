using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 标准产品
    /// </summary>
    public class Product : IUnique
    {
        #region 属性

        string id;
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID
        {
            get
            {
                //编码规则：品牌+制造商+封装+包装的MD5
                return this.id ?? string.Concat(this.PartNumber, this.Manufacturer, this.PackageCase, this.Packaging).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 制造商
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 封装
        /// </summary>
        public string PackageCase { get; set; }

        /// <summary>
        /// 包装
        /// </summary>
        public string Packaging { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        #endregion

        #region 事件

        public event SuccessHanlder EnterSuccess;

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
            {
                //添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvData.Products>().Any(t => t.ID == this.ID))
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvData.Products()
                    {
                        ID = this.ID,
                        PartNumber = this.PartNumber,
                        Manufacturer = this.Manufacturer,
                        PackageCase = this.PackageCase,
                        Packaging = this.Packaging,
                        CreateDate = DateTime.Now
                    });
                }
            }

            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        #endregion
    }
}
