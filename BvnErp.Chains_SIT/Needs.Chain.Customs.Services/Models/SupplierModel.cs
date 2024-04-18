using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class SupplierModel: IUnique
    {
        public string ID { get; set; }
        /// <summary>
        /// 英文名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 中文名称
        /// </summary>
        public string ChineseName { get; set; }

        /// <summary>
        /// 供应商级别
        /// </summary>
        public SupplierGrade? SupplierGrade { get; set; }
        /// <summary>
        /// 所属地区
        /// </summary>
        public string Place { get; set; }

        public string ClientID { get; set; }

        public string ClientName { get; set; }

        public string ClientCode { get; set; }


        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }

    }
}
