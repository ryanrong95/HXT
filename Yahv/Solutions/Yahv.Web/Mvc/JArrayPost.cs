using Newtonsoft.Json.Linq;
using System.Text;
using System.Web;

namespace Yahv.Web.Mvc
{
    /// <summary>
    /// Payload参数数组对象
    /// </summary>
    public class JArrayPost : JArray
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal JArrayPost(string context) : this(Parse(context))
        {

        }

        public JArrayPost(JArray other) : base(other)
        {
        }

        public JArrayPost(params object[] content) : base(content)
        {
        }

        public JArrayPost(object content) : base(content)
        {
        }
    }
}
