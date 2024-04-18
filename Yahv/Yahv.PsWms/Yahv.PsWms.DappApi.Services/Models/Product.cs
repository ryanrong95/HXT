using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Enums;

namespace Yahv.PsWms.DappApi.Services.Models
{
    public class Product : IUnique
    {
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Partnumber { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 封装
        /// </summary>
        public string Package { get; set; }

        /// <summary>
        /// 批次,存储的值 202050, 19+
        /// </summary>
        public string DateCode { get; set; }

        /// <summary>
        /// 最小包装量
        /// </summary>
        public int? Mpq { get; set; }

        /// <summary>
        /// 最小起订量
        /// </summary>
        public int? Moq { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }
        #endregion

        public Product()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (var repository = new Layers.Data.Sqls.PsWmsRepository())
            {
                if (!repository.ReadTable<Layers.Data.Sqls.PsWms.Products>().Any(t => t.ID == this.ID))
                {
                    this.ID = Layers.Data.PKeySigner.Pick(PKeyType.Product);

                    repository.Insert(new Layers.Data.Sqls.PsWms.Products()
                    {
                        ID = this.ID,
                        Partnumber = this.Partnumber,
                        Brand = this.Brand,
                        Package = this.Package,
                        DateCode = this.DateCode,
                        Mpq = this.Mpq,
                        Moq = this.Moq,
                        CreateDate = this.CreateDate,
                        ModifyDate = this.ModifyDate,
                    });
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PsWms.Products>(new
                    {
                        this.Partnumber,
                        this.Brand,
                        this.Package,
                        this.DateCode,
                        this.Mpq,
                        this.Moq,
                        ModifyDate = DateTime.Now,
                    }, t => t.ID == this.ID);
                }
            }
        }
    }
}
