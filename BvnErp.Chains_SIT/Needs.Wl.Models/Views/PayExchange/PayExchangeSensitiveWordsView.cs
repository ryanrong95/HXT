using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Views
{
    public class PayExchangeSensitiveWordsView : View<Models.PayExchangeSensitiveWord, ScCustomsReponsitory>
    {
        public PayExchangeSensitiveWordsView()
        {

        }

        internal PayExchangeSensitiveWordsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.PayExchangeSensitiveWord> GetIQueryable()
        {
            return from payExchangeSensitiveWord in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeSensitiveWords>()
                   where payExchangeSensitiveWord.Status == (int)Enums.Status.Normal
                   select new Models.PayExchangeSensitiveWord
                   {
                       ID = payExchangeSensitiveWord.ID,
                       AreaID = payExchangeSensitiveWord.AreaID,
                       Content = payExchangeSensitiveWord.Content,
                       Status = payExchangeSensitiveWord.Status,
                       CreateDate = payExchangeSensitiveWord.CreateDate,
                       UpdateDate = payExchangeSensitiveWord.UpdateDate,
                       Summary = payExchangeSensitiveWord.Summary,
                   };
        }
    }
}
