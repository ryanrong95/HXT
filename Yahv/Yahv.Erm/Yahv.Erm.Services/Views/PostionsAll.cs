using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views
{
    /// <summary>
    /// 所有岗位
    /// </summary>
    public class PostionsAll : UniqueView<Postion, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PostionsAll()
        { }

        protected override IQueryable<Postion> GetIQueryable()
        {
            var adminsView = new AdminsOrigin(this.Reponsitory);
            var postionsAll = new PostionsOrigin(this.Reponsitory);

            var result = from postion in postionsAll
                         join _admin in adminsView on postion.AdminID equals _admin.ID into joinAdmin
                         from admin in joinAdmin.DefaultIfEmpty()
                         where postion.Status == Status.Normal
                         select new Postion()
                         {
                             Status = postion.Status,
                             Name = postion.Name,
                             ID = postion.ID,
                             CreateDate = postion.CreateDate,
                             AdminID = postion.AdminID,
                             AdminName = admin.RealName,
                         };

            return result;
        }

        /// <summary>
        /// 已重写 索引器
        /// </summary>
        /// <param name="id">唯一码</param>
        /// <returns>Partner</returns>
        public override Postion this[string id]
        {
            get
            {
                return this.SingleOrDefault(item => item.ID == id);
            }
        }

        /// <summary>
        /// 岗位分配工资项
        /// </summary>
        /// <param name="PositionID"></param>
        /// <param name="wages"></param>
        public void BindingWages(string PositionID, WageItem[] wages)
        {
            var position = this[PositionID];
            //删除原来绑定
            this.Reponsitory.Delete<Layers.Data.Sqls.PvbErm.MapsPostion>(item => item.PostionID == position.ID);

            if (wages.Count() == 0)
            {
                return;
            }
            foreach (var wageitem in wages)
            {
                this.Reponsitory.Insert(new Layers.Data.Sqls.PvbErm.MapsPostion
                {
                    PostionID = position.ID,
                    WageItemID = wageitem.ID,
                });
            }
        }

        /// <summary>
        /// 获取岗位的工资项
        /// </summary>
        /// <param name="PositionID"></param>
        public string[] PositionWages(string PositionID)
        {
            var position = this[PositionID];
            var wageids = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsPostion>().
                Where(item => item.PostionID == position.ID).Select(item => item.WageItemID).ToArray();
            return wageids;
        }

        /// <summary>
        /// 工资项绑定值
        /// </summary>
        /// <param name="PositionID"></param>
        /// <param name="wages"></param>
        public void SetWageValue(string WageItem, string StaffID, decimal Value)
        {
            if (Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsWageItem>().Any(item => item.StaffID == StaffID && item.WageItemID == WageItem))
            {
                this.Reponsitory.Delete<Layers.Data.Sqls.PvbErm.MapsWageItem>(item => item.StaffID == StaffID && item.WageItemID == WageItem);
            }
            this.Reponsitory.Insert(new Layers.Data.Sqls.PvbErm.MapsWageItem
            {
                WageItemID = WageItem,
                StaffID = StaffID,
                DefaultValue = Value,
            });
        }

        /// <summary>
        /// 判断是否需要填入past表
        /// </summary>
        /// <param name="PositionID"></param>
        /// <param name="wages"></param>
        public bool WageCheckchange(string WageItem, string StaffID, decimal Value)
        {
            decimal? oldvalue = 0;
            if (Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsWageItem>().Any(item => item.StaffID == StaffID && item.WageItemID == WageItem))
            {
                oldvalue = Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsWageItem>()
                    .Where(item => item.StaffID == StaffID && item.WageItemID == WageItem).Select(item => item.DefaultValue).FirstOrDefault();
                if (oldvalue != Value)
                {
                    return true;
                }
            }
            return false;

        }

        /// <summary>
        /// 判断是否需要填入past表
        /// </summary>
        /// <param name="PositionID"></param>
        /// <param name="wages"></param>
        public decimal? WageOldvalue(string WageItem, string StaffID)
        {
            decimal? oldvalue = 0;
            if (Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsWageItem>().Any(item => item.StaffID == StaffID && item.WageItemID == WageItem))
            {
                oldvalue = Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsWageItem>()
                    .Where(item => item.StaffID == StaffID && item.WageItemID == WageItem).Select(item => item.DefaultValue).FirstOrDefault();
            }
            return oldvalue;

        }

        /// <summary>
        /// 清空工资项
        /// </summary>
        /// <param name="PositionID"></param>
        /// <param name="wages"></param>
        public decimal? ClearWageValue(string WageItem, string StaffID)
        {
            decimal? oldvalue = 0;
            if (Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsWageItem>().Any(item => item.StaffID == StaffID && item.WageItemID == WageItem))
            {
                oldvalue = Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsWageItem>()
                    .Where(item => item.StaffID == StaffID && item.WageItemID == WageItem).Select(item => item.DefaultValue).FirstOrDefault();
                this.Reponsitory.Delete<Layers.Data.Sqls.PvbErm.MapsWageItem>(item => item.StaffID == StaffID && item.WageItemID == WageItem);
                return oldvalue;
            }
            return null;
        }

        /// <summary>
        /// 根据岗位批量更新员工工资项
        /// </summary>
        public void RefreshStaffWageItem(string positionId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(positionId))
                {
                    return;
                }

                string sql = @"EXEC	[dbo].[P_UpdateMapsWageItem]
		                            @postionId={0}";

                Reponsitory.Command(sql, positionId);
            }
            catch (System.Exception ex)
            {

            }
        }
    }
}