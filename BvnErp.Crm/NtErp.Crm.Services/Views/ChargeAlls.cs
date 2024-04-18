using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class ChargeAlls : UniqueView<Charge, BvCrmReponsitory>
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        protected ChargeAlls()
        {

        }

        /// <summary>
        /// 获取费用数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Charge> GetIQueryable()
        {
            //人员视图
            AdminTopView topview = new AdminTopView(this.Reponsitory);
            //客户视图
            ClientAlls ClientView = new ClientAlls(this.Reponsitory);

            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Charges>()
                   join client in ClientView on entity.ClientID equals client.ID
                   join admin in topview on entity.AdminID equals admin.ID
                   select new Charge
                   {
                       ID = entity.ID,
                       ActionID = entity.ActionID,
                       Clients = client,
                       Admin = admin,
                       Name = entity.Name,
                       Count = entity.Count,
                       Price = entity.Price,
                       AdminID = entity.AdminID,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                   };
        }
    }
}
