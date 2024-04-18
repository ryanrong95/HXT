using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 报关单-电子随附单据信息
    /// </summary>
    [Serializable]
    public class EdocRealation : IUnique, IPersist
    {
        #region 属性
        /// <summary>
        /// 主键ID（DeclarationID+EdocCode）.MD5
        /// </summary>
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.DeclarationID, this.EdocCode).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 报关单
        /// </summary>
        public string DeclarationID { get; set; }

        /// <summary>
        /// 文件名、随附单据编号（命名规则是：申报口岸+随附单据类别代码+IM+18位流水号）
        /// </summary>
        public string EdocID { get; set; }

        /// <summary>
        /// 随附单证类别
        /// </summary>
        public string EdocCode { get; set; }

        /// <summary>
        /// 随附单证
        /// </summary>
        public BaseEdocCode Edoc { get; set; }

        /// <summary>
        /// 随附单据格式类型
        /// </summary>
        public string EdocFomatType { get; set; }

        /// <summary>
        /// 操作说明（重传原因）
        /// </summary>
        public string OpNote { get; set; }

        /// <summary>
        /// 随附单据文件企业名
        /// </summary>
        public string EdocCopId { get; set; }

        /// <summary>
        /// 签名时间
        /// </summary>
        public DateTime SignTime { get; set; }

        /// <summary>
        /// 随附单据文件大小
        /// </summary>
        public string EdocSize { get; set; }

        /// <summary>
        /// 所属单位代码
        /// </summary>
        public string EdocOwnerCode { get; set; }

        /// <summary>
        /// 所属单位名称
        /// </summary>
        public string EdocOwnerName { get; set; }

        /// <summary>
        /// url
        /// </summary>
        public string FileUrl { get; set; }

        #endregion

        public EdocRealation()
        {
            //TODO：设置默认值
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EdocRealations>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.EdocRealations
                    {
                        ID = this.ID,
                        DeclarationID = this.DeclarationID,
                        EdocID = this.EdocID,
                        EdocCode = this.EdocCode,
                        EdocFomatType = this.EdocFomatType,
                        OpNote = this.OpNote,
                        EdocCopId = this.EdocCopId,
                        SignTime = this.SignTime,
                        EdocSize = this.EdocSize,
                        EdocOwnerCode = this.EdocOwnerCode,
                        EdocOwnerName = this.EdocOwnerName,
                        FileUrl = this.FileUrl
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.EdocRealations
                    {
                        ID = this.ID,
                        DeclarationID = this.DeclarationID,
                        EdocID = this.EdocID,
                        EdocCode = this.EdocCode,
                        EdocFomatType = this.EdocFomatType,
                        OpNote = this.OpNote,
                        EdocCopId = this.EdocCopId,
                        SignTime = this.SignTime,
                        EdocSize = this.EdocSize,
                        EdocOwnerCode = this.EdocOwnerCode,
                        EdocOwnerName = this.EdocOwnerName,
                        FileUrl = this.FileUrl
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnter();
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 去持久化
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.EdocRealations>(item => item.ID == this.ID);
            }
            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
