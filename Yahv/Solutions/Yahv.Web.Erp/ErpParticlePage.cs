using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;

namespace Yahv.Web.Erp
{
    /// <summary>
    ///  Erp 颗粒化 页面基类
    /// </summary>
    public class ErpParticlePage : ErpSsoPage
    {
        public ErpParticlePage()
        {
            this.Load += ErpParticlePage_Load;
        }

        private void ErpParticlePage_Load(object sender, EventArgs e)
        {
            #region 判断权限
            if (Yahv.Erp.Current.IsSuper)
            {
                return;
            }

#if DEBUG
            if (!Permissions.IsHavePermission(Yahv.Erp.Current.PermissionSettings, Request.Url.AbsoluteUri))
            {
                throw new Exception("您没有权限！");
            }
#endif
            #endregion
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //把最终要输出的html压缩后再输出
            StringWriter htmlWriter = new StringWriter();
            HtmlTextWriter tw = new HtmlTextWriter(htmlWriter);
            base.Render(tw);
            string outhtml = htmlWriter.ToString().Trim();

            List<Models.Label> list = new List<Models.Label>();

            #region 表格颗粒化

            //为方便用户
            Regex regex_table = new Regex(@"<th\s+.*?data-options="".*?field:'(.*?)'.*?"".*?\>(.*?)</th>"
                , RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection matches_table = regex_table.Matches(outhtml);


            foreach (Match match in matches_table)
            {
                var lebal = new Models.Label
                {
                    jField = match.Groups[1].Value.Trim(),
                    Name = match.Groups[2].Value.Trim(),
                    Type = "table"
                };

                //添加数据
                list.Add(lebal);
            }

            #endregion

            #region 标签颗粒化

            //Name: '添加按钮',jField: 'btnCreator'
            Regex regex_role = new Regex(@"<(?<HtmlTag>[\w]+)[^>]*?particle=""([^""]+)""[^>]*?>((?<Nested><\k<HtmlTag>[^>]*>)|</\k<HtmlTag>>(?<-Nested>)|.*?)*",
                    RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection matches_role = regex_role.Matches(outhtml);

            foreach (Match match in matches_role)
            {
                var value = Regex.Unescape(match.Groups[1].Value.Trim());
                var jobject = JObject.Parse($"{{{value}}}");
                list.Add(new Models.Label
                {
                    jField = (jobject["jField"] ?? JValue.CreateString("")).Value<string>(),
                    Name = (jobject["Name"] ?? JValue.CreateString("")).Value<string>(),
                    Type = (jobject["Type"] ?? JValue.CreateString("tags")).Value<string>()
                });
            }

            #endregion

            using (var allView = new Views.ParticlesAll())
            {
                string lower = Request.Url.AbsolutePath.ToLower();
                string md5 = lower.MD5();

                allView.Enter(new Models.Particle
                {
                    ID = md5,
                    Url = lower,
                    Type = "default",
                    UrlCode = md5,
                    Context = list.Where(item => !string.IsNullOrWhiteSpace(item.Name)).Json()
                });
            }

            #region 颗粒化隐藏

            using (Views.ParticleSettingsRoll settings = new Views.ParticleSettingsRoll(Yahv.Erp.Current.Role))
            {
                var setting = settings[Request.Url.AbsolutePath];

                if (setting != null && !string.IsNullOrWhiteSpace(setting.Context))
                {
                    var jarry = JArray.Parse(setting.Context);

                    outhtml = regex_table.Replace(outhtml, (match) =>
                    {
                        if (jarry.Any(item => item["jField"].Value<string>() == match.Groups[1].Value
                            && item["Type"].Value<string>() == "table"
                            && item["IsShow"]?.Value<bool?>() == false))
                        {
                            return "";
                        }
                        return match.Value;
                    });
                    Regex regex_style = new Regex("style=\"(.*?)\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);

                    HashSet<string> toTexts = new HashSet<string>();

                    outhtml = regex_role.Replace(outhtml, (match) =>
                    {
                        var value = Regex.Unescape(match.Groups[1].Value.Trim());
                        var jobject = JObject.Parse($"{{{value}}}");
                        var type = jobject["Type"]?.Value<string>();
                        //var jField = (jobject["jField"] ?? JValue.CreateString("")).Value<string>();
                        var Name = (jobject["Name"] ?? JValue.CreateString("")).Value<string>();

                        #region 标签处理
                        if (jarry.Any(item => item["Name"].Value<string>() == Name
                              && item["Type"].Value<string>() == "tags"
                              && item["IsShow"]?.Value<bool?>() == false))
                        {
                            string tag = match.Value;

                            var style_match = regex_style.Match(tag);
                            if (string.IsNullOrWhiteSpace(style_match.Value))
                            {
                                tag = $"{tag.TrimEnd('>')} style=\"display: none;\">";
                            }
                            else
                            {
                                string style = $"{style_match.Value.TrimEnd('"')};display: none;\"";
                                tag = regex_style.Replace(tag, style);
                            }

                            return tag;
                        }

                        #endregion

                        #region input 处理

                        if (jarry.Any(item => item["Name"].Value<string>() == Name
                            && item["Type"].Value<string>().Contains("input")
                            && item["IsEdit"]?.Value<bool?>() == false))
                        {
                            string tag = match.Value;
                            if ("input" == type)
                            {
                                var style_match = regex_style.Match(tag);
                                if (string.IsNullOrWhiteSpace(style_match.Value))
                                {
                                    tag = $"{tag.TrimEnd('>')} style=\"display: none;\">";
                                }
                                else
                                {
                                    string style = $"{style_match.Value.TrimEnd('"')};display: none;\"";
                                    tag = regex_style.Replace(tag, style);
                                }
                            }

                            if (type.StartsWith("easyui"))
                            {
                                tag = $"{tag.TrimEnd('>')} {type}=\"true\">";
                                toTexts.Add(type);
                            }

                            return tag;
                        }

                        #endregion

                        return match.Value;
                    });

                    //增加需要隐层的easyui的控件
                    if (toTexts.Count > 0)
                    {
                        Regex regex = new Regex("</body>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        var match = regex.Match(outhtml);
                        string html = Properties.Resource.toText.Replace("[value]", string.Join(",", toTexts));
                        outhtml = outhtml.Insert(match.Index, html);
                    }
                }
            }

            #endregion

            writer.Write(outhtml);
        }
    }
}
