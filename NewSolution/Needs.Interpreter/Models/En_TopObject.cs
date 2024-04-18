using Layer.Data.Sqls;
using Needs.Interpreter.Extends;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Interpreter.Models
{
    public class En_TopObject : ITopObject
    {
        public string ID { set; get; }
        public string Name { internal set; get; }
        public string Language { internal set; get; }
        public string Value { set; get; }
        public TopObjectType Type { internal set; get; } = TopObjectType.Public;
        public string Project { internal set; get; }
        string[] path;

        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;


        internal En_TopObject()
        {

        }


        public string[] Path
        {
            get
            {
                if (this.path == null && this.Name != null)
                {
                    this.path = this.Name.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries).SelectMany(item => item.StartsWith("'") ? new[] { item.Trim('\'') } : item.Split('.')).ToArray();
                }
                return this.path;
            }
            internal set
            {
                this.path = value;
            }
        }
        public void Enter()
        {
            using (var repository = new BvOverallsReponsitory())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    repository.Insert(new Layer.Data.Sqls.BvOveralls.TopObjects_En
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        Language = this.Language,
                        Name = this.Name,
                        Project = this.Project,
                        Type = (int)this.Type,
                        Value = this.Value,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    });
                }
                else
                {
                    repository.Update<Layer.Data.Sqls.BvOveralls.TopObjects_En>(new
                    {
                        Value = this.Value,
                        UpdateDate = DateTime.Now
                    }, item => item.ID == this.ID);
                }

                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs());
                }
            }
        }
        public void Abandon()
        {
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                throw new Exception("非法操作");
            }
            using (var repository = new BvOverallsReponsitory())
            {
                repository.Delete<Layer.Data.Sqls.BvOveralls.TopObjects_En>(item => item.ID == this.ID);
            }
            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs());
            }
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="language"></param>
        /// <param name="tops"></param>
        public static void Inputs(string language, params En_TopObject[] tops)
        {
            new System.Threading.Thread(() =>
            {
                using (var repository = new BvOverallsReponsitory())
                {
                    foreach (var top in tops)
                    {
                        int count = repository.ReadTable<Layer.Data.Sqls.BvOveralls.TopObjects_En>().Count(item => item.Language == language && item.Name == top.Name);
                        if (count == 0)
                        {
                            top.ID = Guid.NewGuid().ToString("N");
                            top.Language = language;
                            top.Project = "";
                            top.Type = TopObjectType.Public;
                            repository.Insert(top.ToLinq());
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }).Start();
        }
        /// <summary>
        /// 批量导入翻译数据
        /// </summary>
        /// <param name="jObject"></param>
        /// <param name="lang"></param>
        static public void Import(string lang, params En_TopObject[] tops)
        {
            using (var repository = new BvOverallsReponsitory())
            {
                foreach (var top in tops)
                {
                    var old = repository.ReadTable<Layer.Data.Sqls.BvOveralls.TopObjects_En>().FirstOrDefault(item => item.Language == lang && item.Name == top.Name);
                    if (old == null)
                    {
                        top.ID = Guid.NewGuid().ToString("N");
                        top.Language = lang;
                        top.Type = TopObjectType.Public;
                        repository.Insert(top.ToLinq());
                    }
                    else if (old.Value != top.Value)
                    {
                        repository.Update<Layer.Data.Sqls.BvOveralls.TopObjects_En>(new { top.Value, UpdateDate = DateTime.Now }, item => item.ID == old.ID);
                    }
                }
            }
        }

        public static object Outputs()
        {
            var jobj = new Newtonsoft.Json.Linq.JObject();
            foreach (var langGroup in new Views.En_TopObjectsView().GroupBy(lang => lang.Language))
            {
                jobj[langGroup.Key] = GetObj(langGroup);
            };
            return jobj;
        }

        static Newtonsoft.Json.Linq.JToken GetObj(IEnumerable<ITopObject> data)
        {
            var obj = new Newtonsoft.Json.Linq.JObject();
            var group = data.GroupBy(item => item.Path.First());
            foreach (var item in group)
            {
                if (item.Count() == 1 && item.First().Path.Length == 1)
                {
                    obj[item.Key] = new Newtonsoft.Json.Linq.JValue(item.First().Value);
                }
                else
                {
                    obj[item.Key] = GetObj(item.Select(item2 => (ITopObject)new TopObject
                    {
                        Value = item2.Value,
                        Path = item2.Path.Skip(1).ToArray()
                    }));
                }
            }
            return obj;
        }
    }
}
