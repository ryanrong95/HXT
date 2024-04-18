//using System.Collections.Generic;
//using System.Data.Linq;

//namespace Layers.Data.Sqls.Overalls
//{

//    enum With
//    {
//        Nolock
//    }
//    /// <summary>
//    /// 扩展
//    /// </summary>
//    /// <example>泛型Update，记录更新的字段</example>
//    static public class ExchangeRatesExtends
//    {
//        static public Table<BoxesNolockView> With(this Table<ExchangeRates> table, With value)
//        {
//            switch (value)
//            {
//                case Overalls.With.Nolock:
//                    return table.Context.GetTable<BoxesNolockView>();
//                    break;
//                default:
//                    break;
//            }
          
//        }
//    }
//}
