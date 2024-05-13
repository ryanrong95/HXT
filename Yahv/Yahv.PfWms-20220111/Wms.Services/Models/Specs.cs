using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Wms.Services.Extends;
using Yahv.Linq;
using Yahv.Linq.Persistence;
using Yahv.Underly;
using Yahv.Usually;

namespace Wms.Services.Models
{
    public class Specs : IUnique, IPersisting
    {

        #region 事件
        //Enter成功
        public event SuccessHanlder SpecsSuccess;
        //Enter失败
        public event ErrorHanlder SpecsFailed;
        //名字重复
        public event ErrorHanlder CheckNameRepeat;
        #endregion

        #region 属性

        /// <summary>
        /// 唯一码，建议与Name相同
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 示例：AB12
        /// </summary>
        public string Name { get; set; }

        ///// <summary>
        ///// 规格类型：库房 、  库区、  货架  、  库位
        ///// </summary>
        //public SpecsType Type { get; set; }

        /// <summary>
        /// 宽
        /// </summary>
        public decimal? Width { get; set; }

        /// <summary>
        /// 长
        /// </summary>
        public decimal? Length { get; set; }

        /// <summary>
        /// 高
        /// </summary>
        public decimal? Height { get; set; }

        ///// <summary>
        ///// 行数，层数
        ///// </summary>
        //public int? RowTotal { get; set; }

        ///// <summary>
        ///// 容积（最优设计：通过长宽高）
        ///// </summary>
        //public decimal? Volume { get; set; }

        /// <summary>
        /// 承重
        /// </summary>
        public decimal? Load { get; set; }

        #endregion

    
        #region 方法
        public void Enter()
        {
            try
            {
                using (var repository = new PvWmsRepository())
                {
                    //判断新增/修改的规格名称是否已经存在（加上这句能更好的体现面向对象，即使控制器里不写这个CheckNameRepeat事件也能保证这个类库不报错）
                    if (CheckNameRepeat != null)
                    {
                        if (new Views.SpecsView().Any(item => item.Name == this.Name && item.ID != (this.ID ?? "")))
                        {
                            CheckNameRepeat(this, new ErrorEventArgs());
                            return;
                        }
                    }

                    //ID为空是新增
                    if (string.IsNullOrWhiteSpace(this.ID))
                    {
                        this.ID = this.Name;
                        repository.Insert(this.ToLinq());
                    }
                    else
                    {
                        repository.Update(this.ToLinq(), item => item.ID == this.ID);
                    }

                }
                this.SpecsSuccess?.Invoke(this, new SuccessEventArgs());
            }
            catch
            {
                this.SpecsFailed?.Invoke(this, new ErrorEventArgs("Failed"));
            }
        }

        public void Abandon()
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
