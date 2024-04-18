using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class OutGoodsContext
    {
        public IOutGoodsAdd Strategy { get; set; }

        public OutGoodsContext(IOutGoodsAdd strategy)
        {
            this.Strategy = strategy;
        }

        public void context()
        {
            this.Strategy.addOutGoods();
        }
    }
}
