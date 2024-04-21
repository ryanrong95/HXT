using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbCrm;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Models.Rolls;
using YaHv.Csrm.Services.Views.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    /// <summary>
    /// 财务科目
    /// </summary>
    public class SubjectsTree : Tree<nSubject, PvbCrmReponsitory>
    {
        public SubjectsTree()
        {

        }

        protected override IQueryable<nSubject> GetIQueryable()
        {
            var view = new _SubjectsOrigin();

            return from s in view
                   where s.Status == Status.Normal
                   orderby s.Name
                   select new nSubject()
                   {
                       Status = (Status)s.Status,
                       Name = s.Name,
                       FatherID = s.FatherID,
                       ID = s.ID,
                       Type = (SubjectType)s.Type
                   };
        }

        protected override void GenRoot()
        {
            string name = "全部";
            //this.Reponsitory.Insert(new Layers.Data.Sqls.PvbCrm._Subjects()
            //{
            //    //ID = string.Join("", name).MD5(),
            //    ID = PKeySigner.Pick(PKeyType.Subject),
            //    FatherID = null,
            //    Name = name,
            //    Status = (int)Status.Normal,
            //    Type = 0,
            //});
        }

        /// <summary>
        /// 获取json
        /// </summary>
        /// <returns></returns>
        public string Json()
        {
            return new[] { this.Json(Root) }.Json();
        }

        object Json(nSubject entity)
        {
            List<object> sons = null;

            if (entity.Sons.Count > 0)
            {
                sons = new List<object>();
                foreach (var item in entity.Sons)
                {
                    sons.Add(this.Json(item));
                }
            }

            return new
            {
                id = entity.ID,
                name = entity.Name,
                flag = entity.Type == SubjectType.Subject,
                typeName = entity.Type.GetDescription(),
                type = entity.Type,
                children = sons?.ToArray()
            };
        }

        public object Tree()
        {
            return new[] { this.Tree(Root) };
        }

        private object Tree(nSubject entity)
        {
            List<object> sons = null;

            if (entity.Sons.Count > 0)
            {
                sons = new List<object>();
                foreach (var item in entity.Sons)
                {
                    sons.Add(this.Tree(item));
                }
            }

            return new
            {
                id = entity.ID,
                text = entity.Name,
                children = sons?.ToArray()
            };
        }
    }
}
