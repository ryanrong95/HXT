using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Erps;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class CarriersRoll : Origins.CarriersOrigin
    {




        IErpAdmin admin;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CarriersRoll(IErpAdmin admin)
        {
            this.admin = admin;
        }
        public CarriersRoll()
        {
           
        }
        protected override IQueryable<Carrier> GetIQueryable()
        {
            return base.GetIQueryable();
        }



        
    }
}
