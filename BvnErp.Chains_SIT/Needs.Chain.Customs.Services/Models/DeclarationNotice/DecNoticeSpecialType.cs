using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DecNoticeSpecialType : IUnique, IPersist
    {
        #region 属性

        private string _id;

        public string ID
        {
            get { return this._id ?? string.Concat(this.DecNoticeID, this.Type).MD5(); }
            set { _id = value; }
        }

        /// <summary>
        /// 报关单ID
        /// </summary>
        public string DecNoticeID { get; set; } = string.Empty;

        /// <summary>
        /// 报关单特殊类型
        /// </summary>
        public Enums.DecHeadSpecialTypeEnum Type { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; } = string.Empty;

        #endregion

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public DecNoticeSpecialType()
        {
            this.Status = Enums.Status.Normal;
            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecNoticeSpecialTypes>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DecNoticeSpecialTypes
                    {
                        ID = this.ID,
                        DecNoticeID = this.DecNoticeID,
                        Type = (int)this.Type,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.DecNoticeSpecialTypes
                    {
                        ID = this.ID,
                        DecNoticeID = this.DecNoticeID,
                        Type = (int)this.Type,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnterSuccess();
        }

        public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecNoticeSpecialTypes>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
        }

        public void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// "单方向"插入数据(只判断 DecHeadID 和 Type, 只新增)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        public void OnewayInsert(Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecNoticeSpecialTypes>().Count(t => t.ID == this.ID && t.Status == (int)this.Status);
            if (0 == count)
            {
                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DecNoticeSpecialTypes
                {
                    ID = this.ID,
                    DecNoticeID = this.DecNoticeID,
                    Type = (int)this.Type,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    Summary = this.Summary,
                });
            }
        }
    }
}
