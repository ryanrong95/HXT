using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Enums;

namespace Yahv.PsWms.DappApi.Services.Models
{
    public class Require : IUnique
    {
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 通知ID
        /// </summary>
        public string NoticeID { get; set; }

        /// <summary>
        /// 货运信息ID
        /// </summary>
        public string NoticeTransportID { get; set; }

        /// <summary>
        /// 直接记录特殊要求本身名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 内容, 非空时候显示
        /// </summary>
        public string Contents { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (var repository = new Layers.Data.Sqls.PsWmsRepository())
            {
                if (!repository.ReadTable<Layers.Data.Sqls.PsWms.Requires>().Any(t => t.ID == this.ID))
                {
                    this.ID = Layers.Data.PKeySigner.Pick(PKeyType.Require);

                    repository.Insert(new Layers.Data.Sqls.PsWms.Requires()
                    {
                        ID = this.ID,
                        NoticeID = this.NoticeID,
                        NoticeTransportID = this.NoticeTransportID,
                        Name = this.Name,
                        Contents = this.Contents,
                        CreateDate = DateTime.Now,
                    });
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PsWms.Requires>(new
                    {
                        this.NoticeID,
                        this.NoticeTransportID,
                        this.Name,
                        this.Contents,
                    }, t => t.ID == this.ID);
                }
            }
        }
    }
}
