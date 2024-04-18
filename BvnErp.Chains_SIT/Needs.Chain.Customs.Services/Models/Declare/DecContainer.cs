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
    /// 报关单-集装箱信息
    /// </summary>
    [Serializable]
    public class DecContainer : IUnique, IPersist
    {
        #region 属性
        /// <summary>
        /// 主键ID（DeclarationID+ContainerID）.MD5
        /// </summary>
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.DeclarationID, this.ContainerID).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public string DeclarationID { get; set; }

        /// <summary>
        /// 集装箱号
        /// </summary>
        public string ContainerID { get; set; }

        /// <summary>
        /// 集装箱规格
        /// </summary>
        public string ContainerMd { get; set; }

        /// <summary>
        /// 集装箱
        /// </summary>
        public BaseContainer Container { get; set; }

        /// <summary>
        /// 商品项号，用半角逗号分隔，如“1,3”
        /// </summary>
        public string GoodsNo { get; set; }

        /// <summary>
        /// 拼箱标识 0：否，1：是
        /// </summary>
        public int? LclFlag { get; set; }

        /// <summary>
        /// 箱货重量   集装箱箱体自重+装载货物重量，千克
        /// </summary>
        public decimal? GoodsContaWt { get; set; }

        #endregion

        public DecContainer()
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
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecContainers>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DecContainers {
                        ID = this.ID,
                        DeclarationID = this.DeclarationID,
                        ContainerID = this.ContainerID,
                        ContainerMd = this.ContainerMd,
                        GoodsNo = this.GoodsNo,
                        LclFlag = this.LclFlag,
                        GoodsContaWt = this.GoodsContaWt
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.DecContainers
                    {
                        ID = this.ID,
                        DeclarationID = this.DeclarationID,
                        ContainerID = this.ContainerID,
                        ContainerMd = this.ContainerMd,
                        GoodsNo = this.GoodsNo,
                        LclFlag = this.LclFlag,
                        GoodsContaWt = this.GoodsContaWt
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnter();
        }

        public void PhysicalDelete()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DecContainers>(item => item.ID == this.ID);
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
    }
}
