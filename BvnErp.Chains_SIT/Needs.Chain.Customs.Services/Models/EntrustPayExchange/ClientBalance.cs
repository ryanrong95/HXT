using Needs.Ccs.Services.Enums;
using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ClientBalance : IUnique
    {
        #region 属性
        private string id;
        public string ID {
            get
            {
                return this.id ?? string.Concat(this.ClientID, this.ClientAccount).MD5();
            }
            set
            {
                this.id = value;
            }
        }
        public string ClientID { get; set; }
        public string ClientAccount { get; set; }
        public decimal? Balance { get; set; }
        public string Currency { get; set; }
        public int Version { get; set; }
        public Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Summary { get; set; }
        #endregion

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public ClientBalance()
        {
            this.Status = Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }
            

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientBalance>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientBalance
                    {
                        ID = this.ID,
                        ClientID = this.ClientID,
                        ClientAccount = this.ClientAccount,
                        Balance = this.Balance,
                        Currency = this.Currency,
                        Version = this.Version,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary
                    });
                }

                this.OnEnter();
            }
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
