using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.XDTModels;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    /// <summary>
    /// 申请付汇敏感词检测视图
    /// </summary>
    public class PayExchangeSensitiveWordCheckView : UniqueView<PayExchangeSensitiveWordCheckModel, ScCustomReponsitory>
    {
        PayExchangeSensitiveAreaType AreaType;

        public PayExchangeSensitiveWordCheckView()
        {
        }

        protected override IQueryable<PayExchangeSensitiveWordCheckModel> GetIQueryable()
        {
            var payExchangeSensitiveAreas = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.PayExchangeSensitiveAreas>();
            var payExchangeSensitiveWords = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.PayExchangeSensitiveWords>();

            var linq = from payExchangeSensitiveArea in payExchangeSensitiveAreas
                       join payExchangeSensitiveWord in payExchangeSensitiveWords on payExchangeSensitiveArea.ID equals payExchangeSensitiveWord.AreaID
                       where payExchangeSensitiveArea.Status == (int)Status.Normal
                          && payExchangeSensitiveWord.Status == (int)Status.Normal
                       orderby payExchangeSensitiveArea.Type
                       select new PayExchangeSensitiveWordCheckModel
                       {
                           AreaName = payExchangeSensitiveArea.Name,
                           AreaType = (PayExchangeSensitiveAreaType)payExchangeSensitiveArea.Type,
                           WordContent = payExchangeSensitiveWord.Content,
                       };

            if (this.AreaType != PayExchangeSensitiveAreaType.All)
            {
                linq = linq.Where(t => t.AreaType == this.AreaType);
            }

            return linq;
        }

        /// <summary>
        /// 根据区域类型获取数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IQueryable<PayExchangeSensitiveWordCheckModel> GetDataByAreaType(PayExchangeSensitiveAreaType type)
        {
            this.AreaType = type;
            return this.GetIQueryable();
            //return GetDataByExpressions(expressions);
        }

        /// <summary>
        /// 根据传入参数获取订单数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        virtual protected IQueryable<PayExchangeSensitiveWordCheckModel> GetDataByExpressions(LambdaExpression[] expressions)
        {
            var linq = this.GetIQueryable();
            if (expressions != null)
            {
                foreach (var expression in expressions)
                {
                    linq = linq.Where(expression as Expression<Func<PayExchangeSensitiveWordCheckModel, bool>>);
                }
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
        public PayExchangeSensitiveAreaType AreaType { get; set; }

        /// <summary>
        /// 敏感/禁止词
        /// </summary>
        public string WordContent { get; set; } = string.Empty;
    }

    /// <summary>
    /// 付汇敏感地区类型
    /// </summary>
    public enum PayExchangeSensitiveAreaType
    {
        /// <summary>
        /// 所有
        /// </summary>
        [Description("所有")]
        All = 0,

        /// <summary>
        /// 禁止
        /// </summary>
        [Description("禁止")]
        Forbid = 1,

        /// <summary>
        /// 敏感
        /// </summary>
        [Description("敏感")]
        Sensitive = 2,
    }
}
