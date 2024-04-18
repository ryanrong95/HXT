using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Settings
{
    public class AppSettingsView : View<AppSetting, ScCustomsReponsitory>
    {
        private string name;

        public AppSettingsView()
        {

        }

        public AppSettingsView(string name) : this()
        {
            this.name = name;
        }

        protected override IQueryable<AppSetting> GetIQueryable()
        {
            var query = from c in this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.AppSettings>()
                        join item in this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.AppSettingItems>() on c.ID equals item.AppSettingID into items
                        where c.Name == this.name
                        select new AppSetting
                        {
                            ID = c.ID,
                            Name = c.Name,
                            Enabled = c.Enabled,
                            IsComplex = c.IsComplex,
                            IsMultiValue = c.IsMultiValue,
                            Summary = c.Summary,
                            Items = new AppSettingItems(items.Select(item => new AppSettingItem
                            {
                                ID = item.ID,
                                AppSettingItemID = item.AppSettingID,
                                Key = item.Key,
                                Value = item.Value,
                                Enabled = item.Enabled,
                                SortNo = item.SortNo,
                                Summary = item.Summary
                            }))
                        };

            return query;
        }
    }
}
