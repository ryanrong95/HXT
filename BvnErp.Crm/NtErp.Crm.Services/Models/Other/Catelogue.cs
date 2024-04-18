using Needs.Linq;
using Needs.Overall;
using NtErp.Crm.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    [Needs.Underly.FactoryView(typeof(Views.CatelogueView))]
    public partial class Catelogue : IUnique, IPersist
    {
        #region 属性

        public DateTime CreateDate
        {
            get; set;
        }


        public DateTime UpdateDate
        {
            get; set;
        }


        public DeclareProduct[] DeclareProducts
        {
            get;set;
        }

        public string Summary
        {
            get; set;
        }

        public string ID
        {
            get;set;
        }
        #endregion

        public Catelogue()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        public event SuccessHanlder EnterSuccess;


        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            this.OnEnter();

            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Catalogues>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    if(string.IsNullOrWhiteSpace(this.ID))
                    {
                        this.ID = PKeySigner.Pick(PKeyType.Catelogues);
                    }
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.Catalogues
                    {
                        ID = this.ID,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.BvCrm.Catalogues
                    {
                        ID = this.ID,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary
                    }, item => item.ID == this.ID);
                }
            }
        }
    }
}
