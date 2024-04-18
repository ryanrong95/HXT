using System.Text;
using System.Text.RegularExpressions;

namespace Needs.Web
{
    class EasyuiAlert : HtmlCode
    {
        public string Title { get; set; }
        public string Context { get; set; }
        public string Url { get; set; }
        public bool Closed { get; set; }
        public bool IsTopRedirect { get; set; }

        string header;
        /// <summary>
        /// html引入css script
        /// </summary>
        public string HeaderSrcContext
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.header))
                {
                    this.header = Needs.Settings.SettingsManager<Needs.Settings.IErpSrcContext>.Current.Easyui;
                }
                return this.header;
            }
            set
            {
                this.header = value;
            }
        }

        public EasyuiAlert()
        {

        }

        protected override string Produce()
        {
            string code = base.Produce();
            if (string.IsNullOrWhiteSpace(this.Url))
            {
                Regex regex = new Regex("<script>(.*?)</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                StringBuilder builder = new StringBuilder();
                foreach (Match match in regex.Matches(code))
                {
                    builder.Append(match.Groups[1].Value.Trim());
                }

                return builder.ToString();
            }
            else
            {
                return code;
            }
        }
    }
}
