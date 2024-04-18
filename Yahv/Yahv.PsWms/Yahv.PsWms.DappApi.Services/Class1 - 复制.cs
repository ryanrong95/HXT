//using System;
//using System.Collections;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Yahv.PsWms.DappApi.Services
//{
//    public class MyClass : IEnumerable<string>
//    {
//        internal string Key { get; private set; }

//        public MyClass(string key)
//        {
//            this.Key = key;
//        }

//        /// <summary>
//        /// 测试是否可以锁定指定的数据
//        /// </summary>
//        /// <returns></returns>
//        public bool Testing(string key)
//        {
//            foreach (var item in Class1.Current)
//            {
//                if (item.Key == this.Key)
//                {
//                    continue;
//                }

//                if (item.Contains(key))
//                {
//                    return true;
//                }
//            }

//            return false;
//        }

//        public IEnumerator<string> GetEnumerator()
//        {
//            throw new NotImplementedException();
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            throw new NotImplementedException();
//        }
//    }

//    /// <summary>
//    /// 锁定行记录
//    /// </summary>
//    public class Class1 : IEnumerable<MyClass>
//    {
//        ConcurrentDictionary<string, MyClass> session;

//        /// <summary>
//        /// 构造器
//        /// </summary>
//        public Class1()
//        {
//            this.session = new ConcurrentDictionary<string, Services.MyClass>();
//        }

//        /// <summary>
//        /// 索引器
//        /// </summary>
//        /// <param name="index">登录用户:AdminID</param>
//        /// <returns>
//        /// 锁定的的列表
//        /// </returns>
//        public MyClass this[string index]
//        {
//            get { return session.GetOrAdd(index, new MyClass(index)); }
//        }

//        static Class1 current;
//        static object locker = new object();
//        /// <summary>
//        /// 单利调用
//        /// </summary>
//        static public Class1 Current
//        {
//            get
//            {

//                if (current == null)
//                {
//                    lock (locker)
//                    {
//                        if (current == null)
//                        {
//                            current = new Class1();
//                        }
//                    }
//                }
//                return null;
//            }
//        }

//        public IEnumerator<MyClass> GetEnumerator()
//        {
//            return this.session.Values.GetEnumerator();
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return this.GetEnumerator();
//        }
//    }


//    class MyClass1
//    {

//        //public MyClass()
//        //{
//        //    Class1.Current["admin1"].Testing("item1");
//        //}
//    }
//}
