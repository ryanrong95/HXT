using System.Linq;
using Layers.Data.Sqls;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 客户受益人
    /// </summary>
    //public class MyBeneficaryView : WsBeneficiariesTopView<PvWsOrderReponsitory>
    //{
    //    private string EnterpriseID;

    //    private MyBeneficaryView()
    //    {

    //    }

    //    /// <summary>
    //    /// 构造函数
    //    /// </summary>
    //    /// <param name="enterpriseid">客户ID</param>
    //    /// <param name="business"></param>
    //    public MyBeneficaryView(string enterpriseid)
    //    {
    //        this.EnterpriseID = enterpriseid;
    //    }

    //    /// <summary>
    //    /// 查询结果集
    //    /// </summary>
    //    /// <returns></returns>
    //    protected override IQueryable<Beneficiary> GetIQueryable()
    //    {
    //        return base.GetIQueryable().Where(item => item.EnterpriseID == this.EnterpriseID);
    //    }
    //}
}
