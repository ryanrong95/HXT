using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvRoute.Services.Enums;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.PvRoute.Services.Models
{
    /// <summary>
    /// 账单
    /// </summary>
    public class Bill : IUnique
    {

        #region 事件

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;

        #endregion

        #region 数据库属性
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 面单编号
        /// </summary>
        public string FaceOrderID { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 承运商
        /// </summary>
        public PrintSource Carrier { get; set; }

        /// <summary>
        /// 费用明细
        /// </summary>
        public string FeeDetail { get; set; }

        /// <summary>
        /// 核对人
        /// </summary>
        public string Checker { get; set; }

        /// <summary>
        /// 核对时间
        /// </summary>
        public DateTime? CheckTime { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public string Reviewer { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? ReviewTime { get; set; }

        /// <summary>
        /// 出纳人
        /// </summary>
        public string Cashier { get; set; }

        /// <summary>
        /// 出纳时间
        /// </summary>
        public DateTime? CashierTime { get; set; }

        /// <summary>
        /// 所属期号
        /// </summary>
        public int DateIndex { get; set; }

        /// <summary>
        /// 来源：我方记录  承运商记录
        /// </summary>
        public RecordSource Source { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvRouteReponsitory>.Create())
            {
                //添加,满足根据ID查不到值 并且满足 根据面单编号和对应的信息说明查不到值的时候去添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvRoute.Bills>().Any(item => item.ID == this.ID))
                {
                    this.ID = PKeySigner.Pick(Yahv.PvRoute.Services.Enums.PKeyType.Bill);
                    var now = DateTime.Now;
                    reponsitory.Insert(new Layers.Data.Sqls.PvRoute.Bills()
                    {
                        ID = this.ID,
                        FaceOrderID = this.FaceOrderID,
                        Quantity = this.Quantity,
                        Weight = this.Weight,
                        Price = this.Price,
                        Currency = (int)this.Currency,
                        Carrier = (int)this.Carrier,
                        FeeDetail = this.FeeDetail,
                        Checker = this.Checker,
                        CheckTime = this.CheckTime,
                        Reviewer = this.Reviewer,
                        ReviewTime = this.ReviewTime,
                        Cashier = this.Cashier,
                        CashierTime = this.CashierTime,
                        DateIndex = this.DateIndex,
                        Source = (int)this.Source,
                        CreateDate = now,
                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvRoute.Bills>(new
                    {
                        FaceOrderID = this.FaceOrderID,
                        Quantity = this.Quantity,
                        Weight = this.Weight,
                        Price = this.Price,
                        Currency = (int)this.Currency,
                        Carrier = (int)this.Carrier,
                        FeeDetail = this.FeeDetail,
                        Checker = this.Checker,
                        CheckTime = this.CheckTime,
                        Reviewer = this.Reviewer,
                        ReviewTime = this.ReviewTime,
                        Cashier = this.Cashier,
                        CashierTime = this.CashierTime,
                        DateIndex = this.DateIndex,
                        Source = (int)this.Source,
                    }, item => item.ID == this.ID);
                }

                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }
        #endregion

    }
}
