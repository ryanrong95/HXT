//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Needs.Linq;
//using Layer.Data;
//using Layer.Data.Sqls;
//using Wms.Services.Extends;
//using Needs.Utils.Descriptions;

//namespace Wms.Services.Models
//{
//    public class Menus : IUnique, IPersistence, IFulSuccess, IFulError
//    {
//        #region 属性
//        /// <summary>
//        /// 编号
//        /// </summary>
//        public string ID { get; internal set; }
//        /// <summary>
//        /// 父编号
//        /// </summary>
//        public string FatherID { get; set; }
//        /// <summary>
//        /// 菜单名
//        /// </summary>
//        public string Name { get; set; }
//        /// <summary>
//        /// 菜单地址
//        /// </summary>
//        public string Url { get; set; }
//        /// <summary>
//        /// 菜单图标
//        /// </summary>
//        public string Icon { get; set; }
//        /// <summary>
//        /// 摘要
//        /// </summary>
//        public string Summary { get; set; }
//        /// <summary>
//        /// 状态
//        /// </summary>
//        public MenuStatus Status { get; set; }

//        public string MenuStausDes
//        {
//            get
//            {
//                return this.Status.GetDescription();
//            }
//        }
//        #endregion

//        #region 事件
//        public event ErrorHandler AbandonError;
//        public event SuccessHandler AbandonSuccess;
//        public event ErrorHandler EnterError;
//        public event SuccessHandler EnterSuccess;
//        #endregion

//        #region 方法
//        public void Abandon()
//        {
//            try
//            {
//                if (string.IsNullOrEmpty(this.ID))
//                {
//                    throw new ArgumentException("ID is NUll !");
//                }
//                using (var repository = new PvWmsRepository())
//                {
//                    repository.Update(this.ToLinq(), item => item.ID == this.ID);
//                }

//                AbandonSuccess?.Invoke(this, new SuccessEventArgs(this));

//            }
//            catch (Exception ex)
//            {
//                AbandonError?.Invoke(this, new ErrorEventArgs(ex.Message, ErrorType.System));
//            }
//        }

//        public void Enter()
//        {
//            try
//            {
//                using (var repository=new PvWmsRepository())
//                {
//                    if (string.IsNullOrEmpty(this.ID))
//                    {
//                        this.ID = PKeySigner.Pick(PkeyType.Menus);
//                        repository.Insert(this.ToLinq());
//                    }
//                    else
//                    {
//                        repository.Update(this.ToLinq(), item => item.ID == this.ID);
//                    }
//                }
//                EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
                
//            }
//            catch (Exception ex)
//            {

//                EnterError?.Invoke(this, new ErrorEventArgs(ex.Message, ErrorType.System));
//            }
//        }
//        #endregion


//    }
//}
