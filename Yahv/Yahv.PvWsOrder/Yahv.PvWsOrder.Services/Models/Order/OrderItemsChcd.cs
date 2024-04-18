using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.Usually;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 归类
    /// </summary>
    public class OrderItemsChcd : IUnique
    {
        internal OrderItemsChcd()
        {
            this.CreateDate = DateTime.Now;
        }

        #region 属性

        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 自动归类
        /// </summary>
        public string AutoHSCodeID { get; set; }

        /// <summary>
        /// 自动归类时间
        /// </summary>
        public DateTime? AutoDate { get; set; }

        /// <summary>
        /// 一次归类管理员
        /// </summary>
        public string FirstAdminID { get; set; }
        public Yahv.Services.Models.Admin FirstAdmin { get; set; }

        /// <summary>
        /// 一次归类
        /// </summary>
        public string FirstHSCodeID { get; set; }

        /// <summary>
        /// 一次归类时间
        /// </summary>
        public DateTime? FirstDate { get; set; }

        /// <summary>
        /// 二次归类管理员
        /// </summary>
        public string SecondAdminID { get; set; }
        public Yahv.Services.Models.Admin SecondAdmin { get; set; }

        /// <summary>
        /// 二次归类
        /// </summary>
        public string SecondHSCodeID { get; set; }

        /// <summary>
        /// 二次归类时间
        /// </summary>
        public DateTime? SecondDate { get; set; }

        /// <summary>
        /// 客户要求：海关编码
        /// </summary>
        public string CustomHSCodeID { get; set; }

        /// <summary>
        /// 客户要求：税务编码,可能是税务编码，也可能是税务名称
        /// </summary>
        public string CustomTaxCode { get; set; }

        /// <summary>
        /// 系统（包涵：人工处理）的价格
        /// </summary>
        public string SysPriceID { get; set; }

        /// <summary>
        /// 报关后，海关系统反馈的
        /// </summary>
        public string CustomsPriceID { get; set; }

        /// <summary>
        /// 本地增值后的价格体系
        /// </summary>
        public string VATaxedPriceID { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifyDate { get; set; }

        #endregion

        #region 事件

        #endregion

        #region 持久化

        public void Enter()
        {
            using (Layers.Data.Sqls.PvWsOrderReponsitory Reponsitory = new Layers.Data.Sqls.PvWsOrderReponsitory())
            {
                //保存订单
                var entity = new Views.Origins.OrderItemsChcdOrigin().Where(item => item.ID == this.ID).FirstOrDefault();
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
                    if (!(this.AutoHSCodeID == entity.AutoHSCodeID && this.AutoDate == entity.AutoDate &&
                        this.FirstAdminID == entity.FirstAdminID && this.FirstHSCodeID == entity.FirstHSCodeID && this.FirstDate == entity.FirstDate &&
                        this.SecondAdminID == entity.SecondAdminID && this.SecondHSCodeID == entity.SecondHSCodeID && this.SecondDate == entity.SecondDate &&
                        this.CustomHSCodeID == entity.CustomHSCodeID &&
                        this.CustomTaxCode == entity.CustomTaxCode && this.SysPriceID == entity.SysPriceID && this.CustomsPriceID == entity.CustomsPriceID &&
                        this.VATaxedPriceID == entity.VATaxedPriceID ))
                    {
                        //添加日志Log_OrderItemsTerm
                        Reponsitory.Insert(entity.ToLinqLog());
                        //更新数据
                        Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>(new
                        {
                            AutoHSCodeID = this.AutoHSCodeID,
                            AutoDate = this.AutoDate,
                            FirstAdminID = this.FirstAdminID,
                            FirstHSCodeID = this.FirstHSCodeID,
                            FirstDate = this.FirstDate,
                            SecondAdminID = this.SecondAdminID,
                            SecondHSCodeID = this.SecondHSCodeID,
                            SecondDate = this.SecondDate,
                            CustomHSCodeID = this.CustomHSCodeID,
                            CustomTaxCode = this.CustomTaxCode,
                            SysPriceID = this.SysPriceID,
                            CustomsPriceID = this.CustomsPriceID,
                            VATaxedPriceID = this.VATaxedPriceID,
                            ModifyDate = DateTime.Now,
                        }, item => item.ID == this.ID);
                    }
                }
            }
        }

        #endregion
    }
}
