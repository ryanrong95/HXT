using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 快递公司
    /// </summary>
    public class ExpressCompany : Carrier
    {
        /// <summary>
        /// 账号名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 账号密码
        /// </summary>
        public string CustomerPwd { get; set; }

        /// <summary>
        /// 月结账号
        /// </summary>
        public string MonthCode { get; set; }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExpressCompanys>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ExpressCompanys
                    {
                        ID = this.ID,
                        CustomerName = this.CustomerName,
                        CustomerPwd = this.CustomerPwd,
                        MonthCode = this.MonthCode,
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.ExpressCompanys
                    {
                        CustomerName = this.CustomerName,
                        CustomerPwd = this.CustomerPwd,
                        MonthCode = this.MonthCode,
                    }, item => item.ID == this.ID);
                }
            }
            this.OnEnterSuccess();
        }
    }
}