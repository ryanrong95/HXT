using Layer.Data.Sqls;
using Layer.Linq;
using Needs.Overall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services
{
    /// <summary>
    /// 主键类型
    /// </summary>
    public enum PKeyType
    {
        [Repository(typeof(BvCrmReponsitory))]
        [PKey("Apply", PKeySigner.Mode.Time, 4)]
        Apply = 10000,

        [Repository(typeof(BvCrmReponsitory))]
        [PKey("Beneficiaries", PKeySigner.Mode.Time, 4)]
        Beneficiaries = 20000,

        [Repository(typeof(BvCrmReponsitory))]
        [PKey("Catelogues", PKeySigner.Mode.Time, 6)]
        Catelogues = 30000,

        [Repository(typeof(BvCrmReponsitory))]
        [PKey("Client", PKeySigner.Mode.Normal, 4)]
        Client = 40000,

        [Repository(typeof(BvCrmReponsitory))]
        [PKey("Contact", PKeySigner.Mode.Normal, 4)]
        Contact = 50000,

        [Repository(typeof(BvCrmReponsitory))]
        [PKey("Product", PKeySigner.Mode.Time, 4)]
        Product = 60000,

        [Repository(typeof(BvCrmReponsitory))]
        [PKey("PIE", PKeySigner.Mode.Time, 4)]
        Enquiry = 60001,

        [Repository(typeof(BvCrmReponsitory))]
        [PKey("Order", PKeySigner.Mode.Time, 4)]
        Order = 70000,

        [Repository(typeof(BvCrmReponsitory))]
        [PKey("Problem", PKeySigner.Mode.Time, 4)]
        Problem = 80000,

        [Repository(typeof(BvCrmReponsitory))]
        [PKey("Project", PKeySigner.Mode.Time, 4)]
        Project = 90000,

        [Repository(typeof(BvCrmReponsitory))]
        [PKey("Report", PKeySigner.Mode.Time, 4)]
        Report = 90001,

        [Repository(typeof(BvCrmReponsitory))]
        [PKey("WorksOther", PKeySigner.Mode.Time, 4)]
        WorksOther = 90002,

        [Repository(typeof(BvCrmReponsitory))]
        [PKey("Weekly", PKeySigner.Mode.Time, 4)]
        Weekly = 90003,

        [Repository(typeof(BvCrmReponsitory))]
        [PKey("Trace", PKeySigner.Mode.Time, 4)]
        Trace = 90004,
    }
}
