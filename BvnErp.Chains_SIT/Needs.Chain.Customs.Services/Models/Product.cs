//using Needs.Linq;
//using Needs.Utils.Converters;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Needs.Ccs.Services.Models
//{
//    /// <summary>
//    /// 标准产品
//    /// </summary>
//    public class Product : IUnique, IPersist, IFulError, IFulSuccess
//    {
//        public string ID
//        {
//            get
//            {
//                return string.Concat(this.Name, this.Model, this.Manufacturer, this.Batch).MD5();
//            }
//            set
//            {
//            }
//        }

//        /// <summary>
//        /// 产品名称
//        /// </summary>
//        public string Name { get; set; }

//        /// <summary>
//        /// 产品型号
//        /// </summary>
//        public string Model { get; set; }

//        /// <summary>
//        /// 品牌
//        /// </summary>
//        public string Manufacturer { get; set; }

//        /// <summary>
//        /// 批次
//        /// </summary>
//        public string Batch { get; set; }

//        /// <summary>
//        /// 产品描述（扩展字段）
//        /// </summary>
//        public string Description { get; set; }

//        public Product()
//        {

//        }

//        public event SuccessHanlder AbandonSuccess;
//        public event ErrorHanlder EnterError;
//        public event SuccessHanlder EnterSuccess;
//        public event ErrorHanlder AbandonError;

//        /// <summary>
//        /// 数据插入
//        /// </summary>
//        public void Enter()
//        {
//            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
//            {
//                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Products>().Count(item => item.ID == this.ID);
//                if (count == 0)
//                {
//                    reponsitory.Insert(this.ToLinq());
//                }
//                else
//                {
//                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
//                }
//            }

//            this.OnEnterSuccess();
//        }

//        virtual protected void OnEnterSuccess()
//        {
//            if (this != null && this.EnterSuccess != null)
//            {
//                //成功后触发事件
//                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
//            }
//        }

//        /// <summary>
//        /// 删除
//        /// </summary>
//        public void Abandon()
//        {
//            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
//            {
//                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.Products>(item => item.ID == this.ID);
//            }

//            this.OnAbandonSuccess();
//        }

//        virtual protected void OnAbandonSuccess()
//        {
//            if (this != null && this.AbandonSuccess != null)
//            {
//                //成功后触发事件
//                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
//            }
//        }
//    }
//}
