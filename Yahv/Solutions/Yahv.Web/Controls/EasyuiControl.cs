using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Yahv.Web.Controls
{
    abstract public class EasyuiControl
    {
        static SortedDictionary<string, string> dictionary = new SortedDictionary<string, string>();

        static EasyuiControl()
        {
            Func<string, string> getHtml = (url) =>
            {
                using (WebClient client = new WebClient())
                {
                    return client.DownloadString(url);
                }
            };

            dictionary.Add(typeof(Easyui.Alert).FullName, getHtml($"{Yahv.Underly.DomainConfig.Fixed}/frontframe/customs-easyui/controls/alert.js"));
            dictionary.Add(typeof(Easyui.Redirect).FullName, getHtml($"{Yahv.Underly.DomainConfig.Fixed}/frontframe/customs-easyui/controls/Redirect.html"));

        }

        string html;

        public EasyuiControl()
        {
            this.html = dictionary[this.GetType().FullName];
        }

        public string Execute()
        {
            var type = this.GetType();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            Regex regex = new Regex($"({string.Join("|", properties.Select(item => $@"\[{item.Name}\]"))})", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            string html = regex.Replace(this.html, delegate (Match match)
            {
                var property = properties.SingleOrDefault(item => item.Name.Equals(match.Value.Trim('[', ']'), StringComparison.OrdinalIgnoreCase));

                if (property == null)
                {
                    return "";
                }

                var value = property.GetValue(this);

                if (value == null)
                {
                    return "";
                }

                if (property.PropertyType == typeof(bool))
                {
                    return value.ToString().ToLower();
                }

                return value.ToString();
            });

            return html;
        }
    }
}
