using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.VcCsrm.Service.Models;

namespace YaHv.VcCsrm.Service.Views.Rolls
{
    public class WsAdmins : Yahv.Linq.QueryView<Models.Admin, PvcCrmReponsitory>
    {

        string shipID;

        public WsAdmins()
        {

        }
        protected override IQueryable<Models.Admin> GetIQueryable()
        {

            //读取关系 ,业务下的 shipID
            //获取关系where 角色
            //限定合作公司

            //var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Yahv.Services.Views.AdminsAll<PvcCrmReponsitory>(this.Reponsitory);
            return from admin in adminsView
                   select new Models.Admin
                   {
                       ID = admin.ID,
                       RealName = admin.RealName,
                       UserName = admin.UserName,
                       LastLoginDate = admin.LastLoginDate,
                       RoleID = admin.RoleID,
                       RoleName = admin.RoleName
                   };
        }
    }


}
