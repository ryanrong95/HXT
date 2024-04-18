using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using Needs.Model;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    public class EmailToken : ModelBase<Layer.Data.Sqls.ScCustoms.Companies, ScCustomsReponsitory>, IUnique, IPersist
    {
        #region 属性

        public string Email { get; set; }

        public string Token { get; set; }

        #endregion

        #region 持久化

        public override void Enter()
        {
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EmailTokens>().Count(item => item.ID == this.ID);
            if (count == 0)
            {
                this.Reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.EmailTokens
                {
                    ID = this.ID,
                    Email = this.Email,
                    CreateDate = DateTime.Now,
                    Token = this.Token,
                });
            }
            else
            {
                this.Reponsitory.Update(new Layer.Data.Sqls.ScCustoms.EmailTokens
                {
                    ID = this.ID,
                    Email = this.Email,
                    CreateDate = this.CreateDate,
                    Token = this.Token,
                }, item => item.Token == this.Token);
            }
        }

        #endregion
    }
}