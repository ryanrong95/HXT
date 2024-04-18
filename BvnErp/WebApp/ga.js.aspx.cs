using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp
{
    public partial class ga_js : Needs.Web.Forms.ClientPage
    {
        public ga_js()
        {
            //throw new Exception();
            this.PreRender += Ga_js_PreRender;
        }

        private void Ga_js_PreRender(object sender, EventArgs e)
        {
            Response.ClearContent();
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                base.Render(hw);
                string str = sb.ToString();//这里的即时body里面的html源码

                StringBuilder sbhtml = new StringBuilder();
                //去除html 与 script 代码
                Regex reg_script = new Regex("<script.*?>(.*?)</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                //去除结尾注释
                Regex reg_line1 = new Regex(@";[\s]{0,}//.*?$", RegexOptions.Multiline);
                //去除单行注释
                Regex reg_line2 = new Regex(@"^\s{0,}//.*?$", RegexOptions.Multiline);
                string repstr = string.Join("|", "{}[]();=+-,.<>/?:\"|\\*&^%$#@!~`"
                    .Select(item => Regex.Escape(item.ToString())));
                Regex reg_r = new Regex(string.Concat(@"\s{0,}(", repstr, @")\s{0,}"), RegexOptions.Singleline);
                Regex reg_s = new Regex(@"\s+", RegexOptions.Singleline);

                foreach (Match match in reg_script.Matches(str))
                {
                    var txt = match.Groups[1].Value;
                    if (txt.Length == 0)
                    {
                        continue;
                    }
                    txt = reg_line2.Replace(txt, "");
                    txt = reg_line1.Replace(txt, ";");
                    txt = reg_r.Replace(txt, "$1");
                    txt = reg_s.Replace(txt, " ");

                    sbhtml.Append(txt);
                }
                string html = sbhtml.ToString();
                Response.Write(html);
            }

            Response.End();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            //this.Title = Request["menu"];

            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/javascript";

            Response.Buffer = true;
            Response.ExpiresAbsolute = System.DateTime.Now.AddSeconds(-1);
            Response.Expires = 0;
            Response.CacheControl = "no-cache";

            if (!IsPostBack)
            {
                this.Model = this.readunites();
            }
        }


        #region setting

        protected object writeMenu()
        {
            string json = Request["menu"].Replace("&quot;", "\"");
            JObject obj = JObject.Parse(json);

            var menu = obj.ToObject<NtErp.Services.Models.Menu>();

            var father = new NtErp.Services.Views.MenusAlls().SingleOrDefault(t => t.Name == menu.FatherID && t.FatherID == null);
            if (father == null)
            {
                father = new NtErp.Services.Models.Menu
                {
                    Name = menu.FatherID
                };
                father.Enter();
            }
            menu.FatherID = father.ID;
            menu.Enter();
            return true;
        }


        /// <summary>
        /// 颗粒化写入
        /// </summary>
        protected object roleunites()
        {
            string menu = Request["menu"];
            string url = Request["url"];
            string json = Request["sets"];
            json = json.Replace("&quot;", "\"");
            NtErp.Services.Models.RoleUnite[] arry = new NtErp.Services.Models.RoleUnite[0];

            if (!string.IsNullOrWhiteSpace(json))
            {
                JArray arr = JArray.Parse(json);
                arry = arr.ToObject<NtErp.Services.Models.RoleUnite[]>();

            }

            // 现有数据集合
            var list = new NtErp.Services.Views.UnitesAllsView().Where(item => item.Menu == menu && item.Url == url).ToArray();
            // 需要新增的
            var inserts = arry.Except(list);
            // 需要删除的
            var deletes = list.Except(arry);
            // abandon
            foreach (var item in deletes)
            {
                item.Abandon();
            }
            // enter
            foreach (var item in inserts)
            {
                item.Enter();
            }
            return true;
        }
        /// <summary>
        /// RoleUnites 配置
        /// </summary>
        /// <returns></returns>
        protected JArray readunites()
        {
            var url = Request["rawurl"];
            url = Regex.Replace(url, @"\?.*?$", "", RegexOptions.Singleline).ToLower();
            var admin = Needs.Erp.ErpPlot.Current;



            //在错误一定要修改

            if (admin == null)
            {
                return new JArray();
            }




            var list = admin.Plots.MyUnites.Where(item => item.Url == url).ToArray();

            var jarry = new JArray();
            if (list != null && list.Length > 0)
            {
                jarry = JArray.FromObject(list);
            }

            return jarry;
        }
        #endregion
    }
}