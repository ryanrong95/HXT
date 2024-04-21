using Layers.Data;
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Payments.Views;
using Yahv.Underly;
using System.Collections;

namespace Yahv.Payments
{
    /// <summary>
    /// 优惠券使用关系
    /// </summary>
    public class UsedMap
    {
        public string ReceivableID { get; set; }
        public string CouponID { get; set; }
        public int Quantity { get; set; }
    }

    public class Coupons : MyCouponsView
    {
        public Coupons(string payer, string payee) : base(payer, payee)
        {

        }

        #region 索引器

        /// <summary>
        /// 获取指定ID的优惠券
        /// </summary>
        /// <param name="index">ID</param>
        /// <returns>优惠券</returns>
        public CouponDetail this[string index]
        {
            get
            {
                using (PvbCrmReponsitory reponsitory = new PvbCrmReponsitory())
                {
                    FlowCouponsView fview = new FlowCouponsView(this.payer, this.payee, reponsitory);
                    Views.Origins.CouponsOrigin cview = new Views.Origins.CouponsOrigin(reponsitory);

                    var conpon = cview.SingleOrDefault(item => item.ID == index);
                    var current = fview.Where(item => item.CouponID == index).OrderByDescending(item => item.CreateDate).FirstOrDefault();
                    return new CouponDetail(payer, payee, conpon, current);
                }
            }
        }

        /// <summary>
        /// 获取特定业务下指定名称的优惠券
        /// </summary>
        /// <param name="conduct">业务</param>
        /// <param name="name">优惠券名称</param>
        /// <returns></returns>
        public CouponDetail this[string conduct, string name]
        {
            get
            {
                using (PvbCrmReponsitory reponsitory = new PvbCrmReponsitory())
                {
                    //业务下优惠券的名称不能重复
                    FlowCouponsView fview = new FlowCouponsView(this.payer, this.payee, reponsitory);
                    Views.Origins.CouponsOrigin cview = new Views.Origins.CouponsOrigin(reponsitory);

                    var conpon = cview.SingleOrDefault(item => item.Name == name && item.Conduct == conduct);
                    var current = fview.Where(item => item.CouponID == conpon.ID).OrderByDescending(item => item.CreateDate).FirstOrDefault();
                    return new CouponDetail(payer, payee, conpon, current);
                }
            }
        }

        #endregion

        #region 消费优惠券

        /// <summary>
        /// 会员中心使用优惠券抵扣应收时调用
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="maps"></param>
        /// <remarks>
        /// 前端调用
        /// </remarks>
        public void Pay(string userID, params UsedMap[] maps)
        {
            this.Cost(userID: userID, maps: maps);
        }

        /// <summary>
        /// 管理端使用优惠券抵扣应收时调用
        /// </summary>
        /// <param name="adminID"></param>
        /// <param name="maps"></param>
        /// <remarks>
        /// 后台调用
        /// </remarks>
        public void Confirm(string adminID, params UsedMap[] maps)
        {
            this.Cost(adminID: adminID, maps: maps);
        }

