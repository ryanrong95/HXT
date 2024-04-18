using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Needs.Erp.Models
{
    /// <summary>
    /// Admin 
    /// </summary>
    public partial class Admin
    {
        /// <summary>
        /// 小块基址
        /// </summary>
        public Limit Limits
        {
            get
            {
                return new Limit(this);
            }
        }
    }
}

namespace Needs.Erp
{
    public partial class Limit
    {
        IGenericAdmin admin;

        internal Limit(IGenericAdmin admin)
        {
            this.admin = admin;
        }

        public Models.Admin GetAdmin(string id)
        {
            return this.Admins.Where(item => item.ID == id).FirstOrDefault();
        }

        public Views.Admins Admins
        {
            get { return new Views.Admins(); }
            
        }
    }

    [Obsolete("纯教学建议不用！")]
    class ForCache<T> where T : class, new()
    {
        T source;

        ForCache()
        {
            this.source = new T();
        }


        static public T Current
        {
            get { return new T(); }
        }
    }
}



