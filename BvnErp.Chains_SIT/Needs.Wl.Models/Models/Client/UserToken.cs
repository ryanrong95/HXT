using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    public class UserToken : ModelBase<Layer.Data.Sqls.ScCustoms.Users, ScCustomsReponsitory>, IUnique, IPersist, IPersistence
    {
        public string UserID { get; set; }

        public string Token { get; set; }

        public string IP { get; set; }

        public override void Enter()
        {
            int count = Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.UserTokens>().Count(item => item.Token == this.Token);

            if (count == 0)
            {
                Reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.UserTokens
                {
                    ID = "TOKEN" + DateTime.Now.Ticks,
                    UserID = this.UserID,
                    Token = this.Token,
                    IP = this.IP,
                    CreateDate = DateTime.Now
                });
            }
        }
    }
}
