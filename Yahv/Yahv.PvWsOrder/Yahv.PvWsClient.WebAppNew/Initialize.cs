using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PvWsClient.WebAppNew
{
    public class Initializer
    {
        public Initializer()
        {

        }

        static public void Initialize()
        {
            Yahv.Services.Initializers.WhsBoot();
            Yahv.Services.Initializers.LsBoot();
            Yahv.Services.Initializers.OrderBoot();
        }
    }
}