using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;

namespace Wms.Services.chonggous.Models
{
    public class CgLogs_Operator
    {
        #region 属性

        public string ID { get; set; }

        public LogOperatorType Type { get; set; }

        public string Conduct { get; set; }

        public string MainID { get; set; }

        public string CreatorID { get; set; }

        public string Content { get; set; }

        public DateTime CreateDate { get; set; }
        #endregion


        #region 持久化

        public void Enter()
        {
            using (var reponsitory = new Layers.Data.Sqls.PvWmsRepository())
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvWms.Logs_Operator
                {
                    ID = Layers.Data.PKeySigner.Pick(PkeyType.Logs_Operator),
                    Type = this.Type.ToString(),
                    Conduct = this.Conduct,
                    MainID = this.MainID,
                    CreatorID = this.CreatorID,
                    Content = this.Content,
                    CreateDate = DateTime.Now,
                });
            }

        }

        public void Enter(Layers.Data.Sqls.PvWmsRepository reponsitory)
        {
            reponsitory.Insert(new Layers.Data.Sqls.PvWms.Logs_Operator
            {
                ID = Layers.Data.PKeySigner.Pick(PkeyType.Logs_Operator),
                Type = this.Type.ToString(),
                Conduct = this.Conduct,
                MainID = this.MainID,
                CreatorID = this.CreatorID,
                Content = this.Content,
                CreateDate = DateTime.Now,
            });
        }

        #endregion
    }
}
