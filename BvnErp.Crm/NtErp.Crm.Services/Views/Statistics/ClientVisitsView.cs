using Layer.Data.Sqls;
using Needs.Linq;
using NtErp.Crm.Services.Models.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views.Statistics
{
    /// <summary>
    /// 统计客户拜访数
    /// </summary>
    public class ClientVisitsView : UniqueView<ClientVisit, BvCrmReponsitory>
    {
        #region 构造函数
        public ClientVisitsView()
        { 

        }

        string adminName;
        public ClientVisitsView(string name)
        {
            this.adminName = name;
        }

        internal ClientVisitsView(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        #endregion

        protected override IQueryable<ClientVisit> GetIQueryable()
        {
            var linq = from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ClientVisits>()
                       select new ClientVisit
                       {
                           ID = entity.ID,
                           AdminID = entity.AdminID,
                           DateIndex = entity.DateIndex,
                           Count = entity.Count,
                           CreateDate = entity.CreateDate
                       };

            if (string.IsNullOrEmpty(this.adminName))
            {
                return linq;
            }
            else
            {
                var admin = this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.AdminTopView>()
                    .SingleOrDefault(item => item.RealName == this.adminName);
                if (admin == null)
                {
                    throw new Exception($"未查询到指定用户【{this.adminName}】");
                }

                return linq.Where(item => item.AdminID == admin.ID);
            }
        }
    }
}
