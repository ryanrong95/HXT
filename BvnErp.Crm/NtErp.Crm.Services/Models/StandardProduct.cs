using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using Needs.Erp.Generic;
using NtErp.Crm.Services.Extends;
using Needs.Utils.Descriptions;
using Needs.Utils.Converters;
using Needs.Underly;

namespace NtErp.Crm.Services.Models
{
    [Needs.Underly.FactoryView(typeof(Views.StandardProductAlls))]
    public partial class StandardProduct : IUnique
    {
        #region 属性
        string id;
        /// <summary>
        /// ID
        /// </summary>
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Name).MD5();
            }
            set
            {
                this.id = value;
            }
        }
        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin
        {
            get; set;
        }
        /// <summary>
        /// 型号
        /// </summary>
        public string Name
        {
            get; set;
        }
        /// <summary>
        /// 品牌
        /// </summary>
        public Company Manufacturer
        {
            get; set;
        }

        public string ManufacturerID { get; set; }

        /// <summary>
        /// 包装
        /// </summary>
        public string Packaging
        {
            get; set;
        }
        /// <summary>
        /// 封装
        /// </summary>
        public string PackageCase
        {
            get; set;
        }
        /// <summary>
        /// 批次
        /// </summary>
        public string Batch
        {
            get; set;
        }
        /// <summary>
        /// 封装批次
        /// </summary>
        public string DateCode
        {
            get; set;
        }
        /// <summary>
        /// 新建日期
        /// </summary>
        public DateTime CreateDate
        {
            get; set;
        }
        #endregion

        public event SuccessHanlder EnterSuccess;

        #region 持久化
        /// <summary>
        /// 数据保存触发事件
        /// </summary>
        public void Enter()
        {
            this.OnEnter();

            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 数据保存到数据库
        /// </summary>
        protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                //判定当前数据是否在数据库存在
                int count = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.StandardProducts>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.StandardProducts
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Origin = this.Origin,
                        ManufacturerID = this.Manufacturer.ID,
                        Packaging = this.Packaging,
                        PackageCase = this.PackageCase,
                        Batch = this.Batch,
                        DateCode = this.DateCode,
                        CreateDate = DateTime.Now
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.BvCrm.StandardProducts
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Origin = this.Origin,
                        ManufacturerID = this.Manufacturer.ID,
                        Packaging = this.Packaging,
                        PackageCase = this.PackageCase,
                        Batch = this.Batch,
                        DateCode = this.DateCode
                    }, item => item.ID == this.ID);
                }

            }
        }

        #endregion
    }
}
