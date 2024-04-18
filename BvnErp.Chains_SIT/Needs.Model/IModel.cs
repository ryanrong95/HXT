using System;
using System.Collections.Generic;
using System.Text;

namespace Needs.Model
{
    public interface IModel
    {
        string ID { get; set; }

        DateTime CreateDate { get; set; }

        DateTime UpdateDate { get; set; }

        int Status { get; set; }
    }
}
