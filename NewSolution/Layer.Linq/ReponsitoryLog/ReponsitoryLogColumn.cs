using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Linq
{
    public class ReponsitoryLogColumn : ReponsitoryLogBase
    {
        public string RowID { get; set; } = string.Empty;

        public string ColumnName { get; set; } = string.Empty;

        public string ColumnType { get; set; } = string.Empty;

        public string OldPrimaryKey { get; set; } = string.Empty;

        public string OldValue { get; set; } = string.Empty;

        public string NewPrimaryKey { get; set; } = string.Empty;

        public string NewValue { get; set; } = string.Empty;
    }
}
