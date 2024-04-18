using Needs.Overall.Models;
using Needs.Overall.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;
using System.Linq.Expressions;
using Needs.Utils.Converters;
using Layer.Data.Sqls;
using Needs.Overall.Views;

namespace Needs.Overall
{
    public class Devlopers
    {
        Devlopers()
        {

        }

        static void Origin(int number, Devloper devloper, CsProject project)
        {
#if !DEBUG
            return;
#endif
            StackTrace trace = new StackTrace(true);
            StackFrame sframe = trace.GetFrame(2);
            var method = sframe.GetMethod();
            var type = method.DeclaringType;

            string id = string.Concat(devloper
               , project
               , type.FullName
               , method.ToString(), number).MD5();

            using (var reponsitory = new BvOverallsReponsitory())
            {
                Expression<Func<Layer.Data.Sqls.BvOveralls.DevlopNotes, bool>> predicate = item => item.ID == id;
                var entity = reponsitory.GetTable<Layer.Data.Sqls.BvOveralls.DevlopNotes>().SingleOrDefault(predicate);
                if (entity == null)
                {
                    reponsitory.Insert(new Models.DevlopNote
                    {
                        ID = id,
                        Devloper = devloper,
                        CreateDate = DateTime.Now,
                        MethodName = method.ToString(),
                        Context = null,
                        TypeName = type.FullName,
                        CsProject = project,
                        Number = number,
                        UpdateDate = DateTime.Now,
                    }.ToLinq());
                }
            }
        }

        public static void Create(Devloper devloper, CsProject project = CsProject.BvnErp)
        {
            Origin(0, devloper, project);
        }

        public static void Create(int number, Devloper devloper, CsProject project = CsProject.BvnErp)
        {
            Origin(number, devloper, project);
        }

        static public Linq.ILinqUnique<IDevlopNote> Currents
        {
            get
            {
                return new DevlopersView();
            }
        }
    }
}
