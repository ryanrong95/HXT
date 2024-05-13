using Layers.Data;
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Wms.Services.Extends;
using Yahv.Linq;
using Yahv.Linq.Persistence;
using Yahv.Usually;

namespace Wms.Services.Models
{
    /// <summary>
    /// 拣货类
    /// </summary>
    public class Pickings : IUnique, IPersisting
    {
        #region 属性
        /// <summary>
        /// 唯一码，四位年+2位月+2日+6位流水
        /// </summary>
        public string ID { get;internal set; }

        /// <summary>
        ///库存ID 
        /// </summary>
        public string StorageID { get; set; }

        /// <summary>
        ///通知编号 
        /// </summary>
        public string NoticeID { get; set; }

        /// <summary>
        /// 装箱信息（箱号）
        /// </summary>
        public string BoxingCode { get; set; }

        /// <summary>
        /// 分拣数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 拣货人
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 创建时间(发生时间)
        /// </summary>
        public DateTime CreateDate { get; internal set; }

        /// <summary>
        /// 重量
        /// </summary>
        public decimal? Weight { get; set; }

        private decimal? netWeight;
        /// <summary>
        /// 净重(默认逻辑NetWeight = Weight * 0.7d)
        /// </summary>
        public decimal? NetWeight
        {
            get
            {
                
                if (this.netWeight == null)
                {
                    if (this.Weight == null)
                    {
                        this.netWeight = null;
                    }
                    else
                    {
                        this.netWeight = this.Weight * (decimal)0.7d;
                    }
                }
                return this.netWeight;
            }
            set
            {
                this.netWeight = value;
               
            }
        }

        /// <summary>
        /// 体积
        /// </summary>
        public decimal? Volume { get; set; }

        #endregion

        #region 事件
        public event SuccessHanlder PickingSuccess;
        public event ErrorHanlder EnterError;
        #endregion

        #region 持久化
        public void Enter()
        {
            try
            {
                using (var repository = new PvWmsRepository())
                {

                    this.ID = PKeySigner.Pick(PkeyType.Pickings);
                    this.CreateDate = DateTime.Now;
                    repository.Insert(this.ToLinq());

                }

                this.PickingSuccess?.Invoke(this, new SuccessEventArgs(this));

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
