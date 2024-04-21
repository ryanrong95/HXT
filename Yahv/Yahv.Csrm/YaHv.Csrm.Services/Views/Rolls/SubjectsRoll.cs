using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Views.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class SubjectsRoll : _SubjectsOrigin
    {
        public SubjectsRoll()
        {

        }

        protected override IQueryable<Models.Origins._Subject> GetIQueryable()
        {
            return base.GetIQueryable();
        }

        /// <summary>
        /// 根据名称获取类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SubjectType GetSubjectType(string name)
        {
            return this.FirstOrDefault(item => item.Name == name).Type;
        }
    }
}
