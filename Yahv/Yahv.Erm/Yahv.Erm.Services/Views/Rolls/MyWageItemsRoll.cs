using System.Collections.Generic;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 我的工资项
    /// </summary>
    public class MyWageItemsRoll : UniqueView<StaffWageItem, PvbErmReponsitory>
    {
        private string _staffId;

        public MyWageItemsRoll(string staffId)
        {
            _staffId = staffId;
        }

        public MyWageItemsRoll()
        {
        }

        protected override IQueryable<StaffWageItem> GetIQueryable()
        {
            var staffView = new StaffsOrigin(this.Reponsitory);

            var result = from maps in Reponsitory.ReadTable<MapsWageItem>()
                         join s in staffView on maps.StaffID equals s.ID
                         join w in Reponsitory.ReadTable<WageItems>() on maps.WageItemID equals w.ID
                         select new StaffWageItem()
                         {
                             ID = maps.StaffID,
                             WageItemID = maps.WageItemID,
                             DefaultValue = maps.DefaultValue,
                             WageItemName = w.Name,
                             Description = w.Description,
                             Formula = w.Formula,
                             Type = (WageItemType)w.Type,
                             CalcOrder = w.CalcOrder,
                             IsImport = w.IsImport,
                             Status = (Status)w.Status,
                         };

            if (!string.IsNullOrWhiteSpace(_staffId))
            {
                result = result.Where(item => item.ID == _staffId);
            }

            return result;
        }

        /// <summary>
        /// 根据员工ID清空工资列默认值
        /// </summary>
        /// <param name="staffId"></param>
        public void Delete(string staffId)
        {
            if (!string.IsNullOrWhiteSpace(staffId))
            {
                this.Reponsitory.Delete<MapsWageItem>(t => t.StaffID == staffId);
            }
        }

        /// <summary>
        /// 根据岗位初始化工资项
        /// </summary>
        /// <param name="staffId">员工Id</param>
        /// <param name="postionId">岗位Id</param>
        public void InitWageItems(string staffId, string postionId)
        {
            if (!string.IsNullOrWhiteSpace(postionId) && !string.IsNullOrWhiteSpace(staffId))
            {
                this.Delete(staffId);

                //根据岗位读取工资项
                var array =
                    this.Reponsitory.ReadTable<MapsPostion>()
                        .Where(item => item.PostionID == postionId)
                        .Select(t => new
                        {
                            StaffID = staffId,
                            WageItemID = t.WageItemID,
                            DefaultValue = 0
                        }).ToList();

                //批量插入数据库
                this.Reponsitory.Insert(array.Select(item => new MapsWageItem()
                {
                    StaffID = item.StaffID,
                    WageItemID = item.WageItemID,
                    DefaultValue = 0
                }).ToArray());
            }
        }

        /// <summary>
        /// 更新员工工资项
        /// </summary>
        /// <param name="staffId">员工ID</param>
        /// <param name="postionId">岗位ID</param>
        /// <param name="DefaultValue">金额</param>
        public void UpdateWageItem(string staffId, string wageItemId, decimal DefaultValue)
        {
            if (!string.IsNullOrWhiteSpace(wageItemId) && !string.IsNullOrWhiteSpace(staffId))
            {
                //更新数据
                this.Reponsitory.Update<MapsWageItem>(new
                {
                    DefaultValue = DefaultValue
                }, item => item.StaffID == staffId && item.WageItemID == wageItemId);
            }
        }
    }
}