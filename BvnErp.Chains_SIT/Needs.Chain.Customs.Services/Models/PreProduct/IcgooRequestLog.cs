using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class IcgooRequestLog : IUnique, IPersist, IFulError, IFulSuccess
    {
        public string ID { get; set; }
        public string Supplier { get; set; }
        public int Days { get; set; }
        public DateTime Createtime { get; set; }
        public string RunPara { get; set; }
        public bool IsSuccess { get; set; }
        public string Info { get; set; }
        public bool IsSend { get; set; }
        public DateTime Updatetime { get; set; }
        public CompanyTypeEnums CompanyType { get; set; }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Insert(this.ToLinq());          
            }
        }
    }
}
