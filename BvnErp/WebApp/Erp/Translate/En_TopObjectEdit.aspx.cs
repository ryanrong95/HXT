using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Erp.Translate
{
    public partial class En_TopObjectEdit : Needs.Web.Forms.ClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void treedata()
        {
            Response.Write(this.TopobjectTree().Json());
            Response.End();
        }
        protected void getConfig()
        {
            var param = Request["name"];
            string josnresult = string.Empty;
            if (!string.IsNullOrWhiteSpace(param))
            {
                using (var view = new Needs.Interpreter.Views.En_TopObjectsView())
                {
                    var arr = System.Text.RegularExpressions.Regex.Split(param, "#!@%");
                    if (arr.Length > 0)
                    {
                        var lang = arr[0];
                        var result = view.Where(item => item.Language == lang);
                        if (arr.Length > 1)
                        {
                            var name = string.Concat(arr[1], ".");
                            result = result.Where(item => item.Name.StartsWith(name));
                        }
                        josnresult = result.Json();
                    }
                }
            }
            Response.Write(josnresult);
            Response.End();
        }


        protected void updCofig()
        {
            using (var view = new Needs.Interpreter.Views.En_TopObjectsView())
            {
                var obj = view[Request["ID"]];
                obj.Value = Server.HtmlDecode(Request["value"]);
                obj.EnterSuccess += Obj_EnterSuccess;
                obj.Enter();
            }
        }

        #region 树形数据
        public Newtonsoft.Json.Linq.JArray TopobjectTree()
        {
            var arr = new Newtonsoft.Json.Linq.JArray();
            int cnt = 1;
            var view = new Needs.Interpreter.Views.En_TopObjectsView();
            foreach (var item in view.GroupBy(lang => lang.Language))
            {
                var obj = new Newtonsoft.Json.Linq.JObject();
                obj["id"] = cnt++;
                obj["text"] = item.Key;
                obj["state"] = "closed";
                obj["tag"] = true;
                var names = item.Where(t => t.Path.Length > 1).Select(t => t.Path.Take(t.Path.Length - 1).ToArray()).ToArray();
                if (names != null && names.Count() > 0)
                {
                    obj["id"] = cnt++;
                    obj["children"] = this.GetTree(names, cnt);
                }
                arr.Add(obj);
            }
            return arr;
        }

        Newtonsoft.Json.Linq.JArray GetTree(string[][] names, int cnt)
        {
            var arr = new Newtonsoft.Json.Linq.JArray();
            foreach (var t in names.GroupBy(item => item.First()))
            {
                var obj = new Newtonsoft.Json.Linq.JObject();
                obj["id"] = cnt++;
                obj["text"] = t.Key;
                if (t.Count() > 0)
                {
                    var data = t.Where(item => item.Length > 1).Select(item => item.Skip(1).ToArray()).ToArray();
                    if (data.Length > 1)
                    {
                        obj["children"] = this.GetTree(data, cnt);
                    }
                }
                arr.Add(obj);
            }

            return arr;
        }
        #endregion

        protected void delConfig()
        {
            var ids = Request["ID"];
            if (!string.IsNullOrWhiteSpace(ids))
            {
                string[] array = ids.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string id in array)
                {
                    using (var view = new Needs.Interpreter.Views.En_TopObjectsView())
                    {
                        var obj = view[id];
                        obj.AbandonSuccess += Obj_AbandonSuccess;
                        obj.Abandon();
                    }
                }
            }
        }

        private void Obj_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write("删除成功");
            Response.End();
        }

        private void Obj_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write("修改成功");
            Response.End();
        }
    }
}