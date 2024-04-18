using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Services.Views;
using Yahv.Settings;
using Yahv.Utils.Http;

namespace Yahv.Models
{
    /// <summary>
    /// 组织机构
    /// </summary>
    public class League
    {
        public string ID { get; internal set; }

        public string Name { get; internal set; }

        public string EnterpriseID { get; internal set; }
    }

    /// <summary>
    /// 组织结构
    /// </summary>
    public class PersonalLeagues : IEnumerable<League>
    {
        public League Current
        {
            get
            {
                //读取cookies
                string token = Cookies.Current[SettingsManager<IAdminSettings>.Current.League];
                if (string.IsNullOrWhiteSpace(token))
                {
                    return null;
                }
                return this.data.SingleOrDefault(item => item.Name == token || item.ID == token);
            }
        }

        IEnumerable<League> data;

        internal PersonalLeagues(IEnumerable<League> data)
        {
            this.data = data;
            using (var view = new EnterprisesTopView<PvbCrmReponsitory>())
            {
                var names = data.Select(item => item.Name);
                var enterprises = view.Where(item => names.Contains(item.Name)).ToArray();

                foreach (var item in data)
                {
                    item.EnterpriseID = enterprises.SingleOrDefault(ep => ep.Name == item.Name)?.ID;
                }
            }

        }

        public IEnumerator<League> GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
