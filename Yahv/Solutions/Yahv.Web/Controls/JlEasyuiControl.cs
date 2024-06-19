using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Yahv.Web.Controls
{
    abstract public class JlEasyuiControl
    {
        static SortedDictionary<string, string> dictionary = new SortedDictionary<string, string>();

        static JlEasyuiControl()
        {
            Func<string, string> getHtml = (url) =>
            {
                using (WebClient client = new WebClient())
                {
                    return client.DownloadString(url);
                }
            };

            dictionary.Add(typeof(Easyui.AutoAlert).FullName,
                getHtml($"{Underly.DomainConfig.Fixed}/frontframe/customs-easyui/controls/autoAlert.js"));

            dictionary.Add(typeof(Easyui.AutoRedirect).FullName,
                getHtml($"{Underly.DomainConfig.Fixed}/frontframe/customs-easyui/controls/autoRedirect.html"));

            dictionary.Add(typeof(Easyui.AutoCloseDialog).FullName,
                getHtml($"{Underly.DomainConfig.Fixed}/frontframe/customs-easyui/controls/autoCloseDialog.html"));
            dictionary.Add(typeof(Easyui.AutoCloseWindow).FullName,
                getHtml($"{Underly.DomainConfig.Fixed}/frontframe/customs-easyui/controls/autoCloseWindow.html"));

            dictionary.Add(typeof(Easyui.TopCloseDialog).FullName,
             getHtml($"{Underly.DomainConfig.Fixed}/frontframe/customs-easyui/controls/topCloseDialog.html"));

            dictionary.Add(typeof(Easyui.TopCloseWindow).FullName,
                getHtml($"{Underly.DomainConfig.Fixed}/frontframe/customs-easyui/controls/topCloseWindow.html"));


            //新对话框的内容中使用的js
            dictionary.Add(typeof(Easyui.jsTopWindow).FullName,
                getHtml($"{Underly.DomainConfig.Fixed}/frontframe/customs-easyui/controls/topContentWindow.js"));
            dictionary.Add(typeof(Easyui.jsTopDialog).FullName,
                getHtml($"{Underly.DomainConfig.Fixed}/frontframe/customs-easyui/controls/topContentDialog.js"));
        }

        string html;

        public JlEasyuiControl()
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
