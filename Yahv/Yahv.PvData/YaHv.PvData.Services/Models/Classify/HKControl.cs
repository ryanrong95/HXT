using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace YaHv.PvData.Services.Models
{
    public class HKControl : IUnique
    {
        #region   属性
        public string ID { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        //型号/ 
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// 类别代码
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 产品说明描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        ///是否管制
        /// </summary>
        public bool isControl { get; set; }
        public Yahv.Underly.GeneralStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }
        #endregion

        public HKControl()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = Yahv.Underly.GeneralStatus.Normal;

        }
        #region  持久化


        #endregion
    }
}
