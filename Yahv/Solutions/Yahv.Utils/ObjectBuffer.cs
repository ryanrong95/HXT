//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Yahv.Utils
//{
//    /// <summary>
//    /// 缓存器
//    /// </summary>
//    public class ObjectBuffer : IDisposable
//    {
//        private class Term
//        {
//            public string Key { get; private set; }
//            public object Data { get; private set; }
//            public DateTime CreateTime { get; private set; }
//            public Term(string key, object value)
//            {
//                this.Key = key;
//                this.Data = value;
//                this.CreateTime = DateTime.Now;
//            }
//        }

//        ConcurrentDictionary<string, Term> concurrent;
//        Thread thread;
//        ObjectBuffer()
//        {
//            this.concurrent = new ConcurrentDictionary<string, Term>();
//            (this.thread = new Thread(delegate ()
//            {
//                while (true)
//                {
//                    try
//                    {
//                        var timeour = DateTime.Now.AddSeconds(-2);
//                        Term remover;
//                        foreach (var key in concurrent.Values.
//                            Where(item => item.CreateTime < timeour).
//                            Select(item => item.Key))
//                        {
//                            this.concurrent.TryRemove(key, out remover);
//                        }

//                        Thread.Sleep(200);
//                    }
//                    catch (Exception)
//                    {
//                    }
//                }

//            })).Start();
//            concurrent.Clear();
//        }

//        /// <summary>
//        /// 索引器
//        /// </summary>
//        /// <param name="index">键</param>
//        /// <returns>缓存对象</returns>
//        public object this[string index]
//        {
//            get
//            {
//                Term term;
//                if (this.concurrent.TryGetValue(index, out term))
//                {
//                    return term;
//                }
//                else
//                {
//                    return null;
//                }
//            }
//            set
//            {
//                this.concurrent[index] = new Term(index, value);
//            }
//        }

//        static object locker = new object();
//        static ObjectBuffer current;
//        /// <summary>
//        /// 当前实例
//        /// </summary>
//        static public ObjectBuffer Current
//        {
//            get
//            {
//                if (current == null)
//                {
//                    lock (locker)
//                    {
//                        if (current == null)
//                        {
//                            current = new ObjectBuffer();
//                        }
//                    }
//                }
//                return current;
//            }

//        }

//        /// <summary>
//        /// 释放函数
//        /// </summary>
//        public void Dispose()
//        {
//            this.concurrent.Clear();
//            this.thread.Abort();
//        }
//    }
//}
