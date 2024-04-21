using System.Linq;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// admin通用视图
    /// </summary>
    public class AdminsTopView : UniqueView<Admin, PvbCrmReponsitory>
    {
        public AdminsTopView()
        {

        }

        public AdminsTopView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Admin> GetIQueryable()
        {
            return new Yahv.Services.Views.AdminsAll<PvbCrmReponsitory>();
        }
    }
}