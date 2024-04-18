using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 产品归类特殊类型
    /// </summary>
    public class Other : IUnique
    {
        #region 属性

        string id;
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID
        {
            get
            {
                //编码规则：品牌+制造商的MD5
                return this.id ?? string.Concat(this.PartNumber, this.Manufacturer).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 制造商
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 是否3C
        /// </summary>
        public bool Ccc { get; set; }

        /// <summary>
        /// 是否禁运
        /// </summary>
        public bool Embargo { get; set; }

        /// <summary>
        /// 是否香港管控
        /// </summary>
        public bool HkControl { get; set; }

        /// <summary>
        /// 是否需要原产地证明
        /// </summary>
        public bool Coo { get; set; }

        /// <summary>
        /// 是否需要商检
        /// </summary>
        public bool CIQ { get; set; }

        /// <summary>
        /// 商检费
        /// </summary>
        public decimal CIQprice { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 用于排序的时间字段
        /// </summary>
        public DateTime? OrderDate { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string Summary { get; set; }

        #endregion

        #region 扩展属性

        public List<Yahv.Services.Models.Eccn> Eccns { get; set; }

        #endregion

        #region 事件

        public event SuccessHanlder EnterSuccess;

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var repository = LinqFactory<PvDataReponsitory>.Create())
            {
                //添加
                if (!repository.ReadTable<Layers.Data.Sqls.PvData.Others>().Any(t => t.ID == this.ID))
                {
                    repository.Insert(new Layers.Data.Sqls.PvData.Others()
                    {
                        ID = this.ID,
                        PartNumber = this.PartNumber,
                        Manufacturer = this.Manufacturer,
                        Ccc = this.Ccc,
                        Embargo = this.Embargo,
                        HkControl = this.HkControl,
                        Coo = this.Coo,
                        CIQ = this.CIQ,
                        CIQprice = this.CIQprice,
                        CreateDate = DateTime.Now,
                        OrderDate = DateTime.Now
                    });
                }
                //修改
                else
                {
                    repository.Update<Layers.Data.Sqls.PvData.Others>(new
                    {
                        Ccc = this.Ccc,
                        Embargo = this.Embargo,
                        HkControl = this.HkControl,
                        Coo = this.Coo,
                        CIQ = this.CIQ,
                        CIQprice = this.CIQprice,
                        OrderDate = DateTime.Now
                    }, a => a.ID == this.ID);
                }
            }

            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        #endregion
    }
}
