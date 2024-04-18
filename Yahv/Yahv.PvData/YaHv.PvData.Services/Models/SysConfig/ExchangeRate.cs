using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Linq.Persistence;
using Yahv.Underly;
using Yahv.Usually;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 汇率
    /// </summary>
    public class ExchangeRate : IFormings
    {
        #region 属性

        /// <summary>
        /// 汇率类型 （浮动汇率，手动设置汇率）
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        public District District { get; set; }

        /// <summary>
        /// From Currency
        /// </summary>
        public Currency From { get; set; }

        /// <summary>
        /// To Currency
        /// </summary>
        public Currency To { get; set; }

        /// <summary>
        /// 汇率值
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// 预设汇率的启用时间
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? ModifyDate { get; set; }

        #endregion

        #region 事件

        public event SuccessHanlder EnterSuccess;

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
            {
                var views = new Views.Alls.ExchangeRatesAll(reponsitory);
                bool isExist = views.Any(item => item.Type == this.Type && item.From == this.From && item.To == this.To && item.District == this.District);

                if (isExist)
                {
                    reponsitory.Update<Layers.Data.Sqls.PvData.ExchangeRates>(new
                    {
                        Value = this.Value,
                        StartDate = this.StartDate,
                        ModifyDate = DateTime.Now
                    }, item => item.Type == this.Type && item.From == (int)this.From && item.To == (int)this.To && item.District == (int)this.District);
                }
                else
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvData.ExchangeRates
                    {
                        District = (int)this.District,
                        From = (int)this.From,
                        To = (int)this.To,
                        Type = this.Type,
                        Value = this.Value,
                        StartDate = this.StartDate,
                        ModifyDate = DateTime.Now
                    });
                }
            }
        }

        /// <summary>
        /// 删除汇率
        /// </summary>
        public void Delete()
        {
            using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
            {
                var views = new Views.Alls.ExchangeRatesAll(reponsitory);
                bool isExist = views.Any(item => item.Type == this.Type && item.From == this.From && item.To == this.To && item.District == this.District);

                if (isExist)
                {
                    reponsitory.Delete<Layers.Data.Sqls.Overalls.ExchangeRates>(item => item.Type == this.Type && item.From == (int)this.From && item.To == (int)this.To && item.District == (int)this.District);
                }
            }
        }

        #endregion
    }
}
