//using Needs.Erp.Generic;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;


//namespace NtErp.Wss.Services.Views
//{
//    public class MyClientsView : ClientTopAlls
//    {
//        IGenericAdmin admin;

//        public MyClientsView(IGenericAdmin admin)
//        {
//            this.admin = admin;
//        }

//        protected override IQueryable<Models.ClientTop> GetIQueryable()
//        {
//            if (this.admin.IsSa)
//            {
//                return base.GetIQueryable();
//            }

//            //return from client in base.GetIQueryable()
//            //       join map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnWss.MapsAdminClient>() on client.ID equals map.AdminID
//            //       where map.AdminID == this.admin.ID
//            //       select client;

//            return from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnWss.MapsAdminClient>()
//                   join client in base.GetIQueryable() on map.AdminID equals client.ID
//                   where map.AdminID == this.admin.ID
//                   select client;
//        }

//        public void Bind(string clientid)
//        {
//            var maps = this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnWss.MapsAdminClient>().Where(item => item.AdminID == this.admin.ID && item.ClientID == clientid).SingleOrDefault();
//            if (maps == null)
//            {
//                this.Reponsitory.Insert(new Layer.Data.Sqls.BvnWss.MapsAdminClient { AdminID = this.admin.ID, ClientID = clientid });
//            }
//        }

//        public void UnBind(string clientid)
//        {
//            this.Reponsitory.Delete<Layer.Data.Sqls.BvnWss.MapsAdminClient>(item => item.AdminID == this.admin.ID && item.ClientID == clientid);
//        }
//    }
//}
