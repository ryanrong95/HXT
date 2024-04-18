using Needs.Erp.Generic;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class PublicWarningClientView : ClientAlls
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        private PublicWarningClientView()
        {
        }

        /// <summary>
        /// 有参数构造函数
        /// </summary>
        /// <param name="admin"></param>
        public PublicWarningClientView(IGenericAdmin admin)
        {

        }

        /// <summary>
        /// 返回查询集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Client> GetIQueryable()
        {
            var linq1 = base.GetIQueryable().Where(item => item.Status == Enums.ActionStatus.Complete && item.IsSafe == Enums.IsProtected.Yes &&
                (DateTime.Now - item.UpdateDate).Days <= 7);

            var clientids = linq1.Select(item => item.ID);

            //最近有更新的联系人
            var contactclientid = (from contact in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Contacts>()
                                   join client in clientids on contact.ClientID equals client
                                   where (DateTime.Now - contact.UpdateDate).Days <= 7
                                   select client).ToArray();

            //最近有更新的费用
            var chargeclientid = (from charge in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Charges>()
                                  join client in clientids on charge.ClientID equals client
                                  where (DateTime.Now - charge.CreateDate).Days <= 7
                                  select client).ToArray();

            //最近有更新的客户跟踪记录
            var reportclientid = (from report in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Reports>()
                                  join client in clientids on report.ClientID equals client
                                  where (DateTime.Now - report.UpdateDate).Days <= 7
                                  select client).ToArray();

            //最近有更新的销售机会
            var projectclientid = (from project in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Projects>()
                                   join client in clientids on project.ClientID equals client
                                   where (DateTime.Now - project.UpdateDate).Days <= 7
                                   select client).ToArray();

            var updateclientid = contactclientid.Union(chargeclientid).Union(reportclientid).Union(projectclientid);

            return linq1.Where(item => !updateclientid.Contains(item.ID));
        }
    }
}
