using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Wms.Services.Extends;
using System.ComponentModel.DataAnnotations;
using Yahv.Linq;
using Yahv.Linq.Persistence;
using Yahv.Usually;
using Layers.Data.Sqls;
using Layers.Data;
using Wms.Services.Views;
using Yahv.Underly;

namespace Wms.Services.Models
{
    /// <summary>
    /// 进项类
    /// </summary>
    public class Inputs : IUnique, IPersisting
    {

        #region 属性
        /// <summary>
        /// 唯一码，四位年+2位月+2日+6位流水
        /// </summary>
        public string ID { get; internal set; }

        /// <summary>
        /// 全局唯一码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// MainID(订单ID)
        /// </summary>
        public string OrderID { get; set; }

        public string TinyOrderID { get; set; }

        /// <summary>
        /// 项ID
        /// </summary>
        public string ItemID { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 所属企业
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 跟单员
        /// </summary>
        public string TrackerID { get; set; }

        /// <summary>
        /// 销售员ID（admin）
        /// </summary>
        public string SalerID { get; set; }

        /// <summary>
        /// 采购员ID
        /// </summary>
        public string PurchaserID { get; set; }

        /// <summary>
        /// 币种（保值）
        /// </summary>
        public Currency? Currency { get; set; }

        /// <summary>
        /// 单价（保值）
        /// </summary>
        public decimal? UnitPrice { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string DateCode { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string OriginID { get; set; }

        public string Origin { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string OriginDes { get {
                if (!string.IsNullOrEmpty(this.OriginID))
                {
                    return ((Yahv.Underly.Origin)int.Parse(this.OriginID)).GetDescription();
                }
                return "";
            } }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 收款人
        /// </summary>
        public string PayeeID { get; set; }

        /// <summary>
        /// 第三方收款人
        /// </summary>
        public string ThirdID { get; set; }


        #endregion


        #region 事件
        public event SuccessHanlder InputSuccess;
        public event ErrorHanlder EnterError;
        #endregion

        #region 持久化
        public void Enter()
        {
            try
            {
                using (var repository = new PvWmsRepository())
                {
                    //ID为空是新增
                    if (string.IsNullOrWhiteSpace(this.ID))
                    {
                        this.ID = PKeySigner.Pick(PkeyType.Inputs);
                        this.CreateDate = DateTime.Now;
                        repository.Insert(this.ToLinq());
                    }

                }

                InputSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
            catch (Exception ex)
            {
                EnterError?.Invoke(this, new ErrorEventArgs(ex.Message, ErrorType.System));
            }
        }

        public void Abandon()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
