using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Settings
{
    public class AppSettingItemsView : View<AppSettingItem, ScCustomsReponsitory>
    {
        protected override IQueryable<AppSettingItem> GetIQueryable()
        {
            var query = from c in this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.AppSettingItems>()
                        orderby c.SortNo ascending
                        select new AppSettingItem
                        {
                            ID = c.ID,
                            AppSettingItemID = c.AppSettingID,
                            Key = c.Key,
                            Value = c.Value,
                            Enabled = c.Enabled,
                            SortNo = c.SortNo,
                            Summary = c.Summary
                        };

            return query.Where(this.Predicate);
        }
    }
}
