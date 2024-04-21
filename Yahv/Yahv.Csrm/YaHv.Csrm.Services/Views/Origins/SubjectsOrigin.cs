using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbCrm;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    /// <summary>
    /// 财务科目原始视图
    /// </summary>
    public class _SubjectsOrigin : Yahv.Linq.UniqueView<_Subject, PvbCrmReponsitory>
    {
        public _SubjectsOrigin()
        {

        }

        protected override IQueryable<_Subject> GetIQueryable()
        {
            return null;
            //return from entity in this.Reponsitory.ReadTable<_Subjects>()
            //       select new _Subject()
            //       {
            //           Status = (Status)entity.Status,
            //           FatherID = entity.FatherID,
            //           Name = entity.Name,
            //           Type = (SubjectType)entity.Type,
            //           ID = entity.ID,
            //       };
        }
    }
}
