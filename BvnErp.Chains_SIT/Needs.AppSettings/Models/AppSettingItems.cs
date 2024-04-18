using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Needs.Wl.Settings
{
    public class AppSettingItems : IEnumerable<AppSettingItem>
    {
        private readonly IList<AppSettingItem> list;

        public AppSettingItems()
        {

        }

        public AppSettingItems(IEnumerable<AppSettingItem> items)
        {
            this.list = items.ToList();
        }

        public AppSettingItem this[string key]
        {
            get
            {
                return this.list.Where(s => s.Key == key).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取序列中的元素数
        /// </summary>
        public int Count
        {
            get
            {
                return this.list.Count();
            }
        }

        public IEnumerator<AppSettingItem> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
