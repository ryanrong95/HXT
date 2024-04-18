using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using YaHv.PvData.Services.Models;

namespace YaHv.PvData.Services.Views.Alls
{
    /// <summary>
    /// 归类历史记录
    /// </summary>
    public class Logs_ClassifiedPartNumberAll : UniqueView<Models.Log_ClassifiedPartNumber, PvDataReponsitory>
    {
        public Logs_ClassifiedPartNumberAll()
        {
        }

        internal Logs_ClassifiedPartNumberAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Log_ClassifiedPartNumber> GetIQueryable()
        {
            return new Origins.Logs_ClassifiedPartNumberOrigin(this.Reponsitory);
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <param name="months">月数</param>
        /// <returns></returns>
        public IQueryable<Models.Log_ClassifiedPartNumber> this[string partNumber, int months]
        {
            get
            {
                return this.Where(cpn => cpn.PartNumber == partNumber && cpn.CreateDate >= DateTime.Now.AddMonths(months));
            }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <param name="manufacturer">品牌</param>
        /// <param name="months">月数</param>
        /// <returns></returns>
        public IQueryable<Models.Log_ClassifiedPartNumber> this[string partNumber, string manufacturer, int months]
        {
            get
            {
                return this.Where(cpn => cpn.PartNumber == partNumber && cpn.Manufacturer == manufacturer && cpn.CreateDate >= DateTime.Now.AddMonths(months));
            }
        }
    }

    public class Logs_ClassifiedPartNumberView : Logs_ClassifiedPartNumberAll
    {
        private string PartNumber = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partNumber">产品型号</param>
        public Logs_ClassifiedPartNumberView(string partNumber)
        {
            this.PartNumber = partNumber.Trim();
        }

        protected override IQueryable<Log_ClassifiedPartNumber> GetIQueryable()
        {
            return base.GetIQueryable().Where(cpn => cpn.PartNumber == this.PartNumber);
        }
    }
}
