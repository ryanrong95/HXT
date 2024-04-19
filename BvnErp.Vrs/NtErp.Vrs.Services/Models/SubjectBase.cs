using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services.Models
{
    abstract public class SubjectBase : Needs.Linq.IUnique
    {
        string id;

        public string ID
        {
            get
            {
                return this.id ?? this.GenID();
            }
            set
            {
                this.id = value;
            }
        }
        public string Name { get; set; }

        string GenID()
        {
            return this.Name.MD5();
        }

        abstract public void Enter();
        abstract public void Abandon();
    }
}
