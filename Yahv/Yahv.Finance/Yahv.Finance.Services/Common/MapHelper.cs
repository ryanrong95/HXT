using System;
using System.Reflection;

namespace Yahv.Finance.Services
{
    /// <summary>
    /// 映射工具类
    /// </summary>
    public class MapHelper
    {
        /// <summary>
        /// 映射
        /// </summary>
        /// <typeparam name="R">目标实体</typeparam>
        /// <typeparam name="T">数据源实体</typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static R Map<R, T>(T model)
        {
            //动态实例化对象
            R result = Activator.CreateInstance<R>();
            foreach (PropertyInfo info in typeof(R).GetProperties())
            {
                //判断是否是相同属性
                PropertyInfo pro = typeof(T).GetProperty(info.Name);
                if (pro != null)
                    //赋值
                    info.SetValue(result, pro.GetValue(model));
            }
            return result;
        }
    }
}