using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Extends;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 附加税率
    /// </summary>
    public class OrderItemsTerm : IUnique
    {
        public OrderItemsTerm()
        {

        }

        #region 属性

        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 原产地附加税率
        /// </summary>
        public decimal OriginRate { get; set; }

        /// <summary>
        /// 强制增值（0.2%）（海关的附加费率）
        /// </summary>
        public decimal FVARate { get; set; }

        /// <summary>
        /// CCC
        /// </summary>
        public bool Ccc { get; set; }

        /// <summary>
        /// 禁运
        /// </summary>
        public bool Embargo { get; set; }

        /// <summary>
        /// 香港管制
        /// </summary>
        public bool HkControl { get; set; }

        /// <summary>
        /// 原产地证明
        /// </summary>
        public bool Coo { get; set; }

        /// <summary>
        /// 商检
        /// </summary>
        public bool CIQ { get; set; }

        /// <summary>
        /// 商检费
        /// </summary>
        public decimal CIQprice { get; set; }

        /// <summary>
        /// 高价值
        /// </summary>
        public bool IsHighPrice { get; set; }

        /// <summary>
        /// 是否需要消毒
        /// </summary>
        public bool IsDisinfected { get; set; }

        #endregion

        #region 事件
        #endregion

        #region 持久化
        public void Enter()
        {
            using (Layers.Data.Sqls.PvWsOrderReponsitory Reponsitory = new Layers.Data.Sqls.PvWsOrderReponsitory())
            {
                //保存订单
                var entity = new Views.Origins.OrderItemTermsOrigin().Where(item => item.ID == this.ID).FirstOrDefault();
                if (entity == null)
                {
                    if (string.IsNullOrEmpty(this.ID))
                    {
                        throw new Exception("订单项ID为空，无法保存海关归类信息");
                    }
                    Reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    if (!(this.OriginRate == entity.OriginRate && this.FVARate == entity.FVARate && this.Ccc == entity.Ccc &&
                        this.Embargo == entity.Embargo && this.HkControl == entity.HkControl && this.Coo == entity.Coo &&
                        this.CIQ == entity.CIQ && this.CIQprice == entity.CIQprice && this.IsHighPrice == entity.IsHighPrice &&
                        this.IsDisinfected == entity.IsDisinfected))
                    {
                        //添加日志Log_OrderItemsTerm
                        Reponsitory.Insert(entity.ToLinqLog());
                        //更新数据
                        Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>(new
                        {
                            OriginRate = this.OriginRate,
                            FVARate = this.FVARate,
                            Ccc = this.Ccc,
                            Embargo = this.Embargo,
                            HkControl = this.HkControl,
                            Coo = this.Coo,
                            CIQ = this.CIQ,
                            CIQprice = this.CIQprice,
                            IsHighPrice = this.IsHighPrice,
                            IsDisinfected = this.IsDisinfected,
                        }, item => item.ID == this.ID);
                    }
                }
            }
        }

        public void UpdateCcc()
        {
            using (Layers.Data.Sqls.PvWsOrderReponsitory Reponsitory = new Layers.Data.Sqls.PvWsOrderReponsitory())
            {
                Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>(new
                {
                    Ccc = this.Ccc,
                }, item => item.ID == this.ID);
            }
        }
        #endregion
    }
}
