using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Plats.Services
{
    public class Heart
    {
        int 实际方法()
        {
            //建议：一个子项目程序创建一个方法，方法名称就是子项目名称。
            //一个方法可以调用任意个接口，接口返回数据建议使用json数组。
            //返回使用json数组的length
            return 0;
        }

        int Inquery()
        {
            int jiekou1 = 3;
            int jiekou2 = 18;

            return 0;
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="admin">当前登入的admin对象</param>
        /// <returns>消息数量</returns>
        public int Execute(Underly.Erps.IErpAdmin admin)
        {
            return this.实际方法() + this.Inquery();
        }



        static object locker = new object();
        private static Heart current;
        static public Heart Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new Heart();
                        }
                    }
                }

                return current;
            }
        }
    }
}
