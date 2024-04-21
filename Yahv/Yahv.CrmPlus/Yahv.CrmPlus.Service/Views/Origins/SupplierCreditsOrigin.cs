using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Models.Origins.Rolls;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Models.Origins;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    /// <summary>
    /// 结算方式
    /// </summary>
    public class CreditsOrgin : Yahv.Linq.UniqueView<Credit, PvdCrmReponsitory>
    {
        internal CreditsOrgin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal CreditsOrgin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Credit> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var linq = from credit in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Credits>()
                       join enterprise1 in enterprisesView on credit.MakerID equals enterprise1.ID
                       join enterprise2 in enterprisesView on credit.TakerID equals enterprise2.ID
                
                       select new Credit
                       {
                           ID = credit.ID,
                           Maker = enterprise1,
                           Taker = enterprise2,
                           SiteUserID = credit.SiteuserID,
                           MakerID = credit.MakerID,
                           TakerID = credit.TakerID,
                           Type = (CreditType)credit.Type,
                           ClearType = (ClearType)credit.ClearType,
                           Months = credit.Months,
                           Days = credit.Days,
                           IsAvailable = credit.IsAvailable,
                           Summary = credit.Summary,
                           CreateDate = credit.CreateDate,
                           ModifyDate = credit.ModifyDate,
                       };
            return linq;
        }
    }
    /// <summary>
    /// 流水账
    /// </summary>
    public class FlowCreditsOrigin : Yahv.Linq.UniqueView<FlowCredit, PvdCrmReponsitory>
    {
        internal FlowCreditsOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal FlowCreditsOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<FlowCredit> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            var linq = from credit in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.FlowCredits>()
                       
                           //创建人
                       join creat_admin in adminsView on credit.CreatorID equals creat_admin.ID into creatAdmin
                       from  creator in creatAdmin.DefaultIfEmpty()
                       select new FlowCredit
                       {
                           ID = credit.ID,
                           MakerID = credit.MakerID,
                           TakerID = credit.TakerID,
                           Subject = credit.Subject,
                           Catalog = credit.Catalog,
                           Conduct = (ConductType)credit.Type,
                           Type = (CreditType)credit.Type,
                           Currency = (Currency)credit.Currency,
                           Summary = credit.Summary,
                           CreateDate = credit.CreateDate,
                           Price = credit.Price,
                           Creator= creator
                       };
            return linq;
        }
    }
    
}


