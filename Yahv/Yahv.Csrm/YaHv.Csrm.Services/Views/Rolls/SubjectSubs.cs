using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class SubjectSubs : UniqueView<_Subject, PvbCrmReponsitory>
    {
        private _Subject father;

        public SubjectSubs(_Subject father)
        {
            this.father = father;
        }

        protected override IQueryable<_Subject> GetIQueryable()
        {
            var view = new _SubjectsOrigin();

            return from s in view
                   where s.FatherID == this.father.ID
                   select s;
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="entity"></param>
        public void Add(_Subject entity)
        {
            entity.Enter();
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="id"></param>
        public void Delete(string id)
        {
            var entity = new _Subject() { ID = id };
            entity.Abandon();
        }
    }
}
