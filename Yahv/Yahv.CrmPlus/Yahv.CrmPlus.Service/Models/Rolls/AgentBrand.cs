using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Linq;
using Yahv.Usually;

namespace Yahv.CrmPlus.Service.Models.Rolls
{
    //public class AgentBrand : nBrand
    //{
    //    /// <summary>
    //    /// 我方公司
    //    /// </summary>
    //    public string CompanyName { internal set; get; }
    //    /// <summary>
    //    /// 生产供应商
    //    /// </summary>
    //    public string SupllierName { internal set; get; }
    //    /// <summary>
    //    /// 生产关系
    //    /// </summary>
    //    public nBrand Produce { set; get; }


    //    #region 持久化
    //    override public void Enter()
    //    {
    //        using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
    //        {
    //            var exist = new Views.Rolls.AgentBrandsRoll().Where(item => item.Type == Underly.CrmPlus.nBrandType.Agent 
    //            && item.BrandID==this.BrandID            
    //            &&item.EnterpriseID==this.EnterpriseID
    //            &&item.Produce.EnterpriseID==this.Produce.EnterpriseID);
    //            if (exist.Any())
    //            {
    //                this.Fire(new ErrorEventArgs());
    //                return;
    //            }
    //            else
    //            {
    //                this.ID = Guid.NewGuid().ToString("N");
    //                this.Produce.Enter();
    //                base.Enter();
    //                //reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.nBrands
    //                //{
    //                //    ID = this.ID,
    //                //    Type = (int)this.Type,
    //                //    BrandID = this.BrandID,
    //                //    EnterpriseID = this.EnterpriseID,
    //                //    CreatorID = this.CreatorID,
    //                //    Summary = this.Summary
    //                //});
    //            }
    //            this.Fire(new SuccessEventArgs(this));
    //        }
    //    }
    //    override public void Abandon()
    //    {
    //        using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
    //        {
    //            reponsitory.Delete<Layers.Data.Sqls.PvdCrm.nBrands>(item => item.ID == this.ID);
    //            reponsitory.Delete<Layers.Data.Sqls.PvdCrm.vBrands>(item => item.BrandID == this.BrandID);
    //            this.Fire(new AbandonedEventArgs(this));
    //        }
    //    }
    //    #endregion
    //}
}
