using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Descriptions;

namespace NtErp.Crm.Services.Models
{
    /// <summary>
    /// 产品送样表
    /// </summary>
    public class Sample : Needs.Linq.IUnique
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 对应的产品ID
        /// </summary>
        public string ProductItemID { get; set; }

        /// <summary>
        /// 送样类型
        /// </summary>
        public SampleType Type { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 送样数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 送样日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contactor { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }
        #endregion

        public Sample()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        public Sample(string productItemID) : this()
        {
            this.ProductItemID = productItemID;
        }

        #region 持久化方法

        public void Enter()
        {
            using (var reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.ProductItemSamples
                    {
                        ID = this.ProductItemID,
                        Type = (int)this.Type,
                        UnitPrice = this.UnitPrice,
                        Quantity = this.Quantity,
                        TotalPrice = this.UnitPrice * this.Quantity,
                        Date = this.Date,
                        Contactor = this.Contactor,
                        Phone = this.Phone,
                        Address = this.Address,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.BvCrm.ProductItemSamples>(new
                    {
                        Type = (int)this.Type,
                        UnitPrice = this.UnitPrice,
                        Quantity = this.Quantity,
                        TotalPrice = this.UnitPrice * this.Quantity,
                        Date = this.Date,
                        Contactor = this.Contactor,
                        Phone = this.Phone,
                        Address = this.Address,
                        UpdateDate = DateTime.Now,
                    }, item => item.ID == this.ID);
                }
            }
        }
        #endregion
    }


    public enum SampleType
    {
        [Description("原厂赠送")]
        Origin = 1,

        [Description("公司赠送")]
        Company = 2,
    }
}
