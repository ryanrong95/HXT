using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class SwapLimitCountryLog : IUnique, IPersist
    {
        #region 属性
        public string ID { get; set; }

        public string BankID { get; set; }

        /// <summary>
        /// 后台管理员
        /// </summary>
        public Admin Admin { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }
        #endregion
        public SwapLimitCountryLog()
        {
            this.CreateDate = DateTime.Now;
        }
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.ID = ChainsGuid.NewGuidUp();
                reponsitory.Insert(this.ToLinq());
            }
        }
    }
}
