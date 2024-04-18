using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class IcgooMQ : IUnique,IPersist
    {
        public string ID { get; set; }
        public string PostData { get; set; }
        /// <summary>
        /// 是否解析成功，内单没有海关编码，也会改为false
        /// </summary>
        public bool IsAnalyzed { get; set; }
        public Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }
        /// <summary>
        /// 是否有叉车
        /// </summary>
        public bool? IsForklift { get; set; }
        /// <summary>
        /// 卡板数
        /// </summary>
        public int? AdditionWeight { get; set; }
        public CompanyTypeEnums CompanyType { get; set; }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooPostLog>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }
        }

    }
}
