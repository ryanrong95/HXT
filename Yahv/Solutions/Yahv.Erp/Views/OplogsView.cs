using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;

namespace Yahv.Views
{
    public class OplogsView : UniqueView<Yahv.Models.Oplogs, PvbErmReponsitory>
    {
        protected override IQueryable<Yahv.Models.Oplogs> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Oplogs>()
                   join admin in new AdminsAlls(this.Reponsitory) on entity.AdminID equals admin.ID
                   select new Yahv.Models.Oplogs
                   {
                       CreateDate = entity.CreateDate,
                       Operation = entity.Operation,
                       Remark = entity.Remark,
                       Sys = entity.Sys,
                       Type = entity.Type,
                       Url = entity.Url,
                       Admin = new Models.ErpAdmin
                       {
                           ID = admin.ID,
                           RealName = admin.RealName,
                           UserName = admin.UserName,
                           Role = admin.Role,
                           StaffID = admin.StaffID,
                           Status = admin.Status,
                       }
                   };
        }
    }
}
