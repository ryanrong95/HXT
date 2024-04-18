//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Needs.Erp.Generic;
//using Needs.Linq;
//using NtErp.Services.Extends;

//namespace NtErp.Services.Models
//{
//    public class MapsAdmin : IMapsAdmin
//    {
//        public string MainID { get; set; }
//        public string SubID { get; set; }
//        public MapsType Type { get; set; }

//        public event ErrorHanlder AbandonError;
//        public event SuccessHanlder AbandonSuccess;
//        public event ErrorHanlder EnterError;
//        public event SuccessHanlder EnterSuccess;

//        public void Abandon()
//        {
//            try
//            {
//                using (var repository=new Layer.Data.Sqls.BvnErpReponsitory())
//                {
//                    repository.Delete<Layer.Data.Sqls.BvnErp.MapsAdmin>(item => item.MainID == this.MainID && item.SubID == this.SubID && item.Type == (int)this.Type);
//                }

//                if (this != null && this.AbandonSuccess != null)
//                {
//                    this.AbandonSuccess(this, new SuccessEventArgs(this));
//                }

//            }
//            catch (Exception ex)
//            {

//                if (this!=null && this.AbandonError!=null)
//                {
//                    this.AbandonError(this, new ErrorEventArgs($"Abandon Fail !!,{ex.Message}"));
//                }

//            }
//        }

//        public void Enter()
//        {
//            try
//            {
//                using (var repository = new Layer.Data.Sqls.BvnErpReponsitory())
//                {                 
//                    if (!repository.ReadTable<Layer.Data.Sqls.BvnErp.MapsAdmin>().Any((item => item.MainID == this.MainID && item.SubID == this.SubID && item.Type == (int)this.Type)))
//                    {
//                        repository.Insert(this.ToLinq());
//                    }                  
//                }

//                if (this != null && this.EnterSuccess != null)
//                {
//                    this.EnterSuccess(this, new SuccessEventArgs(this));
//                }
//            }
//            catch (Exception ex)
//            {
//                if (this!=null && this.EnterError!=null)
//                {
//                    this.EnterError(this, new ErrorEventArgs($"Enter Fail !!,{ex.Message}", ErrorType.Customer));
//                }
//            }

           
//        }
//    }
//}
