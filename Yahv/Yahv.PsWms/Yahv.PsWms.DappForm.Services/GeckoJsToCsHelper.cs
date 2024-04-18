using Gecko;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;

namespace Yahv.PsWms.DappForm.Services
{
    /// <summary>
    /// Js调用C#函数
    /// </summary>
    public class GeckoJsToCsHelper
    {
        static GeckoJsToCsHelper()
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        static public void Initialize()
        {
            Initialize(null, typeof(GeckoHelper));
        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="firefox"></param>
        static public void Initialize(GeckoWebBrowser firefox)
        {
            Initialize(firefox, typeof(GeckoHelper));
        }


        static string Coding(string name)
        {

            return "_" + $"%giubhoui{name}hboin".MD5();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="firefox"></param>
        /// <param name="type"></param>
        static public void Initialize(GeckoWebBrowser firefox,Type type)
        {

            string jsGeckoFuncHelper = Properties.Resource.jsGeckoFuncHelper;

            StringBuilder txtFunc = new StringBuilder();

            txtFunc.AppendLine(Properties.Resource.jsGeckoHelper);

            foreach (var method in type.GetMethods().Where(item => item.GetCustomAttribute<GeckoFuntionAttribute>() != null))
            {
                var code = Coding(method.Name);
                var js = jsGeckoFuncHelper.Replace("methodName", method.Name).Replace("guid", code);
                if (method.GetParameters().Length == 0)
                {
                    js = js.Replace("(data)", "()").Replace("data);", "null);");
                }


                txtFunc.Append(js);
                txtFunc.AppendLine();
                if (firefox != null)
                {
                    firefox.AddMessageEventListener(method.Name, (param) =>
                    {
                        var fparam = method.GetParameters().FirstOrDefault();
                        var parameters = param == null ? null : new object[] { param.JsonTo(fparam.ParameterType) };
                        object value = null;
                        try
                        {
                            value = method.Invoke(null, parameters);
                        }
                        catch (Exception ex)
                        {
                            //记录错误日志
                            //Common.Tools.Log(ex.InnerException ?? ex);
                        }

                        //给前端做返回值使用
                        if (value != null)
                        {
                            var vtype = value.GetType();
                            object jvalue;
                            if (vtype == typeof(string))
                            {
                                jvalue = $"'{value}'";
                            }
                            else if (vtype == typeof(bool))
                            {
                                jvalue = value.ToString().ToLower();
                            }
                            else
                            {
                                jvalue = value.Json();
                            }

                            using (AutoJSContext context = new AutoJSContext(firefox.Window))
                            {
                                string result;
                                context.EvaluateScript($"this['{Coding(method.Name)}']={jvalue}",
                                    firefox.Window.DomWindow, out result);
                            }
                        }

                    });
                }
            };

            ////线程异常

#if DEBUG
            if (firefox == null)
            {
                var file = @"..\..\..\Yahv.PsWms.DappVue\dappvue\src\js\browser.js";
                System.IO.FileInfo fi = new System.IO.FileInfo(file);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);
                System.IO.File.WriteAllText(file, txtFunc.ToString(), Encoding.UTF8);
            }
#endif
        }

    }
}
