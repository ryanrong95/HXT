using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Needs.Web
{
    abstract class HtmlCode
    {
        string html;

        protected HtmlCode()
        {
            string fileName = Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, nameof(HtmlCode), $"{this.GetType().Name}.html");
            if (!File.Exists(fileName))
            {
                fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nameof(HtmlCode), $"{this.GetType().Name}.html");
            }
            this.html = File.ReadAllText(fileName, Encoding.UTF8);
        }

        virtual protected string Produce()
        {
            var type = this.GetType();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            Regex regex = new Regex($"({string.Join("|", properties.Select(item => $@"\[{item.Name}\]"))})", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            return regex.Replace(this.html, delegate (Match match)
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
        }

        public string Execute()
        {
            return this.Produce();
        }
    }
}
