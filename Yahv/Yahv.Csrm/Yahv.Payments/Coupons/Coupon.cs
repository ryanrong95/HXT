using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.Payments
{
    /// <summary>
    /// 优惠券
    /// </summary>
    public class Coupon : IUnique
    {
        #region 属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 优惠券编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 优惠券类型：定额、据实
        /// </summary>
        public CouponType Type { get; set; }

        /// <summary>
        /// 业务
        /// </summary>
        public string Conduct { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string Catalog { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 币种，默认CNY
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 价值，定额优惠券的抵扣金额
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 一个订单最多可以使用该优惠券的数量
        /// </summary>
        public int? InOrderCount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { get; set; }
        public Services.Models.Admin Creator { get; set; }

        /// <summary>
        /// 状态，优惠券原则上只新增
        /// </summary>
        public GeneralStatus Status { get; set; }

        #endregion

        #region 事件

        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder DeleteSuccess;

        #endregion

        public Coupon()
        {
            this.CreateDate = DateTime.Now;
            this.Status = GeneralStatus.Normal;
        }

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
            {
                //新增优惠券
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Coupons>().Any(item => item.ID == this.ID))
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Coupons()
                    {
                        ID = PKeySigner.Pick(PKeyType.Coupon),
                        Name = this.Name,
                        Code = this.Code,
                        Type = (int)this.Type,
                        Conduct = this.Conduct,
                        Catalog = this.Catalog,
                        Subject = this.Subject,
                        Currency = (int)this.Currency,
                        Price = this.Price,
                        InOrderCount = this.InOrderCount,
                        CreateDate = DateTime.Now,
                        CreatorID = this.CreatorID,
                        Status = (int)GeneralStatus.Normal
                    });
                }
            }

            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        public void Delete()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.Coupons>(new
                {
                    Status = (int)GeneralStatus.Deleted
                }, item => item.ID == this.ID);
            }

            if (this != null && this.DeleteSuccess != null)
            {
                this.DeleteSuccess(this, new SuccessEventArgs(this));
            }
        }

        #endregion
    }


}
