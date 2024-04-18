using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.User.Plat.Views
{
    /// <summary>
    /// 申请付汇敏感词检测视图
    /// </summary>
    public class PayExchangeSensitiveWordCheckView : View<PayExchangeSensitiveWordCheckModel, ScCustomsReponsitory>
    {
        Wl.Models.Enums.PayExchangeSensitiveAreaType AreaType { get; set; }

        public PayExchangeSensitiveWordCheckView(Wl.Models.Enums.PayExchangeSensitiveAreaType areaType)
        {
            this.AreaType = areaType;
        }

        protected override IQueryable<PayExchangeSensitiveWordCheckModel> GetIQueryable()
        {
            var payExchangeSensitiveAreas = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeSensitiveAreas>();
            var payExchangeSensitiveWords = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeSensitiveWords>();

            var linq = from payExchangeSensitiveArea in payExchangeSensitiveAreas
                       join payExchangeSensitiveWord in payExchangeSensitiveWords on payExchangeSensitiveArea.ID equals payExchangeSensitiveWord.AreaID
                       where payExchangeSensitiveArea.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                          && payExchangeSensitiveWord.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                       orderby payExchangeSensitiveArea.Type
                       select new PayExchangeSensitiveWordCheckModel
                       {
                           AreaName = payExchangeSensitiveArea.Name,
                           AreaType = (Wl.Models.Enums.PayExchangeSensitiveAreaType)payExchangeSensitiveArea.Type,
                           WordContent = payExchangeSensitiveWord.Content,
                       };

            if (this.AreaType != Wl.Models.Enums.PayExchangeSensitiveAreaType.All)
            {
                linq = linq.Where(t => t.AreaType == this.AreaType);
            }

            return linq;
        }
    }

    public class PayExchangeSensitiveWordCheckModel : IUnique
    {
        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// 敏感/禁止地区名称
        /// </summary>
        public string AreaName { get; set; } = string.Empty;

        /// <summary>
        /// 敏感/禁止类型
        /// </summary>
        public Needs.Wl.Models.Enums.PayExchangeSensitiveAreaType AreaType { get; set; }

        /// <summary>
        /// 敏感/禁止词
        /// </summary>
        public string WordContent { get; set; } = string.Empty;
    }
}
