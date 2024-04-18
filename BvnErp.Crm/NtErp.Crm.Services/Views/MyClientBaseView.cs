using Layer.Data.Sqls;
using Needs.Erp.Generic;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class MyClientBaseView : ClientAlls
    {
        //人员对象
        AdminTop Admin;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        private MyClientBaseView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">当前管理员</param>
        public MyClientBaseView(IGenericAdmin admin)
        {
            this.Admin = Extends.AdminExtends.GetTop(admin.ID);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员对象</param>
        public MyClientBaseView(AdminTop admin, BvCrmReponsitory reponsitory) : base(reponsitory)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 获取客户集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Client> GetIQueryable()
        {
            var clients = base.GetIQueryable().Where(item => item.IsSafe == Enums.IsProtected.Yes);

            if (Admin.JobType == Enums.JobType.Sales)
            {
                //获取所有员工
                var mystaffs = new MyStaffsView(this.Admin, Reponsitory);

                clients = from maps in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>()
                          join Client in clients on maps.ClientID equals Client.ID
                          join mystaff in mystaffs on maps.AdminID equals mystaff.ID
                          select Client;
            }
            if (Admin.JobType == Enums.JobType.FAE)
            {
                var mystaffs = new MyStaffsView(this.Admin, Reponsitory);

                clients = from manumap in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsAdmin>()
                          join maps in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>() on manumap.ManufactureID equals maps.ManufacturerID
                          join Client in clients on maps.ClientID equals Client.ID
                          join mystaff in mystaffs on maps.AdminID equals mystaff.ID
                          select Client;
            }

            return clients.Distinct();
        }


        public IQueryable<Admin>  GetMyClientAdmin()
        {
            var clients = this.GetIQueryable();
            return from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>()
                   join client in clients on map.ClientID equals client.ID
                   join admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.AdminTopView>()
                   on map.AdminID equals admin.ID
                   select new Admin
                   {
                       ID = admin.ID,
                       UserName = admin.UserName,
                       RealName = admin.RealName,
                       UpdateDate = admin.UpdateDate,
                       CreateDate = admin.CreateDate,
                       Summary = admin.Summary,
                   };
        }
    }
}
