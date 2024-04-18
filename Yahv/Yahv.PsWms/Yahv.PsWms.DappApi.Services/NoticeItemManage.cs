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
    /// NoticeItem所有人
    /// </summary>
    public class NoticeItemOwner
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


        public NoticeItemOwner(string adminID)
        {
            this.AdminID = adminID;
            this.CreateDate = DateTime.Now;
            this.LastDate = DateTime.Now;
        }

        public void UpdateOwner(string adminID)
        {
            this.AdminID = adminID;
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
    /// NoticeItem管理器, 单例实现
    /// </summary>
    public class NoticeItemManage
    {
        private static NoticeItemManage current;
        private static object locker = new object();

        ConcurrentDictionary<string, NoticeItemOwner> session;
        Thread thread;

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private NoticeItemManage()
        {
            this.session = new ConcurrentDictionary<string, NoticeItemOwner>();
            (this.thread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        foreach (var item in session)
                        {
                            NoticeItemOwner deleteOwner = null;
                            if ((item.Value.LastDate - item.Value.CreateDate).Milliseconds > 500)
                            {
                                session.TryRemove(item.Key, out deleteOwner);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }

            })).Start();

        }
        
        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public NoticeItemOwner this[string index]
        {
            get
            {
                return session.GetOrAdd(index, new NoticeItemOwner(null));
            }
        }        

        /// <summary>
        /// 获取单例实例
        /// </summary>
        public static NoticeItemManage Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new NoticeItemManage();
                        }
                    }
                }

                return current;
            }
        }

        /// <summary>
        /// 保存并更新Session
        /// </summary>
        /// <param name="noticeItemID"></param>
        /// <param name="adminID"></param>
        public void Enter(string noticeItemID, string adminID)
        {
            NoticeItemOwner noticeItemOwner = session.GetOrAdd(noticeItemID, new NoticeItemOwner(adminID));
            noticeItemOwner.Flush();
        }

        public void UnLock(string noticeItemID)
        {
            NoticeItemOwner owner;
            Current.session.TryRemove(noticeItemID, out owner);
        }

    }

    /// <summary>
    /// NoticeItemSession视图
    /// </summary>
    public class NoticeItemSessionView : IDisposable
    {
        public NoticeItemSessionView()
        {

        }

        public void Dispose()
        {
            //不需要下面的实现否则会导致内存溢出
            //if (this != null)
            //{
            //    this.Dispose();
            //}
        }

        /// <summary>
        /// 保存Session, 并判断是否锁定
        /// </summary>
        /// <param name="noticeItemID"></param>
        /// <param name="adminID"></param>
        /// <returns></returns>
        public bool Session(string noticeItemID, string adminID)
        {
            bool isLock = false;
            var owner = NoticeItemManage.Current[noticeItemID];

            // 不存在该Session时，创建Session，同时对自己来说是非锁定状态
            if (owner == null)
            {
                NoticeItemManage.Current.Enter(noticeItemID, adminID);
                return false;
            }

            if (owner != null && !string.IsNullOrEmpty(owner.AdminID) && owner.AdminID != adminID)
            {                
                isLock = true;                
                return isLock;
            }

            //此种情况应该不存在的也没有用的
            //if (string.IsNullOrEmpty(owner.AdminID))
            //{
            //    owner.UpdateOwner(adminID); // 当新加入的对于自己是未锁定, 对于别人是锁定的.
            //    return false;                
            //}

            if(owner.LastDate <= DateTime.Now.AddMilliseconds(-500))
            {
                // 失去心跳500毫秒就认为不再占有锁定的NoticeItemID了
                NoticeItemManage.Current.UnLock(noticeItemID);                
            }
            return false;
        }
    }
}
