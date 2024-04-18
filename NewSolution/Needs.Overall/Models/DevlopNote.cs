using Needs.Overall.Extends;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Overall.Models
{
    sealed class DevlopNote : IDevlopNote
    {
        string id;
        public string ID
        {
            get
            {
                return id ?? string.Concat(this.Devloper
                    , this.CsProject
                , this.TypeName
                , this.MethodName);
            }
            set { this.id = value; }
        }
        public Devloper Devloper { get; set; }
        public int Number { get; set; }
        public string TypeName { get; set; }
        public string MethodName { get; set; }
        public string Context { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public CsProject CsProject { get; set; }

        public DevlopNote()
        {

        }

        public DevlopNote(Devloper devloper)
        {
            this.Devloper = devloper;
        }

        public void Note(string txt)
        {
            using (var reponsitory = new Layer.Data.Sqls.BvOverallsReponsitory())
            {
                Expression<Func<Layer.Data.Sqls.BvOveralls.DevlopNotes, bool>> predicate = item => item.ID == this.ID;
                var entity = reponsitory.GetTable<Layer.Data.Sqls.BvOveralls.DevlopNotes>().SingleOrDefault(predicate);

                if (entity == null)
                {

                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.BvOveralls.DevlopNotes
                    {
                        Context = txt,
                        UpdateDate = DateTime.Now
                    }, predicate);
                }
            }
        }
    }
}
