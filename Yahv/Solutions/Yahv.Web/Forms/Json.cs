using Newtonsoft.Json.Linq;

namespace Yahv.Web.Forms
{
    /// <summary>
    /// Json返回结果
    /// </summary>
    public class Json
    {
        string json;
        public Json(string json = null)
        {
            this.json = json;
        }

        internal string Content { get { return this.json ?? new JObject().ToString(); } }
    }
}
