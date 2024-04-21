using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Payments
{
    /// <summary>
    /// 科目管理
    /// </summary>
    public class SubjectManager
    {
        /// <summary>
        /// 业务
        /// </summary>
        SortedList<string, Catalogs> conduct;

        public Catalogs this[string business]
        {
            get { return this.conduct[business]; }
        }

        /// <summary>
        /// 获取指定业务下的科目的枚举值
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal IEnumerable<Subject> GetEnumbers(string index)
        {
            return this.conduct[index].SelectMany(item => item.Subjects);
        }

        SubjectManager()
        {
            this.conduct = new SortedList<string, Catalogs>();

            if (true)
            {
                var data = SubjectCollection.Current[ConductConsts.供应链];

                List<Subject> dcc = (from d in data
                                     from str in d.Value
                                     select new Subject
                                     {
                                         Catalog = d.Key,
                                         Name = str
                                     }).ToList();

                this.conduct[ConductConsts.代仓储] = new Catalogs(dcc);
                this.conduct[ConductConsts.代报关] = new Catalogs(dcc);
                this.conduct[ConductConsts.供应链] = new Catalogs(dcc);
            }

            if (true)
            {
                List<Subject> dcc = new List<Subject>();
            }

            //数据读取出来
            //流水表

        }


        private static SubjectManager subjectManager;
        static object locker = new object();
        static public SubjectManager Current
        {
            get
            {
                if (subjectManager == null)
                {
                    lock (locker)
                    {
                        if (subjectManager == null)
                        {
                            subjectManager = new SubjectManager();
                        }
                    }
                }

                return subjectManager;
            }
        }
    }
}
