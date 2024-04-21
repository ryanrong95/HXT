//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Yahv.Payments
//{
//    /// <summary>
//    /// 现金
//    /// </summary>
//    public class AccountManager
//    {
//        public AccountCatalogs Catalogs { get; private set; }

//        string payer;
//        string payee;

//        public AccountManager(string conduct, string payer, string payee)
//        {
//            this.payer = payer;
//            this.payee = payee;

//            var dcc = SubjectManager.Current.GetEnumbers(conduct);
//            //this.Catalogs = new AccountCatalogs(dcc.Select(item =>
//            //{
//            //    return new AccountSubject
//            //    {
//            //        Payer = payer,
//            //        Payee = payee,
//            //        Catalog = item.Catalog,
//            //        Name = item.Name
//            //    };
//            //}));


//            //数据读取出来
//            //流水表
//        }
//    }
//}
