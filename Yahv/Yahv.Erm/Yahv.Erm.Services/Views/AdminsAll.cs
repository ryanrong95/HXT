using System.Collections.Generic;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Utils.Converters.Contents;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Yahv.Erm.Services.Views
{
    /// <summary>
    /// 管理员
    /// </summary>
    /// <remarks>Erp专用</remarks>
    public class AdminsAll : UniqueView<Admin, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminsAll()
        {
        }

        public AdminsAll(PvbErmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Admin> GetIQueryable()
        {
            var adminsView = new AdminsOrigin(this.Reponsitory);
            var rolesView = new RolesOrigin(this.Reponsitory);
            var staffView = new StaffsOrigin(this.Reponsitory);

            var result = from admin in adminsView
                         join _role in rolesView on admin.RoleID equals _role.ID into joinRole
                         from role in joinRole.DefaultIfEmpty()
                         join _staff in staffView on admin.StaffID equals _staff.ID into joinStaff
                         from staff in joinStaff.DefaultIfEmpty()
                         select new Admin()
                         {
                             ID = admin.ID,
                             Status = admin.Status,
                             RoleID = admin.RoleID,
                             RealName = admin.RealName,
                             StaffID = admin.StaffID,
                             CreateDate = admin.CreateDate,
                             SelCode = admin.SelCode,
                             UserName = admin.UserName,
                             UpdateDate = admin.UpdateDate,
                             RoleName = role.Name,
                             LastLoginDate = admin.LastLoginDate,
                             Password = admin.Password,
                             //StaffInfo = staff == null ? "添加用户" : (staff.Status == StaffStatus.Normal ? staff.ID : (staff.Status == StaffStatus.Departure ? "已离职" : (staff.Status == StaffStatus.Delete ? "已废弃" : ""))),
                             StaffStatus = staff.Status == null ? 0 : (int.Parse(staff.Status.ToString())),
                             StaffDyjCode = staff == null ? "" : staff.DyjCode
                         };

            return result;
        }

        /// <summary>
        /// 已重写 索引器
        /// </summary>
        /// <param name="id">唯一码</param>
        /// <returns>Partner</returns>
        public override Admin this[string id]
        {
            get
            {
                return this.SingleOrDefault(item => item.ID == id);
            }
        }

        /// <summary>
        /// 绑定职位
        /// </summary>
        /// <param name="adminid"></param>
        /// <param name="leagues"></param>
        public void BindingPosition(string adminid, League[] leagues)
        {
            var admin = this[adminid];
            this.Reponsitory.Delete<Layers.Data.Sqls.PvbErm.MapsLeague>(item => item.AdminID == admin.ID);
            if (leagues.Count() == 0)
            {
                return;
            }
            foreach (var league in leagues)
            {
                this.Reponsitory.Insert(new Layers.Data.Sqls.PvbErm.MapsLeague
                {
                    AdminID = admin.ID,
                    LeagueID = league.ID,
                });
            }
        }

        /// <summary>
        /// 绑定库房
        /// </summary>
        /// <param name="adminid"></param>
        /// <param name="ids">库房ID和子库房ID</param>
        public void BindingWareHouse(string adminid, string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
            {
                return;
            }

            var admin = this[adminid];
            this.Reponsitory.Delete<Layers.Data.Sqls.PvbErm.MapsWareHouse>(item => item.AdminID == admin.ID);
            foreach (dynamic obj in ids.JsonTo<dynamic>())
            {
                string id = string.Join("", admin.ID, obj.fatherId, obj.id.ToString());

                this.Reponsitory.Insert(new Layers.Data.Sqls.PvbErm.MapsWareHouse
                {
                    ID = id.MD5(),
                    AdminID = admin.ID,
                    WareHouseID = obj.fatherId,
                    ConsigneeID = obj.id,
                });
            }
        }

        /// <summary>
        /// 获取库房Ids
        /// </summary>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public string[] GetWareHouseIds(string adminId)
        {
            if (string.IsNullOrWhiteSpace(adminId))
            {
                return null;
            }
            var admin = this[adminId];
            return this.Reponsitory.ReadTable<MapsWareHouse>()
                .Where(item => item.AdminID == admin.ID)
                .Select(item => item.ConsigneeID)
                .ToArray();
        }

        /// <summary>
        /// 获取当前管理员的职位
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public string[] AdminPositions(string adminid)
        {
            var admin = this[adminid];
            var leagueids = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsLeague>().Where(item => item.AdminID == admin.ID).
                Select(item => item.LeagueID).ToArray();
            return leagueids;
        }

        /// <summary>
        /// 停用
        /// </summary>
        /// <param name="adminID">adminID</param>
        public void Disable(string adminID)
        {
            this.Reponsitory.Update<Layers.Data.Sqls.PvbErm.Admins>(new
            {
                Status = AdminStatus.Closed
            }, item => item.ID == adminID);
        }

        /// <summary>
        /// 根据角色获取管理员
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <param name="isCompose">是否包含合并角色，默认包含</param>
        /// <returns></returns>
        public IQueryable<Admin> GetByRoleID(string roleID, bool isCompose = true)
        {
            var roleIds_linq = from map in this.Reponsitory.ReadTable<MapsRoleCompose>()
                               where map.ChildID == roleID
                               select map.RoleID;

            var roleIds = roleIds_linq.Distinct().ToArray();

            return this.GetIQueryable().Where(item => item.RoleID == roleID || roleIds.Contains(item.RoleID));

        }
    }
}