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
    /// 私有用户控件
    /// </summary>
    abstract public class vUserControl : UserControl
    {
        /// <summary>
        /// 受保护的构造器
        /// </summary>
        protected vUserControl()
        {
            this.Init += UcBase_Init_Ajax;
        }

        private void UcBase_Init_Ajax(object sender, EventArgs e)
        {
            string name = Request["ucAction"];

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


    }
}
