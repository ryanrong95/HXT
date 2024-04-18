using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Overall.Extends
{
    static class DevlopNoteExtends
    {
        public static Layer.Data.Sqls.BvOveralls.DevlopNotes ToLinq(this Models.DevlopNote entity)
        {
            return new Layer.Data.Sqls.BvOveralls.DevlopNotes
            {
                ID = entity.ID,
                Devloper = (int)entity.Devloper,
                CreateDate = entity.CreateDate,
                MethodName = entity.MethodName,
                Context = null,
                TypeName = entity.TypeName,
                CsProject = (int)entity.CsProject,
                Number = entity.Number,
                UpdateDate = DateTime.Now,
            };
        }
    }
}
