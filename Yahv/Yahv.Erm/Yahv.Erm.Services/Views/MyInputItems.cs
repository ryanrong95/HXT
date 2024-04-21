using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly.Erps;

namespace Yahv.Erm.Services.Views
{
    /// <summary>
    /// 我的录入项
    /// </summary>
    public class MyInputItems : UniqueView<WageItem, PvbErmReponsitory>
    {
        private IErpAdmin _admin;

        public MyInputItems(IErpAdmin admin)
        {
            this._admin = admin;
        }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<WageItem> GetIQueryable()
        {
            var admins = new AdminsOrigin(this.Reponsitory);
            var wageitems = new WageItemsOrigin(this.Reponsitory);
            var maps = this.Reponsitory.ReadTable<MapsItem>();

            return from wageitem in wageitems
                   join admin in admins on wageitem.AdminID equals admin.ID
                   join inputer in admins on wageitem.InputerId equals inputer.ID into joinInputer
                   from inputer in joinInputer.DefaultIfEmpty()
                   where wageitem.Status != Status.Delete && wageitem.InputerId == _admin.ID
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
                       Formula = wageitem.Formula,
                       Description = wageitem.Description,
                       IsImport = wageitem.IsImport,
                       InputerId = wageitem.InputerId,
                       InputerName = inputer.RealName,
                       StaffIds = maps.Where(s => s.WageItemID == wageitem.ID).Select(t => t.StaffID).ToArray()
                   };
        }


        /// <summary>
        /// 获取我的工资项下的所有员工Id
        /// </summary>
        /// <returns></returns>
        public string[] GetMyAllStaffIds()
        {
            var myItems = this.Select(item => item.ID).ToArray();

            //获取我的工资项所有员工
            return this.Reponsitory.ReadTable<MapsItem>()
               .Where(item => myItems.Contains(item.WageItemID))
               .Select(item => item.StaffID).Distinct().ToArray();
        }

        /// <summary>
        /// 获取我的工资项下的员工Id
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string[]> GetMyStaffIds()
        {
            Dictionary<string, string[]> result = new Dictionary<string, string[]>();

            //获取我的所有工资项
            var myWageItems =
                this.Reponsitory.ReadTable<WageItems>()
                    .Where(item => item.Status != (int)Status.Delete && item.InputerId == _admin.ID);

            if (myWageItems.Any())
            {
                var myItems = this.Select(item => item.ID).ToArray();

                foreach (var item in myWageItems)
                {
                    //根据工资项名称获取工资项ID
                    var wageItemId = this.Reponsitory.ReadTable<WageItems>().FirstOrDefault(t => t.Name == item.Name)?.ID;
                    var list = this.Reponsitory.ReadTable<MapsItem>()
                       .Where(t => t.WageItemID == wageItemId)
                       .Select(t => t.StaffID).ToArray();

                    if (!result.Keys.Contains(item.Name))
                    {
                        result.Add(item.Name, list);
                    }
                }
            }

            return result;
        }
        }
}
