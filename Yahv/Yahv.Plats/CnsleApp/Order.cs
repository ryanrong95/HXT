using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnsleApp
{
    public class Order
    {
        /// <summary>
        /// asdfasdffq3f
        /// </summary>
        public event System.EventHandler Completed;
        /// <summary>
        /// 下单事件
        /// </summary>
        public event System.EventHandler Placed;

        /// <summary>
        /// sdfsdf
        /// </summary>
        public CnsleApp.OrderItems[] Items
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

      

        public void Place()
        {
            throw new System.NotImplementedException();
        }

        public void Complete()
        {
            throw new System.NotImplementedException();
        }
    }
}