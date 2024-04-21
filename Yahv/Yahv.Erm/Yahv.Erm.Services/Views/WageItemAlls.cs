using Yahv.Linq;
using Yahv.Erm.Services.Models.Origins;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Views.Origins;

namespace Yahv.Erm.Services.Views
{
    /// <summary>
    /// 工资项
    /// </summary>
    public class WageItemAlls : UniqueView<WageItem, PvbErmReponsitory>
    {
        public WageItemAlls()
        {
        }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<WageItem> GetIQueryable()
        {
            var admins = new AdminsOrigin(this.Reponsitory);
            var wageitems = new WageItemsOrigin(this.Reponsitory);

            return from wageitem in wageitems
                   join admin in admins on wageitem.AdminID equals admin.ID
                   join inputer in admins on wageitem.InputerId equals inputer.ID into joinInputer
                   from inputer in joinInputer.DefaultIfEmpty()
                   where wageitem.Status != Status.Delete
                   select new WageItem
                   {
                       ID = wageitem.ID,
                       Name = wageitem.Name,
                       AdminID = wageitem.AdminID,
                       OrderIndex = wageitem.OrderIndex,
                       CreateDate = wageitem.CreateDate,
                       Status = wageitem.Status,
                       AdminName = admin.RealName,
                       Type = wageitem.Type,
                       CalcOrder = wageitem.CalcOrder,
                       Formula = wageitem.Formula.Replace("&gt;", ">").Replace("&lt;", "<"),
                       Description = wageitem.Description,
                       IsImport = wageitem.IsImport,
                       InputerId = wageitem.InputerId,
                       InputerName = inputer.RealName,
                   };
        }

        public override WageItem this[string id]
        {
            get { return this.SingleOrDefault(item => item.ID == id); }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        public void Delete(string[] ids)
        {
            this.Reponsitory.Update<Layers.Data.Sqls.PvbErm.WageItems>(new
            {
                Status = Status.Delete,
            }, item => ids.Contains(item.ID));
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="OrderIndexs"></param>
        public void Order(string[] ids, string[] OrderIndexs)
        {
            for (int i = 0; i < ids.Count() - 1; i++)
            {
                if (string.IsNullOrWhiteSpace(OrderIndexs[i]))
                {
                    continue;
                }
                int index = int.Parse(OrderIndexs[i]);
                this.Reponsitory.Update<Layers.Data.Sqls.PvbErm.WageItems>(new
                {
                    OrderIndex = index,
                }, item => item.ID == ids[i]);
            }
        }


        /// <summary>
        /// 工资项绑定员工
        /// </summary>
        /// <param name="itemId">工资项ID</param>
        /// <param name="staffIds"></param>
        public void BindStaffs(string itemId, string[] staffIds)
        {
            if (string.IsNullOrWhiteSpace(itemId) || staffIds.Length <= 0)
            {
                return;
            }

            //根据工资表获取已经绑定的员工
            var staffs_binded =
                this.Reponsitory.ReadTable<MapsItem>()
                    .Where(item => item.WageItemID == itemId)
                    .Select(item => item.StaffID)
                    .ToArray();

            var list = staffIds.Where(item => !staffs_binded.Contains(item)).Select(item => new MapsItem()
            {
                WageItemID = itemId,
                StaffID = item,
            }).ToArray();

            this.Reponsitory.Insert(list);
        }

        /// <summary>
        /// 工资项解绑员工
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="staffIds"></param>
        public void UnbindStaffs(string itemId, string[] staffIds)
        {
            if (string.IsNullOrWhiteSpace(itemId) || staffIds.Length <= 0)
            {
                return;
            }

            //根据工资表获取已经绑定的员工
            var staffs_binded =
                this.Reponsitory.ReadTable<MapsItem>()
                    .Where(item => item.WageItemID == itemId)
                    .Select(item => item.StaffID)
                    .ToArray();

            var list = staffIds.Where(item => staffs_binded.Contains(item));

            foreach (var l in list)
            {
                this.Reponsitory.Delete<MapsItem>(item => item.WageItemID == itemId && item.StaffID == l);
            }
        }

        /// <summary>
        /// 获取工资项的员工列表
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public string[] GetStaffs(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId)) return null;

            return
                this.Reponsitory.ReadTable<MapsItem>()
                    .Where(item => item.WageItemID == itemId)
                    .Select(item => item.StaffID)
                    .ToArray();
        }
    }
}