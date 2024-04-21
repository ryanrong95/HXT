//using Layers.Data.Sqls;
//using Layers.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Yahv.Usually;
//using Yahv.Utils.Converters.Contents;
//using YaHv.Csrm.Services.Extends;

//namespace YaHv.Csrm.Services.Models.Origins
//{
//    /// <summary>
//    /// 报关公司
//    /// </summary>
//    public class Customsbroker : Yahv.Linq.IUnique
//    {
//        #region 属性
//        string id;
//        /// <summary>
//        /// 唯一码
//        /// </summary>
//        /// <chenhan>保障全局唯一</chenhan>
//        public string ID
//        {
//            get
//            {
//                return this.id ?? this.Name.MD5();

//            }
//            set
//            {
//                this.id = value;
//            }
//        }
//        public string AdminCode { set; get; }
//        /// <summary>
//        /// 库房名称
//        /// </summary>
//        public string Name { set; get; }

//        /// <summary>
//        /// 大赢家编码
//        /// </summary>
//        public string DyjCode { set; get; }

//        /// <summary>
//        /// 等级
//        /// </summary>
//        public Grade Grade { set; get; }
//        /// <summary>
//        /// 
//        /// </summary>
//        public bool IsOwn { set; get; }
//        #endregion


//        #region 事件
//        /// <summary>
//        /// EnterSuccess
//        /// </summary>
//        public event SuccessHanlder EnterSuccess;
//        /// <summary>
//        /// AbandonSuccess
//        /// </summary>
//        public event SuccessHanlder AbandonSuccess;
//        #endregion

//        #region 持久化
//        public void Enter()
//        {
//            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
//            {
//                if (!repository.GetTable<Layers.Data.Sqls.PvbCrm.Customsbrokers>().Any(item => item.ID == this.ID))
//                {
//                    repository.Insert(this.ToLinq());
//                }
//                else
//                {
//                    repository.Update(this.ToLinq(), item => item.ID == this.ID);
//                }
//                if (this != null && this.EnterSuccess != null)
//                {
//                    this.EnterSuccess(this, new SuccessEventArgs(this));
//                }

//            }
//        }
//        public void Abandon()
//        {
//            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
//            {
//                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Customsbrokers>().Any(item => item.ID == this.ID))
//                {
//                    repository.Delete<Layers.Data.Sqls.PvbCrm.Customsbrokers>(item => item.ID == this.ID);
//                }
//                if (this != null && this.AbandonSuccess != null)
//                {
//                    this.AbandonSuccess(this, new SuccessEventArgs(this));
//                }
//            }

//        }
//        #endregion
//    }
//}
