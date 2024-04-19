using Needs.Linq;
using NtErp.Wss.Oss.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Models
{
    /// <summary>
    /// 当事人 [提货人、交货人等]
    /// </summary>

    [Needs.Underly.FactoryView(typeof(Views.PartiesView))]
    public class Party : PartyBase, IPersist, IEnterSuccess
    {

        /// <summary>
        /// 交货地
        /// </summary>
        public Needs.Underly.District District { get; set; }

        public Party() : base()
        {

        }

        #region 持久化
        public event SuccessHanlder EnterSuccess;

        override public void Enter()
        {
            this.Company.Enter();
            this.CompanyID = this.Company.ID;
            this.Contact.Enter();
            this.ContactID = this.Contact.ID;

            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                if (!reponsitory.ReadTable<Layer.Data.Sqls.CvOss.Parties>().Any(item => item.ID == this.ID))
                {
                    reponsitory.Insert(this.ToLinq());
                }
            }
            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        #endregion

    }
}