        /// <summary>
        /// 消费优惠券
        /// </summary>
        /// <param name="adminID"></param>
        /// <param name="userID"></param>
        /// <param name="maps"></param>
        private void Cost(string adminID = null, string userID = null, params UsedMap[] maps)
        {
            if (maps == null)
                return;

            maps = maps.Where(map => !string.IsNullOrEmpty(map.CouponID)).ToArray();
            if (maps.Count() == 0)
                return;

            using (PvbCrmReponsitory reponsitory = new PvbCrmReponsitory())
            {
                #region 视图查询

                //应收
                var receivableIDs = maps.Select(map => map.ReceivableID).ToArray();
                var receivables = new ReceivablesView(reponsitory).Where(rb => receivableIDs.Contains(rb.ID)).ToArray();
                //实收
                var receiveds = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Receiveds>().Where(rd => receivableIDs.Contains(rd.ReceivableID)).ToArray();

                //优惠券
                var couponIDs = maps.Select(map => map.CouponID).Distinct().ToArray();
                var coupons = new Views.Origins.CouponsOrigin(reponsitory).Where(c => couponIDs.Contains(c.ID)).ToArray();
                //优惠券流水
                var flowCoupons = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.FlowCoupons>()
                                .Where(fc => fc.Payer == this.payer && fc.Payee == this.payee && couponIDs.Contains(fc.CouponID))
                                .OrderByDescending(fc => fc.CreateDate).ToArray();

                #endregion

                #region 数据校验

                foreach (var map in maps)
                {
                    var receivable = receivables.SingleOrDefault(rb => rb.ID == map.ReceivableID);
                    if (receivable == null)
                    {
                        throw new Exception($"应收款项{map.ReceivableID}不存在!");
                    }

                    //判断这个应收是否已经使用过优惠券，目前约定一个应收最多使用一张优惠券
                    int count = receiveds.Where(rd => rd.ReceivableID == map.ReceivableID && rd.CouponID != null).Count();
                    if (count > 0)
                    {
                        throw new Exception($"应收款项【{map.ReceivableID}】已经使用过优惠券, 不能重复抵扣!");
                    }

                    var coupon = coupons.SingleOrDefault(c => c.ID == map.CouponID);
                    if (coupon == null)
                    {
                        throw new Exception($"优惠券{map.CouponID}不存在!");
                    }

                    //判断这个应收是否可以使用这个优惠券, 优惠券只能用于抵扣相同科目的应收
                    if (receivable.Business != coupon.Conduct || receivable.Catalog != coupon.Catalog || receivable.Subject != coupon.Subject)
                    {
                        throw new Exception($"优惠券【{coupon.Name}】只能用于抵扣【{coupon.Subject}】的应收!");
                    }

                    var flow = flowCoupons.FirstOrDefault(fc => fc.CouponID == map.CouponID);
                    if (flow == null)
                    {
                        throw new Exception($"平台公司{this.payer}没有给客户{this.payee}分配优惠券{map.CouponID}!");
                    }

                    if (map.Quantity > 1)
                    {
                        throw new Exception($"应收{map.ReceivableID}最多使用一张优惠券{map.CouponID}!");
                    }

                    if (flow.Balance < map.Quantity)
                    {
                        throw new Exception($"优惠券【{map.CouponID}】的消费数量【{map.Quantity}】超出可用数量【{flow.Balance}】!");
                    }
                }

                #endregion

                #region 数据持久化

                foreach (var map in maps)
                {
                    var receivable = receivables.SingleOrDefault(rb => rb.ID == map.ReceivableID);
                    var coupon = coupons.SingleOrDefault(c => c.ID == map.CouponID);
                    var flow = flowCoupons.FirstOrDefault(fc => fc.CouponID == map.CouponID);

                    //优惠券流水累加
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowCoupons()
                    {
                        ID = PKeySigner.Pick(PKeyType.FlowCoupon),
                        Payer = this.payer,
                        Payee = this.payee,
                        CouponID = map.CouponID,
                        Input = 0,
                        Output = map.Quantity,
                        Balance = flow.Balance - map.Quantity,
                        CreateDate = DateTime.Now,
                        AdminID = adminID,
                        UserID = userID,
                    });

                    //调用王辉接口
                    PaymentManager.Npc.Received.For(receivable).Coupon(coupon);
                }

                #endregion
            }
        }

        #endregion
    }

    public class CouponDetail
    {
        #region 属性

        Coupon coupon;
        FlowCoupon flow;
        Receivable receivable;

        /// <summary>
        /// 优惠券的授予人
        /// </summary>
        /// <remarks>
        /// 应该是实实在在的企业信息对象
        /// </remarks>
        string payer;

        /// <summary>
        /// 优惠券的使用者
        /// </summary>
        string payee;

        public string Catalog { get { return this.coupon.Catalog; } }

        public string Subject { get { return this.coupon.Subject; } }

        public int Balance { get { return this.flow?.Balance ?? 0; } }

        #endregion

        #region 构造函数

        internal CouponDetail(string payer, string payee, Coupon coupon, FlowCoupon flow)
        {
            this.flow = flow;
            this.coupon = coupon;
            this.payee = payee;
            this.payer = payer;
        }

        #endregion

        #region 分配优惠券

        /// <summary>
        /// 收
        /// </summary>
        /// <param name="value"></param>
        /// <param name="summary"></param>
        /// <param name="adminID"></param>
        public void Grant(int value, string summary = null, string adminID = null)
        {
            value = Math.Abs(value);

            using (PvbCrmReponsitory reponsitory = new PvbCrmReponsitory())
            {
                //优惠券流水累加
                reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowCoupons()
                {
                    ID = PKeySigner.Pick(PKeyType.FlowCoupon),
                    Payer = this.payer,
                    Payee = this.payee,
                    CouponID = this.coupon.ID,
                    Input = value,
                    Output = 0,
                    Balance = this.Balance + value,
                    CreateDate = DateTime.Now,
                    AdminID = adminID,
                    UserID = null,
                    Summary = summary
                });
            }
        }

        #endregion
    }
}
