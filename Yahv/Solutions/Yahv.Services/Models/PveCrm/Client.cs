using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models.PveCrm
{
    public class Client : Enterprise
    {
        public ClientGrade? Grade { get; set; }

        public Underly.CrmPlus.ClientType Type { get; set; }

        public VIPLevel Vip { get; set; }

        public string Source { get; set; }

        public bool IsMajor { get; set; }

        public bool IsSpecial { get; set; }

        public string Industry { get; set; }

        public AuditStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        public decimal? ProfitRate { get; set; }

        public bool IsSupplier { get; set; }

        public string Owner { get; set; }

        public bool IsNew { get; set; }

        #region 纯粹为了兼容原来的PvbCrm的命名，让其他使用公共视图的系统少一点代码改动

        public string GradeDes
        {
            get
            {
                return this.Grade?.GetDescription();
            }
        }

        public Underly.CrmPlus.ClientType Nature { get; set; }

        public string NatureDes
        {
            get
            {
                return this.Nature.GetDescription();
            }
        }

        public AreaType AreaType
        {
            get
            {
                if (FixedArea.MainLand.GetFixedID() == this.District)
                    return AreaType.domestic;
                else
                    return AreaType.International;
            }
        }

        public string TaxperNumber { get; set; }

        public string AreaTypeDes
        {
            get
            {
                return this.AreaType.GetDescription();
            }
        }

        #endregion

        /// <summary>
        /// SiteID
        /// </summary>
        public string SiteID { get; set; }

        /// <summary>
        /// Site合作公司     
        /// </summary>
        public string CompanyName { get; set; }
    }
}
