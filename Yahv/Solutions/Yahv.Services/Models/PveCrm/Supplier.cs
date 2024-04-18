using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Services.Models.PveCrm
{
    public class Supplier : Enterprise
    {
        public SupplierGrade? Grade { get; set; }

        public string Products { get; set; }

        public string Source { get; set; }

        public Underly.CrmPlus.SupplierType Type { get; set; }

        public string TypeDes { get { return this.Type.GetDescription(); } }

        public Underly.CrmPlus.SettlementType SettlementType { get; set; }

        public Underly.CrmPlus.OrderType OrderType { set; get; }

        public InvoiceType InvoiceType { set; get; }

        public DateTime CreateDate { get; set; }

        public bool IsSpecial { get; set; }

        public bool IsClient { get; set; }

        public bool IsProtected { get; set; }

        public bool IsAgent { get; set; }

        public bool IsAccount { get; set; }

        public string WorkTime { get; set; }

        public bool IsFixed { get; set; }

        public AuditStatus Status { get; set; }

        public string CreatorID { get; set; }

        public string OwnerID { get; set; }

        public string OrderCompanyID { get; set; }

        #region 纯粹为了兼容原来的PvbCrm的命名，让其他使用公共视图的系统少一点代码改动

        public string TaxperNumber { get; set; }

        public string Nature { get; set; }

        /// <summary>
        /// 新版已经存储实际字符串，故NatureDes=Nature
        /// </summary>
        public string NatureDes
        {
            get
            {
                return this.Nature;
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

        public string AreaTypeDes
        {
            get
            {
                return this.AreaType.GetDescription();
            }
        }

        public string GradeDes
        {
            get { return this.Grade.HasValue ? this.Grade.GetDescription() : ""; }
        }

        public bool? IsFactory { get; set; }

        public string AgentCompany { get; set; }

        public Currency Currency { get; set; }

        public int RepayCycle { get; set; }

        public decimal? Price { get; set; }

        #endregion
    }
}
