using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class DecNoticeVoyage : IUnique, IPersist
    {
        /// <summary>
        /// DecNoticeVoyage 主键ID
        /// </summary>
        public string ID { get; set; }

        //报关通知
        public DeclarationNotice DeclarationNotice { get; set; }

        /// <summary>
        /// 运输批次
        /// </summary>
        public Voyage Voyage { get; set; }

        /// <summary>
        /// 管理员(添加人)
        /// </summary>
        public Admin Admin { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public Enums.Status Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        public string VoyageID { get; set; }

        public string OrderID { get; set; }

        public DecNoticeVoyage()
        {
            this.Status = Enums.Status.Normal;
            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update(new Layer.Data.Sqls.ScCustoms.DecNoticeVoyages { Status = (int)Enums.Status.Delete, }, item => item.DecNoticeID == this.DeclarationNotice.ID);


                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecNoticeVoyages>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DecNoticeVoyages
                    {
                        ID = ChainsGuid.NewGuidUp(),
                        DecNoticeID = this.DeclarationNotice.ID,
                        VoyageID = this.Voyage.ID,
                        AdminID = this.Admin.OriginID,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.DecNoticeVoyages
                    {
                        ID = this.ID,
                        DecNoticeID = this.DeclarationNotice.ID,
                        VoyageID = this.Voyage.ID,
                        AdminID = this.Admin.OriginID,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    }, item => item.ID == this.ID);
                }
            }
        }
    }
}
