using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Linq
{
    public class ReponsitoryLogRow : ReponsitoryLogBase
    {
        public string DataSource { get; set; } = string.Empty;

        public string DatabaseName { get; set; } = string.Empty;

        public string TableName { get; set; } = string.Empty;

        public RowOperationEnum Operation { get; set; }
    }
}
