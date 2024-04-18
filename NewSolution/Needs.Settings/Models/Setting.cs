using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Settings.Models
{
    /// <summary>
    /// 相当于接口
    /// 不论数据库设计的如何，程序都如此去读取
    /// </summary>
    class Setting : IUnique
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string DataType { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Summary { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
