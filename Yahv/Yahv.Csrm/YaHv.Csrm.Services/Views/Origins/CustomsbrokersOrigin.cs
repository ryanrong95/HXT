//using Layers.Data.Sqls;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using YaHv.Csrm.Services.Models.Origins;

//namespace YaHv.Csrm.Services.Views.Origins
//{
//    public class CustomsbrokersOrigin : Yahv.Linq.UniqueView<Customsbroker, PvbCrmReponsitory>
//    {
//        internal CustomsbrokersOrigin()
//        {

//        }
//        /// <summary>
//        /// 构造函数
//        /// </summary>
//        /// <param name="reponsitory">数据库连接</param>
//        internal CustomsbrokersOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
//        {
//        }
//        protected override IQueryable<Customsbroker> GetIQueryable()
//        {
//            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Customsbrokers>()
//                   select new Customsbroker
//                   {
//                       ID = entity.ID,
//                       AdminCode = entity.AdminCode,
//                       Name = entity.Name,
//                       Grade = (Grade)entity.Grade,
//                       DyjCode = entity.DyjCode,
//                       IsOwn = entity.IsOwn
//                   };
//        }
//    }
//}
