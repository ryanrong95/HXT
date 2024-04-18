using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yahv.PsWms.DappApi.Services
{
    /// <summary>
    /// 所有人
    /// </summary>
    public class Owner
    {
        /// <summary>
        /// 所属人AdminID
        /// </summary>
        internal string AdminID { get; private set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; private set; }

        /// <summary>
        /// 最后刷新时间
        /// </summary>
        public DateTime LastDate { get; private set; }


        public Owner(string adminID)
        {
            this.AdminID = adminID;
            this.CreateDate = DateTime.Now;
            this.LastDate = DateTime.Now;
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Flush()
        {
            this.LastDate = DateTime.Now;
        }


    }

    /// <summary>
    /// 锁定行记录
    /// </summary>
    public class Class1
    {
        ConcurrentDictionary<string, Owner> session;

        Thread thread;

        /// <summary>
        /// 构造器
        /// </summary>
        public Class1()
        {
            this.session = new ConcurrentDictionary<string, Services.Owner>();

            (this.thread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        foreach (var item in session.Values)
                        {
                            if (item.LastDate <= DateTime.Now.AddMilliseconds(-500))
                            {

                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            })).Start();

        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index">登录用户:AdminID</param>
        /// <returns>
        /// 锁定的的列表
        /// </returns>
        public Owner this[string index]
        {
            get { return session.GetOrAdd(index, new Owner(index)); }
        }

        static Class1 current;
        static object locker = new object();
        /// <summary>
        /// 单利调用
        /// </summary>
        static public Class1 Current
        {
            get
            {

                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new Class1();
                        }
                    }
                }
                return null;
            }
        }


        static public void Unlock(string key)
        {
            //Current.session.TryRemove(key, out new Owner());
        }
    }


    class MyClass1
    {

        object _11111()
        {

            var owner = Class1.Current["noticeitemid"];
            if (owner.AdminID != "")
            {
                return true;
            }

            owner.Flush();

            if (owner.LastDate <= DateTime.Now.AddHours(-1))
            {

            }



            return false;
        }
    }
}
