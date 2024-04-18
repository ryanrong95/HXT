using Yahv.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Threading;
using System.Text;
using Yahv.Linq.Extends;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Yahv.Web.Forms
{
    /// <summary>
    /// 页面展示形式
    /// </summary>
    public enum PageMode
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal,
        /// <summary>
        /// 弹出窗体
        /// </summary>
        Window,

        /// <summary>
        /// 弹出窗体
        /// </summary>
        Dialog
    }

    /// <summary>
    /// 来访者 Forms 基类
    /// </summary>
    abstract public class ClientPage : Page
    {
        /// <summary>
        /// 模式窗口参数名称
        /// </summary>
        static readonly internal string PageModeParamName = "pagemode";

        /// <summary>
        /// 模式窗口参数名称
        /// </summary>
        static readonly internal string PageParamName = "mornalpage";

        /// <summary>
        /// 模拟model
        /// </summary>
        protected dynamic Model { get; set; } = new ExpandoObject();

        /// <summary>
        /// 上传文件时间
        /// </summary>
        public event FileUploadedHandle FileUploaded;

        /// <summary>
        /// 受保护的构造器
        /// </summary>
        protected ClientPage()
        {
            this.PreInit += ClientPage_PreInit;
            this.PreInit += ClientPage_Init_Ajax;
            this.Unload += ClientPage_Unload;
            this.PreInit += ClientPage_Uploader;

            this.Page.ClientIDMode = ClientIDMode.Static;
        }

        private void ClientPage_Uploader(object sender, EventArgs e)
        {
            Guid guid;

            if (Guid.TryParse(Request.QueryString["uploader_g"], out guid))
            {
                if (this.Request.Files.Count > 0)
                {
                    FileUploaderEventArgs fue = new FileUploaderEventArgs(this.Request.Files);
                    if (this != null && this.FileUploaded != null)
                    {
                        this.FileUploaded(sender, fue);
                    }

                    string uploaderPathRoot = ConfigurationManager.AppSettings["uploaderPathRoot"]?.Trim();
                    string uploaderUrlRoot = ConfigurationManager.AppSettings["uploaderUrlRoot"]?.Trim();

                    string root = Server.MapPath("/");
                    if (!string.IsNullOrWhiteSpace(uploaderPathRoot))
                    {
                        DirectoryInfo di = new DirectoryInfo(uploaderPathRoot);
                        if (di.Attributes == FileAttributes.Directory && di.Root.Exists)
                        {
                            root = di.FullName;
                        }
                    }

                    Uri baseUrl = Request.Url;
                    if (!string.IsNullOrWhiteSpace(uploaderUrlRoot))
                    {
                        Uri.TryCreate(uploaderUrlRoot, UriKind.Absolute, out baseUrl);
                    }

                    //上传类型
                    string type = Request.Form["type"]?.Trim() ?? "Commons";

                    //自动保存
                    foreach (var file in fue.Files)
                    {
                        string fileName = Path.Combine(root,
                            "_uploader",
                            type,
                            DateTime.Now.ToString("yyyyMMdd"),
                            Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));

                        FileInfo fi = new FileInfo(fileName);

                        if (!fi.Directory.Exists)
                        {
                            fi.Directory.Create();
                        }

                        file.SaveAs(fi.FullName);
                        var url = new Uri(baseUrl, fileName.Substring(root.Length - 1).Replace(@"\", "/"));
                        file.CallUrl = url.OriginalString;
                    }

                    Response.ClearContent();
                    Response.ContentType = "application/json";
                    Response.Write(fue.Files.Select(item => new
                    {
                        //刘芳要求
                        //item.ContentLength, 
                        //item.ContentType,
                        item.FileName,
                        item.CallUrl,
                    }).Json());
                    Response.End();
                }
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //base.Render(writer);

            StringWriter html = new StringWriter();
            HtmlTextWriter tw = new HtmlTextWriter(html);
            base.Render(tw);
            string outhtml = html.ToString().Trim();

            Regex regex_model = new Regex(@"<head\s*.*?>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            //Regex regex = new Regex(rgx, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            var match = regex_model.Match(outhtml);
            //outhtml = outhtml.Insert(match.Index + match.Value.Length, $"var model={((object)this.Model).Json()};");
            outhtml = outhtml.Insert(match.Index + match.Value.Length, $"<script>var model={((object)this.Model).Json()};</script>");


            if (!string.IsNullOrWhiteSpace(Request[PageModeParamName]))
            {
                var mode = (PageMode)Enum.Parse(typeof(PageMode), Request[PageModeParamName], true);

                //窗体模式 增加对话框
                switch (mode)
                {
                    case PageMode.Dialog:
                    case PageMode.Window:
                        {
                            string guid = "_" + Guid.NewGuid().ToString("N");
                            string startContent = $"<div id=\"{guid}\">";
                            string endContent = "</div>";

                            Regex regex_js_start = null;
                            string jsContect = null;
                            if (mode == PageMode.Dialog)
                            {
                                jsContect = new Controls.Easyui.jsTopDialog
                                {
                                    TopID = guid
                                }.Execute();
                                //regex_js_start = new Regex(@"<script.*?src="".*?/Yahv/customs-easyui/Scripts/easyui.myDialog.fuse.js""(.*?/>|>\s*</script>)", RegexOptions.IgnoreCase);
                            }
                            if (mode == PageMode.Window)
                            {
                                jsContect = new Controls.Easyui.jsTopWindow
                                {
                                    TopID = guid
                                }.Execute();
                                //regex_js_start = new Regex(@"<script.*?src="".*?/Yahv/customs-easyui/Scripts/easyui.myWindow.fuse.js""(.*?/>|>\s*</script>)", RegexOptions.IgnoreCase);
                            }

                            //var regex_body = new Regex(@"<body[^>]*>([\s\S]*)<\/body>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            //body开始增加
                            var regex_body_start = new Regex(@"<body[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            var match_body_start = regex_body_start.Match(outhtml);
                            outhtml = outhtml.Insert(match_body_start.Index + match_body_start.Value.Length, startContent);
                            //body结束增加
                            var regex_body_end = new Regex(@"<\/body>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            var match_body_end = regex_body_end.Match(outhtml);
                            outhtml = outhtml.Insert(match_body_end.Index, endContent);

                            //head结束增加：窗体js

                            regex_js_start = new Regex(@"<script.*?src="".*?/jquery.easyui.min.js""(.*?/>|>\s*</script>)", RegexOptions.IgnoreCase);

                            Match match_js_start = regex_js_start.Matches(outhtml).Cast<Match>().OrderByDescending(item => item.Index).FirstOrDefault();
                            if (match_js_start == null)
                            {
                                throw new NotImplementedException("请在适当为的位置引用：myWindow|myDialog).fuse.js");
                            }
                            outhtml = outhtml.Insert(match_js_start.Index + match_js_start.Value.Length, $"<script>{jsContect}</script>");

                            //var regex_head_start = new Regex(@"<\/head>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                            //var match_head_start = regex_head_start.Match(outhtml);
                            //outhtml = outhtml.Insert(match_head_start.Index, $"<script>{jsContect}</script>");

                        }
                        break;
                    case PageMode.Normal:
                    default:
                        break;
                }
            }

            writer.Write(outhtml);
        }

        //private void ClientPage_LoadComplete_InitModel(object sender, EventArgs e)
        //{
        //    if (this.Header == null)
        //    {
        //        return;
        //    }

        //    //this.Header.FindControl("cphHead") as ContentPlaceHolder


        //    HtmlGenericControl script = new HtmlGenericControl("script");
        //    this.Header.Controls.AddAt(0, script);

        //    string json = ((object)this.Model).Json();

        //    script.InnerHtml = $"var model={((object)this.Model).Json()};";

        //}

        private void ClientPage_PreInit(object sender, EventArgs e)
        {
            Thread.SetData(Thread.GetNamedDataSlot("ClientPage"), this);
        }

        private void ClientPage_Unload(object sender, EventArgs e)
        {
            Linq.LinqContext.Current.Dispose();
        }

        void ClientPage_Init_Ajax(object sender, EventArgs e)
        {
            string name = Request["action"];

            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }
            MethodInfo minfo = this.GetType().GetMethod(name, BindingFlags.Instance
                    | BindingFlags.NonPublic
                    | BindingFlags.IgnoreCase);

            if (minfo == null)
            {
                throw new NotImplementedException("No implementation of the Ajax method !");
            }

            Response.ClearContent();

            object content = minfo.Invoke(this, null);

            if (content != null)
            {
                if (content is string)
                {
                    Response.ContentType = "text/plain";
                    Response.Write(content as string);
                }
                else if (content is HtmlString)
                {
                    Response.ContentType = "text/html";
                    Response.Write(content as string);
                }
                else if (content is Json)
                {
                    Response.ContentType = "application/json";
                    Response.Write((content as Json).Content);
                }
                else
                {
                    Response.ContentType = "application/json";
                    Response.Write(content.Json());
                }
            }

            Response.End();
        }

        #region 分页


        /// <summary>
        /// 分页数据
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="queryable">集合</param>
        /// <param name="pageindex">页</param>
        /// <param name="pagesize">数量</param>
        /// <param name="converter">转换</param>
        /// <returns></returns>
        //[Obsolete("建议使用：Yahv.Linq.Extends.Enumerable中的扩展方法，要仔细学习并应用")]
        protected object Paging<T>(IQueryable<T> queryable, Func<T, object> converter = null)
        {
            int page = 1; int.TryParse(Request.QueryString["page"], out page);
            int rows = 20; int.TryParse(Request.QueryString["rows"], out rows);
            if (page <= 0 || rows <= 0) // 不分页
            {
                page = 1;
                rows = 1000;
            }

            var types = new[] { typeof(int), typeof(int) };
            var method = queryable.GetType().GetMethod("ToPaging", BindingFlags.Public | BindingFlags.Instance, null, types, null);

            if (method == null)
            {
                return queryable.Paging(page, rows, converter);
            }
            else
            {
                return method.Invoke(queryable, new object[] { page, rows });
            }
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="queryable">集合</param>
        /// <param name="pageindex">页</param>
        /// <param name="pagesize">数量</param>
        /// <param name="converter">转换</param>
        /// <returns></returns>
        //[Obsolete("建议使用：Yahv.Linq.Extends.Enumerable中的扩展方法，要仔细学习并应用")]
        protected object Paging<T>(IEnumerable<T> queryable, Func<T, object> converter = null)
        {
            int page = 1; int.TryParse(Request.QueryString["page"], out page);
            int rows = 20; int.TryParse(Request.QueryString["rows"], out rows);
            if (page <= 0 || rows <= 0) // 不分页
            {
                page = 1;
                rows = 1000;
            }
            return queryable.Paging(page, rows, converter);
        }



        #endregion


        #region 导出文件
        /// <summary>
        /// 导出 CSV 文件
        /// </summary>
        /// <typeparam name="T">集合类型</typeparam>
        /// <param name="queryable">集合</param>
        /// <param name="converter">转换</param>
        /// <returns></returns>
        protected object ExportCSV<T>(IEnumerable<T> queryable, Func<T, object> converter = null) where T : class//, new()
        {
            this.EnableViewState = false;
            Response.ClearHeaders();
            string codingName = "gb2312";
            Encoding coding = Encoding.GetEncoding("gb2312");
            Response.ContentType = "text/csv;charset=" + codingName + ";";
            Response.ContentEncoding = coding;
            Response.Charset = codingName;
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + DateTime.Now.Ticks + ".csv");
            byte[] _bytes;
            if (converter == null)
            {
                _bytes = this.GenCsvBtyes<T>(queryable);
            }
            else
            {
                _bytes = this.GenCsvBtyes<object>(queryable.Select(converter));
            }
            string context = Encoding.UTF8.GetString(_bytes);
            byte[] bytes = coding.GetBytes(context);
            int bytesCount = coding.GetByteCount(context);
            Response.OutputStream.Write(bytes, 0, bytesCount);
            Response.Flush();
            Response.End();
            return null;
        }

        /// <summary>
        /// 集合转成字节
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ienums"></param> 
        /// <returns></returns>
        protected byte[] GenCsvBtyes<T>(IEnumerable<T> ienums) where T : class//, new()
        {
            if (ienums == null && ienums.Count() == 0)
            {
                throw new NotImplementedException("不支持此种实现");
            }
            StringBuilder body = new StringBuilder();

            var arry = ienums.ToArray();
            if (arry.Length > 0)
            {
                PropertyInfo[] infos = arry[0].GetType().GetProperties();
                string header = string.Join(",", infos.Select(item => item.Name));

                foreach (var item in ienums)
                {
                    List<string> list = new List<string>();

                    foreach (PropertyInfo info in infos)
                    {
                        var val = info.GetValue(item, null).ToString().Replace("\n", " ");
                        if (val.Contains(","))
                            val = "\"" + val + "\"";
                        list.Add(val);
                    }

                    body.Append(string.Join(",", list)).AppendLine();
                }
                string txt = new StringBuilder().Append(header).AppendLine().Append(body).ToString();
                return System.Text.Encoding.UTF8.GetBytes(txt);
            }
            return System.Text.Encoding.UTF8.GetBytes(body.ToString());

        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileName"></param>
        public void DownLoadFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            string name = fileName.Substring(fileName.LastIndexOf('\\') + 1, fileName.Length - Path.GetExtension(fileName).Length - fileName.LastIndexOf('\\') - 1);

            FileInfo fileInfo = new FileInfo(fileName);
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Content-Disposition", "attachment;filename=" + name + Path.GetExtension(fileName).ToLower());
            Response.AddHeader("Content-Length", fileInfo.Length.ToString());
            Response.AddHeader("Content-Transfer-Encoding", "binary");
            Response.ContentType = "application/octet-stream";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
            Response.WriteFile(fileInfo.FullName);
            Response.Flush();
            Response.End();
        }
        #endregion

        /// <summary>
        /// 是否为Ajax Action 调用
        /// </summary>
        protected bool IsAction
        {
            get { return string.IsNullOrWhiteSpace(Request["action"]); }
        }
    }




}