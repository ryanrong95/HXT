using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Needs.Ccs.Services;

namespace WebApp
{
    public class Initializer
    {
        public Initializer()
        {

        }

        static public void Initialize()
        {
            Yahv.Services.Initializers.WhsBoot();
            Yahv.Services.Initializers.OrderBoot();
            //Yahv.Services.Initializers.OrderBoot();
        }
    }
}