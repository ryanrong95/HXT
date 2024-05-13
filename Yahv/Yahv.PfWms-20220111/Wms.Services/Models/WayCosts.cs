using Layers.Data;
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Extends;
using Yahv.Linq;
using Yahv.Linq.Persistence;
using Yahv.Underly;
using Yahv.Usually;

namespace Wms.Services.Models
{
    public class WayCosts : IUnique, IPersisting
    {

        #region 事件
        //Enter成功
        public event SuccessHanlder WayCostSuccess;
        //Enter失败
        public event ErrorHanlder WayCostFailed;
        #endregion

        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 运单编号
        /// </summary>
        public string WaybillID { get; set; }

        /// <summary>
        /// 科目（只能由我们公司规定并执行的科目）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 运单的价值
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { get; set; }

        #endregion

        #region 扩展属性
        public string CurrencyDes
        {
            get
            {
                return this.Currency.GetDescription();
            }
        }

        #endregion

        #region 方法
     
        public void Enter()
        {
            try
            {
                using (var repository = new PvWmsRepository())
                {
                    this.ID = PKeySigner.Pick(PkeyType.WayCosts);
                    this.CreateDate = DateTime.Now;
                    repository.Insert(this.ToLinq());
                }
                this.WayCostSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
            catch 
            {
                this.WayCostFailed?.Invoke(this, new ErrorEventArgs("Failed!!"));
            }
            
        }

        public void Abandon()
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
