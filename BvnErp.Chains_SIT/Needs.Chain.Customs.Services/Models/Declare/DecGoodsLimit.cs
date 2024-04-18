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
    /// 报关单项-许可证信息
    /// </summary>
    [Serializable]
    public class DecGoodsLimit : IUnique,IPersist
    {
        /// <summary>
        /// 主键ID（DeclarationID+AppCertCode）.MD5
        /// </summary>
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.DecListID, this.LicTypeCode, this.LicenceNo).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 报关单项Id
        /// </summary>
        public string DecListID { get; set; }

        /// <summary>
        /// 商品项号
        /// </summary>
        public int GoodsNo { get; set; }

        /// <summary>
        /// 许可证类别
        /// </summary>
        public string LicTypeCode { get; set; }

        /// <summary>
        /// 许可证编号
        /// </summary>
        public string LicenceNo { get; set; }

        /// <summary>
        /// 核销货物序号
        /// </summary>
        public string LicWrtofDetailNo { get; set; }

        /// <summary>
        /// 核销数量
        /// </summary>
        public string LicWrtofQty { get; set; }

        /// <summary>
        /// 核销数量单位
        /// </summary>
        public string LicWrtofQtyUnit { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// 随附单证
        /// </summary>
        public BaseGoodsLimit BaseGoodsLimit { get; set; }

        public DecGoodsLimit()
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
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecGoodsLimits>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DecGoodsLimits
                    {
                        ID = this.ID,
                        DecListID = this.DecListID,
                        GoodsNo = this.GoodsNo,
                        LicTypeCode = this.LicTypeCode,
                        LicenceNo = this.LicenceNo,
                        LicWrtofDetailNo = this.LicWrtofDetailNo,
                        LicWrtofQty = this.LicWrtofQty,
                        LicWrtofQtyUnit = this.LicWrtofQtyUnit,
                        FileUrl = this.FileUrl
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.DecGoodsLimits
                    {
                        ID = this.ID,
                        DecListID = this.DecListID,
                        GoodsNo = this.GoodsNo,
                        LicTypeCode = this.LicTypeCode,
                        LicenceNo = this.LicenceNo,
                        LicWrtofDetailNo = this.LicWrtofDetailNo,
                        LicWrtofQty = this.LicWrtofQty,
                        LicWrtofQtyUnit = this.LicWrtofQtyUnit,
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
    }
}
