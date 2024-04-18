using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Linq
{
    public enum RowOperationEnum
    {
        Insert = 1,

        Update = 2,

        Delete = 3,
    }

    public enum Status
    {
        Normal = 200,

        Delete = 400,
    }
}
