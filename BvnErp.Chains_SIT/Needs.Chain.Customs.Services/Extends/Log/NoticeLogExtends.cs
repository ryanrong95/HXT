using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services
{
    public static class NoticeLogExtends
    {
        public static Layer.Data.Sqls.ScCustoms.Logs_Notices ToLinq(this Models.NoticeLog entity)
        {
            return new Layer.Data.Sqls.ScCustoms.Logs_Notices
            {
              Title = entity.Title,
              Context = entity.Context,
              CreateDate = entity.CreateDate,
              Readed = entity.Readed,
              ReadDate = entity.ReadDate,
              AdminID = entity.AdminID
            };
        }
    }
}
