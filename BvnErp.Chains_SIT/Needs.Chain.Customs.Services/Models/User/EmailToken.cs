using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class EmailToken : IUnique, IPersist
    {
        #region 属性

        public string ID { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public DateTime CreateDate { get; set; }

        public MailVerificationStatus Status { get; set; }

        #endregion

        #region 持久化

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EmailTokens>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.EmailTokens
                    {
                        ID = this.ID,
                        Email = this.Email,
                        CreateDate = DateTime.Now,
                        Token = this.Token,
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.EmailTokens
                    {
                        ID = this.ID,
                        Email = this.Email,
                        CreateDate = DateTime.Now,
                        Token = this.Token,
                    }, item => item.Token == this.Token);
                }
            }
        }

        #endregion
    }
}